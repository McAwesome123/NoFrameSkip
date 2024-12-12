# NoFrameSkip

Have you ever been annoyed at just how laggy the game gets whenever you hit anything 10+ loops into a run? Well, this mod... won't necessarily fix that, but should make the lag more bearable by reducing the max delta time in a frame.

Maximum delta time can be configured. [Risk Of Options](https://thunderstore.io/package/Rune580/Risk_Of_Options/) is supported.

### Notes and warnings

This mod has not been tested in multiplayer and may result in weird behaviour.

If your GPU cannot run this game at 60 fps, this mod will result in the game running slower. If you're fine with this, you don't need to do anything. If you're not, try increasing the max delta time to be at least 1 / fps (e.g. 0.05 for 20 fps). However, if this results in significantly lower fps, you are likely CPU bottlenecked.

For both of the above, having Risk Of Options is recommended.

## Explanation

### Background

Risk of Rain 2, like many Unity games, have various calculations (mainly physics) run on a fixed tickrate. In ROR2, this is 60 tps (or once every 16.7 ms). This means that your survivor's movement, the Elder Lemurian fireball coming at you, or your ATG Missile going towards the Elder Lemurian have their calculations run 60 times every second.

### The Cause

Well, to be more specific, it's 60 times every _in-game_ second. You see, when you have several thousand projectiles, whether they're ATG Missiles, Molten Perforator fireballs, Ukulele chain lightnings, etc, those calculations can take _quite a while_.

The issue happens when they start to take longer than 16.7 ms to calculate. Let's say they take 20 ms to calculate in this hypothetical example.

T = 0s, the previous frame has been processed. We do the next one, starting with physics. They take 20 ms to calculate. T = 0.02s. It's been longer than 16.7 ms, so we need to calculate physics again. T = 0.04s. And again. T = 0.06s. And again...

You see the problem? The physics updates take longer than the tick interval, meaning that it'll just continue to calculate physics over and over and over and over. But not forever. Unity has a cap on how much time can pass in a single frame. By default (and in this game), the cap is 0.333 seconds of in-game time (maximum delta time).

As such, at T = 0.4s\*, we break out of this loop and **finally** get to other calculations, including _actually rendering the frame_. Assuming all that happens instantly, this puts an effective cap on our framerate at 2.5 fps. Not very fluid.

\* Math: 0.333 seconds of max delta time, multiplied by 0.02 seconds of physics calculation time, divided by 0.0167 seconds of fixed delta time. \
(Note: The numbers shown here are rounded)

### The Solution

Normally, this maximum delta time means that if your slow ass GPU can only render 1 frame every 50 ms, your CPU can calculate 3 physics ticks (3 \* 16.7 ms = 50 ms) to compensate and so you get smoother gameplay. But when the bottleneck is the CPU, this simply results in a feedback loop until Unity decides that too much physics has happened and moves to render the frame anyway.

So the solution here is pretty simple: reduce the max delta time to be as low as possible (in this case, 0.0167 seconds).

### Sources / Further Information

[Unity Execution Order](https://docs.unity3d.com/Manual/execution-order.html) \
[Unity Time](https://docs.unity3d.com/ScriptReference/Time.html)

Thumbnail source: Screenshot I took while using the mod on [Debugging Plains](https://thunderstore.io/c/riskofrain2/p/Dragonyck/DebuggingPlains/) ([Original](https://github.com/user-attachments/assets/decca44b-14ac-4d69-b52c-63fba0f93dbc)).
