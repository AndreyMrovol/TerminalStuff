using OpenLib.Menus;
using System.Collections.Generic;
using System.Text;
using TerminalStuff.VisualCore;
using UnityEngine;
using static OpenLib.Menus.MenuBuild;
using static TerminalStuff.AllMyTerminalPatches;
using static TerminalStuff.MoreCamStuff;
using static TerminalStuff.StringStuff;
using static TerminalStuff.VisualCore.CamEvents;

namespace TerminalStuff
{
    internal class ViewCommands
    {
        //internal static Dictionary<TerminalNode, int> termViewNodes = []; //defaultListing.specialListNum
        //internal static Dictionary<int, string> termViewNodeNums = []; //defaultListing.ListNumToString
        internal static bool externalcamsmod = false;
        internal static bool isVideoPlaying = false;
        internal static Camera playerCam = null;
        internal static GameObject darmCamObject;

        //internal static int cullingMaskInt;
        internal static int targetInt = 0;
        internal static float radarZoom;

        internal static string TermMapEvent()
        {
            if (StartOfRound.Instance != null && StartOfRound.Instance.shipDoorsEnabled)
            {
                HandleMapEvent(out string message);
                return message;
            }
            else
            {
                HandleOrbitMapEvent(out string message);
                return message;
            }
        }

        private static void HandleMapEvent(out string displayText)
        {
            if (!Plugin.instance.isOnMap)
            {
                UpdateCamsEvent.Invoke("map");
                DisplayTextUpdater(out string message);
                displayText = message;
                return;
            }
            else
            {
                SplitViewChecks.DisableSplitView("map");
                displayText = $"{ConfigSettings.MapOffString.Value}\r\n";
                return;
            }
        }

        private static void HandleOrbitMapEvent(out string displayText)
        {
            TerminalNode node = Plugin.instance.Terminal.currentNode;

            Plugin.MoreLogs("This should only trigger in orbit");
            node.clearPreviousText = true;
            node.loadImageSlowly = false;
            displayText = "Radar view not available in orbit.\r\n";
            ResetPluginInstanceBools();
            return;
        }

        internal static string HandlePreviousSwitchEvent()
        {
            Plugin.MoreLogs("switching to previous player event detected");

            if (!AnyActiveMonitoring())
                return "There is no active monitoring to switch!";

            if (Plugin.instance.TwoRadarMapsMod)
                TwoRadarMapsCompatibility.UpdateTerminalRadarTarget(Plugin.instance.Terminal, -2);
            else
            {
                int newTarget = GetPrevValidTarget(StartOfRound.Instance.mapScreen.radarTargets, StartOfRound.Instance.mapScreen.targetTransformIndex);
                StartOfRound.Instance.mapScreen.SwitchRadarTargetAndSync(newTarget);
                UpdateCamsTarget(newTarget);
            }

            DisplayTextUpdater(out string message);

            return message;
        }

        internal static string MirrorEvent()
        {
            isVideoPlaying = false;

            if (Plugin.instance.isOnMirror == false && Plugin.instance.splitViewCreated)
            {
                SetMirrorState(true);
                CamEvents.UpdateCamsEvent.Invoke("mirror");

                Plugin.MoreLogs("Mirror added to terminal screen");
                DisplayTextUpdater(out string displayText);
                return displayText;
            }
            else
            {
                SetMirrorState(false);
                SplitViewChecks.DisableSplitView("mirror");
                Plugin.MoreLogs("mirror removed");
                return $"\n\n\t>>Mirror Camera removed from terminal.\r\n\r\n";
            }
        }

        internal static string TermCamsEvent()
        {
            isVideoPlaying = false;

            if (Plugin.instance.OpenBodyCamsMod && ConfigSettings.CamsUseDetectedMods.Value)
            {
                if (!OpenLib.Compat.OpenBodyCamFuncs.BodyCamIsUnlocked() && ConfigSettings.ObcRequireUpgrade.Value)
                    return "\tThis command is currently <color=#ff1a1a>unavailable</color>!\n\nPlease purchase the <color=#ffff66>BodyCam upgrade</color> to use this command.\r\n\r\n";
            }

            if (Plugin.instance.isOnCamera == false && Plugin.instance.splitViewCreated)
            {
                UpdateCamsEvent.Invoke("cams");

                // Enable split view and update bools
                SplitViewChecks.EnableSplitView("cams");

                Plugin.MoreLogs("Cam added to terminal screen");
                DisplayTextUpdater(out string displayText);
                return displayText;
            }
            else
            {
                SplitViewChecks.DisableSplitView("cams");
                string displayText = $"{ConfigSettings.CamOffString.Value}\r\n";
                Plugin.MoreLogs("Cams removed");
                return displayText;
            }
        }

        internal static string MiniCamsTermEvent()
        {
            isVideoPlaying = false;

            if (Plugin.instance.OpenBodyCamsMod && ConfigSettings.CamsUseDetectedMods.Value)
            {
                if (!OpenLib.Compat.OpenBodyCamFuncs.BodyCamIsUnlocked() && ConfigSettings.ObcRequireUpgrade.Value)
                    return "\tThis command is currently <color=#ff1a1a>unavailable</color>!\n\nPlease purchase the <color=#ffff66>BodyCam upgrade</color> to use this command.\r\n\r\n";
            }

            if (Plugin.instance.splitViewCreated && !Plugin.instance.isOnMiniCams)
            {
                UpdateCamsEvent.Invoke("minicams");

                DisplayTextUpdater(out string displayText);
                return displayText;
            }
            else
            {
                SplitViewChecks.DisableSplitView("minicams");
                return $"{ConfigSettings.MiniCamsOffString.Value}\r\n";
            }
        }

        internal static string RadarZoomEvent()
        {
            string val = GetAfterKeyword(GetKeywordsPerConfigItem(ConfigSettings.RadarZoomKWs.Value));

            if (!AnyActiveMonitoring() && Plugin.instance.splitViewCreated)
            {
                return $"No active monitoring detected, unable to change zoom.\r\n\r\n";
            }
            else if (!Plugin.instance.splitViewCreated && !(bool)Plugin.instance.Terminal.displayingPersistentImage)
            {
                return $"No active monitoring detected, unable to change zoom.\r\n\r\n";
            }
            else
            {
                if (val.Length < 1)
                {
                    if (Plugin.instance.TwoRadarMapsMod)
                    {
                        TwoRadarMapsCompatibility.ChangeMapZoom(GetNewZoom(ref radarZoom));
                        Plugin.MoreLogs($"Radar Zoom for TwoRadarMaps set to {radarZoom}");
                    }
                    else
                    {
                        StartOfRound.Instance.mapScreen.cam.orthographicSize = GetNewZoom(ref radarZoom);
                        Plugin.MoreLogs($"Radar Zoom set to {radarZoom}");
                    }

                    if (ConfigSettings.NetworkedNodes.Value)
                        NetHandler.Instance.SyncRadarZoomServerRpc(radarZoom);


                    return $"\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\nRadar Zoom level adjusted.\r\n";
                }
                else
                {
                    if (int.TryParse(val, out int newZoomVal))
                    {
                        if (newZoomVal >= 5 && newZoomVal <= 50)
                        {
                            radarZoom = newZoomVal;
                            if (Plugin.instance.TwoRadarMapsMod)
                            {
                                TwoRadarMapsCompatibility.ChangeMapZoom(radarZoom);
                                Plugin.MoreLogs($"Radar Zoom for TwoRadarMaps set to {radarZoom}");
                            }
                            else
                            {
                                StartOfRound.Instance.mapScreen.cam.orthographicSize = radarZoom;
                                Plugin.MoreLogs($"Radar Zoom set to {radarZoom}");
                            }

                            if (ConfigSettings.NetworkedNodes.Value)
                                NetHandler.Instance.SyncRadarZoomServerRpc(radarZoom);

                            return $"\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\nRadar Zoom level adjusted to new value: {val}\r\n";
                        }
                        else
                            return $"Cannot change zoom to value: {val}.\nValue is too high or too low.\r\n\r\n";
                    }
                    else
                        return $"Cannot change zoom to invalid value: {val}.";
                }
            }
        }

        internal static float GetNewZoom(ref float currentZoom)
        {
            if (currentZoom >= 10f)
            {
                currentZoom -= 5f;
            }
            else
                currentZoom = 30f;

            return currentZoom;
        }

        internal static string MiniMapTermEvent()
        {
            isVideoPlaying = false;

            if (Plugin.instance.OpenBodyCamsMod && ConfigSettings.CamsUseDetectedMods.Value)
            {
                if (!OpenLib.Compat.OpenBodyCamFuncs.BodyCamIsUnlocked() && ConfigSettings.ObcRequireUpgrade.Value)
                    return "\tThis command is currently <color=#ff1a1a>unavailable</color>!\n\nPlease purchase the <color=#ffff66>BodyCam upgrade</color> to use this command.\r\n\r\n";
            }

            if (Plugin.instance.splitViewCreated && !Plugin.instance.isOnMiniMap)
            {
                UpdateCamsEvent.Invoke("minimap");

                DisplayTextUpdater(out string displayText);
                return displayText;
            }
            else
            {
                SplitViewChecks.DisableSplitView("minimap");
                return $"{ConfigSettings.MiniMapOffString.Value}\r\n";
            }
        }

        internal static string OverlayTermEvent()
        {
            isVideoPlaying = false;

            if (Plugin.instance.OpenBodyCamsMod && ConfigSettings.CamsUseDetectedMods.Value)
            {
                if (!OpenLib.Compat.OpenBodyCamFuncs.BodyCamIsUnlocked() && ConfigSettings.ObcRequireUpgrade.Value)
                    return "\tThis command is currently <color=#ff1a1a>unavailable</color>!\n\nPlease purchase the <color=#ffff66>BodyCam upgrade</color> to use this command.\r\n\r\n";
            }

            if (Plugin.instance.splitViewCreated && !Plugin.instance.isOnOverlay)
            {
                UpdateCamsEvent.Invoke("overlay");

                DisplayTextUpdater(out string displayText);
                return displayText;
            }
            else
            {
                SplitViewChecks.DisableSplitView("overlay");
                return $"{ConfigSettings.OverlayOffString.Value}\r\n";
            }
        }



        internal static void SetAnyCamsTrue()
        {
            Plugin.instance.activeCam = true;
            NetHandler.SyncMyCamsBoolToEveryone(true);
        }



        internal static string LolVideoPlayerEvent()
        {
            Plugin.MoreLogs("Start of LolEvent");

            TerminalNode node = Plugin.instance.Terminal.currentNode;

            SplitViewChecks.CheckForSplitView("neither"); // Disables split view components if enabled

            if (VideoManager.Videos.Count == 0) //if videos failed to load at launch
                return "No videos available to play!\n\nWomp Womp.\r\n\r\n";

            node.clearPreviousText = true;
            FixVideoPatch.sanityCheckLOL = true;

            string displayText = VideoManager.PickVideoToPlay(Plugin.instance.Terminal.videoPlayer);
            return displayText;
        }

        internal static string NoVanillaView()
        {
            StringBuilder message = new();
            message.AppendLine("\tThis command has been <color=#ff1a1a>replaced</color>!\n\nPlease use one of the following alternatives:\n");
            List<TerminalMenuItem> menus = TerminalMenuItems(ConfigSettings.ViewConfig);

            foreach (TerminalMenuItem menuItem in menus)
            {
                message.AppendLine($"> <color=#ffff66>{OpenLib.Common.CommonStringStuff.GetKeywordsForMenuItem(menuItem.itemKeywords)}</color>\r\n{menuItem.itemDescription}\r\n");
            }

            return message.ToString();
        }

        internal static void DisplayTextUpdater(out string displayText)
        {
            Plugin.MoreLogs("updating displaytext!!!");
            GetCurrentMode(out string mode);
            string playerName;
            if (!Plugin.instance.TwoRadarMapsMod)
                playerName = StartOfRound.Instance.mapScreen.radarTargets[StartOfRound.Instance.mapScreen.targetTransformIndex].name;
            else
                playerName = TwoRadarMapsCompatibility.TargetedPlayerOnSecondRadar();

            if (mode == "Mirror")
                displayText = "\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\nMirror Enabled.";
            else
                displayText = $"\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\nMonitoring: {playerName} [{mode}]\r\n\n";
            return;
        }

        private static void GetCurrentMode(out string mode)
        {
            if (Plugin.instance.isOnCamera)
            {
                mode = ConfigSettings.CamOnString.Value;
                Plugin.MoreLogs("cams mode detected");
                return;
            }
            else if (Plugin.instance.isOnMap)
            {
                mode = ConfigSettings.MapOnString.Value;
                Plugin.MoreLogs("map mode detected");
                return;
            }
            else if (Plugin.instance.isOnOverlay)
            {
                mode = ConfigSettings.OverlayOnString.Value;
                Plugin.MoreLogs("overlay mode detected");
                return;
            }
            else if (Plugin.instance.isOnMiniMap)
            {
                mode = ConfigSettings.MiniMapOnString.Value;
                Plugin.MoreLogs("minimap mode detected");
                return;
            }
            else if (Plugin.instance.isOnMiniCams)
            {
                mode = ConfigSettings.MiniCamsOnString.Value;
                Plugin.MoreLogs("minicams mode detected");
                return;
            }
            else if (Plugin.instance.isOnMirror)
            {
                mode = "Mirror";
                Plugin.MoreLogs("Mirror mode detected");
                return;
            }
            else
            {
                Plugin.Log.LogError("Error with mode return, setting to default value");
                mode = "???";
                return;
            }
        }

        internal static bool AnyActiveMonitoring()
        {
            if (Plugin.instance.isOnMap || Plugin.instance.isOnCamera || Plugin.instance.isOnMiniMap || Plugin.instance.isOnMiniCams || Plugin.instance.isOnOverlay || Plugin.instance.activeCam)
                return true;
            else
                return false;
        }

    }

}
