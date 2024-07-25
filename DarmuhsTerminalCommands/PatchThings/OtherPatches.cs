using BepInEx.Bootstrap;
using GameNetcodeStuff;
using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;


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

            if (!ViewCommands.IsExternalCamsPresent() && ViewCommands.AnyActiveMonitoring())
            {
                ViewCommands.targetInt = setRadarTargetIndex;
                Plugin.MoreLogs("Updating homebrew target");
                SwitchedRadarEvent();
            }
            else if (ViewCommands.IsExternalCamsPresent() && ViewCommands.AnyActiveMonitoring())
            {
                if (Plugin.instance.OpenBodyCamsMod && !OpenBodyCamsCompatibility.showingBodyCam)
                    Plugin.MoreLogs("OBC Terminal Body Cam is NOT active");
                else
                    ViewCommands.GetPlayerCamsFromExternalMod();
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
                ViewCommands.UpdateCamsTarget(ViewCommands.targetInt);
                return;
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
