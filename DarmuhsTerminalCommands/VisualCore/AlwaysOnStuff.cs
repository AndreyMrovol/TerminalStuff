﻿using System.Collections;
using UnityEngine;
using static TerminalStuff.EventSub.TerminalQuit;

namespace TerminalStuff
{
    internal class AlwaysOnStuff
    {
        internal static bool dynamicStatus = false;
        internal static ScreenSettings screenSettings;
        internal static IEnumerator AlwaysOnDynamic(Terminal instance)
        {
            if (dynamicStatus)
            {
                Plugin.Log.LogInfo("AlwaysOnDynamic coroutine called again while it's still active");
                yield break;
            }

            Plugin.MoreLogs("Starting AlwaysOnDynamic Coroutine");
            dynamicStatus = true;

            while (instance.terminalUIScreen.gameObject != null && !MoreCommands.keepAlwaysOnDisabled && !DisableScreenOnDeath())
            {
                //Debug Logs, dont leave in release version
                //Plugin.Spam($"DEBUG LOGS\nisInHangarShipRoom: {StartOfRound.Instance.localPlayerController.isInHangarShipRoom}\nscreenActive: {instance.terminalUIScreen.gameObject.activeSelf}\nscreenSettings.inUse {screenSettings.inUse}\n ");


                //player not in ship & screen is active
                if (!StartOfRound.Instance.localPlayerController.isInHangarShipRoom && instance.terminalUIScreen.gameObject.activeSelf)
                {
                    if (ConfigSettings.ScreenOffDelay.Value >= 0)
                        yield return new WaitForSeconds(ConfigSettings.ScreenOffDelay.Value);

                    if (StartOfRound.Instance.localPlayerController.isInHangarShipRoom && instance.terminalUIScreen.gameObject.activeSelf && !screenSettings.inUse)
                        continue;

                    instance.terminalUIScreen.gameObject.SetActive(false);

                    if (ViewCommands.externalcamsmod && Plugin.instance.OpenBodyCamsMod && ViewCommands.AnyActiveMonitoring())
                        TerminalCameraStatus(false);

                    Plugin.Log.LogInfo("Disabling terminal screen.");
                    if (ConfigSettings.TerminalLightBehaviour.Value == "alwayson")
                        TerminalEvents.ShouldDisableTerminalLight(true, "alwayson");
                }
                //player is in ship and screen is inactive
                else if (StartOfRound.Instance.localPlayerController.isInHangarShipRoom && !instance.terminalUIScreen.gameObject.activeSelf)
                {
                    //inuse setting check
                    if (!screenSettings.inUse || instance.terminalInUse)
                        instance.terminalUIScreen.gameObject.SetActive(true);
                    else if (screenSettings.inUse && instance.placeableObject.inUse)
                        instance.terminalUIScreen.gameObject.SetActive(true);
                    else
                    {
                        yield return new WaitForSeconds(0.1f);
                        continue;
                    }

                    if (ViewCommands.externalcamsmod && Plugin.instance.OpenBodyCamsMod && ViewCommands.AnyActiveMonitoring())
                        TerminalCameraStatus(true);

                    Plugin.Log.LogInfo("Enabling terminal screen.");
                    if (ConfigSettings.TerminalLightBehaviour.Value == "alwayson")
                        TerminalEvents.ShouldDisableTerminalLight(false, "alwayson");
                }
                //player is in ship, screen is active, and screenSettings is set to inUse
                else if (StartOfRound.Instance.localPlayerController.isInHangarShipRoom && instance.terminalUIScreen.gameObject.activeSelf && screenSettings.inUse)
                {
                    if (!instance.placeableObject.inUse)
                    {
                        instance.terminalUIScreen.gameObject.SetActive(false);
                    }
                    else
                        yield return new WaitForSeconds(0.1f);

                    continue;
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
                    TerminalEvents.ShouldDisableTerminalLight(true, "alwayson");
            }

            dynamicStatus = false; //end of coroutine, opening this up again for another run
        }

        internal static bool DisableScreenOnDeath()
        {
            if (ConfigSettings.ScreenOnWhileDead.Value)
                return false;

            return StartOfRound.Instance.localPlayerController.isPlayerDead;
        }
    }

    internal class ScreenSettings
    {
        //"nochange", "alwayson", "inship", "inuse"
        internal bool AlwaysOn;
        internal bool Dynamic;
        internal bool inUse;

        internal ScreenSettings(string setting)
        {
            if (setting.ToLower() == "alwayson")
            {
                this.AlwaysOn = true;
                this.Dynamic = false;
                this.inUse = false;
            }
            else if (setting.ToLower() == "inship")
            {
                this.AlwaysOn = true;
                this.Dynamic = true;
                this.inUse = false;
            }
            else if (setting.ToLower() == "inuse")
            {
                this.AlwaysOn = true;
                this.Dynamic = true;
                this.inUse = true;
            }
            else
            {
                this.AlwaysOn = false;
                this.Dynamic = false;
                this.inUse = false;
            }

            Plugin.Spam($"ScreenSettings set to: {setting}");
        }
    }
}
