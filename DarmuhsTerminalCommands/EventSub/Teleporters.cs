using System.Collections.Generic;
using static TerminalStuff.StringStuff;
using static OpenLib.ConfigManager.ConfigSetup;
using static OpenLib.CoreMethods.AddingThings;
using static OpenLib.Menus.MenuBuild;

namespace TerminalStuff.EventSub
{
    internal class Teleporters
    {
        internal static void OnInverseAwake()
        {
            if (!ConfigSettings.terminalITP.Value)
                return;

            Plugin.MoreLogs("InverseTP instance detected, adding keyword");
            List<string> getKeywords = GetKeywordsPerConfigItem(ConfigSettings.itpKeywords.Value);

            AddNodeManual("Use Inverse Teleporter", getKeywords, ShipControls.InverseTeleporterCommand, true, 0, defaultListing, defaultManagedBools, "CONTROLS", "Active the Inverse Teleporter");
            if(MenuBuild.myMenuItems.Count > 0)
                MenuBuild.RefreshMyMenu(); //refresh menu items
        }

        internal static void OnNormalAwake()
        {
            if (!ConfigSettings.terminalTP.Value)
                return;

            Plugin.MoreLogs("NormalTP instance detected, adding keyword");
            List<string> getKeywords = GetKeywordsPerConfigItem(ConfigSettings.tpKeywords.Value);

            AddNodeManual("Use Teleporter", getKeywords, ShipControls.RegularTeleporterCommand, true, 0, defaultListing, defaultManagedBools, "CONTROLS", "Activate the Teleporter. Type a crewmate's name after the command to target them");
            if(MenuBuild.myMenuItems.Count > 0)
                MenuBuild.RefreshMyMenu(); //refresh menu items
        }
    }
}
