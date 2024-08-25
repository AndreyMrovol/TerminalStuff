using GameNetcodeStuff;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static TerminalStuff.StringStuff;
using static TerminalStuff.PluginCore.TerminalCustomizer;
using static OpenLib.CoreMethods.AddingThings;

namespace TerminalStuff
{

    public static class TerminalEvents
    {
        //public static Dictionary<TerminalNode, Func<string>> darmuhsTerminalStuff = [];
        //internal static List<TerminalKeyword> darmuhsKeywords = [];
        internal static TerminalNode lastNode = CreateDummyNode("cachedNode1", true, "");
        internal static TerminalNode justText = CreateDummyNode("cachedNode2", true, "");
        internal static string lastText = "";
        internal static string TotalValueFormat = "";
        internal static string VideoErrorMessage = "";
        public static bool clockDisabledByCommand = false;

        internal static bool quitTerminalEnum = false;


        //internal static GameObject dummyObject;
        internal static TerminalNode switchNode = CreateDummyNode("switchDummy", true, "this should not display, switch command");

        internal static void StorePacks()
        {
            if (!ConfigSettings.terminalPurchasePacks.Value)
                return;

            if (ConfigSettings.purchasePackCommands.Value == "")
                return;

            Dictionary<string, string> keywordAndItems = GetKeywordAndItemNames(ConfigSettings.purchasePackCommands.Value);

            if (keywordAndItems.Count == 0)
                return;

            foreach (KeyValuePair<string, string> item in keywordAndItems)
            {
                Plugin.Spam($"setting {item.Key} keyword to purchase pack with items: {item.Value}");
                AddNodeManual($"{item.Key}_PP", item.Key, CostCommands.AskPurchasePack, true, 2, ConfigSettings.TerminalStuffMain, 0, CostCommands.CompletePurchasePack, null, "", $"You have cancelled the purchase of Purchase Pack [{item.Key}].\r\n\r\n", true, 0, item.Key, true, item.Value);
            }
        }

        internal static TerminalNode GetNodeFromList(string query, Dictionary<string, TerminalNode> nodeListing)
        {
            foreach (KeyValuePair<string, TerminalNode> pairValue in nodeListing)
            {
                if (pairValue.Key == query)
                {
                    return pairValue.Value;
                }
            }
            return null; // No matching command found for the given query
        }
        internal static string RandomSuit()
        {
            SuitCommands.GetRandomSuit(out string suitString);
            return suitString;
        }
        internal static string QuitTerminalCommand()
        {
            string text = $"{ConfigSettings.quitString.Value}\n";
            Plugin.instance.Terminal.StartCoroutine(TerminalQuitter(Plugin.instance.Terminal));
            return text;
        }
        internal static IEnumerator TerminalQuitter(Terminal terminal)
        {
            if (quitTerminalEnum)
                yield break;
            quitTerminalEnum = true;
            yield return new WaitForSeconds(0.5f);
            terminal.QuitTerminal();
            quitTerminalEnum = false;
        }

        internal static string ClockToggle()
        {
            if (!TerminalClockStuff.showTime)
            {
                TerminalClockStuff.showTime = true;
                clockDisabledByCommand = false;
                string displayText = "Terminal Clock [ENABLED].\r\n";
                return displayText;
            }
            else
            {
                TerminalClockStuff.showTime = false;
                clockDisabledByCommand = true;
                string displayText = "Terminal Clock [DISABLED].\r\n";
                return displayText;
            }
        }

        internal static string GetCleanedScreenText(Terminal __instance)
        {
            string s = __instance.screenText.text.Substring(__instance.screenText.text.Length - __instance.textAdded);

            return RemovePunctuation(s);
        }

        private static string RemovePunctuation(string s) //copied from game files
        {
            StringBuilder stringBuilder = new();
            foreach (char c in s)
            {
                if (!char.IsPunctuation(c))
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().ToLower();
        }

        internal static void ShouldLockPlayerCamera(bool value, PlayerControllerB localPlayer)
        {
            if (!ConfigSettings.LockCameraInTerminal.Value)
                return;

            if (localPlayer != null)
            {
                localPlayer.disableLookInput = !value;
                Plugin.MoreLogs($"ShouldLockPlayerCamera set to: {!value}");
            }
        }

        internal static void ShouldDisableTerminalLight(bool value, string setting)
        {
            if (setting == "nochange")
                return;

            if (Plugin.instance.Terminal.terminalLight.enabled == value)
            {
                Plugin.instance.Terminal.terminalLight.enabled = !value;
                Plugin.MoreLogs($"terminalLight set to {!value} for setting: {setting}");
            }

        }

        internal static string RefreshCustomizationCommand()
        {
            string text = $"Refreshing TerminalCustomization from config.\n\n";
            TerminalCustomization();
            return text;
        }
    }
}

