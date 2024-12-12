using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using System;
using UnityEngine;

namespace NoFrameSkip
{
	// This attribute is required, and lists metadata for your plugin.
	[BepInPlugin(PluginGUID, PluginName, PluginVersion)]
	[BepInDependency("com.rune580.riskofoptions", BepInDependency.DependencyFlags.SoftDependency)]

	// This is the main declaration of our plugin class.
	// BepInEx searches for all classes inheriting from BaseUnityPlugin to initialize on startup.
	// BaseUnityPlugin itself inherits from MonoBehaviour,
	// so you can use this as a reference for what you can declare and use in your plugin class
	// More information in the Unity Docs: https://docs.unity3d.com/ScriptReference/MonoBehaviour.html
	public class NoFrameSkip : BaseUnityPlugin
	{
		// The Plugin GUID should be a unique ID for this plugin,
		// which is human readable (as it is used in places like the config).
		// If we see this PluginGUID as it is on thunderstore,
		// we will deprecate this mod.
		// Change the PluginAuthor and the PluginName !
		public const string PluginGUID = $"{PluginAuthor}.ROR2.{PluginName}";
		public const string PluginAuthor = "McAwesome";
		public const string PluginName = "NoFrameSkip";
		public const string PluginVersion = "1.0.0";

		public static NoFrameSkip Instance { get; private set; }
		internal static new ManualLogSource Logger { get; private set; } = null!;

		internal ConfigEntry<float> configMaxDeltaTime;

		public float DefaultMaxDeltaTime { get; private set; }

		private float? configUpdateTimer = null;

		// The Awake() method is run at the very start when the game is initialized.
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "shut the fuck up")]
		private void Awake()
		{
			Logger = base.Logger;
			Instance = this;

			configMaxDeltaTime = Config.Bind<float>("General", "maxDeltaTime", 0.0f, "The max length of time that can pass between two rendered frames.\nUnity default is 0.333.\nSet to 0 to ensure no more than 1 physics update happens per frame.\nSet negative to leave unchanged.");

			Logger.LogInfo($"{PluginGUID} v{PluginVersion} has loaded!");
		}

		// The Start() method is run on the frame the script is initialized before any Update() methods are run.
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "shut the fuck up")]
		private void Start()
		{
			DefaultMaxDeltaTime = Time.maximumDeltaTime;

			configMaxDeltaTime.SettingChanged += ConfigUpdated;

			// Check for risk of options
			bool hasRiskOfOptions = false;
			foreach (PluginInfo pluginInfo in BepInEx.Bootstrap.Chainloader.PluginInfos.Values)
			{
				if (pluginInfo.Metadata.GUID == "com.rune580.riskofoptions")
				{
					hasRiskOfOptions = true;
					Version version = pluginInfo.Metadata.Version;
					if (version >= Version.Parse("2.4.2") && version < Version.Parse("3.0.0"))
					{
						RiskOfOptionsCompatibility.Initialize();
					}
					else
					{
						try
						{
							RiskOfOptionsCompatibility.Initialize();
							Logger.LogWarning($"Risk of Options detected, but version may be incompatible (Found: {version}, Expected: >=2.4.2, <3.0.0");
						}
						catch
						{
							Logger.LogError($"Risk of Options detected, but version is incompatible (Found: {version}, Expected: >=2.4.2, <3.0.0");
						}
					}
					break;
				}
			}

			if (!hasRiskOfOptions)
			{
				Logger.LogInfo("Risk of Options not found");
			}

			UpdateMaximumDeltaTime();
		}

		// The Update() method is run on every frame of the game.
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "shut the fuck up")]
		private void Update()
		{
			if (configUpdateTimer is not null)
			{
				if (configUpdateTimer > 0)
				{
					configUpdateTimer -= Time.unscaledDeltaTime;
				}
				else
				{
					UpdateMaximumDeltaTime();
					configUpdateTimer = null;
				}
			}
		}

		private void ConfigUpdated(object sender, EventArgs e)
		{
			configUpdateTimer = 1.0f;
		}

		// Update the maximum delta time when the config is changed.
		private void UpdateMaximumDeltaTime()
		{
			if (configMaxDeltaTime.Value < 0.0f)
			{
				Time.maximumDeltaTime = DefaultMaxDeltaTime;
				Logger.LogInfo($"maxDeltaTime config is negative ({configMaxDeltaTime.Value}), leaving maximumDeltaTime unchanged ({Time.maximumDeltaTime})");
			}
			else
			{
				Time.maximumDeltaTime = configMaxDeltaTime.Value;
				Logger.LogInfo($"Time.maximumDeltaTime set to {Time.maximumDeltaTime}");
				if (Time.maximumDeltaTime > configMaxDeltaTime.Value)
				{
					Logger.LogInfo($"(Note: This is higher than the config value ({configMaxDeltaTime.Value}) because Time.maximumDeltaTime cannot be less than Time.fixedDeltaTime)");
				}
			}
		}
	}
}
