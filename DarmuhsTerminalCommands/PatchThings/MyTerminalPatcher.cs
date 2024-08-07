using HarmonyLib;
using TerminalStuff.SpecialStuff;
using UnityEngine;
using UnityEngine.Video;


namespace TerminalStuff
{
    public class AllMyTerminalPatches : MonoBehaviour
    {
        [HarmonyPatch(typeof(Terminal), "ParseWord")]
        public class ConflictResolution : Terminal
        {
            static void Postfix(string playerWord, ref TerminalKeyword __result)
            {
                if (!ConfigSettings.TerminalConflictResolution.Value)
                    return;

                ConflictRes.InitRes(playerWord, ref __result); //should modify the keyword to whatever resolution finds as the best match
            }
        }

        [HarmonyPatch(typeof(Terminal), "TextPostProcess")]
        public class ZeekersTypo : Terminal
        {
            static void Postfix(ref string __result)
            {
                if (__result.Contains("\n\n\n\n\n\n\n\n\n\n\n\n\n\nn\n\n\n\n\n\n"))
                {
                    __result = __result.Replace("\n\n\n\n\n\n\n\n\n\n\n\n\n\nn\n\n\n\n\n\n", "\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n");
                }
            }
        }

        [HarmonyPatch(typeof(Terminal), "LoadTerminalImage")]
        public class FixVideoPatch : Terminal
        {
            public static bool sanityCheckLOL = false;
            static void Postfix(TerminalNode node)
            {
                if (node.name == "darmuh's videoPlayer" && sanityCheckLOL)
                {
                    if (!ViewCommands.isVideoPlaying)
                    {
                        Plugin.instance.Terminal.videoPlayer.enabled = true;
                        Plugin.instance.Terminal.terminalImage.enabled = true;
                        Plugin.instance.Terminal.videoPlayer.loopPointReached += vp => OnVideoEnd(Plugin.instance.Terminal);

                        Plugin.instance.Terminal.videoPlayer.Play();
                        ViewCommands.isVideoPlaying = true;
                        Plugin.MoreLogs("isVideoPlaying set to TRUE");
                        sanityCheckLOL = false;
                        return;
                    }
                }
                else
                {
                    if (!Plugin.instance.splitViewCreated)
                        return;

                    Plugin.instance.Terminal.terminalImage.enabled = BoolStuff.ShouldEnableImage();
                    //full screen image should always be enabled for cam views
                    Plugin.Spam($"full screen image set to {BoolStuff.ShouldEnableImage()}");
                }

            }

            public static void OnVideoEnd(Terminal instance)
            {
                // This method will be called when the video is done playing
                // Disable the video player and terminal image here
                if (ViewCommands.isVideoPlaying)
                {
                    instance.videoPlayer.enabled = false;
                    instance.terminalImage.enabled = false;
                    ViewCommands.isVideoPlaying = false;
                    sanityCheckLOL = false;
                    Plugin.MoreLogs("isVideoPlaying set to FALSE");
                    instance.videoPlayer.audioOutputMode = VideoAudioOutputMode.None;
                    instance.videoPlayer.source = VideoSource.VideoClip;
                    instance.videoPlayer.aspectRatio = VideoAspectRatio.FitHorizontally;
                    instance.videoPlayer.isLooping = true;
                    instance.videoPlayer.playOnAwake = true;

                }
            }

        }
    }
}