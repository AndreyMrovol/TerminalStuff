using OpenLib.Common;
using OpenLib.CoreMethods;
using System.Collections;
using System.Collections.Generic;
using TerminalStuff.VisualCore;
using UnityEngine;
using static OpenLib.ConfigManager.ConfigSetup;
using static OpenLib.CoreMethods.LogicHandling;
using static TerminalStuff.MoreCamStuff;
using static TerminalStuff.TerminalEvents;

namespace TerminalStuff
{
    internal class StartofHandling
    {
        internal static Coroutine delayedUpdater;
        //internal static TerminalNode dummyNode = CreateDummyNode("handling_node", true, "");

        internal static bool textUpdater = false;

        internal static TerminalNode HandleShortcut(Terminal terminal, TerminalNode currentNode, string[] words, out TerminalNode resultNode)
        {
            string firstWord = words[0].ToLower();
            HandleAnyNode(terminal, currentNode, words, firstWord, out resultNode);

            List<MainListing> fullListings =
            [
                defaultListing, ConfigSettings.TerminalStuffMain
            ];

            if (GetNewDisplayText(fullListings, ref resultNode))
                Plugin.MoreLogs("command found in one of the listings for shortcut...");
            return resultNode;
        }

        internal static TerminalNode HandleParsed(Terminal terminal, TerminalNode currentNode, string[] words, out TerminalNode resultNode)
        {
            string firstWord = words[0].ToLower();
            HandleAnyNode(terminal, currentNode, words, firstWord, out resultNode);

            if (resultNode == null)
                Plugin.WARNING("resultNode is null in HandleParsed");

            if (GetNewDisplayText(ConfigSettings.TerminalStuffMain, ref resultNode))
                Plugin.Spam("command found in special terminalStuff listing");
            else
                Plugin.Spam($"terminalstuffmain listing count: {ConfigSettings.TerminalStuffMain.Listing.Count} - listing count did not find node");

            if (ConfigSettings.TerminalStuffMain.storePacks.ContainsKey(resultNode))
            {
                EscapeConfirmCheck(ref resultNode);
            }


            return resultNode;
        }

        internal static void EscapeConfirmCheck(ref TerminalNode resultNode)
        {
            if (Plugin.instance.escapeConfirmation)
            {
                Plugin.Spam("escape confirmation detected");
                resultNode.overrideOptions = false;
                Plugin.instance.escapeConfirmation = false;
            }
            else
            {
                Plugin.Spam("escape confirmation is false");
                resultNode.overrideOptions = true;
            }
        }

        internal static int FindViewInt(TerminalNode givenNode)
        {
            foreach (KeyValuePair<TerminalNode, int> pairValue in ConfigSettings.TerminalStuffMain.specialListNum)
            {
                if (pairValue.Key == givenNode)
                {
                    int nodeNum = pairValue.Value;
                    return nodeNum;
                }
            }

            return -1;
        }

        internal static int FindViewIntByString()
        {
            string currentMode = GetViewMode();

            if (currentMode == "none")
                return -1;
            else
            {
                foreach (KeyValuePair<int, string> pairValue in ConfigSettings.TerminalStuffMain.ListNumToString)
                {
                    if (pairValue.Value == currentMode)
                    {
                        int nodeNum = pairValue.Key;
                        return nodeNum;
                    }
                }

                return -1;
            }
        }

        private static string GetViewMode()
        {
            string mode;

            if (Plugin.instance.isOnCamera)
            {
                mode = "cams";
                Plugin.MoreLogs("cams mode detected");
                return mode;
            }
            else if (Plugin.instance.isOnMap)
            {
                mode = "map";
                Plugin.MoreLogs("map mode detected");
                return mode;
            }
            else if (Plugin.instance.isOnOverlay)
            {
                mode = "overlay";
                Plugin.MoreLogs("overlay mode detected");
                return mode;
            }
            else if (Plugin.instance.isOnMiniMap)
            {
                mode = "minimap";
                Plugin.MoreLogs("minimap mode detected");
                return mode;
            }
            else if (Plugin.instance.isOnMiniCams)
            {
                mode = "minicams";
                Plugin.MoreLogs("minicams mode detected");
                return mode;
            }
            else if (Plugin.instance.isOnMirror)
            {
                mode = "mirror";
                Plugin.MoreLogs("Mirror mode detected");
                return mode;
            }
            else
            {
                Plugin.Log.LogError("Error with mode return, setting to default value");
                mode = "none";
                return mode;
            }
        }

        internal static TerminalNode FindViewNode(int givenInt)
        {
            if (givenInt < 0 || !ConfigSettings.TerminalStuffMain.specialListNum.ContainsValue(givenInt))
                return null;
            foreach (KeyValuePair<TerminalNode, int> pairValue in ConfigSettings.TerminalStuffMain.specialListNum)
            {
                if (pairValue.Value == givenInt)
                {
                    TerminalNode foundNode = pairValue.Key;
                    return foundNode;
                }
            }
            return null;
        }

        internal static void CheckNetNode(TerminalNode resultNode)
        {
            if (!ConfigSettings.NetworkedNodes.Value || !ConfigSettings.ModNetworking.Value)
                return;

            Plugin.MoreLogs("Networked nodes enabled, sending result to server.");
            if (resultNode != null)
            {
                if (ConfigSettings.TerminalStuffMain.specialListNum.ContainsKey(resultNode)) //should be the listing that contains the viewnodes
                {
                    int nodeNum = FindViewInt(resultNode);
                    NetHandler.NetNodeReset(true);
                    NetHandler.Instance.NodeLoadServerRpc(Plugin.instance.Terminal.topRightText.text, resultNode.name, resultNode.displayText, nodeNum);
                    Plugin.MoreLogs($"Valid node detected, nNS true & nodeNum: {nodeNum}");
                    return;
                }
                else
                {
                    NetHandler.NetNodeReset(true);
                    Plugin.MoreLogs("Valid node detected, nNS true");
                    NetHandler.Instance.NodeLoadServerRpc(Plugin.instance.Terminal.topRightText.text, resultNode.name, resultNode.displayText);
                    return;
                }
            }
            else
            {
                Plugin.MoreLogs("Invalid node for sync");
                return;
            }

        }

        internal static TerminalNode HandleAnyNode(Terminal terminal, TerminalNode currentNode, string[] words, string firstWord, out TerminalNode resultNode)
        {
            if (firstWord == "switch")
            {
                if (words.Length == 1)
                {
                    Plugin.MoreLogs("switch command detected");
                    resultNode = switchNode;

                    if (Plugin.instance.TwoRadarMapsMod)
                        TwoRadarMapsCompatibility.UpdateTerminalRadarTarget(terminal);
                    else
                        StartOfRound.Instance.mapScreen.SwitchRadarTargetForward(callRPC: true);

                    UpdateCamsTarget(StartOfRound.Instance.mapScreen.targetTransformIndex);
                    ViewCommands.DisplayTextUpdater(out string displayText);

                    resultNode.displayText = displayText;
                    Plugin.Spam(displayText);
                    return resultNode;
                }
                else
                {
                    Plugin.MoreLogs("switch to specific player command detected");
                    resultNode = terminal.terminalNodes.specialNodes[20];

                    if (Plugin.instance.TwoRadarMapsMod)
                    {
                        int playernum = TwoRadarMapsCompatibility.CheckForPlayerNameCommand(words[1].ToLower());
                        Plugin.Spam($"PlayerNameToTarget determined playernum - {playernum}");
                        if (playernum != -1)
                        {
                            TwoRadarMapsCompatibility.UpdateTerminalRadarTarget(terminal, playernum);
                            CamEvents.UpdateTextures.Invoke();
                            ViewCommands.DisplayTextUpdater(out string displayText);
                            resultNode.displayText = displayText;
                            return resultNode;
                        }
                        Plugin.MoreLogs("PlayerName returned invalid number");
                        resultNode = terminal.terminalNodes.specialNodes[12];
                        return resultNode;
                    }
                    else
                    {
                        int playernum = PlayerNameToTarget(words[1].ToLower(), StartOfRound.Instance.mapScreen.radarTargets);
                        Plugin.Spam($"PlayerNameToTarget determined playernum - {playernum}");
                        if (playernum != -1)
                        {
                            StartOfRound.Instance.mapScreen.SwitchRadarTargetAndSync(playernum);
                            UpdateCamsTarget(playernum);
                            CamEvents.UpdateTextures.Invoke();
                            ViewCommands.DisplayTextUpdater(out string displayText);
                            resultNode.displayText = displayText;
                            DelayedUpdateText(terminal);
                            return resultNode;
                        }

                        Plugin.MoreLogs("PlayerName returned invalid number");
                        resultNode = terminal.terminalNodes.specialNodes[12];
                        return resultNode;
                    }
                }
            }
            else
            {
                Plugin.Spam("returning current node");
                resultNode = currentNode;
                return currentNode;
            }
        }

        internal static void DelayedUpdateText(Terminal terminal)
        {
            if (delayedUpdater != null)
            {
                terminal.StopCoroutine(delayedUpdater);
            }

            delayedUpdater = terminal.StartCoroutine(DelayedUpdateTextRoutine(terminal));
        }

        internal static IEnumerator DelayedUpdateTextRoutine(Terminal terminal)
        {
            if (textUpdater)
                yield break;

            textUpdater = true;

            yield return new WaitForSeconds(0.045f);
            ViewCommands.DisplayTextUpdater(out string displayText);
            CamEvents.UpdateTextures.Invoke();
            switchNode.displayText = displayText;
            terminal.LoadNewNode(switchNode);

            textUpdater = false;

        }

        internal static void FirstCheck(TerminalNode initialResult)
        {
            if (ConfigSettings.TerminalHistory.Value)
            {
                string s = CommonStringStuff.GetCleanedScreenText(Plugin.instance.Terminal);
                TerminalHistory.AddToCommandHistory(CommonStringStuff.RemovePunctuation(s));
            }

            if (initialResult == null)
                return;

            VideoPersist(initialResult.name);
            CamPersistance(initialResult.name, initialResult);

            return;
        }

    }
}
