namespace TerminalStuff
{
    internal class throwawaycode
    {
        /*
         * 
         * 
         * 
         * 
         * 
         * 
         
        internal static void AttemptConfigSync(int fromClient)
        {
            Plugin.Spam("Attempting config sync!");
            List<ConfigEntryBase> configItems = OpenLib.ConfigManager.ConfigSync.PullMyConfig(Plugin.instance.Config);
            Plugin.Spam($"config count - {configItems.Count}");
            Dictionary<string, bool> configBools = OpenLib.ConfigManager.ConfigSync.ConfigBools(configItems);
            foreach(var item in configBools)
            {
                Instance.AttemptConfigSyncServerRpc(fromClient, item.Key, 0, item.Value);
            }

            Dictionary<string, string> configStrings = OpenLib.ConfigManager.ConfigSync.ConfigStrings(configItems);
            foreach (var item in configStrings)
            {
                Instance.AttemptConfigSyncServerRpc(fromClient, item.Key, 0, false, item.Value);
            }

            Dictionary<string, float> configFloats = OpenLib.ConfigManager.ConfigSync.ConfigFloats(configItems);
            foreach (var item in configFloats)
            {
                Instance.AttemptConfigSyncServerRpc(fromClient, item.Key, 0, false, "", -1, item.Value);
            }

            Dictionary<string, int> configInts = OpenLib.ConfigManager.ConfigSync.ConfigInts(configItems);
            foreach (var item in configInts)
            {
                Instance.AttemptConfigSyncServerRpc(fromClient, item.Key, 0, false, "", item.Value);
            }

        }

        [ServerRpc(RequireOwnership = true)]
        internal void AttemptConfigSyncServerRpc(int toClient, string item, int type, bool value = false, string value2 = "", int value3 = -1, float value4 = -1)
        {
            Plugin.Spam($"attempting sync of {item} to {toClient}");
            AttemptConfigSyncClientRpc(toClient, item, type, value, value2, value3, value4);
        }

        [ClientRpc]
        internal void AttemptConfigSyncClientRpc(int toClient, string item, int type, bool value = false, string value2 = "", int value3 = -1, float value4 = -1)
        {
            Plugin.Spam($"attempting sync of {item} to {toClient}");
            if ((int)StartOfRound.Instance.localPlayerController.actualClientId != toClient)
                return;

            Plugin.Spam("We are the client specified!");

            if (type == 0)
                OpenLib.ConfigManager.ConfigSync.UpdateFromHost(item, value, Plugin.instance.Config);
            if(type == 1)
                OpenLib.ConfigManager.ConfigSync.UpdateFromHost(item, value2, Plugin.instance.Config);
            if(type == 2)
                OpenLib.ConfigManager.ConfigSync.UpdateFromHost(item, value3, Plugin.instance.Config);
            if(type == 3)
                OpenLib.ConfigManager.ConfigSync.UpdateFromHost(item, value4, Plugin.instance.Config);

            Plugin.instance.Config.Save();
        }

        [ServerRpc(RequireOwnership = false)]
        internal void AskHostSyncServerRpc(int fromClient)
        {
            Plugin.Spam("Asking host for config sync!");
            Plugin.Spam($"from - {fromClient}");
            AskHostSyncClientRpc(fromClient);
        }

        [ClientRpc]
        internal void AskHostSyncClientRpc(int fromClient)
        {
            if (GameNetworkManager.Instance == null)
                return;

            Plugin.Spam("checking if we are the host");

            if (GameNetworkManager.Instance.isHostingGame && ConfigSettings.SyncConfigs.Value)
                AttemptConfigSync(fromClient);
        }
         * 
         * 
         * 
         * 
         * 
         * 
         * 
         */
    }
}
