using static TerminalStuff.AdminCommands;
using static TerminalStuff.AlwaysOnStuff;
using static TerminalStuff.DynamicCommands;
using static TerminalStuff.EventSub.TerminalQuit;
using static TerminalStuff.EventSub.TerminalStart;
using static TerminalStuff.NetHandler;
using static TerminalStuff.ShipControls;
using static TerminalStuff.ShortcutBindings;
using static TerminalStuff.StartofHandling;
using static TerminalStuff.TerminalClockStuff;
using static TerminalStuff.TerminalEvents;
using static TerminalStuff.WalkieTerm;

namespace TerminalStuff
{
    internal class BoolStuff
    {
        internal static bool ListenForShortCuts()
        {

            if (Plugin.instance.suitsTerminal && SuitsTerminalCompatibility.CheckForSuitsMenu())
                return false;

            if (!Plugin.instance.Terminal.terminalInUse)
                return false;

            return true;
        }

        internal static void ResetEnumBools()
        {
            delayStartEnum = false;
            dynamicStatus = false;
            videoQuitEnum = false;
            quitTerminalEnum = false;
            leverEnum = false;
            fovEnum = false;
            shortcutListenEnum = false;
            terminalClockEnum = false;
            rainbowFlashEnum = false;
            kickEnum = false;
            walkieEnum = false;
            textUpdater = false;
        }

        internal static bool ShouldAddCamsLogic()
        {
            if (ConfigSettings.TerminalCams.Value)
                return true;
            if (ConfigSettings.TerminalMap.Value)
                return true;
            if (ConfigSettings.TerminalMinicams.Value)
                return true;
            if (ConfigSettings.TerminalMinimap.Value)
                return true;
            if (ConfigSettings.TerminalOverlay.Value)
                return true;
            return false;
        }

        internal static bool ShouldEnableImage()
        {
            if (ViewCommands.AnyActiveMonitoring())
                return true;
            if (Plugin.instance.isOnMirror)
                return true;
            if (!Plugin.instance.splitViewCreated && (bool)Plugin.instance.Terminal.displayingPersistentImage)
                return true;

            return false;
        }

        //internal static bool 

    }
}
