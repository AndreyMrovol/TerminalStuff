﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TerminalStuff.StringStuff;

namespace TerminalStuff
{
    internal class DynamicCommands //Non-terminalAPI commands
    {
        //stuff
        public static int ParsedValue { get; internal set; }
        internal static bool newParsedValue = false;

        //fov
        internal static bool validFovNum = false;
        internal static bool fovEnum = false;

        internal static string fColor;
        internal static string Gamble;
        internal static string sColor;
        internal static string Link;
        internal static string Link2;
        internal static string Restart;
        public static string Linktext { get; internal set; } //public static string


        internal static string BindKeyToCommand()
        {
            string[] words = GetWords();
            ShortcutBindings.BindToCommand(words, words.Length, out string displayText);
            return displayText;
        }
        internal static string UnBindKeyToCommand()
        {
            string[] words = GetWords();
            ShortcutBindings.UnbindKey(words, words.Length, out string displayText);
            return displayText;
        }

        internal static string FovPrompt()
        {
            if (!Plugin.instance.FovAdjust)
            {
                validFovNum = false;
                string displayText = "Unable to change your fov at this time...\r\n\tRequired mod [FOVAdjust] is not loaded!\r\n\r\n";
                Plugin.ERROR("not enough words for the fov command!");
                return displayText;
            }

            string[] words = GetWords();

            if (words.Length < 2 || words.Length > 2)
            {
                validFovNum = false;
                string displayText = "Unable to change your fov at this time...\r\n\tInvalid input detected, no digits were provided!\r\n\r\n";
                Plugin.ERROR("not enough words for the fov command!");
                return displayText;
            }

            if (int.TryParse(words[1], out int parsedValue))
            {
                newParsedValue = true;
                validFovNum = true;
                Plugin.Spam("))))))))))))))))))Integer Established");
                ParsedValue = parsedValue;
                string displayText = $"Set your FOV to {ParsedValue}?\n\n\n\n\n\n\n\n\n\n\n\nPlease CONFIRM or DENY.\n";
                return displayText;
            }
            else
            {
                validFovNum = false;
                string displayText = "Unable to change your fov at this time...\r\n\tInvalid input detected, digits were provided!\r\n\r\n";
                Plugin.ERROR("there are no digits for the fov command!");
                return displayText;
            }
        }

        internal static string FovConfirm()
        {
            if (validFovNum)
            {
                Plugin.MoreLogs("Valid fov value detected, returning fov command action");
                FovNodeText(out string displayText);
                return displayText;
            }
            else
            {
                Plugin.MoreLogs("attempting to confirm invalid fov. Returning error");
                string displayText = Plugin.instance.Terminal.terminalNodes.specialNodes[5].displayText;
                return displayText;
            }
        }

        internal static string FovDeny()
        {
            if (validFovNum)
            {
                Plugin.MoreLogs("Valid fov value detected, but fov has been canceled");
                string displayText = $"Fov change to {ParsedValue} has been canceled.\r\n\r\n\r\n";
                return displayText;
            }
            else
            {
                Plugin.MoreLogs("attempting to confirm invalid fov. Returning error");
                string displayText = Plugin.instance.Terminal.terminalNodes.specialNodes[5].displayText;
                return displayText;
            }
        }

        private static void FovNodeText(out string displayText)
        {

            TerminalNode node = Plugin.instance.Terminal.currentNode;

            if (!Plugin.instance.FovAdjust)
            {
                displayText = "FovAdjust mod is not installed, command can not be run.\r\n";
            }
            else
            {
                int num = ParsedValue;
                float number = num;
                if (number != 0 && number >= 66f && number <= 130f && newParsedValue)  // Or use an appropriate default value
                {
                    node.clearPreviousText = true;
                    displayText = ("Setting FOV to - " + num.ToString() + "\n\n");
                    Plugin.instance.Terminal.StartCoroutine(FovEnum(Plugin.instance.Terminal, number));
                }
                else
                {
                    displayText = "Fov can only be set between 66 and 130\n";
                }

            }
        }

        private static IEnumerator FovEnum(Terminal term, float number)
        {
            if (fovEnum)
                yield break;

            fovEnum = true;
            yield return new WaitForSeconds(0.5f);
            FovAdjustStuff.FovAdjustFunc(term, number);
            fovEnum = false;
        }

    }
}
