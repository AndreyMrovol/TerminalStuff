using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using static TerminalStuff.EventSub.TerminalQuit;
using static TerminalStuff.BoolStuff;
using OpenLib.CoreMethods;

namespace TerminalStuff
{
    internal class SplitViewChecks : MonoBehaviour
    {
        internal static bool enabledSplitObjects = false;
        internal static CompatibleNoun[] vanillaViewNouns;
        internal static TerminalKeyword viewKeyword;

        private static void HandleVanillaMap(bool shouldRemove)
        {
            if(shouldRemove)
            {
                if (DynamicBools.TryGetKeyword("view", out TerminalKeyword viewKW) && DynamicBools.TryGetKeyword("monitor", out TerminalKeyword monitorKW))
                {
                    Plugin.Spam("removing vanilla compat noun - monitor, from view");
                    viewKeyword = viewKW;
                    vanillaViewNouns = viewKW.compatibleNouns;
                    RemoveThings.RemoveCompatibleNoun(ref viewKW, monitorKW);
                }
            }
            else
            {
                if(vanillaViewNouns != null)
                {
                    Plugin.Spam("returning to vanilla compat nouns");
                    viewKeyword.compatibleNouns = vanillaViewNouns;
                }
            }
        }

        public static void InitSplitViewObjects()
        {
            if (!ShouldAddCamsLogic())
            {
                HandleVanillaMap(false);
                return;
            }
            else
                HandleVanillaMap(true);
                

            if (Plugin.instance.Terminal.terminalImage == null || Plugin.instance.splitViewCreated)
            {
                Plugin.MoreLogs("Original terminalImage not found or split view already created");
                return;
            }

            //ViewCommands.miniScreen = Instantiate(Plugin.instance.Terminal.terminalImage.gameObject, Plugin.instance.Terminal.terminalImage.gameObject.transform);
            //ViewCommands.miniScreen.name = "Terminal Small Screen (Clone)";
            //ViewCommands.miniScreenImage = ViewCommands.miniScreen.GetComponent<RawImage>();

            ViewCommands.miniScreenImage = Instantiate(Plugin.instance.Terminal.terminalImage, Plugin.instance.Terminal.terminalImage.transform);
            
            if(ViewCommands.miniScreenImage.gameObject.GetComponent<VideoPlayer>() != null)
            {
                VideoPlayer extraPlayer = ViewCommands.miniScreenImage.GetComponent<VideoPlayer>();
                Destroy(extraPlayer);
                Plugin.Spam("extraPlayer deleted");
            }

            ViewCommands.miniScreenImage.gameObject.name = "MiniScreen";
            Plugin.instance.Terminal.terminalImage.gameObject.name = "terminalImage";

            Plugin.instance.splitViewCreated = true;
        }

        public static void CheckForSplitView(string whatIsIt)
        {
            if (!Plugin.instance.splitViewCreated && whatIsIt == "neither")
            {
                DisableVanillaViewMonitor();
                return;
            }
            else if (!Plugin.instance.splitViewCreated)
                return;
                
            List<string> singleViewModes = ["cams", "map", "mirror"];

            if (enabledSplitObjects == false)
            {
                SetMiniScreen(whatIsIt, false);
                ResetPluginInstanceBools();
            }
            else if (whatIsIt == "neither")
            {
                enabledSplitObjects = false;
                //DisableVanillaViewMonitor();
                SetMiniScreen(whatIsIt, false);
                ResetPluginInstanceBools();
            }
            else if (enabledSplitObjects == true && (!singleViewModes.Contains(whatIsIt)))
            {
                //DisableVanillaViewMonitor(disableImage: false);
                SetMiniScreen(whatIsIt, true);
                ViewCommands.SetAnyCamsTrue();
                UpdatePluginInstanceBools(whatIsIt);
            }
            else if (enabledSplitObjects == true && (singleViewModes.Contains(whatIsIt)))
            {
                //DisableVanillaViewMonitor(disableImage: false);
                SetMiniScreen(whatIsIt, false);
                UpdatePluginInstanceBools(whatIsIt);
            }
            else
            {
                Plugin.MoreLogs("No matches for split view objects");
            }
        }

        private static void SetMiniScreen(string whatIsIt, bool state)
        {
            //Plugin.instance.Terminal.terminalImage.enabled = state;
            ViewCommands.miniScreenImage.enabled = state;
            Plugin.MoreLogs($"{whatIsIt} has set miniscreen to {state}");
            //ViewCommands.miniScreen.gameObject.SetActive(state);
        }

        internal static void ResetPluginInstanceBools()
        {
            ViewCommands.ResetPluginInstanceBools();
        }

        internal static void EnableSplitView(string whatIsIt)
        {
            enabledSplitObjects = true;
            CheckForSplitView(whatIsIt);
            UpdatePluginInstanceBools(whatIsIt);
            ShowCameraView(true);
        }

        internal static void DisableVanillaViewMonitor(bool disableImage = true)
        {
            Plugin.Spam("disabling vanilla view monitor");
            Plugin.instance.Terminal.displayingPersistentImage = null;
            if(disableImage) 
                Plugin.instance.Terminal.terminalImage.enabled = false;
        }

        internal static void DisableSplitView(string whatIsIt)
        {
            if (!Plugin.instance.splitViewCreated && whatIsIt == "neither")
            {
                DisableVanillaViewMonitor();
                return;
            }
            else if (!Plugin.instance.splitViewCreated)
                return;

            enabledSplitObjects = false;
            CheckForSplitView(whatIsIt);
            ShowCameraView(false);
        }

        private static void ShowCameraView(bool state)
        {
            if (ConfigSettings.camsUseDetectedMods.Value && Plugin.instance.OpenBodyCamsMod)
            {
                TerminalCameraStatus(state);
            }
            else
                ViewCommands.SetCameraState(state);
        }

        private static void UpdatePluginInstanceBools(string whatIsIt)
        {
            ResetPluginInstanceBools();

            switch (whatIsIt)
            {
                case "minimap":
                    Plugin.instance.isOnMiniMap = true;
                    break;
                case "minicams":
                    Plugin.instance.isOnMiniCams = true;
                    break;
                case "overlay":
                    Plugin.instance.isOnOverlay = true;
                    break;
                case "cams":
                    Plugin.instance.isOnCamera = true;
                    break;
                case "mirror":
                    Plugin.instance.isOnMirror = true;
                    break;
                case "map":
                    Plugin.instance.isOnMap = true;
                    break;
                default:
                    Plugin.MoreLogs($"Unexpected value for whatIsIt: {whatIsIt}");
                    break;
            }
        }
    }
}
