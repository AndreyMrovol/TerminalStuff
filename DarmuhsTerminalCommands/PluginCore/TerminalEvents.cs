using GameNetcodeStuff;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using static TerminalStuff.StringStuff;
using static OpenLib.CoreMethods.AddingThings;
using static OpenLib.ConfigManager.ConfigSetup;
using Steamworks.Ugc;

namespace TerminalStuff
{

    public static class TerminalEvents
    {
        public static Dictionary<TerminalNode, Func<string>> darmuhsTerminalStuff = [];
        internal static List<TerminalKeyword> darmuhsKeywords = [];
        internal static string TotalValueFormat = "";
        internal static string VideoErrorMessage = "";
        public static bool clockDisabledByCommand = false;

        internal static bool quitTerminalEnum = false;


        internal static GameObject dummyObject;
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
                NewManagedBool(ref ConfigSettings.TerminalStuffBools, $"{item.Key}_PP", true, "", false, "", GetKeywordsPerConfigItem(item.Key), CostCommands.AskPurchasePack, 2, true, CostCommands.CompletePurchasePack, null, "", $"You have cancelled the purchase of Purchase Pack [{item.Key}.]\r\n\r\n", "", -1, $"{item.Key}", $"{item.Value}", 0, $"{item.Key}", true, 0, true);
            }
        }

        internal static void ShortcutCommands()
        {
            if (!ConfigSettings.terminalShortcuts.Value)
                return;
            Plugin.Spam("adding bindCommand managedbool");
            NewManagedBool(ref defaultManagedBools, "bindCommand", true, "Use this command to bind new shortcuts", false, "COMFORT", GetKeywordsPerConfigItem("bind"), DynamicCommands.BindKeyToCommand, 0, true, null, null, "", "", "bind");

            Plugin.Spam("adding unbindCommand managedbool");
            NewManagedBool(ref defaultManagedBools, "unbindCommand", true, "Use this command to unbind a terminal shortcut from a key", false, "COMFORT", GetKeywordsPerConfigItem("unbind"), DynamicCommands.UnBindKeyToCommand, 0, true, null, null, "", "", "unbind");

        }
        internal static Func<string> GetCommandDisplayTextSupplier(TerminalNode query)
        {
            foreach (KeyValuePair<TerminalNode, Func<string>> pairValue in darmuhsTerminalStuff)
            {
                if (pairValue.Key == query)
                {
                    return pairValue.Value;
                }
            }
            return null; // No matching command found for the given query
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

            if (ConfigSettings.TerminalHistory.Value)
                TerminalHistory.AddToCommandHistory(RemovePunctuation(s));

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

        internal static void TerminalCustomization()
        {
            if (!ConfigSettings.TerminalCustomization.Value)
                return;

            MeshRenderer termMesh = GameObject.Find("Environment/HangarShip/Terminal").GetComponent<MeshRenderer>();

            if (termMesh != null)
            {
                termMesh.material.color = ColorCommands.HexToColor(ConfigSettings.TerminalColor.Value);
            }
            else
                Plugin.MoreLogs("termMesh is null");


            Plugin.instance.Terminal.screenText.textComponent.color = ColorCommands.HexToColor(ConfigSettings.TerminalTextColor.Value);
            Plugin.instance.Terminal.topRightText.color = ColorCommands.HexToColor(ConfigSettings.TerminalMoneyColor.Value);
            Plugin.instance.Terminal.screenText.caretColor = ColorCommands.HexToColor(ConfigSettings.TerminalCaretColor.Value);
            Plugin.instance.Terminal.scrollBarVertical.image.color = ColorCommands.HexToColor(ConfigSettings.TerminalScrollbarColor.Value);
            Plugin.instance.Terminal.scrollBarVertical.gameObject.GetComponent<Image>().color = ColorCommands.HexToColor(ConfigSettings.TerminalScrollBGColor.Value);
            Plugin.instance.Terminal.terminalLight.color = ColorCommands.HexToColor(ConfigSettings.TerminalLightColor.Value);

            if (TerminalClockStuff.textComponent != null)
            {
                Plugin.MoreLogs("setting clock color to: {}");
                TerminalClockStuff.textComponent.color = ColorCommands.HexToColor(ConfigSettings.TerminalClockColor.Value);
            }
        }
    }
}

