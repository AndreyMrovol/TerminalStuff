﻿using OpenLib.Common;
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
            if (!alwaysOnDisplay)
            {
                HandleRegularQuit();
            }
        }

        internal static void OBCTerminalCameraStatus(bool status)
        {
            if (status == false)
            {
                if (Plugin.instance.suitsTerminal)
                {
                    if (SuitsTerminalCompatibility.CheckForSuitsMenu())
                        OpenLib.Compat.OpenBodyCamFuncs.TerminalCameraStatus(status);
                    else
                    {
                        OpenLib.Compat.OpenBodyCamFuncs.TerminalMirrorStatus(status);
                        OpenLib.Compat.OpenBodyCamFuncs.TerminalCameraStatus(status);
                    }
                }
                else
                {
                    OpenLib.Compat.OpenBodyCamFuncs.TerminalMirrorStatus(status);
                    OpenLib.Compat.OpenBodyCamFuncs.TerminalCameraStatus(status);
                }
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
            if (ViewCommands.AnyActiveMonitoring() || Plugin.instance.isOnMirror)
            {
                Plugin.MoreLogs("Leaving terminal and disabling any active cameras");
                SplitViewChecks.ShowCameraView(false);
            }
        }
    }
}
