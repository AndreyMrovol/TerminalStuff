using OpenLib.ConfigManager;
using OpenLib.CoreMethods;
using static OpenLib.CoreMethods.CommandRegistry;

namespace TerminalStuff.PluginCore
{
    internal class StuffForLibrary
    {
        internal static void Init()
        {
            ConfigSettings.TerminalStuffBools = new();
            ConfigSettings.TerminalStuffMain = new();

            InitListing(ref ConfigSettings.TerminalStuffMain);
            Plugin.Log.LogInfo("TerminalStuffMain listing initialized");
        }

        internal static void AddCommands()
        {
            Plugin.Log.LogInfo("AddCommands called for TerminalStuffMain listing");
            GetCommandsToAdd(ConfigSettings.TerminalStuffBools, ConfigSettings.TerminalStuffMain);
        }

        internal static void ManualCommands() //for any commands that can be added at awake that are not managed by one config item per command
        {
            TerminalEvents.StorePacks();
            TerminalEvents.ShortcutCommands();
        }
    }
}
