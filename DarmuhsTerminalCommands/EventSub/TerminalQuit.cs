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
            if(ConfigSettings.CacheLastTerminalPage.Value && Plugin.instance.Terminal.currentNode != null && Plugin.instance.Terminal.currentNode.name != "terminalQuit")
            {

                lastText = CommonStringStuff.GetCleanedScreenText(Plugin.instance.Terminal);

                if (Plugin.instance.suitsTerminal && SuitsTerminalCompatibility.CheckForSuitsMenu())
                {
                    Plugin.Spam("setting last node text to empty");
                    justText.displayText = "";
                    lastNode = justText;
                }
                else if(Plugin.instance.Terminal.currentNode.playClip != null || Plugin.instance.Terminal.currentNode.playSyncedClip != -1)
                {
                    string cachedText = Plugin.instance.Terminal.screenText.text;
                    Plugin.Spam(cachedText);
                    cachedText = cachedText.TrimStart('\r', '\n');

                    if (cachedText.EndsWith(lastText))
                    {
                        cachedText = cachedText.Substring(0, cachedText.LastIndexOf(lastText));
                        cachedText += "\r\n";
                    }

                    justText.displayText = cachedText;
                    lastNode = justText;
                    Plugin.Spam("caching node from quit");
                }
                else
                    lastNode = Plugin.instance.Terminal.currentNode;
                
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
            if (screenSettings.Dynamic && !dynamicStatus)
                instance.StartCoroutine(AlwaysOnDynamic(instance));
        }
    }
}
