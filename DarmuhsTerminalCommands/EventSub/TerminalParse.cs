using OpenLib.ConfigManager;
using OpenLib.CoreMethods;
using OpenLib.Events;
using System.Collections.Generic;
using static OpenLib.Menus.MenuBuild;

namespace TerminalStuff.EventSub
{
    internal class TerminalParse
    {

        internal static TerminalNode OnParseSent(ref TerminalNode node)
        {
            if (ConfigSettings.networkedNodes.Value)
                NetHandler.NetNodeReset(false);

            StartofHandling.FirstCheck(node);

            if (node.name.Equals("0_StoreHub") && ConfigSettings.TerminalStuffMain.storePacks.Count > 0)
                GetDynamicCost();

            if (InMainMenu(node, MenuBuild.myMenu))
                return node;

            if (LogicHandling.GetNewDisplayText(ConfigSettings.TerminalStuffMain, ref node))
            {
                Plugin.MoreLogs($"node found: {node.name}");
            }

            return node;

        }

        internal static void GetDynamicCost()
        {
            foreach (KeyValuePair<TerminalNode, string> item in ConfigSettings.TerminalStuffMain.storePacks)
            {
                if (!item.Key.name.Contains("_confirm"))
                {
                    int itemCost = CostCommands.GetItemListCost(item.Value);
                    item.Key.itemCost = itemCost;
                    Plugin.Spam($"Updating price for {item.Key.name} to {item.Key.itemCost}");
                }
            }
        }

        internal static void OnNewDisplayText(TerminalNode node)
        {
            Plugin.Spam("newdisplaytext event!");

            if (ConfigSettings.TerminalStuffMain.storePacks.TryGetValue(node, out string value))
            {
                node.itemCost = 0;
                Plugin.MoreLogs("Updating currentPackList");
                CostCommands.currentPackList = value;
                if (node.creatureName != string.Empty)
                    CostCommands.currentPackName = node.creatureName;
            }

            return;
        }
    }
}
