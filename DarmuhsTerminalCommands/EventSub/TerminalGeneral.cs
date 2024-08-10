using static TerminalStuff.TerminalEvents;


namespace TerminalStuff.EventSub
{
    internal class TerminalGeneral
    {
        internal static TerminalNode lastNodeFormatted;
        internal static void OnTerminalDisable()
        {
            if (Plugin.instance.OpenBodyCamsMod)
                OpenBodyCamsCompatibility.ResidualCamsCheck();

            //Plugin.instance.Config.Reload();
            MenuBuild.ClearMyMenustuff();
            ConfigSettings.TerminalStuffMain.DeleteAll();
            lastNode.displayText = ""; //stop persisting nodes between lobbies
            lastText = "";
            //Plugin.ClearLists();
            //Terminal disabled, disabling ESC key listener OnDisable
        }

        internal static void OnLoadNode(TerminalNode node)
        {
            Plugin.MoreLogs($"LoadNewNode patch, nNS: {NetHandler.netNodeSet}");
            Plugin.Spam(Plugin.instance.Terminal.screenText.textComponent.textInfo.lineCount.ToString());

            if (ConfigSettings.TerminalFillEmptyText.Value == "nochange")
                return;

            if (Plugin.instance.Terminal.currentNode == Plugin.instance.Terminal.terminalNodes.specialNodes[1])
                return;

            if(Plugin.instance.Terminal.screenText.textComponent.textInfo.lineCount < 24 && node != lastNodeFormatted)
            {
                int spaceToFill = 24 - Plugin.instance.Terminal.screenText.textComponent.textInfo.lineCount;
                lastNodeFormatted = Plugin.instance.Terminal.currentNode;
                //if configitem >= 0 < 2, filltext with configitem choice
                FillText(ConfigSettings.TerminalFillEmptyText.Value, ref lastNodeFormatted, spaceToFill);
            }
        }

        private static void FillText(string formatChoice, ref TerminalNode fixLength, int spaceToFill)
        {

            if(formatChoice == "fillbottom")
            {
                for (int i = 0; i < spaceToFill; i++)
                {
                    fixLength.displayText += "\n";
                }
                Plugin.Spam("added space to bottom only");
                Plugin.instance.Terminal.LoadNewNode(fixLength);
                Plugin.Spam($"added {spaceToFill} lines to displayText, reloading node");
                return;
            }
            else if (formatChoice == "textmiddle")
            {
                for (int i = 0; i < spaceToFill; i++)
                {
                    if (i < spaceToFill / 2)
                        fixLength.displayText = fixLength.displayText.Insert(0, "\n");
                    else
                        fixLength.displayText += "\n";
                }
                Plugin.Spam("pushed text to middle of screen and added space to bottom");
                Plugin.instance.Terminal.LoadNewNode(fixLength);
                Plugin.Spam($"added {spaceToFill} lines to displayText, reloading node");
                return;
            }
            else if (formatChoice == "textbottom")
            {
                for (int i = 0; i < spaceToFill; i++)
                {
                    fixLength.displayText = fixLength.displayText.Insert(0, "\n");
                }
                Plugin.Spam("added space to top only");
                Plugin.instance.Terminal.LoadNewNode(fixLength);
                Plugin.Spam($"added {spaceToFill} lines to start of displayText, reloading node");
                return;
            }
            else
            {
                Plugin.Spam("invalid FillText formatChoice, returning");
                return;
            }
        }

        internal static void OnLoadAffordable(TerminalNode node)
        {
            if (!ConfigSettings.terminalRefund.Value || !ConfigSettings.ModNetworking.Value)
                return;

            if(node == null)
            {
                Plugin.WARNING("WARNING: node is null at OnLoadAffordable, using early return & dropship will not be synced!");
                return;
            }

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
