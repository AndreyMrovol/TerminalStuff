using static OpenLib.Common.CommonStringStuff;
using static OpenLib.ConfigManager.ConfigSetup;
using static OpenLib.CoreMethods.CommandRegistry;

namespace TerminalStuff.PluginCore
{
    internal class StuffForLibrary
    {
        internal static void Init()
        {
            ConfigSettings.TerminalStuffBools = [];
            ConfigSettings.TerminalStuffMain = new();

            InitListing(ref ConfigSettings.TerminalStuffMain);
            Plugin.Log.LogInfo("TerminalStuffMain listing initialized");
        }

        internal static void AddCommands()
        {
            Plugin.Log.LogInfo("AddCommands called for TerminalStuffMain listing");
            GetCommandsToAdd(ConfigSettings.TerminalStuffBools, ConfigSettings.TerminalStuffMain);
            TerminalEvents.StorePacks();
        }

        internal static void ManualCommands() //for any commands that can be added before awake that are not managed by one config item per command
        {
            if (Plugin.instance.Terminal == null)
                return;

            if (!ConfigSettings.TerminalShortcuts.Value && ConfigSettings.TerminalShortcutCommands.Value)
            {
                ConfigSettings.TerminalShortcutCommands.Value = false;
                Plugin.WARNING("TerminalShortcutCommands was enabled while feature, TerminalShortcuts, was disabled. Setting to FALSE");
                Plugin.instance.Config.Save();
            }

            Plugin.Spam($"TerminalShortcutCommands Value: {ConfigSettings.TerminalShortcutCommands.Value}");

            NewManagedBool(ref defaultManaged, "bindCommand", ConfigSettings.TerminalShortcutCommands.Value, "Use this command to bind new shortcuts", false, "COMFORT", GetKeywordsPerConfigItem("bind"), DynamicCommands.BindKeyToCommand, 0, true, null, null, "", "", "bind");
            NewManagedBool(ref defaultManaged, "unbindCommand", ConfigSettings.TerminalShortcutCommands.Value, "Use this command to unbind a terminal shortcut from a key", false, "COMFORT", GetKeywordsPerConfigItem("unbind"), DynamicCommands.UnBindKeyToCommand, 0, true, null, null, "", "", "unbind");
        }
    }
}
