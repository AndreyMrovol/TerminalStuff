using System.Collections;
using UnityEngine;

namespace TerminalStuff
{
    internal class AlwaysOnStuff
    {
        internal static bool dynamicStatus = false;
        internal static bool delayOff = false;
        internal static ScreenSettings screenSettings;

        internal static void IsPlayerDead()
        {
            if (StartOfRound.Instance.localPlayerController.isPlayerDead && DisableScreenOnDeath())
            {
                if (Plugin.instance.Terminal.terminalUIScreen.gameObject.activeSelf)
                    Plugin.instance.Terminal.terminalUIScreen.gameObject.SetActive(false);
            }
        }

        internal static void OnSpecateShipCheck()
        {
            if (DisableScreenOnDeath() || !screenSettings.Dynamic)
                return;

            Plugin.Spam($"Spectated Player detected in ship [ {StartOfRound.Instance.localPlayerController.spectatedPlayerScript.isInHangarShipRoom} ]");

            if (screenSettings.inUse && !Plugin.instance.Terminal.terminalUIScreen.gameObject.activeSelf)
            {
                if(!Plugin.instance.Terminal.terminalUIScreen.gameObject.activeSelf && StartOfRound.Instance.localPlayerController.spectatedPlayerScript.isInHangarShipRoom && Plugin.instance.Terminal.placeableObject.inUse)
                    Plugin.instance.Terminal.terminalUIScreen.gameObject.SetActive(true);
            }

            if (!Plugin.instance.Terminal.terminalUIScreen.gameObject.activeSelf && StartOfRound.Instance.localPlayerController.spectatedPlayerScript.isInHangarShipRoom)
                Plugin.instance.Terminal.terminalUIScreen.gameObject.SetActive(true);
            else if (Plugin.instance.Terminal.terminalUIScreen.gameObject.activeSelf && !StartOfRound.Instance.localPlayerController.spectatedPlayerScript.isInHangarShipRoom)
                Plugin.instance.Terminal.terminalUIScreen.gameObject.SetActive(false);
        }

        internal static void PlayerShipChanged()
        {
            if (StartOfRound.Instance.localPlayerController == null || screenSettings == null)
                return;

            Plugin.Spam($"Player detected in ship change - {StartOfRound.Instance.localPlayerController.isInHangarShipRoom}");

            if (StartOfRound.Instance.localPlayerController.isPlayerDead && DisableScreenOnDeath())
            {
                if (Plugin.instance.Terminal.terminalUIScreen.gameObject.activeSelf)
                    Plugin.instance.Terminal.terminalUIScreen.gameObject.SetActive(false);
            }

            if(StartOfRound.Instance.localPlayerController.isInHangarShipRoom)
            {
                if(screenSettings.Dynamic && !screenSettings.inUse)
                {
                    if (!Plugin.instance.Terminal.terminalUIScreen.gameObject.activeSelf)
                        Plugin.instance.Terminal.terminalUIScreen.gameObject.SetActive(true);
                }
                else if (screenSettings.inUse && OpenLib.TerminalUpdatePatch.inUse)
                {
                    if (!Plugin.instance.Terminal.terminalUIScreen.gameObject.activeSelf)
                        Plugin.instance.Terminal.terminalUIScreen.gameObject.SetActive(true);
                }
            }
            else
            {
                if (screenSettings.Dynamic)
                {
                    Plugin.Spam($"disabling screen - screenSetting Dynamic {screenSettings.Dynamic}");
                    if (Plugin.instance.Terminal.terminalUIScreen.gameObject.activeSelf)
                    {
                        if (ConfigSettings.ScreenOffDelay.Value < 1)
                            Plugin.instance.Terminal.terminalUIScreen.gameObject.SetActive(false);
                        else
                            Plugin.instance.StartCoroutine(DelayScreenOff(ConfigSettings.ScreenOffDelay.Value));
                    }

                }
            }
        }

        internal static IEnumerator DelayScreenOff(int delay)
        {
            if (delayOff)
                yield break;

            delayOff = true;
            yield return new WaitForSeconds(delay);
            if(!StartOfRound.Instance.localPlayerController.isInHangarShipRoom)
                Plugin.instance.Terminal.terminalUIScreen.gameObject.SetActive(false);
            delayOff = false;
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

        internal void Update(string setting)
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

        internal ScreenSettings(string setting)
        {
            this.Update(setting);
        }
    }
}
