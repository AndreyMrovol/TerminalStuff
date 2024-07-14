using System.Collections;
using UnityEngine;
using static TerminalStuff.EventSub.TerminalQuit;

namespace TerminalStuff
{
    internal class AlwaysOnStuff
    {
        internal static bool dynamicStatus = false;
        internal static IEnumerator AlwaysOnDynamic(Terminal instance)
        {
            if (dynamicStatus)
                yield break;

            Plugin.MoreLogs("Starting AlwaysOnDynamic Coroutine");
            dynamicStatus = true;

            while (instance.terminalUIScreen.gameObject != null && !MoreCommands.keepAlwaysOnDisabled)
            {
                if (!StartOfRound.Instance.localPlayerController.isInHangarShipRoom && instance.terminalUIScreen.gameObject.activeSelf)
                {
                    instance.terminalUIScreen.gameObject.SetActive(false);

                    if (ViewCommands.externalcamsmod && Plugin.instance.OpenBodyCamsMod && ViewCommands.AnyActiveMonitoring())
                        TerminalCameraStatus(false);

                    Plugin.Spam("Disabling terminal screen.");
                    if (ConfigSettings.TerminalLightBehaviour.Value == "alwayson")
                        TerminalEvents.ShouldDisableTerminalLight(true, "alwayson");
                }
                else if (StartOfRound.Instance.localPlayerController.isInHangarShipRoom && !instance.terminalUIScreen.gameObject.activeSelf)
                {
                    instance.terminalUIScreen.gameObject.SetActive(true);

                    if (ViewCommands.externalcamsmod && Plugin.instance.OpenBodyCamsMod && ViewCommands.AnyActiveMonitoring())
                        TerminalCameraStatus(true);

                    Plugin.Spam("Enabling terminal screen.");
                    if (ConfigSettings.TerminalLightBehaviour.Value == "alwayson")
                        TerminalEvents.ShouldDisableTerminalLight(false, "alwayson");
                }

                yield return new WaitForSeconds(0.5f);
            }

            if (DisableScreenOnDeath()) 
            {
                instance.terminalUIScreen.gameObject.SetActive(false);
                if (ViewCommands.externalcamsmod && Plugin.instance.OpenBodyCamsMod && ViewCommands.AnyActiveMonitoring())
                {
                    TerminalCameraStatus(false);
                    Plugin.Spam("Cams disabled on player death");
                }

                Plugin.Spam("Player detected dead, disabling terminal screen.");
                if (ConfigSettings.TerminalLightBehaviour.Value == "alwayson")
                    TerminalEvents.ShouldDisableTerminalLight(false, "alwayson");
            }

            dynamicStatus = false; //end of coroutine, opening this up again for another run
        }

        internal static bool DisableScreenOnDeath()
        {
            if (ConfigSettings.alwaysOnWhileDead.Value)
                return false;

            return StartOfRound.Instance.localPlayerController.isPlayerDead;
        }
    }
}
