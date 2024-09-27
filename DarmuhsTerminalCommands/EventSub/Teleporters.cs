using static OpenLib.ConfigManager.ConfigSetup;
using static OpenLib.CoreMethods.AddingThings;

namespace TerminalStuff.EventSub
{
    internal class Teleporters
    {
        internal static void OnInverseAwake()
        {
            if (!ConfigSettings.TerminalITP.Value)
                return;

            Plugin.MoreLogs("InverseTP instance detected, adding keyword");

            AddNodeManual("Use Inverse Teleporter", ConfigSettings.ItpKeywords, ShipControls.InverseTeleporterCommand, true, 0, defaultListing, defaultManaged, "CONTROLS", "Active the Inverse Teleporter");
            if (MenuBuild.myMenuItems.Count > 0)
                MenuBuild.RefreshMyMenu(); //refresh menu items
        }

        internal static void OnNormalAwake()
        {
            if (!ConfigSettings.TerminalTP.Value)
                return;

            Plugin.MoreLogs("NormalTP instance detected, adding keyword");

            AddNodeManual("Use Teleporter", ConfigSettings.TpKeywords, ShipControls.RegularTeleporterCommand, true, 0, defaultListing, defaultManaged, "CONTROLS", "Activate the Teleporter. Type a crewmate's name after the command to target them");
            if (MenuBuild.myMenuItems.Count > 0)
                MenuBuild.RefreshMyMenu(); //refresh menu items
        }
    }
}
