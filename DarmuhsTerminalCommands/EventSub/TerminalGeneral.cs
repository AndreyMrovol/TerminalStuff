using static TerminalStuff.TerminalEvents;
using TerminalStuff.PluginCore;
using OpenLib.ConfigManager;


namespace TerminalStuff.EventSub
{
    internal class TerminalGeneral
    {
        internal static void OnTerminalDisable()
        {
            if (Plugin.instance.OpenBodyCamsMod)
                OpenBodyCamsCompatibility.ResidualCamsCheck();

            MenuBuild.ClearMyMenustuff();
            ConfigSettings.TerminalStuffMain.DeleteAll();
            //Plugin.ClearLists();
            //Terminal disabled, disabling ESC key listener OnDisable
        }

        internal static void OnLoadNode(TerminalNode node)
        {
            Plugin.Spam($"LoadNewNode patch, nNS: {NetHandler.netNodeSet}");

            if (node != null && node.name != null)
                Plugin.Spam($"{node.name} has been loaded");
            else if (node != null)
                Plugin.Log.LogWarning("node loaded, name is NULL");
            else
                Plugin.Log.LogWarning("WARNING: TerminalNode is NULL");
        }

        internal static void OnLoadAffordable(TerminalNode node)
        {
            if (!ConfigSettings.terminalRefund.Value || !ConfigSettings.ModNetworking.Value || node == null)
                return;

            NetHandler.Instance.SyncDropShipServerRpc();
            Plugin.Spam($"items: {Plugin.instance.Terminal.orderedItemsFromTerminal.Count}");
        }

        internal static void OnSetTerminalInUse()
        {
            string setting = ConfigSettings.TerminalLightBehaviour.Value;
            if (setting == "nochange")
                return;
            else if (setting == "disable")
                ShouldDisableTerminalLight(true, setting);
            else if (setting == "alwayson")
                ShouldDisableTerminalLight(false, setting);
        }
    }
}
