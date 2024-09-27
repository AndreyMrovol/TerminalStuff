using OpenLib.Common;
using static TerminalStuff.AlwaysOnStuff;
using static TerminalStuff.EventSub.TerminalStart;
using static TerminalStuff.TerminalEvents;

namespace TerminalStuff.EventSub
{
    internal class TerminalQuit
    {
        internal static bool videoQuitEnum = false;
        internal static void OnTerminalQuit()
        {
            if (ConfigSettings.SaveLastInput.Value && Plugin.instance.Terminal.currentNode != null && Plugin.instance.Terminal.currentNode.name != "TerminalQuit")
            {
                lastText = CommonStringStuff.GetCleanedScreenText(Plugin.instance.Terminal);
                Plugin.Spam("grabbed lastText");
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
                OpenLib.Compat.OpenBodyCamFuncs.TerminalMirrorStatus(status);
                OpenLib.Compat.OpenBodyCamFuncs.TerminalCameraStatus(status);
            }
            else if (Plugin.instance.isOnMirror)
            {
                OpenLib.Compat.OpenBodyCamFuncs.TerminalMirrorStatus(status);
            }
            else
                OpenLib.Compat.OpenBodyCamFuncs.TerminalCameraStatus(status);
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
