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

            AddNodeManual("Use Inverse Teleporter", ConfigSettings.itpKeywords, ShipControls.InverseTeleporterCommand, true, 0, defaultListing, defaultManaged, "CONTROLS", "Active the Inverse Teleporter");
            if(MenuBuild.myMenuItems.Count > 0)
                MenuBuild.RefreshMyMenu(); //refresh menu items
        }

        internal static void OnNormalAwake()
        {
            if (!ConfigSettings.terminalTP.Value)
                return;

            Plugin.MoreLogs("NormalTP instance detected, adding keyword");

            AddNodeManual("Use Teleporter", ConfigSettings.tpKeywords, ShipControls.RegularTeleporterCommand, true, 0, defaultListing, defaultManaged, "CONTROLS", "Activate the Teleporter. Type a crewmate's name after the command to target them");
            if(MenuBuild.myMenuItems.Count > 0)
                MenuBuild.RefreshMyMenu(); //refresh menu items
        }
    }
}
