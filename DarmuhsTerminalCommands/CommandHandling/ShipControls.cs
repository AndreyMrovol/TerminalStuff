﻿using GameNetcodeStuff;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using static TerminalStuff.Misc;
using static TerminalStuff.StringStuff;
using static UnityEngine.Object;
using Object = UnityEngine.Object;
using static OpenLib.Common.Teleporter;

namespace TerminalStuff
{
    internal class ShipControls
    {

        internal static bool leverEnum = false;
        internal static bool DoorSpaceCheck()
        {
            if (ConfigSettings.canOpenDoorInSpace.Value)
            {
                return true;
            }
            else if (StartOfRound.Instance.shipDoorsEnabled)
                return true;
            else
                return false;
        }

        internal static string BasicDoorCommand()
        {

            //TerminalNode node = getTerm.currentNode;
            string displayText = string.Empty;

            if (DoorSpaceCheck())
            {

                // Determine the button name based on the hangar doors state
                string buttonName = StartOfRound.Instance.hangarDoorsClosed ? "StartButton" : "StopButton";

                // Find the corresponding button GameObject
                GameObject buttonObject = GameObject.Find(buttonName);

                // Get the InteractTrigger component from the button
                InteractTrigger interactTrigger = buttonObject.GetComponentInChildren<InteractTrigger>();

                // Determine the action based on the hangar doors state
                string action = StartOfRound.Instance.hangarDoorsClosed ? "opened" : "closed";

                // Log the door state
                Plugin.MoreLogs($"Hangar doors are {action}.");

                // Invoke the onInteract event if the button and event are found
                if (interactTrigger != null)
                {
                    if (interactTrigger.onInteract is UnityEvent<PlayerControllerB> onInteractEvent)
                    {
                        onInteractEvent.Invoke(GameNetworkManager.Instance.localPlayerController);

                        // Log individual messages for open and close events
                        if (action == "opened")
                        {
                            displayText = $"{ConfigSettings.doorOpenString.Value}\n";
                            Plugin.MoreLogs($"Hangar doors {action} successfully by interacting with button {buttonName}.");
                            return displayText;
                        }
                        else if (action == "closed")
                        {
                            displayText = $"{ConfigSettings.doorCloseString.Value}\n";
                            Plugin.MoreLogs($"Hangar doors {action} successfully by interacting with button {buttonName}.");
                            return displayText;
                        }
                    }
                    else
                    {
                        // Log if onInteractEvent is null
                        Plugin.Log.LogWarning($"Warning: onInteract event is null for button {buttonName}.");
                        displayText = $"Unable to close doors, button could not be found!";
                        return displayText;
                    }
                }
                else
                {
                    // Log if interactTrigger is null
                    Plugin.Log.LogWarning($"Warning: InteractTrigger not found on button {buttonName}.");
                    displayText = $"Unable to close doors, button could not be found!";
                    return displayText;
                }
            }
            else
            {
                displayText = $"{ConfigSettings.doorSpaceString.Value}\n";
                return displayText;
            }

            return displayText;
        }

        internal static string BasicLightsCommand()
        {
            string displayText;

            StartOfRound.Instance.shipRoomLights.ToggleShipLights();
            if (StartOfRound.Instance.shipRoomLights.areLightsOn)
                displayText = $"Ship Lights are [ON]\r\n\r\n";
            else
                displayText = $"Ship Lights are [OFF]\r\n\r\n";
            return displayText;
        }

        internal static string RegularTeleporterCommand()
        {
            string[] screenWords = GetWords();
            string[] words = GetWordsAndKeyword(GetKeywordsPerConfigItem(ConfigSettings.tpKeywords.Value), screenWords);
            string displayText;
            ShipTeleporter tp = NormalTP;
            if ((Object)tp != (Object)null)
            {
                float cooldownTime = tp.cooldownTime;
                if (Mathf.Round(cooldownTime) == 0 && tp.buttonTrigger.interactable)
                {
                    if (words.Length > 1)
                    {
                        Plugin.MoreLogs("attempting to tp specific player");
                        string playerName = words[1];
                        PlayerControllerB player = GetPlayerFromName(playerName);
                        if (player != null && player.isPlayerControlled)
                        {
                            StartOfRound.Instance.mapScreen.targetedPlayer = player;
                            tp.PressTeleportButtonOnLocalClient();
                            displayText = $"{ConfigSettings.tpMessageString.Value}\n";
                            return displayText;
                        }
                        else
                        {
                            BaseUseNormalTP(out displayText, tp);
                            return displayText;
                        }
                    }
                    else
                    {
                        BaseUseNormalTP(out displayText, tp);
                        return displayText;
                    }


                }
                else displayText = $"Teleporter has {Mathf.Round(cooldownTime)} seconds remaining on cooldown.\r\n";
                return displayText;
            }
            else displayText = "Can't teleport at this time.\n Do you even have a teleporter?\n";
            return displayText;
        }

        private static string BaseUseNormalTP(out string displayText, ShipTeleporter tp)
        {
            if (Plugin.instance.TwoRadarMapsMod && ViewCommands.AnyActiveMonitoring())
            {
                Plugin.MoreLogs("using TP on target from Terminal Radar");
                displayText = TwoRadarMapsCompatibility.TeleportCompatibility();
                return displayText;
            }
            else
            {
                tp.PressTeleportButtonOnLocalClient();
                displayText = $"{ConfigSettings.tpMessageString.Value}\n";
                return displayText;
            }
        }

        internal static string InverseTeleporterCommand()
        {
            string displayText;
            ShipTeleporter tp = InverseTP;
            if ((Object)tp != (Object)null)
            {
                float cooldownTime = tp.cooldownTime;
                if (!(StartOfRound.Instance.inShipPhase) && tp.buttonTrigger.interactable)
                {
                    tp.PressTeleportButtonOnLocalClient();
                    displayText = $"{ConfigSettings.itpMessageString.Value}\n";
                    return displayText;
                }
                else if (Mathf.Round(cooldownTime) > 0)
                {
                    displayText = $"Inverse Teleporter has {Mathf.Round(cooldownTime)} seconds remaining on cooldown.\r\n";
                    return displayText;
                }
                else
                {
                    displayText = $"Can't Inverse Teleport from space...\r\n"; //test
                    return displayText;
                }


            }
            else displayText = "Can't Inverse Teleport at this time.\n Do you even have an Inverse Teleporter?\n";
            return displayText;
        }

        internal static string LeverControlCommand()
        {

            StartMatchLever leverInstance = FindObjectOfType<StartMatchLever>();
            NetworkManager networkManager = Plugin.instance.Terminal.NetworkManager;
            string getLevelName = StartOfRound.Instance.currentLevel.PlanetName;

            if (CanPullLever(networkManager))
            {
                string displayText = $"{ConfigSettings.leverString.Value}\n";
                leverInstance.StartCoroutine(LeverPull(leverInstance));
                Plugin.MoreLogs("lever pulled");
                return displayText;
            }
            else if (StartOfRound.Instance.travellingToNewLevel)
            {
                string displayText = $"We have not yet arrived to {getLevelName}, please wait.\r\n";
                return displayText;
            }
            else if (GameNetworkManager.Instance.gameHasStarted)
            {
                string displayText = $"{ConfigSettings.leverString.Value}\n";
                leverInstance.StartCoroutine(LeverPull(leverInstance));
                Plugin.MoreLogs("lever pulled");
                return displayText;
            }
            else
            {
                string displayText = "Cannot pull the lever at this time.\r\n\r\n\tNOTE: If the game has not been started, only the host can do this.\r\n\r\n";
                return displayText;
            }
        }

        internal static string AskLever()
        {
            string getLevelName = StartOfRound.Instance.currentLevel.PlanetName;
            if (StartOfRound.Instance.inShipPhase)
            {
                string displayText = $"Pull the Lever and land on {getLevelName}?\n\n\n\n\n\n\n\n\n\n\n\nPlease CONFIRM or DENY.\n";
                return displayText;
            }
            else
            {
                string displayText = $"Pull the Lever and leave {getLevelName}?\n\n\n\n\n\n\n\n\n\n\n\nPlease CONFIRM or DENY.\n";
                return displayText;
            }
        }

        internal static string DenyLever()
        {
            string displayText = "Lever pull canceled...\r\n\r\n\r\n";
            return displayText;
        }

        private static bool CanPullLever(NetworkManager networkManager)
        {
            return !GameNetworkManager.Instance.gameHasStarted &&
                   !StartOfRound.Instance.travellingToNewLevel &&
                   networkManager is not null &&
                   networkManager.IsHost;
        }

        static IEnumerator LeverPull(StartMatchLever leverInstance)
        {
            if (leverEnum)
                yield break;

            leverEnum = true;

            if (leverInstance != null)
            {
                yield return new WaitForSeconds(0.3f);
                leverInstance.LeverAnimation();
                yield return new WaitForSeconds(0.3f);
                leverInstance.PullLever();
            }
            else
            {
                Plugin.Log.LogError("StartMatchLever instance not found!");
            }

            leverEnum = true;
        }
    }
}
