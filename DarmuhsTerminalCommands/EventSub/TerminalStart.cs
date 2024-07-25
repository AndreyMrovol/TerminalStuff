using static TerminalStuff.TerminalEvents;
using static TerminalStuff.AlwaysOnStuff;
using UnityEngine;
using System;
using System.Collections;
using static OpenLib.ConfigManager.ConfigSetup;
using static OpenLib.CoreMethods.CommonThings;
using static OpenLib.CoreMethods.LogicHandling;

namespace TerminalStuff.EventSub
{
    internal class TerminalStart
    {
        internal static bool alwaysOnDisplay = false;
        internal static bool isTermInUse = false;
        internal static TerminalNode startNode = null;
        internal static TerminalNode helpNode = null;
        internal static bool firstload = false;
        internal static bool delayStartEnum = false;

        internal static void OnTerminalStart()
        {
            TerminalStartGroup();
            TerminalStartGroupDelay();
        }

        internal static void OnTerminalStartDelayed()
        {
            //not using this yet
        }

        internal static void TerminalStartGroup()
        {
            Plugin.MoreLogs("Upgrading terminal with my stuff, smile.");
            Plugin.Allnodes = GetAllNodes();
            OverWriteTextNodes();
            TerminalClockStuff.MakeClock();
            ViewCommands.DetermineCamsTargets();
            ShortcutBindings.InitSavedShortcuts();
            TerminalCustomization();
            MenuBuild.CategoryList();
        }


        private static void OverWriteTextNodes()
        {
            Plugin.MoreLogs("updating displaytext for help and home");
            if (!GameStuff.oneTimeOnly)
            {
                startNode = Plugin.instance.Terminal.terminalNodes.specialNodes.ToArray()[1];
                helpNode = Plugin.instance.Terminal.terminalNodes.specialNodes.ToArray()[13];
                string original = helpNode.displayText;
                //Plugin.Spam(original);
                string replacement = original.Replace("To see the list of moons the autopilot can route to.", "List of moons the autopilot can route to.").Replace("To see the company store's selection of useful items.", "Company store's selection of useful items.").Replace("[numberOfItemsOnRoute]", ">MORE\r\nTo see a list of commands added via darmuhsTerminalStuff\r\n\r\n[numberOfItemsOnRoute]");
                //Plugin.Spam($"{replacement}");

                Plugin.instance.Terminal.terminalNodes.specialNodes.ToArray()[13].displayText = replacement;
                Plugin.Spam("~~~~~~~~~~~~~~~~~~~~~~~~~~~~ HELP MODIFIED ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");

                //string maskasciiart = "     ._______.\r\n     | \\   / |\r\n  .--|.O.|.O.|______.\r\n__).-| = | = |/   \\ |\r\np__) (.'---`.)Q.|.Q.|--.\r\n      \\\\___// = | = |-.(__\r\n       `---'( .---. ) (__&lt;\r\n             \\\\.-.//\r\n              `---'\r\n\t\t\t  ";
                string asciiArt = ConfigSettings.homeTextArt.Value;
                asciiArt = asciiArt.Replace("[leadingSpace]", " ");
                asciiArt = asciiArt.Replace("[leadingSpacex4]", "    ");
                //no known compatibility issues with home screen
                startNode.displayText = $"{ConfigSettings.homeLine1.Value}\r\n{ConfigSettings.homeLine2.Value}\r\n\r\n{ConfigSettings.homeHelpLines.Value}\r\n{asciiArt}\r\n\r\n{ConfigSettings.homeLine3.Value}\r\n\r\n";
                GameStuff.oneTimeOnly = true;
            }

            OpenLib.CoreMethods.AddingThings.AddKeywordToExistingNode("home", Plugin.instance.Terminal.terminalNodes.specialNodes.ToArray()[1], true); //startNode
            //StopPersistingKeywords();
            ChangeVanillaKeywords();
        }

        internal static void TerminalStartGroupDelay()
        {
            Plugin.MoreLogs("Starting TerminalDelayStartEnumerator");
            Plugin.instance.Terminal.StartCoroutine(TerminalDelayStartEnumerator());
        }

        internal static IEnumerator TerminalDelayStartEnumerator()
        {
            if (delayStartEnum)
                yield break;

            delayStartEnum = true;

            yield return new WaitForSeconds(1);
            Plugin.MoreLogs("1 Second delay methods starting.");
            //StoreCommands(); //adding after delay for storerotation mod
            SplitViewChecks.CheckForSplitView("neither");
            Plugin.MoreLogs("disabling cams views");
            ViewCommands.isVideoPlaying = false;
            StartOfRound.Instance.mapScreen.SwitchRadarTargetAndSync(0); //fix vanilla bug where you need to switch map target at start
            NetHandler.UpgradeStatusCheck(); // sync upgrades status for this save
            TerminalClockStuff.StartClockCoroutine();
            AlwaysOnStart(Plugin.instance.Terminal, startNode);
            yield return new WaitForSeconds(0.1f);
            StartCheck(Plugin.instance.Terminal, startNode);
            DebugShowInfo();
            delayStartEnum = false;
        }

        public static void ToggleScreen(bool status)
        {
            Plugin.instance.Terminal.StartCoroutine(Plugin.instance.Terminal.waitUntilFrameEndToSetActive(status));
            Plugin.Spam($"Screen set to {status}");
        }

        private static void AlwaysOnStart(Terminal thisterm, TerminalNode startNode)
        {

            if (ConfigSettings.alwaysOnAtStart.Value && !firstload)
            {
                Plugin.Spam("Setting AlwaysOn Display.");
                if (ConfigSettings.networkedNodes.Value && ConfigSettings.ModNetworking.Value)
                {
                    Plugin.Spam("network nodes enabled, syncing alwayson status");
                    NetHandler.Instance.StartAoDServerRpc(true);
                    firstload = true;
                }
                else
                {
                    alwaysOnDisplay = true;
                    ToggleScreen(true);
                    thisterm.LoadNewNode(startNode);
                    firstload = true;
                }

                if (ConfigSettings.TerminalLightBehaviour.Value == "alwayson")
                    ShouldDisableTerminalLight(false, "alwayson");

            }
        }

        private static void StartCheck(Terminal thisterm, TerminalNode startNode)
        {
            if (!ConfigSettings.ModNetworking.Value || !ConfigSettings.networkedNodes.Value)
            {
                Plugin.Spam("Networking disabled, returning...");
                thisterm.LoadNewNode(startNode);
                return;
            }

            NetHandler.Instance.SyncRadarZoomServerRpc(ConfigSettings.TerminalRadarDefaultZoom.Value); //sync config at load-in

            if (GameNetworkManager.Instance.localPlayerController.IsHost)
            {
                thisterm.LoadNewNode(startNode);
                StartofHandling.CheckNetNode(startNode);
                return;
            }
            else
            {
                Plugin.Spam("------------ CLIENT JUST LOADED --------------");
                Plugin.Spam("grabbing node from host");
                Plugin.Spam("------------ CLIENT JUST LOADED --------------");

                int hostClient = Misc.HostClientID();
                NetHandler.Instance.GetCurrentNodeServerRpc(((int)StartOfRound.Instance.localPlayerController.playerClientId), hostClient);
                return;
            }
        }

        private static void ChangeVanillaKeywords()
        {
            //deletes keywords at game start if they exist from previous plays

            CheckForAndDeleteKeyWord("view monitor");

            //MakeCommand("ViewInsideShipCam 1", "view monitor", "view monitor", false, true, ViewCommands.TermMapEvent, darmuhsTerminalStuff, 5, "map", ViewCommands.termViewNodes, ViewCommands.termViewNodeNums);
            //AddCommand(string textFail, bool clearText, List<TerminalNode> nodeGroup, string keyWord, bool isVerb, string nodeName, string category, string description, CommandDelegate methodName)
        }
        private static void DebugShowInfo()
        {
            Plugin.Spam($"Terminal Keywords Count: {Plugin.instance.Terminal.terminalNodes.allKeywords.Length}");
            Plugin.Spam($"Plugin.Allnodes: {Plugin.Allnodes.Count}");
            Plugin.Spam($"TerminalStuffBools.Count: {ConfigSettings.TerminalStuffBools.Count}");
            Plugin.Spam($"TerminalStuffMain.Listing.Count: {ConfigSettings.TerminalStuffMain.Listing.Count}");
            Plugin.Spam($"defaultListing.Listing.Count: {defaultListing.Listing.Count}");
            Plugin.Spam($"defaultManaged.Count: {defaultManaged.Count}");

            Plugin.Spam("------------------------ end of darmuh's debug info ------------------------");

        }
    }
}
