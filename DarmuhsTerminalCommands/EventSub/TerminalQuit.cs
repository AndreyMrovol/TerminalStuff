using System.Collections;
using UnityEngine;
using static TerminalStuff.TerminalEvents;
using static TerminalStuff.AlwaysOnStuff;
using static TerminalStuff.EventSub.TerminalStart;
using OpenLib.Common;

namespace TerminalStuff.EventSub
{
    internal class TerminalQuit
    {
        internal static bool videoQuitEnum = false;
        internal static void OnTerminalQuit()
        {
            if(Plugin.instance.Terminal.currentNode != null && Plugin.instance.Terminal.currentNode.name != "terminalQuit")
            {
                if(Plugin.instance.suitsTerminal && SuitsTerminalCompatibility.CheckForSuitsMenu())
                {
                    Plugin.Spam("setting last node to help commands");
                    lastNode = Plugin.instance.Terminal.terminalNodes.specialNodes[13];
                }
                else
                {
                    lastNode = Plugin.instance.Terminal.currentNode;
                    Plugin.Spam("caching node from quit");
                }

                lastText = CommonStringStuff.GetCleanedScreenText(Plugin.instance.Terminal);
                
            }

            if (StartOfRound.Instance.localPlayerController != null)
                ShouldLockPlayerCamera(true, StartOfRound.Instance.localPlayerController);

            //Plugin.Log.LogInfo($"terminuse set to {__instance.terminalInUse}");
            if (alwaysOnDisplay)
            {
                HandleAlwaysOnQuit(Plugin.instance.Terminal);
            }
            else
            {
                HandleRegularQuit();
            }
        }

        internal static void TerminalCameraStatus(bool status)
        {
            if (status == false)
            {
                OpenBodyCamsCompatibility.TerminalMirrorStatus(status);
                OpenBodyCamsCompatibility.TerminalCameraStatus(status);
            }
            else if (Plugin.instance.isOnMirror)
            {
                OpenBodyCamsCompatibility.TerminalMirrorStatus(status);
            }
            else
                OpenBodyCamsCompatibility.TerminalCameraStatus(status);
        }

        private static void HandleRegularQuit()
        {
            if (ViewCommands.externalcamsmod && Plugin.instance.OpenBodyCamsMod && ViewCommands.AnyActiveMonitoring())
            {
                Plugin.MoreLogs("Leaving terminal and disabling any active monitoring");
                TerminalCameraStatus(false);
                SplitViewChecks.CheckForSplitView("neither");
            }
        }

        private static void HandleAlwaysOnQuit(Terminal instance)
        {
            instance.StartCoroutine(instance.waitUntilFrameEndToSetActive(active: true));
            Plugin.Spam("Screen set to active");
            if (ViewCommands.isVideoPlaying)
            {
                instance.videoPlayer.Pause();
                instance.StartCoroutine(WaitUntilFrameEndVideo(instance));
            }

            if (ConfigSettings.alwaysOnDynamic.Value)
                instance.StartCoroutine(AlwaysOnDynamic(instance));
        }

        private static IEnumerator WaitUntilFrameEndVideo(Terminal instance)
        {
            if (videoQuitEnum)
                yield break;

            videoQuitEnum = true;

            yield return new WaitForEndOfFrame();
            if (ViewCommands.isVideoPlaying)
                instance.videoPlayer.Play();
            Plugin.MoreLogs("attemtped to resume videoplayer");

            videoQuitEnum = false;
        }
    }
}
