using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;
using static TerminalStuff.MoreCamStuff;


namespace TerminalStuff
{

    [HarmonyPatch(typeof(ManualCameraRenderer), "updateMapTarget")]
    public class SwitchRadarPatch
    {

        public static void Postfix(int setRadarTargetIndex)
        {
            if (StartOfRound.Instance.mapScreen == null || StartOfRound.Instance.mapScreen.radarTargets == null || StartOfRound.Instance.mapScreen.radarTargets[setRadarTargetIndex] == null)
            {
                Plugin.ERROR("ERROR: Postfix failed, StartOfRound.Instance.mapScreen has null variables");
                return;
            }

            if (Plugin.instance.TwoRadarMapsMod || Plugin.instance.isOnMirror)
                return;

            Plugin.instance.radarNonPlayer = StartOfRound.Instance.mapScreen.radarTargets[setRadarTargetIndex].isNonPlayer;

            if (!IsExternalCamsPresent() && ViewCommands.AnyActiveMonitoring())
            {
                ViewCommands.targetInt = setRadarTargetIndex;
                Plugin.MoreLogs("Updating homebrew target");
                SwitchedRadarEvent();
            }
            else if (IsExternalCamsPresent() && ViewCommands.AnyActiveMonitoring())
            {
                if (Plugin.instance.OpenBodyCamsMod && !OpenLib.Compat.OpenBodyCamFuncs.ShowingBodyCam)
                    Plugin.MoreLogs("OBC Terminal Body Cam is NOT active");
                else
                    GetPlayerCamsFromExternalMod();
            }

            UpdateDisplayText();
        }

        internal static void UpdateDisplayText()
        {
            Terminal getTerm = Plugin.instance.Terminal;

            if (!Plugin.instance.radarNonPlayer && StartOfRound.Instance.mapScreen.targetedPlayer == null)
                return;
            if (!Plugin.instance.activeCam || !ViewCommands.AnyActiveMonitoring())
                return;
            if (Plugin.instance.isOnMirror)
                return;

            if (getTerm != null && getTerm.currentNode != null)
            {
                ViewCommands.DisplayTextUpdater(out string displayText);
                getTerm.currentNode.displayText = displayText;
            }
        }

        private static void SwitchedRadarEvent()
        {
            //Plugin.MoreLogs($"startround: {StartOfRound.Instance.mapScreen.targetTransformIndex}\n--------------\nviewcommands {ViewCommands.targetInt}\n-------------");
            if (ViewCommands.AnyActiveMonitoring() && !ViewCommands.externalcamsmod)
            {
                Plugin.MoreLogs($"targetNum = {ViewCommands.targetInt}");
                UpdateCamsTarget(ViewCommands.targetInt);
                return;
            }

        }
    }

    [HarmonyPatch(typeof(TimeOfDay), "Awake")]
    public class TimeAwakePatch
    {
        public static void Postfix()
        {
            Plugin.MoreLogs("TimeAwakePatch");
            StartCreds();
        }

        private static void StartCreds()
        {
            if (TimeOfDay.Instance.quotaVariables != null && ConfigSettings.StartingCreds.Value > -1)
            {
                TimeOfDay.Instance.quotaVariables.startingCredits = ConfigSettings.StartingCreds.Value;
                Plugin.Log.LogInfo($"Starting credits modified to {TimeOfDay.Instance.quotaVariables.startingCredits}");
            }
        }
    }

    [HarmonyPatch(typeof(FlashlightItem), "Start")]
    public class Flashlights_Start_Patch
    {
        internal static Color? DefaultRegColor { get; private set; }
        internal static Color? DefaultProColor { get; private set; }

        public static void Postfix(FlashlightItem __instance)
        {
            Plugin.Spam($"{__instance.itemProperties.itemName} start!");
            if (DefaultRegColor.HasValue && __instance.itemProperties.itemName.ToLower() == "flashlight")
                return;

            if (DefaultProColor.HasValue && __instance.itemProperties.itemName.ToLower() == "pro-flashlight")
                return;

            if(__instance.itemProperties.itemName.ToLower() == "flashlight")
                DefaultRegColor = __instance.flashlightBulb.color;

            if (__instance.itemProperties.itemName.ToLower() == "pro-flashlight")
                DefaultProColor = __instance.flashlightBulb.color;

            Plugin.Spam($"default color has been set for {__instance.itemProperties.itemName}!");
        }
    }

    [HarmonyPatch(typeof(FlashlightItem), "SwitchFlashlight")]
    public class FlashLights_Color_Patch
    {
        public static void Postfix(FlashlightItem __instance, bool on)
        {
            if (!on)
                return;

            if(!ConfigSettings.ModNetworking.Value)
                return;

            if (ColorCommands.RainbowFlash)
            {
                NetHandler.Instance.CycleThroughRainbowFlash();
                return;
            }
                

            Color def;

            if (__instance.itemProperties.itemName.ToLower() == "flashlight")
                def = Flashlights_Start_Patch.DefaultRegColor.Value;
            else if (__instance.itemProperties.itemName.ToLower() == "pro-flashlight")
                def = Flashlights_Start_Patch.DefaultProColor.Value;
            else
            {
                def = Color.white;
                Plugin.Spam($"Unknown flashlight item [ {__instance.itemProperties.itemName} ]");
            }

            Plugin.Spam($"Color def: {def}\n{__instance.itemProperties.itemName} color: {__instance.flashlightBulb.color}");
            if (__instance.flashlightBulb.color == def)
            {
                if (!ColorCommands.CustomFlashColor.HasValue)
                {
                    if (StartOfRound.Instance.localPlayerController.helmetLight.color != def)
                        NetHandler.Instance.HelmetLightColorServerRpc(def, StartOfRound.Instance.localPlayerController.actualClientId);
                    return;
                }
                    

                Plugin.Spam("Updating from default flashlight color!");
                NetHandler.SetFlash(ref __instance, ColorCommands.CustomFlashColor.Value);
                NetHandler.SetHelmetLight(ColorCommands.CustomFlashColor.Value, StartOfRound.Instance.localPlayerController.actualClientId);
                NetHandler.Instance.FlashColorServerRpc(ColorCommands.CustomFlashColor.Value, StartOfRound.Instance.localPlayerController.actualClientId, StartOfRound.Instance.localPlayerController.playerUsername);
            }
            else
            {
                if (ColorCommands.CustomFlashColor.HasValue && __instance.bulbLight.color == ColorCommands.CustomFlashColor.Value)
                    return;

                Plugin.Spam("Updating to new flashlight color!");
                NetHandler.Instance.HelmetLightColorServerRpc(__instance.bulbLight.color, StartOfRound.Instance.localPlayerController.actualClientId);
            }
        }
    }

    public class LoadGrabbablesOnShip
    {
        public static List<GrabbableObject> ItemsOnShip = [];
        public static void LoadAllItems()
        {
            ItemsOnShip.Clear();
            GameObject ship = GameObject.Find("/Environment/HangarShip");
            var grabbableObjects = ship.GetComponentsInChildren<GrabbableObject>();
            foreach (GrabbableObject item in grabbableObjects)
            {
                ItemsOnShip.Add(item);
                Plugin.MoreLogs($"{item.itemProperties.itemName} added to list");
            }

        }

    }
}
