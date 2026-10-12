using UnityEngine;

namespace Saury.Core
{
    /// <summary>
    /// Demo Mode (default ON): TV-show chrome, no debug dump, AI seats 2–4, F1/F5 still work but tiny.
    /// Toggle with F3 in Play Mode, or set <see cref="demoMode"/> before KickOff.
    /// </summary>
    public static class DemoSettings
    {
        /// <summary>When true (default): clean HUD, AI for P2–P4, shorter waits.</summary>
        public static bool demoMode = true;

        /// <summary>Verbose body panel + cheat labels. Default off in demo mode.</summary>
        public static bool debugHud;

        public static bool ShowDebug => !demoMode || debugHud;

        /// <summary>Solo demo: only seat 0 is human; others get simple AI.</summary>
        public static bool AiOtherSeats = true;

        public static void TickHotkeys()
        {
            if (Input.GetKeyDown(KeyCode.F3))
            {
                debugHud = !debugHud;
                if (!demoMode && !debugHud)
                    demoMode = true;
                else if (debugHud)
                    demoMode = false;
            }
        }
    }
}
