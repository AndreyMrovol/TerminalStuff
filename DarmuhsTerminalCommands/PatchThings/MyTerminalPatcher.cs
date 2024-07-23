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
            static void Postfix(ref TerminalKeyword __result)
            {
                if (!ConfigSettings.TerminalConflictResolution.Value)
                    return;

                ConflictRes.InitRes(ref __result); //should modify the keyword to whatever resolution finds as the best match
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
                    Plugin.Spam("testing patch");
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