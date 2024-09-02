using GameNetcodeStuff;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using static TerminalStuff.EventSub.TerminalStart;
using static OpenLib.CoreMethods.AddingThings;
using static OpenLib.CoreMethods.LogicHandling;
using Object = UnityEngine.Object;

namespace TerminalStuff
{
    public class NetHandler : NetworkBehaviour
    {

        internal static NetHandler Instance { get; private set; }
        internal static Terminal patchTerminal = null;
        internal static bool netNodeSet = false;
        internal bool endFlashRainbow = false;
        internal static TerminalNode netNode = CreateDummyNode("", true, "");
        internal static bool rainbowFlashEnum = false;

        //Load New Node SYNC

        [ServerRpc(RequireOwnership = false)]
        internal void NodeLoadServerRpc(string topRightText, string nodeName, string nodeText, int nodeNumber = -1)
        {
            NetworkManager networkManager = base.NetworkManager;
            if (netNodeSet && (networkManager.IsHost || networkManager.IsServer))
            {
                Plugin.MoreLogs("RPC called from host, sending to client RPC ");
                NodeLoadClientRpc(topRightText, nodeName, nodeText, true, nodeNumber);
                return;
            }
            else if (!netNodeSet && networkManager.IsHost || networkManager.IsServer)
            {
                //if (!Plugin.instance.Terminal.terminalUIScreen.gameObject.activeSelf)
                    //return;

                Plugin.MoreLogs($"Host: attempting to sync node {nodeName}/{nodeNumber}");
                SyncNodes(topRightText, nodeName, nodeText, nodeNumber);
            }
            else
            {
                Plugin.MoreLogs($"Server: This should only be coming from clients");
                NodeLoadClientRpc(topRightText, nodeName, nodeText, true, nodeNumber);
            }

            Plugin.MoreLogs("Server: Attempting to sync nodes between clients.");
        }

        [ClientRpc]
        internal void NodeLoadClientRpc(string topRightText, string nodeName, string nodeText, bool fromHost, int nodeNumber = -1)
        {
            //if (!Plugin.instance.Terminal.terminalUIScreen.gameObject.activeSelf)
               //return;

            NetworkManager networkManager = base.NetworkManager;
            if (fromHost && (networkManager.IsHost || networkManager.IsServer))
            {
                NetNodeReset(false);
                Plugin.MoreLogs("Node detected coming from host, resetting nNS and ending RPC");
                return;
            }

            if (!netNodeSet)
            {
                Plugin.MoreLogs($"Client: attempting to sync node, {nodeName}/{nodeNumber}");
                SyncNodes(topRightText, nodeName, nodeText, nodeNumber);
            }
            else
            {
                Plugin.MoreLogs("Client: netNodeSet is true, no sync required.");
                NetNodeReset(false);
                return;
            }
        }

        internal static bool NetNodeReset(bool set)
        {
            netNodeSet = set;
            return netNodeSet;
        }

        private static void SyncViewNodeWithNum(ref TerminalNode node, int nodeNumber, string nodeText)
        {
            Plugin.MoreLogs("---------------- Loading view node triggered by another player ----------------");

            if (nodeNumber == 0) //VideoPlayer
            {
                if (ConfigSettings.videoSync.Value)
                {
                    node.displayText = nodeText;
                    VideoManager.PlaySyncedVideo();
                }
                else
                    node.displayText = ViewCommands.LolVideoPlayerEvent();
                
                return;
            }
            else if (nodeNumber == 1) // cams
            {
                node.displayText = ViewCommands.TermCamsEvent();
                return;
            }
            else if (nodeNumber == 2) //overlay
            {
                node.displayText = ViewCommands.OverlayTermEvent();
                return;
            }
            else if (nodeNumber == 3) //minimap
            {
                node.displayText = ViewCommands.MiniMapTermEvent();
                return;
            }
            else if (nodeNumber == 4) //minicams
            {
                node.displayText = ViewCommands.MiniCamsTermEvent();
                return;
            }
            else if (nodeNumber == 5) //map
            {
                node.displayText = ViewCommands.TermMapEvent();
                return;
            }
            else if (nodeNumber == 6) //mirror
            {
                node.displayText = ViewCommands.MirrorEvent();
            }
            else
                Plugin.MoreLogs("No matching views detected");
        }

        private void SyncNodes(string topRightText, string nodeName, string nodeText, int nodeNumber = -1)
        {

            TerminalNode node = GetFromAllNodes(nodeName);

            if (node == null)
            {
                DefaultSync(nodeName, nodeText);
                Plugin.MoreLogs($"{nodeName} not matching known nodes. Only displaying text:\n{nodeText}");
                return;
            }

            NetNodeReset(true);

            if (nodeNumber != -1 && nodeNumber <= ConfigSettings.TerminalStuffMain.ListNumToString.Count)
            {
                TerminalNode viewNode = StartofHandling.FindViewNode(nodeNumber);
                
                if (viewNode == null)
                    return;

                SyncViewNodeWithNum(ref viewNode, nodeNumber, nodeText);
                Plugin.instance.Terminal.LoadNewNode(viewNode);
                //Plugin.instance.Terminal.currentNode.displayText = viewNode.displayText;
                Plugin.MoreLogs($"Non terminal user: Attempting to load {nodeName}, ViewNode: {nodeNumber}\n {viewNode.displayText}");
            }
            else
            {
                DefaultSync(nodeName, nodeText, node);
                Plugin.MoreLogs($"Non terminal user: Attempting to load {nodeName}'s displayText:\n {nodeText}");
            }


            Plugin.instance.Terminal.topRightText.text = topRightText;
            NetNodeReset(false);

        }

        private void DefaultSync(string nodeName, string nodeText, TerminalNode newNode = null)
        {
            if(newNode != null && vanillaNodes.Contains(newNode))
            {
                MoreCamStuff.CamPersistance(nodeName);
                MoreCamStuff.VideoPersist(nodeName);
                newNode.displayText = Plugin.instance.Terminal.TextPostProcess(newNode.displayText, newNode);
                Plugin.instance.Terminal.LoadNewNode(newNode);
                if (ConfigSettings.CacheLastTerminalPage.Value)
                    TerminalEvents.lastNode = newNode;
                return;
            }

            MoreCamStuff.CamPersistance(nodeName);
            MoreCamStuff.VideoPersist(nodeName);
            netNode.displayText = nodeText;
            Plugin.instance.Terminal.LoadNewNode(netNode);
            
            if(ConfigSettings.CacheLastTerminalPage.Value)
                TerminalEvents.lastNode = netNode;

            Plugin.MoreLogs($"Only displaying {nodeName} text.");
        }

        [ServerRpc(RequireOwnership = false)]
        internal void SyncDropShipServerRpc(bool isRefund = false)
        {
            Plugin.MoreLogs($"Server: Attempting to sync dropship between players...");
            SyncDropShipClientRpc(isRefund);
        }

        [ClientRpc]
        internal void SyncDropShipClientRpc(bool isRefund)
        {
            NetworkManager networkManager = base.NetworkManager;
            if (networkManager.IsHost || networkManager.IsServer)
            {
                if (isRefund)
                {
                    Plugin.MoreLogs("Host received refund command, clearing orderedItems list");
                    Plugin.instance.Terminal.orderedItemsFromTerminal.Clear();
                    CostCommands.storeCart.Clear();
                    return;
                }

                Plugin.MoreLogs("Host syncing dropship to storeCart after purchase");
                int[] itemsOrdered = [.. Plugin.instance.Terminal.orderedItemsFromTerminal];
                CostCommands.storeCart = Plugin.instance.Terminal.orderedItemsFromTerminal;
                SendItemsToAllServerRpc(itemsOrdered);
            }
        }

        internal static void SyncMyVideoChoiceToEveryone(string videoPlaying)
        {
            if (!ConfigSettings.networkedNodes.Value || !ConfigSettings.ModNetworking.Value || !ConfigSettings.videoSync.Value)
                return;

            if (Instance == null || StartOfRound.Instance == null || StartOfRound.Instance.localPlayerController == null)
                return;

            Plugin.MoreLogs($"Sending {videoPlaying} as videoPlaying to other clients with active screens");
            Instance.SyncVideoChoiceServerRpc(((int)StartOfRound.Instance.localPlayerController.playerClientId), videoPlaying);
        }

        [ServerRpc(RequireOwnership = false)]
        internal void SyncTwoRadarMapsServerRpc(int fromClient, int playerNum)
        {
            Plugin.Spam($"Server: Client ({fromClient}) attempting to sync tworadarmaps target to {playerNum}");
            SyncTwoRadarMapsClientRpc(fromClient, playerNum);
        }

        [ClientRpc]
        internal void SyncTwoRadarMapsClientRpc(int fromClient, int playerNum)
        {
            if (((int)StartOfRound.Instance.localPlayerController.playerClientId) == fromClient)
            {
                Plugin.Spam($"This is the client updating target to {playerNum}");
                return;
            }
            else
            {
                Plugin.Spam("SyncTwoRadarMapsClientRpc called from another client");
                TwoRadarMapsCompatibility.SyncTarget(playerNum);
            }
            
        }

        [ServerRpc(RequireOwnership = false)]
        internal void SyncVideoChoiceServerRpc(int fromClient, string videoPlaying)
        {
            Plugin.Spam($"Server: Attempting to sync video choice: {videoPlaying}");
            SyncVideoChoiceClientRpc(fromClient, videoPlaying);
        }

        [ClientRpc]
        internal void SyncVideoChoiceClientRpc(int fromClient, string videoPlaying)
        {
            if (!Plugin.instance.Terminal.terminalUIScreen.gameObject.activeSelf)
                return;

            if (((int)StartOfRound.Instance.localPlayerController.playerClientId) == fromClient)
            {
                Plugin.MoreLogs($"This is the client sending the video name {videoPlaying}");
                return;
            }
            else if (VideoManager.currentlyPlaying == videoPlaying)
            {
                Plugin.MoreLogs($"video already set to {videoPlaying}");
                return;
            }
            else
            {
                VideoManager.currentlyPlaying = videoPlaying;
                Plugin.MoreLogs($"currentlyPlaying set to {videoPlaying}");
                return;
            }

        }

        internal static void SyncMyCamsBoolToEveryone(bool myCams)
        {
            if (!ConfigSettings.networkedNodes.Value || !ConfigSettings.ModNetworking.Value)
                return;

            if (Instance == null || StartOfRound.Instance == null || StartOfRound.Instance.localPlayerController == null)
                return;

            if (Plugin.instance.activeCam == myCams)
                return;

            Instance.SyncActiveCamsBoolServerRpc(((int)StartOfRound.Instance.localPlayerController.playerClientId), myCams);
        }

        [ServerRpc(RequireOwnership = false)]
        internal void SyncActiveCamsBoolServerRpc(int fromClient, bool value)
        {

            Plugin.MoreLogs($"Server: Attempting to sync active cams bool...");
            SyncActiveCamsClientRpc(fromClient, value);
        }

        [ClientRpc]
        internal void SyncActiveCamsClientRpc(int fromClient, bool value)
        {
            if (!Plugin.instance.Terminal.terminalUIScreen.gameObject.activeSelf)
                return;

            if (((int)StartOfRound.Instance.localPlayerController.playerClientId) == fromClient)
            {
                Plugin.MoreLogs($"This is the client syncing the bool");
                return;
            }
            else if (Plugin.instance.activeCam == value)
            {
                Plugin.MoreLogs("Catching extra sync and returning");
                return;
            }

            else
            {
                Plugin.instance.activeCam = value;
                Plugin.MoreLogs($"activeCam set to {value}");
                return;
            }

        }

        [ServerRpc(RequireOwnership = false)]
        internal void GetCurrentNodeServerRpc(int fromClient, int otherClient)
        {
            if (fromClient == -1 || otherClient == -1)
            {
                Plugin.ERROR($"GetCurrentNodeServerRpc FATAL ERROR: Invalid client ID detected.\nfromClient: {fromClient}\notherClient: {otherClient}");
                return;
            }

            Plugin.MoreLogs($"Server: Client [{fromClient}] requesting terminalNode from: [{otherClient}]");
            GetCurrentNodeClientRpc(fromClient, otherClient);
        }

        [ClientRpc]
        internal void GetCurrentNodeClientRpc(int fromClient, int otherClient)
        {
            if (((int)StartOfRound.Instance.localPlayerController.playerClientId) == fromClient)
            {
                Plugin.MoreLogs($"This is the client requesting the node");
                return;
            }
            else if (((int)StartOfRound.Instance.localPlayerController.playerClientId) == otherClient)
            {
                Plugin.MoreLogs($"This is the client the node is being requested from");
                //NetHandler.Instance.SyncActiveCamsBoolServerRpc(otherClient, netCamStatus);
                StartofHandling.CheckNetNode(Plugin.instance.Terminal.currentNode);
                return;
            }
            else
            {
                Plugin.MoreLogs("This client is neither the client requesting node status or the client being requested to provide it.");
                return;
            }
        }

        [ServerRpc(RequireOwnership = false)]
        internal void GetItemStatusServerRpc(string upgradeName, bool upgradeStatus)
        {
            Plugin.MoreLogs("Server: Requesting item status");
            GetItemStatusClientRpc(upgradeName, upgradeStatus);
        }

        [ClientRpc]
        internal void GetItemStatusClientRpc(string upgradeName, bool upgradeStatus)
        {
            NetworkManager networkManager = base.NetworkManager;
            if (networkManager.IsHost || networkManager.IsServer)
            {
                CostCommands.CheckUpgradeStatus(ref upgradeStatus, upgradeName);
                SendItemStatusServerRpc(upgradeName, upgradeStatus);
                Plugin.MoreLogs($"returning - {upgradeName} is {upgradeStatus}");
            }
        }

        [ServerRpc(RequireOwnership = true)]
        internal void SendItemStatusServerRpc(string upgradeName, bool upgradeStatus)
        {
            Plugin.MoreLogs("Server: Sending item status to clients");
            SendItemStatusClientRpc(upgradeName, upgradeStatus);
        }

        [ClientRpc]
        internal void SendItemStatusClientRpc(string upgradeName, bool upgradeStatus)
        {
            if (upgradeName.Contains("vitalspatch"))
                CostCommands.vitalsUpgradeEnabled = upgradeStatus;
            else if (upgradeName.Contains("bioscanpatch"))
                CostCommands.enemyScanUpgradeEnabled = upgradeStatus;
            else
                Plugin.MoreLogs($"{upgradeName} could not be updated to {upgradeStatus}");
        }

        [ServerRpc(RequireOwnership = true)]
        internal void SendItemsToAllServerRpc(int[] itemsOrdered)
        {
            Plugin.MoreLogs("Server: Sending itemsOrdered to clients...");
            SendItemsToAllClientRpc(itemsOrdered);
        }
        [ClientRpc]
        internal void SendItemsToAllClientRpc(int[] itemsOrdered)
        {
            NetworkManager networkManager = base.NetworkManager;
            if (!networkManager.IsHost || !networkManager.IsServer)
            {
                Plugin.MoreLogs("Client: Setting storeCart value to host's orderedItemsFromTerminal list");
                List<int> receivedList = new(itemsOrdered);
                CostCommands.storeCart = receivedList;
            }
        }

        [ServerRpc(RequireOwnership = false)]
        internal void SyncCreditsServerRpc(int newCreds, int items)
        {
            Plugin.MoreLogs("Server: syncing credits and items...");
            SyncCreditsClientRpc(newCreds, items);
        }
        [ClientRpc]
        internal void SyncCreditsClientRpc(int newCreds, int items)
        {

            NetworkManager networkManager = base.NetworkManager;
            if (networkManager.IsHost || networkManager.IsServer)
            {
                Plugin.instance.Terminal.SyncGroupCreditsServerRpc(newCreds, items);
                CostCommands.storeCart = Plugin.instance.Terminal.orderedItemsFromTerminal;
            }
        }

        [ServerRpc(RequireOwnership = false)]
        internal void SyncRadarZoomServerRpc(float zoom)
        {
            Plugin.MoreLogs("Server: syncing radar zoom level...");
            SyncRadarZoomClientRpc(zoom);
        }
        [ClientRpc]
        internal void SyncRadarZoomClientRpc(float zoom)
        {
            if (ViewCommands.radarZoom == zoom)
                return;

            ViewCommands.radarZoom = zoom;

            if (Plugin.instance.TwoRadarMapsMod)
            {
                TwoRadarMapsCompatibility.ChangeMapZoom(ViewCommands.radarZoom);
                Plugin.MoreLogs($"Radar Zoom for TwoRadarMaps set to {ViewCommands.radarZoom}");
            }
            else
            {
                StartOfRound.Instance.mapScreen.cam.orthographicSize = ViewCommands.radarZoom;
                Plugin.MoreLogs($"Radar Zoom set to {ViewCommands.radarZoom}");
            }
        }

        [ServerRpc(RequireOwnership = false)]
        internal void StartAoDServerRpc(bool aod)
        {
            Plugin.MoreLogs($"Server: syncing alwaysondisplay to {aod}");
            AoDClientRpc(aod);
        }

        [ServerRpc(RequireOwnership = false)]
        internal void AoDServerRpc(bool aod)
        {
            Plugin.MoreLogs($"Server: syncing alwaysondisplay to {aod}");
            AoDClientRpc(aod);
        }
        [ClientRpc]
        internal void AoDClientRpc(bool aod)
        {
            Plugin.MoreLogs($"Client: setting alwaysondisplay to {aod}");
            alwaysOnDisplay = aod;
            if (Plugin.instance.Terminal.terminalInUse == false)
                ToggleScreen(aod);
        }


        //Ship Color changes
        [ServerRpc(RequireOwnership = false)]
        internal void ShipColorALLServerRpc(Color newColor, string target)
        {
            ShipColorALLClientRpc(newColor, target);
        }

        [ClientRpc]
        internal void ShipColorALLClientRpc(Color newColor, string target)
        {
            GameObject.Find("Environment/HangarShip/ShipElectricLights/Area Light (3)").GetComponent<Light>().color = newColor;
            GameObject.Find("Environment/HangarShip/ShipElectricLights/Area Light (4)").GetComponent<Light>().color = newColor;
            GameObject.Find("Environment/HangarShip/ShipElectricLights/Area Light (5)").GetComponent<Light>().color = newColor;
            Plugin.MoreLogs($"Client: Ship Color change for all lights received. Color: {newColor} Name: {target} ");
        }

        [ServerRpc(RequireOwnership = false)]
        internal void ShipColorFRONTServerRpc(Color newColor, string target)
        {
            ShipColorFRONTClientRpc(newColor, target);
        }

        [ClientRpc]
        internal void ShipColorFRONTClientRpc(Color newColor, string target)
        {
            GameObject.Find("Environment/HangarShip/ShipElectricLights/Area Light (3)").GetComponent<Light>().color = newColor;
            Plugin.MoreLogs($"Client: Ship Color change received for front lights. Color: {newColor} Name: {target} ");
        }

        [ServerRpc(RequireOwnership = false)]
        internal void ShipColorMIDServerRpc(Color newColor, string target)
        {
            Plugin.MoreLogs("serverRpc called");
            ShipColorMIDClientRpc(newColor, target);
        }

        [ClientRpc]
        internal void ShipColorMIDClientRpc(Color newColor, string target)
        {
            GameObject.Find("Environment/HangarShip/ShipElectricLights/Area Light (4)").GetComponent<Light>().color = newColor;
            Plugin.MoreLogs($"Client: Ship Color change received for middle lights. Color: {newColor} Name: {target} ");
        }

        [ServerRpc(RequireOwnership = false)]
        internal void ShipColorBACKServerRpc(Color newColor, string target)
        {
            Plugin.MoreLogs("serverRpc called");
            ShipColorBACKClientRpc(newColor, target);
        }

        [ClientRpc]
        internal void ShipColorBACKClientRpc(Color newColor, string target)
        {
            GameObject.Find("Environment/HangarShip/ShipElectricLights/Area Light (5)").GetComponent<Light>().color = newColor;
            Plugin.MoreLogs($"Client: Ship Color change received for back lights. Color: {newColor} Name: {target} ");
        }


        //Flashlights

        [ServerRpc(RequireOwnership = false)]
        internal void FlashColorServerRpc(Color newColor, ulong playerID, string playerName)
        {
            //Plugin.MoreLogs("Fcolor serverRpc called");
            FlashColorClientRpc(newColor, playerID, playerName);
        }

        [ClientRpc]
        internal void FlashColorClientRpc(Color newColor, ulong playerID, string playerName)
        {
            //Plugin.MoreLogs("Fcolor clientRpc called");
            SetFlash(newColor, playerID, playerName);
        }

        private GrabbableObject FindFlashlightObject(string playerName)
        {
            GrabbableObject[] objectsOfType = Object.FindObjectsOfType<GrabbableObject>();
            GrabbableObject getMyFlash = null;

            foreach (GrabbableObject thisFlash in objectsOfType)
            {
                if (thisFlash.playerHeldBy != null)
                {
                    if (thisFlash.playerHeldBy.playerUsername == playerName && thisFlash.gameObject.name.Contains("Flashlight"))
                    {
                        getMyFlash = thisFlash;
                        break;
                    }
                }
            }

            return getMyFlash;
        }

        private void SetFlash(Color newColor, ulong playerID, string playerName)
        {
            GrabbableObject getMyFlash = FindFlashlightObject(playerName);

            // Move the null check outside the loop
            if (getMyFlash != null)
            {
                // Use TryGetComponent to safely get the FlashlightItem component
                if (getMyFlash.gameObject.TryGetComponent<FlashlightItem>(out FlashlightItem flashlightItem))
                {
                    if (flashlightItem.flashlightBulb != null && flashlightItem.flashlightBulbGlow != null)
                    {
                        flashlightItem.flashlightBulb.color = newColor;
                        flashlightItem.flashlightBulbGlow.color = newColor;
                        Plugin.instance.fSuccess = true;

                        if (StartOfRound.Instance.allPlayerScripts[playerID].helmetLight)
                        {
                            StartOfRound.Instance.allPlayerScripts[playerID].helmetLight.color = newColor;
                            Plugin.instance.hSuccess = true;
                        }
                    }
                    else
                    {
                        Plugin.MoreLogs($"flashlightBulb or flashlightBulbGlow is null on {getMyFlash.gameObject}");
                    }
                }
                else
                {
                    Plugin.Log.LogError($"FlashlightItem component not found on {getMyFlash.gameObject}");
                }
            }
        }

        internal void CycleThroughRainbowFlash()
        {

            // Start the new coroutine for the rainbow effect
            string playerName = GameNetworkManager.Instance.localPlayerController.playerUsername;
            ulong playerID = GameNetworkManager.Instance.localPlayerController.playerClientId;
            PlayerControllerB getPlayer = StartOfRound.Instance.localPlayerController;

            endFlashRainbow = false;
            StartCoroutine(RainbowFlashCoroutine(playerName, playerID, getPlayer));
            Plugin.MoreLogs($"{playerName} trying to set flashlight to rainbow mode!");

        }

        private IEnumerator RainbowFlashCoroutine(string playerName, ulong playerID, PlayerControllerB player)
        {
            if (rainbowFlashEnum)
                yield break;

            rainbowFlashEnum = true;

            GrabbableObject getMyFlash = FindFlashlightObject(playerName);
            if (getMyFlash != null)
            {
                getMyFlash.itemProperties.itemName += "(Rainbow)";

                while (!player.isPlayerDead && !endFlashRainbow)
                {
                    float rainbowSpeed = 0.4f;
                    float hue = Mathf.PingPong(Time.time * rainbowSpeed, 1f);
                    Color flashlightColor = Color.HSVToRGB(hue, 1f, 1f);

                    if (getMyFlash.isHeld && !getMyFlash.deactivated)
                    {
                        Instance.FlashColorServerRpc(flashlightColor, playerID, playerName);

                        // Wait for a short duration before updating the color again
                        yield return new WaitForSeconds(0.05f);
                    }
                    else
                    {
                        yield return new WaitForSeconds(0.05f);
                    }


                    if (StartOfRound.Instance.allPlayersDead || getMyFlash.insertedBattery.empty || !getMyFlash.isHeld)
                    {
                        Plugin.MoreLogs("ending flashy rainbow");
                        endFlashRainbow = true;
                    }

                }
                string returnItemName = getMyFlash.itemProperties.itemName.Replace("(Rainbow)", "");
                getMyFlash.itemProperties.itemName = returnItemName;
            }
            else
                Plugin.Log.LogError("no flashlights found");

            rainbowFlashEnum = false;
        }

        //QuickRestart RPC
        [ServerRpc(RequireOwnership = true)]
        internal void QuickRestartServerRpc()
        {
            QuickRestartClientRpc();
        }

        [ClientRpc]
        internal void QuickRestartClientRpc()
        {
            GameNetworkManager.Instance.localPlayerController.DropAllHeldItemsAndSync();
            //DeleteInventory();
            Plugin.instance.Terminal.ClearBoughtItems();

            if (StartOfRound.Instance.IsServer)
            {
                GameNetworkManager.Instance.ResetSavedGameValues();
            }

            StartOfRound.Instance.ResetShip();
            StartOfRound.Instance.currentPlanetPrefab.transform.position = StartOfRound.Instance.planetContainer.transform.position;
        }



        //DO NOT REMOVE
        public override void OnNetworkSpawn()
        {
            if (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsServer)
            {
                if (Instance != null && Instance.gameObject != null)
                {
                    NetworkObject networkObject = Instance.gameObject.GetComponent<NetworkObject>();

                    if (networkObject != null)
                    {
                        networkObject.Despawn();
                        Plugin.Log.LogInfo("Nethandler despawned!");
                    }
                }
            }

            Instance = this;
            base.OnNetworkSpawn();
            Plugin.Log.LogInfo("Nethandler Spawned!");

            if (GameNetworkManager.Instance.isHostingGame)
                return;
        }

        internal static void UpgradeStatusCheck()
        {
            if (ConfigSettings.terminalBioScan.Value && ConfigSettings.terminalBioScanPatch.Value && ConfigSettings.ModNetworking.Value)
                Instance.GetItemStatusServerRpc("BioscanPatch", CostCommands.enemyScanUpgradeEnabled);

            if (ConfigSettings.terminalVitals.Value && ConfigSettings.terminalVitalsUpgrade.Value && ConfigSettings.ModNetworking.Value)
                Instance.GetItemStatusServerRpc("VitalsPatch", CostCommands.vitalsUpgradeEnabled);
        }


    }
}

