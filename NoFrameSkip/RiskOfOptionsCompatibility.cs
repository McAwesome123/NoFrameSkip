using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using RiskOfOptions;
using RiskOfOptions.OptionConfigs;
using RiskOfOptions.Options;

namespace NoFrameSkip
{
	internal static class RiskOfOptionsCompatibility
	{
		[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
		internal static void Initialize()
		{
			ModSettingsManager.AddOption(
				new StepSliderOption(
					NoFrameSkip.Instance.configMaxDeltaTime,
					new StepSliderConfig
					{
						name = "Maximum Delta Time",
						min = -1.0f / 60.0f,
						max = 0.5f,
						increment = 1.0f / 60.0f,
						restartRequired = false,
						FormatString = "{0:N3}",
					}
				)
			);
			ModSettingsManager.AddOption(
				new FloatFieldOption(
					NoFrameSkip.Instance.configMaxDeltaTime,
					new FloatFieldConfig
					{
						name = "Maximum Delta Time",
						description = "Same as above but you can put in any number that you want",
						restartRequired = false,
						FormatString = "{0:N10}",
					}
				)
			);
		}
	}
}
