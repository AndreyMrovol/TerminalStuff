using OpenLib.Common;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using static TerminalStuff.MoreCamStuff;
using static TerminalStuff.ViewCommands;
using static TwoRadarMaps.Plugin;

namespace TerminalStuff
{
    internal class TwoRadarMapsCompatibility
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static Texture RadarCamTexture()
        {
            Plugin.MoreLogs("Getting Radar texture for Zaggy's Unique Radar from TwoRadarMaps");
            radarZoom = TerminalMapRenderer.cam.orthographicSize;
            Texture ZaggyRadarTexture = TerminalMapRenderer.cam.targetTexture;
            return ZaggyRadarTexture;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static String TargetedPlayerOnSecondRadar()
        {
            Plugin.MoreLogs($"Getting playername from TerminalMapScreenPlayerName.text which is {TerminalMapScreenPlayerName.text}");
            string playerName = TerminalMapScreenPlayerName.text.Replace("MONITORING: ", "");
            return playerName;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static string TeleportCompatibility()
        {
            TeleportTarget(TerminalMapRenderer.targetTransformIndex);
            Plugin.MoreLogs("Valid player attached to tworadarmaps, teleporting");
            string displayText = $"{ConfigSettings.TpMessageString.Value} (Targeted Player: {TerminalMapRenderer.radarTargets[TerminalMapRenderer.targetTransformIndex].name})";
            return displayText;

        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void UpdateTerminalRadarTarget(Terminal terminal, int playerNum = -1)
        {
            if (playerNum == -1)
            {
                int next = GetNextValidTarget(TerminalMapRenderer.radarTargets, TerminalMapRenderer.targetTransformIndex);
                StartTargetTransition(TerminalMapRenderer, next);
                NetCheck(next);
                Plugin.Spam("Setting to next player");
                StartofHandling.DelayedUpdateText(terminal);
            }
            else if (playerNum == -2)
            {
                int prev = GetPrevValidTarget(TerminalMapRenderer.radarTargets, TerminalMapRenderer.targetTransformIndex);
                StartTargetTransition(TerminalMapRenderer, prev);
                NetCheck(prev);
                Plugin.Spam("Setting to prev player");
                StartofHandling.DelayedUpdateText(terminal);
            }
            else
            {
                StartTargetTransition(TerminalMapRenderer, playerNum);
                NetCheck(playerNum);
                Plugin.Spam("Setting to specific player");
                StartofHandling.DelayedUpdateText(terminal);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ChangeMapZoom(float newOrtho)
        {
            if (TerminalMapRenderer.cam.orthographicSize == newOrtho)
                return;

            TerminalMapRenderer.cam.orthographicSize = newOrtho;
        }

        internal static void NetCheck(int playerNum)
        {
            if (!ConfigSettings.NetworkedNodes.Value)
                return;

            NetHandler.Instance.SyncTwoRadarMapsServerRpc(((int)StartOfRound.Instance.localPlayerController.playerClientId), playerNum);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void SyncTarget(int playerNum)
        {
            StartTargetTransition(TerminalMapRenderer, playerNum);
            Plugin.MoreLogs("Syncing radar to specific player");
        }

        internal static Texture UpdateCamsTarget()
        {
            if (Plugin.instance.activeCam && !StartOfRound.Instance.mapScreen.radarTargets[StartOfRound.Instance.mapScreen.targetTransformIndex].isNonPlayer)
            {
                Plugin.MoreLogs("Using internal mod camera on valid player");
                return PlayerCamTexture();
            }
            else
            {
                Plugin.MoreLogs("Using internal mod camera on valid non-player");
                return NonPlayerCamTexture();
            }
        }

        private static Texture PlayerCamTexture()
        {
            if (playerCam == null)
            {
                Plugin.MoreLogs("Creating home-brew PlayerCam");
                playerCam = CamStuff.HomebrewCam(ref mycamTexture, ref CamStuff.MyCameraHolder);
            }

            playerCam.orthographic = false;
            playerCam.enabled = true;
            playerCam.cameraType = CameraType.SceneView;
            Transform camTransform;
            if (TerminalMapRenderer.targetedPlayer != null)
            {
                camTransform = TerminalMapRenderer.targetedPlayer.gameplayCamera.transform;
                Plugin.MoreLogs("Valid player for cams update");
            }
            else
            {
                camTransform = TerminalMapRenderer.radarTargets[TerminalMapRenderer.targetTransformIndex].transform;
                Plugin.MoreLogs("Invalid player for cams update, sending to backup");
            }

            playerCam.transform.rotation = camTransform.rotation;
            playerCam.transform.position = camTransform.transform.position;
            playerCam.usePhysicalProperties = false;

            playerCam.farClipPlane = 25f;
            playerCam.nearClipPlane = 0.4f;
            playerCam.fieldOfView = 90f;
            playerCam.transform.SetParent(camTransform.transform);
            Texture spectateTexture = playerCam.targetTexture;
            return spectateTexture;
        }

        private static Texture NonPlayerCamTexture()
        {
            if (playerCam == null)
            {
                Plugin.MoreLogs("Creating home-brew PlayerCam");
                playerCam = CamStuff.HomebrewCam(ref mycamTexture, ref CamStuff.MyCameraHolder);
            }

            playerCam.orthographic = false;
            playerCam.enabled = true;
            playerCam.cameraType = CameraType.SceneView;
            Transform camTransform = TerminalMapRenderer.radarTargets[TerminalMapRenderer.targetTransformIndex].transform;
            playerCam.transform.rotation = camTransform.rotation;
            playerCam.transform.position = camTransform.transform.position;

            playerCam.usePhysicalProperties = true;
            playerCam.farClipPlane = 50f;
            playerCam.nearClipPlane = 0.4f;
            playerCam.fieldOfView = 110f;
            playerCam.transform.SetParent(camTransform.transform);
            Texture spectateTexture = playerCam.targetTexture;
            return spectateTexture;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static int CheckForPlayerNameCommand(string query) 
        {
            return TerminalEvents.PlayerNameToTarget(query, TerminalMapRenderer.radarTargets);
        }


        //copied the below methods as they are not available to be referenced from external sources
        //There is a public method that uses the below methods but it will update BOTH the real radar and the terminal radar
        //I needed to use a method that will only update the terminalmap

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void StartTargetTransition(ManualCameraRenderer mapRenderer, int targetIndex) //copied from TwoRadarMaps, no changes
        {
            if (mapRenderer.updateMapCameraCoroutine != null)
            {
                mapRenderer.StopCoroutine(mapRenderer.updateMapCameraCoroutine);
            }

            if (mapRenderer.radarTargets[targetIndex].isNonPlayer)
                Plugin.instance.radarNonPlayer = true;
            else
                Plugin.instance.radarNonPlayer = false;

            mapRenderer.updateMapCameraCoroutine = mapRenderer.StartCoroutine(mapRenderer.updateMapTarget(targetIndex));
        }

        //moved GetNextValidTarget and TargetIsValid to ViewCommands
        //They are useful for more than just TwoRadarsCompatibility and will be used for other functions

    }
}
