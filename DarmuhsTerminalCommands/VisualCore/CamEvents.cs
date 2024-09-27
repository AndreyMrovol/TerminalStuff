using UnityEngine;
using UnityEngine.UI;
using static TerminalStuff.MoreCamStuff;
using static TerminalStuff.ViewCommands;

namespace TerminalStuff.VisualCore
{
    public class CamEvents
    {
        //public delegate void UpdateTexture(CamsClass cams, Texture texture);
        //public delegate void UpdateStyle(CamsClass cams, Texture mainTexture, Texture smallTexture = null);
        internal static CamsClass CamsThings = new();
        public static OpenLib.Events.Events.CustomEvent UpdateTextures = new();
        public static OpenLib.Events.Events.CustomEvent<string> UpdateCamsEvent = new();

        internal static void SetTextures(Texture texture, Texture mini = null)
        {
            Plugin.instance.Terminal.terminalImage.texture = texture;

            if (mini != null)
                SplitViewChecks.miniScreenImage.texture = mini;
        }

        internal static void GetTextures()
        {
            CamsThings.camsTexture = UpdateCamsTexture();
            CamsThings.radarTexture = UpdateRadarTexture();
            UpdateCamsEvent.Invoke(CamsThings.Mode);
        }

        internal static void UpdateStyle(Texture main, float mainOpacity, Texture mini = null, float miniOpacity = 0f, bool isOverlay = false)
        {
            if (mini == null)
            {
                SetTextures(main);
                SetRawImageTransparency(Plugin.instance.Terminal.terminalImage, mainOpacity); // Full mainOpacity for map
                SetRawImageDimensions(Plugin.instance.Terminal.terminalImage.rectTransform, isFullScreen: true);
            }
            else
            {
                SetTextures(main, mini);
                SetRawImageTransparency(Plugin.instance.Terminal.terminalImage, mainOpacity);
                SetRawImageTransparency(SplitViewChecks.miniScreenImage, miniOpacity);

                SetRawImageDimensions(SplitViewChecks.miniScreenImage.rectTransform, isFullScreen: isOverlay);
                SetRawImageDimensions(Plugin.instance.Terminal.terminalImage.rectTransform, isFullScreen: true);
            }
        }

        internal static void OnUpdateCamsEvent(string mode)
        {
            CamsThings.Mode = mode;
            if (mode == "map")
            {
                CamsThings.radarTexture = UpdateRadarTexture();
                UpdateStyle(CamsThings.radarTexture, 1f);
                // Enable split view and update bools
                SplitViewChecks.EnableSplitView("map");
            }
            else if (mode == "cams")
            {
                SetAnyCamsTrue();
                CamsThings.camsTexture = UpdateCamsTexture();
                UpdateStyle(CamsThings.camsTexture, 1f);
                // Enable split view and update bools
            }
            else if (mode == "minicams")
            {

                SetAnyCamsTrue(); //needs to be set before initializing textures
                CamsThings.radarTexture = UpdateRadarTexture();
                CamsThings.camsTexture = UpdateCamsTexture();
                UpdateStyle(CamsThings.radarTexture, 1f, CamsThings.camsTexture, 0.7f);

                // Enable split view and update bools
                SplitViewChecks.EnableSplitView("minicams");
            }
            else if (mode == "minimap")
            {
                SetAnyCamsTrue(); //needs to be set before initializing textures
                CamsThings.radarTexture = UpdateRadarTexture();
                CamsThings.camsTexture = UpdateCamsTexture();
                UpdateStyle(CamsThings.camsTexture, 1f, CamsThings.radarTexture, 0.7f);

                // Enable split view and update bools
                SplitViewChecks.EnableSplitView("minimap");
            }
            else if (mode == "overlay")
            {
                SetAnyCamsTrue(); //needs to be set before initializing textures
                CamsThings.radarTexture = UpdateRadarTexture();
                CamsThings.camsTexture = UpdateCamsTexture();
                UpdateStyle(CamsThings.radarTexture, 1f, CamsThings.camsTexture, ConfigSettings.OverlayOpacity.Value / 100f, true);

                // Enable split view and update bools
                SplitViewChecks.EnableSplitView("overlay");
            }
            else if (mode == "mirror")
            {
                SetAnyCamsTrue();
                UpdateStyle(GetMirrorTexture(), 1f);

                // Enable split view and update bools
                SplitViewChecks.EnableSplitView("mirror");
            }
            else
            {
                Plugin.WARNING($"Unexpected mode - [ {mode} ] OnUpdateCamsEvent");
            }
        }

        private static Texture UpdateRadarTexture()
        {
            Texture texture;
            if (!Plugin.instance.TwoRadarMapsMod)
                texture = StartOfRound.Instance.mapScreen.cam.targetTexture;
            else
                texture = TwoRadarMapsCompatibility.RadarCamTexture();

            return texture;
        }

        private static Texture UpdateCamsTexture()
        {
            Plugin.Spam("Updating Cams");
            if (IsExternalCamsPresent())
                return GetPlayerCamsFromExternalMod();
            else if (Plugin.instance.TwoRadarMapsMod)
                return TwoRadarMapsCompatibility.UpdateCamsTarget();
            else
                return UpdateCamsTarget(StartOfRound.Instance.mapScreen.targetTransformIndex);

            //radarTexture = GetTexture("Environment/HangarShip/ShipModels2b/MonitorWall/Cube.001", 1);
            //camsTexture = GetTexture("Environment/HangarShip/ShipModels2b/MonitorWall/Cube.001", 2);
        }

        private static void SetRawImageTransparency(RawImage rawImage, float Opacity)
        {
            Color currentColor = rawImage.color;
            Color newColor = new(currentColor.r, currentColor.g, currentColor.b, Opacity); // 70% mainOpacity
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
                rectTrans.sizeDelta = new Vector2(180, 100);
                rectTrans.anchoredPosition = new Vector2(123, 90);
            }
        }

        internal static Texture GetMirrorTexture()
        {
            if (Plugin.instance.OpenBodyCamsMod && ConfigSettings.CamsUseDetectedMods.Value)
            {
                Plugin.Spam("Sending to OBC for camera info");
                OpenLib.Compat.OpenBodyCamFuncs.OpenBodyCamsMirrorStatus(true, ConfigSettings.ObcResolutionMirror.Value, ConfigSettings.MirrorZoom.Value, ConfigSettings.Mirror2DStyle.Value);
                return OpenLib.Compat.OpenBodyCamFuncs.GetTexture(OpenLib.Compat.OpenBodyCamFuncs.TerminalMirrorCam);
            }
            else
                return HomebrewMirror();

        }

        //Homebrew Mirror
        private static Texture HomebrewMirror()
        {
            if (playerCam == null)
            {
                Plugin.MoreLogs("Creating home-brew PlayerCam");
                PlayerCamSetup();
            }

            MoreCamStuff.CamInitMirror(playerCam);

            playerCam.orthographic = true;
            playerCam.orthographicSize = ConfigSettings.MirrorZoom.Value;
            playerCam.usePhysicalProperties = false;
            playerCam.farClipPlane = 30f;
            playerCam.nearClipPlane = 0.05f;
            playerCam.fieldOfView = 130f;

            SetAnyCamsTrue();
            return playerCam.targetTexture;
        }


        //States
        internal static void HomebrewCameraState(bool active)
        {
            if (darmCamObject == null)
                return;

            if (active == true)
                darmCamObject.SetActive(active);
            else
                GameObject.Destroy(darmCamObject);
        }

        internal static void SetMirrorState(bool active)
        {
            if (Plugin.instance.OpenBodyCamsMod && ConfigSettings.CamsUseDetectedMods.Value)
            {
                OpenLib.Compat.OpenBodyCamFuncs.OpenBodyCamsMirrorStatus(false, ConfigSettings.ObcResolutionMirror.Value, ConfigSettings.MirrorZoom.Value, ConfigSettings.Mirror2DStyle.Value);
            }
            else
                HomebrewCameraState(active);
        }
    }
}
