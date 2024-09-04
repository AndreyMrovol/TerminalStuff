﻿using GameNetcodeStuff;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using static TerminalStuff.AllMyTerminalPatches;
using static OpenLib.Menus.MenuBuild;
using OpenLib.Menus;

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
        private static Texture radarTexture = null;
        internal static Texture camsTexture = null;
        internal static int cullingMaskInt;
        internal static int targetInt = 0;
        internal static float radarZoom;
        internal static RawImage miniScreenImage;

        internal static void InitializeTextures()
        {
            Plugin.Spam("Updating Radar");
            UpdateRadarTexture();

            Plugin.Spam("Updating Cams");
            if (IsExternalCamsPresent())
                GetPlayerCamsFromExternalMod();
            else if (Plugin.instance.TwoRadarMapsMod)
                TwoRadarMapsCompatibility.UpdateCamsTarget();
            else
                UpdateCamsTarget(StartOfRound.Instance.mapScreen.targetTransformIndex);

            Plugin.Spam("Textures Updated for both Cams/Radar");

            //radarTexture = GetTexture("Environment/HangarShip/ShipModels2b/MonitorWall/Cube.001", 1);
            //camsTexture = GetTexture("Environment/HangarShip/ShipModels2b/MonitorWall/Cube.001", 2);

        }

        private static Texture UpdateRadarTexture()
        {
            if (!Plugin.instance.TwoRadarMapsMod)
                radarTexture = StartOfRound.Instance.mapScreen.cam.targetTexture;
            else
                radarTexture = TwoRadarMapsCompatibility.RadarCamTexture();

            return radarTexture;
        }

        internal static void SetBodyCamTexture(RenderTexture texture)
        {
            camsTexture = texture;
        }

        internal static void InitializeTextures4Mirror(bool state)
        {
            if (Plugin.instance.OpenBodyCamsMod)
                OpenBodyCamsCompatibility.OpenBodyCamsMirrorStatus(state);
            else
            {
                 MirrorTexture(state);
            }

        }

        internal static void GetPlayerCamsFromExternalMod()
        {
            if (Plugin.instance.OpenBodyCamsMod)
            {
                Plugin.Spam("Sending to OBC for camera info");
                OpenBodyCamsCompatibility.UpdateCamsTarget();
            }
            else if (Plugin.instance.SolosBodyCamsMod || Plugin.instance.HelmetCamsMod)
            {
                Plugin.Spam("Grabbing monitor texture for other external bodycams mods");
                camsTexture = PlayerCamsCompatibility.PlayerCamTexture();
            }
            else
            {
                Plugin.Spam("No external mods detected, defaulting to internal cams system.");
                if (Plugin.instance.TwoRadarMapsMod)
                    TwoRadarMapsCompatibility.UpdateCamsTarget();
                else
                    UpdateCamsTarget(StartOfRound.Instance.mapScreen.targetTransformIndex);
            }
        }

        internal static void DetermineCamsTargets()
        {
            if (IsExternalCamsPresent())
            {
                externalcamsmod = true;
                Plugin.Log.LogInfo("External PlayerCams Mod Detected and will be used for all Cams Commands.");
            }

            else
                externalcamsmod = false;
        }

        internal static bool IsExternalCamsPresent()
        {
            if (ConfigSettings.camsUseDetectedMods.Value && (Plugin.instance.HelmetCamsMod || Plugin.instance.SolosBodyCamsMod || Plugin.instance.OpenBodyCamsMod))
                return true;
            else
                return false;
        }

        internal static void UpdateCamsTarget(int targetNum)
        {
            if (ConfigSettings.camsUseDetectedMods.Value && (Plugin.instance.HelmetCamsMod || Plugin.instance.OpenBodyCamsMod || Plugin.instance.SolosBodyCamsMod))
                return;

            if (!Plugin.instance.radarNonPlayer)
            {
                Plugin.Spam($"Using internal mod camera on valid player{targetNum}");
                camsTexture = PlayerCamTexture(targetNum);
            }
            else if (Plugin.instance.radarNonPlayer)
            {
                Plugin.Spam("Using internal mod camera on valid non-player");
                camsTexture = RadarCamTexture(targetNum);
            }
        }

        internal static string TermMapEvent()
        {
            string displayText;
            if (StartOfRound.Instance != null && StartOfRound.Instance.shipDoorsEnabled)
            {
                HandleMapEvent(out string message);
                displayText = message;
                return displayText;
            }
            else
            {
                HandleOrbitMapEvent(out string message);
                displayText = message;
                return displayText;
            }
        }

        private static void HandleMapEvent(out string displayText)
        {
            if (Plugin.instance.isOnMap == false)
            {
                InitializeTextures();
                SetTexturesAndVisibility(Plugin.instance.Terminal, radarTexture);
                SetRawImageTransparency(Plugin.instance.Terminal.terminalImage, 1f); // Full opacity for map

                // Set dimensions and position for radar image (rawImage2)
                SetRawImageDimensions(Plugin.instance.Terminal.terminalImage.rectTransform, isFullScreen:true);

                // Enable split view and update bools
                SplitViewChecks.EnableSplitView("map");

                DisplayTextUpdater(out string message);
                displayText = message;
                return;
            }
            else if (Plugin.instance.isOnMap)
            {
                SplitViewChecks.DisableSplitView("map");
                displayText = $"{ConfigSettings.mapString2.Value}\r\n";
                return;
            }
            else
            {
                Plugin.Log.LogError("Map command ERROR, isOnMap neither true nor false!!!");
                displayText = "Map Command ERROR, please report this to as a bug";
                return;
            }
        }

        private static void HandleOrbitMapEvent(out string displayText)
        {
            TerminalNode node = Plugin.instance.Terminal.currentNode;

            HideAllTextures(node);
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
            string displayText;
            isVideoPlaying = false;

            if (Plugin.instance.isOnMirror == false && Plugin.instance.splitViewCreated)
            {
                InitializeTextures4Mirror(true);

                SetTexturesAndVisibility(Plugin.instance.Terminal, camsTexture);
                SetRawImageTransparency(Plugin.instance.Terminal.terminalImage, 1f); // Full opacity for cams

                SetRawImageDimensions(Plugin.instance.Terminal.terminalImage.rectTransform, isFullScreen: true);

                // Enable split view and update bools
                SplitViewChecks.EnableSplitView("mirror");

                Plugin.MoreLogs("Mirror added to terminal screen");
                //DisplayTextUpdater(out string message);
                displayText = "\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\nMirror Camera added to terminal.\r\n";
                return displayText;
            }
            else if (Plugin.instance.isOnMirror)
            {
                InitializeTextures4Mirror(false);
                SplitViewChecks.DisableSplitView("mirror");
                displayText = $"\n\n\t>>Mirror Camera removed from terminal.\r\n\r\n";
                Plugin.MoreLogs("mirror removed");
                return displayText;
            }
            else
            {
                //Plugin.MoreLogs("Unable to run mirror event for some reason...");
                displayText = "Error with Mirror Event! Report this as a bug please.";
                Plugin.Log.LogError("Mirror Command ERROR");
                return displayText;
            }
        }

        internal static string TermCamsEvent()
        {
            isVideoPlaying = false;
            
            if(Plugin.instance.OpenBodyCamsMod && ConfigSettings.camsUseDetectedMods.Value )
            {
                if (!OpenBodyCamsCompatibility.BodyCamIsUnlocked() && ConfigSettings.obcRequireUpgrade.Value)
                    return "\tThis command is currently <color=#ff1a1a>unavailable</color>!\n\nPlease purchase the <color=#ffff66>BodyCam upgrade</color> to use this command.\r\n\r\n";
            }

            if (Plugin.instance.isOnCamera == false && Plugin.instance.splitViewCreated)
            {
                SetAnyCamsTrue();
                InitializeTextures();
                SetTexturesAndVisibility(Plugin.instance.Terminal, camsTexture);
                SetRawImageTransparency(Plugin.instance.Terminal.terminalImage, 1f); // Full opacity for cams

                SetRawImageDimensions(Plugin.instance.Terminal.terminalImage.rectTransform, isFullScreen: true);

                // Enable split view and update bools
                SplitViewChecks.EnableSplitView("cams");


                Plugin.MoreLogs("Cam added to terminal screen");
                DisplayTextUpdater(out string displayText);
                return displayText;
            }
            else if (Plugin.instance.isOnCamera)
            {
                SplitViewChecks.DisableSplitView("cams");
                string displayText = $"{ConfigSettings.camString2.Value}\r\n";
                Plugin.MoreLogs("Cams removed");
                return displayText;
            }
            else
            {
                Plugin.MoreLogs("Unable to run cameras event for some reason...");
                string displayText = "Error with Cams Event! Report this as a bug please.";
                Plugin.Log.LogError("Cams Command ERROR");
                return displayText;
            }
        }

        internal static string MiniCamsTermEvent()
        {
            isVideoPlaying = false;
            string displayText;

            if (Plugin.instance.OpenBodyCamsMod && ConfigSettings.camsUseDetectedMods.Value)
            {
                if (!OpenBodyCamsCompatibility.BodyCamIsUnlocked() && ConfigSettings.obcRequireUpgrade.Value)
                    return "\tThis command is currently <color=#ff1a1a>unavailable</color>!\n\nPlease purchase the <color=#ffff66>BodyCam upgrade</color> to use this command.\r\n\r\n";
            }

            // Extract player name from map screen
            string playerNameText = StartOfRound.Instance.mapScreenPlayerName.text;
            string removeText = "MONITORING: ";
            string playerName = playerNameText.Remove(0, removeText.Length);

            if (Plugin.instance.splitViewCreated && !Plugin.instance.isOnMiniCams)
            {
                SetAnyCamsTrue(); //needs to be set before initializing textures
                InitializeTextures();

                SetTexturesAndVisibility(Plugin.instance.Terminal, radarTexture, camsTexture);

                SetRawImageTransparency(miniScreenImage, 0.7f);

                SetRawImageDimensions(miniScreenImage.rectTransform, isFullScreen: false);
                SetRawImageDimensions(Plugin.instance.Terminal.terminalImage.rectTransform, isFullScreen: true);

                // Enable split view and update bools
                SplitViewChecks.EnableSplitView("minicams");

                // Display text based on whether playerName is empty or not
                displayText = $"\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\nMonitoring: {playerName} {ConfigSettings.mcString.Value}\r\n";
                return displayText;
            }
            else if (Plugin.instance.splitViewCreated && Plugin.instance.isOnMiniCams)
            {
                SplitViewChecks.DisableSplitView("minicams");
                displayText = $"{ConfigSettings.mcString2.Value}\r\n";
                return displayText;
            }
            else
            {
                Plugin.Log.LogError("Unexpected condition");
                displayText = "Unexpected error encountered!\r\n";
                return displayText;
            }
        }

        internal static string RadarZoomEvent()
        {
            string[] words = StringStuff.GetWords();

            if (!AnyActiveMonitoring() && Plugin.instance.splitViewCreated)
            {
                return $"No active monitoring detected, unable to change zoom.\r\n\r\n";
            }
            else if(!Plugin.instance.splitViewCreated && !(bool)Plugin.instance.Terminal.displayingPersistentImage)
            {
                return $"No active monitoring detected, unable to change zoom.\r\n\r\n";
            }
            else
            {
                if(words.Length < 2)
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

                    if (ConfigSettings.networkedNodes.Value)
                        NetHandler.Instance.SyncRadarZoomServerRpc(radarZoom);


                    return $"\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\nRadar Zoom level adjusted.\r\n";
                }
                else
                {
                    if (int.TryParse(words[1], out int newZoomVal))
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

                            if (ConfigSettings.networkedNodes.Value)
                                NetHandler.Instance.SyncRadarZoomServerRpc(radarZoom);

                            return $"\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\nRadar Zoom level adjusted to new value: {words[1]}\r\n";
                        }
                        else
                            return $"Cannot change zoom to value: {words[1]}.\nValue is too high or too low.\r\n\r\n";
                    }
                    else
                        return $"Cannot change zoom to invalid value: {words[1]}.";
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
            string displayText;

            if (Plugin.instance.OpenBodyCamsMod && ConfigSettings.camsUseDetectedMods.Value)
            {
                if (!OpenBodyCamsCompatibility.BodyCamIsUnlocked() && ConfigSettings.obcRequireUpgrade.Value)
                    return "\tThis command is currently <color=#ff1a1a>unavailable</color>!\n\nPlease purchase the <color=#ffff66>BodyCam upgrade</color> to use this command.\r\n\r\n";
            }

            string playerNameText = StartOfRound.Instance.mapScreenPlayerName.text;
            string removeText = "MONITORING: ";
            string playerName = playerNameText.Remove(0, removeText.Length);

            if (Plugin.instance.splitViewCreated && !Plugin.instance.isOnMiniMap)
            {
                SetAnyCamsTrue(); //needs to be set before initializing textures

                InitializeTextures();

                SetTexturesAndVisibility(Plugin.instance.Terminal, camsTexture, radarTexture);

                SetRawImageTransparency(miniScreenImage, 0.7f);
                SetRawImageDimensions(miniScreenImage.rectTransform, isFullScreen: false);
                SetRawImageDimensions(Plugin.instance.Terminal.terminalImage.rectTransform, isFullScreen: true);

                // Enable split view and update bools
                SplitViewChecks.EnableSplitView("minimap");

                // Display text based on whether playerName is empty or not
                displayText = $"\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\nMonitoring: {playerName} {ConfigSettings.mmString.Value}\r\n";
                return displayText;
            }
            else if (Plugin.instance.splitViewCreated && Plugin.instance.isOnMiniMap)
            {
                SplitViewChecks.DisableSplitView("minimap");
                displayText = $"{ConfigSettings.mmString2.Value}\r\n";
                return displayText;
            }
            else
            {
                Plugin.Log.LogError("Unexpected condition");
                displayText = "Unexpected error encountered!\r\n";
                return displayText;
            }
        }

        internal static string OverlayTermEvent()
        {
            isVideoPlaying = false;
            string displayText;

            if (Plugin.instance.OpenBodyCamsMod && ConfigSettings.camsUseDetectedMods.Value)
            {
                if (!OpenBodyCamsCompatibility.BodyCamIsUnlocked() && ConfigSettings.obcRequireUpgrade.Value)
                    return "\tThis command is currently <color=#ff1a1a>unavailable</color>!\n\nPlease purchase the <color=#ffff66>BodyCam upgrade</color> to use this command.\r\n\r\n";
            }

            string playerNameText = StartOfRound.Instance.mapScreenPlayerName.text;
            string removeText = "MONITORING: ";
            string playerName = playerNameText.Remove(0, removeText.Length);
            float opacityConfig = ConfigSettings.ovOpacity.Value / 100f;
            Plugin.MoreLogs($"Overlay Opacity: {opacityConfig}");

            if (Plugin.instance.splitViewCreated && !Plugin.instance.isOnOverlay)
            {
                SetAnyCamsTrue(); //needs to be set before initializing textures

                InitializeTextures();

                SetTexturesAndVisibility(Plugin.instance.Terminal, radarTexture, camsTexture);

                // Set transparency for rawImage1
                SetRawImageTransparency(miniScreenImage, opacityConfig);

                // Set dimensions and position for radar image (rawImage1)
                SetRawImageDimensions(miniScreenImage.rectTransform, isFullScreen: true);
                SetRawImageDimensions(Plugin.instance.Terminal.terminalImage.rectTransform, isFullScreen: true);

                // Enable split view and update bools
                SplitViewChecks.EnableSplitView("overlay");

                // Display text based on whether playerName is empty or not
                displayText = $"\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\nMonitoring: {playerName} {ConfigSettings.ovString.Value}\r\n";
                return displayText;
            }
            else if (Plugin.instance.splitViewCreated && Plugin.instance.isOnOverlay)
            {
                SplitViewChecks.DisableSplitView("overlay");
                displayText = $"{ConfigSettings.ovString2.Value}\r\n";
                return displayText;
            }
            else
            {
                Plugin.Log.LogError("Unexpected condition");
                displayText = "Unexpected error encountered!\r\n";
                return displayText;
            }
        }

        internal static void ResetPluginInstanceBools()
        {
            Plugin.instance.isOnMiniMap = false;
            Plugin.instance.isOnMiniCams = false;
            Plugin.instance.isOnMap = false;
            Plugin.instance.isOnCamera = false;
            Plugin.instance.isOnMirror = false;
            Plugin.instance.isOnOverlay = false;
            Plugin.instance.activeCam = false;
            NetHandler.SyncMyCamsBoolToEveryone(false);
        }

        internal static void SetAnyCamsTrue()
        {
            Plugin.instance.activeCam = true;
            NetHandler.SyncMyCamsBoolToEveryone(true);
        }

        private static void HideAllTextures(TerminalNode node)
        {
            node.displayTexture = null;
            SplitViewChecks.DisableSplitView("map");
        }

        internal static void PlayerCamSetup()
        {
            if (IsExternalCamsPresent())
                return;

            darmCamObject = new("darmuh's PlayerCam (Clone)");
            RenderTexture renderTexture = new(StartOfRound.Instance.localPlayerController.gameplayCamera.targetTexture);
            playerCam = darmCamObject.AddComponent<Camera>();
            playerCam.targetTexture = renderTexture;
            cullingMaskInt = StartOfRound.Instance.localPlayerController.gameplayCamera.cullingMask & ~LayerMask.GetMask(layerNames: ["Ignore Raycast", "UI", "HelmetVisor"]);
            cullingMaskInt |= (1 << 23);
            
            //Plugin.instance.Terminal.transform.gameObject.layer = 0;
            playerCam.cullingMask = cullingMaskInt;
            darmCamObject.SetActive(false);
            Plugin.MoreLogs("playerCam instantiated");
        }

        internal static void SetCameraState(bool active)
        {
            if (darmCamObject == null)
                return;

            if (active == true)
                darmCamObject.SetActive(active);
            else
                GameObject.Destroy(darmCamObject);
        }

        private static void MirrorTexture(bool state)
        {

            if (playerCam == null && state)
            {
                Plugin.MoreLogs("Creating home-brew PlayerCam");
                PlayerCamSetup();
            }

            SetCameraState(state);
            if (state)
            {
                MoreCamStuff.CamInitMirror(playerCam);

                playerCam.orthographic = true;
                playerCam.orthographicSize = ConfigSettings.mirrorZoom.Value;
                playerCam.usePhysicalProperties = false;
                playerCam.farClipPlane = 30f;
                playerCam.nearClipPlane = 0.05f;
                playerCam.fieldOfView = 130f;

                SetAnyCamsTrue();
                camsTexture = playerCam.targetTexture;
                return;
            }

        }

        private static Texture PlayerCamTexture(int targetPlayer)
        {
            if (playerCam == null)
            {
                Plugin.MoreLogs("Creating home-brew PlayerCam");
                PlayerCamSetup();
            }

            playerCam.orthographic = false;
            playerCam.enabled = true;
            playerCam.cameraType = CameraType.Game;
            
            Transform camTransform;
            PlayerControllerB targetedPlayer = StartOfRound.Instance.mapScreen.radarTargets[targetPlayer].transform.gameObject.GetComponent<PlayerControllerB>();
            if (targetedPlayer != null)
            {
                camTransform = targetedPlayer.gameplayCamera.transform;
                //targetedPlayer.thisPlayerModelArms.gameObject.layer = 23;
                Plugin.MoreLogs($"Valid player for cams update {targetedPlayer.playerUsername}");
            }
            else
            {
                camTransform = StartOfRound.Instance.mapScreen.radarTargets[targetPlayer].transform;
                Plugin.MoreLogs($"Invalid player{targetPlayer} for cams update, sending to backup trasnsform");
            }

            playerCam.transform.rotation = camTransform.rotation;
            playerCam.transform.position = camTransform.transform.position;

            playerCam.farClipPlane = 25f;
            playerCam.nearClipPlane = 0.5f;
            playerCam.fieldOfView = 90f;
            playerCam.transform.SetParent(camTransform.transform);
            Texture spectateTexture = playerCam.targetTexture;
            return spectateTexture;
        }

        private static Texture RadarCamTexture(int targetNum)
        {
            if (playerCam == null)
            {
                Plugin.MoreLogs("Creating home-brew PlayerCam");
                PlayerCamSetup();
            }

            playerCam.orthographic = false;
            playerCam.enabled = true;
            playerCam.cameraType = CameraType.SceneView;
            Transform camTransform = StartOfRound.Instance.mapScreen.radarTargets[targetNum].transform;
            playerCam.transform.rotation = camTransform.rotation;
            playerCam.transform.position = camTransform.transform.position;

            playerCam.farClipPlane = 50f;
            playerCam.nearClipPlane = 0.4f;
            playerCam.fieldOfView = 110f;
            playerCam.transform.SetParent(camTransform.transform);
            Texture spectateTexture = playerCam.targetTexture;
            return spectateTexture;
        }

        private static void SetTexturesAndVisibility(Terminal getTerm, Texture mainTexture)
        {
            getTerm.terminalImage.texture = mainTexture;
            miniScreenImage.enabled = false;
        }
        private static void SetTexturesAndVisibility(Terminal getTerm, Texture mainTexture, Texture smallTexture)
        {
            getTerm.terminalImage.texture = mainTexture;
            miniScreenImage.texture = smallTexture;
            miniScreenImage.enabled = true;
        }

        private static void SetRawImageTransparency(RawImage rawImage, float Opacity)
        {
            Color currentColor = rawImage.color;
            Color newColor = new(currentColor.r, currentColor.g, currentColor.b, Opacity); // 70% opacity
            rawImage.color = newColor;
        }

        private static void SetRawImageDimensions(RectTransform rectTrans, bool isFullScreen)
        {
            if (isFullScreen)
            {
                rectTrans.sizeDelta = new Vector2(425, 280);
                rectTrans.anchoredPosition = new Vector2(0, -25);
            }
            else
            {
                rectTrans.sizeDelta = new Vector2(180,100);
                rectTrans.anchoredPosition = new Vector2(123, 90);
            }
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

            foreach(TerminalMenuItem menuItem in menus)
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
                mode = ConfigSettings.camString.Value;
                Plugin.MoreLogs("cams mode detected");
                return;
            }
            else if (Plugin.instance.isOnMap)
            {
                mode = ConfigSettings.mapString.Value;
                Plugin.MoreLogs("map mode detected");
                return;
            }
            else if (Plugin.instance.isOnOverlay)
            {
                mode = ConfigSettings.ovString.Value;
                Plugin.MoreLogs("overlay mode detected");
                return;
            }
            else if (Plugin.instance.isOnMiniMap)
            {
                mode = ConfigSettings.mmString.Value;
                Plugin.MoreLogs("minimap mode detected");
                return;
            }
            else if (Plugin.instance.isOnMiniCams)
            {
                mode = ConfigSettings.mcString.Value;
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
                mode = ConfigSettings.defaultCamsView.Value;
                return;
            }
        }

        internal static void ReInitCurrentMode(Texture texture)
        {
            camsTexture = texture;

            if (Plugin.instance.isOnCamera)
            {
                SetTexturesAndVisibility(Plugin.instance.Terminal, camsTexture);
                Plugin.MoreLogs("cams mode detected, reinitializing textures");
                return;
            }
            else if (Plugin.instance.isOnMap)
            {
                SetTexturesAndVisibility(Plugin.instance.Terminal, radarTexture);
                Plugin.MoreLogs("map mode detected, reinitializing textures");
                return;
            }
            else if (Plugin.instance.isOnOverlay)
            {
                SetTexturesAndVisibility(Plugin.instance.Terminal, radarTexture, camsTexture);
                Plugin.MoreLogs("overlay mode detected, reinitializing textures");
                return;
            }
            else if (Plugin.instance.isOnMiniMap)
            {
                SetTexturesAndVisibility(Plugin.instance.Terminal, camsTexture, radarTexture);
                Plugin.MoreLogs("minimap mode detected");
                return;
            }
            else if (Plugin.instance.isOnMiniCams)
            {
                SetTexturesAndVisibility(Plugin.instance.Terminal, radarTexture, camsTexture);
                Plugin.MoreLogs("minicams mode detected");
                return;
            }
            else
            {
                Plugin.Log.LogError("Error with mode reinit, disabling any active cams");
                SplitViewChecks.DisableSplitView("neither");
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

        internal static int GetNextValidTarget(List<TransformAndName> targets, int initialIndex) //copied from TwoRadarMaps, slightly modified
        {
            int count = targets.Count;
            for (int i = 1; i < count; i++) //modified i to start at 1 to get next target rather than current target
            {
                int num = (initialIndex + i) % count;
                if (TargetIsValid(targets[num]?.transform))
                {
                    return num;
                }
            }

            return initialIndex; //changed this to return the original number if there are no other valid targets than the current one
        }

        internal static int GetPrevValidTarget(List<TransformAndName> targets, int initialIndex)
        {
            int count = targets.Count;
            Plugin.Spam($"Count:{targets.Count}");
            Plugin.Spam($"initialIndex: {initialIndex}");

            // Handle the case when initialIndex is zero
            if (initialIndex == 0)
            {
                // Set initialIndex to the last index
                initialIndex = count;
                Plugin.Spam($"initialIndex is 0, setting it to {initialIndex}");
            }

            // Iterate through the list of targets
            for (int i = 1; i < count; i++)
            {
                // Calculate the index of the previous target
                int num = (initialIndex - i) % count;

                Plugin.Spam($"{num} = {initialIndex} - {i} % {count}");
                Plugin.Spam($"{num} + {count} % {count}");
                // Ensure num is non-negative
                num = (num + count) % count;
                Plugin.Spam($"= {num}");
                // Check if the target at the calculated index is valid
                if (TargetIsValid(targets[num]?.transform))
                {
                    return num;
                }
            }

            // If no valid target is found, return the original index
            return initialIndex;
        }
        internal static bool TargetIsValid(Transform targetTransform) //copied from TwoRadarMaps, added log statements just to see how it works
        {
            if (targetTransform == null)
            {
                Plugin.MoreLogs("not a valid target");
                return false;
            }

            PlayerControllerB component = targetTransform.transform.GetComponent<PlayerControllerB>();
            if (component == null)
            {
                Plugin.MoreLogs("Null player component, must be radar (returning true)");
                return true;
            }

            if (!component.isPlayerControlled && !component.isPlayerDead)
            {
                Plugin.MoreLogs("player is not player controlled and is not dead, masked?");
                return component.redirectToEnemy != null;
            }

            Plugin.MoreLogs("returning true, no specific conditions met");
            return true;
        }

    }

}
