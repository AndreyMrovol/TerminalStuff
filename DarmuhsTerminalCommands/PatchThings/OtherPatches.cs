﻿using HarmonyLib;
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
