using BepInEx.Bootstrap;
using static TerminalStuff.AlwaysOnStuff;

namespace TerminalStuff.EventSub
{
    internal class GameStuff
    {
        internal static bool oneTimeOnly = false;
        internal static void OnGameStart()
        {
            CompatibilityCheck();
            oneTimeOnly = false;
        }

        internal static void OnStartOfRoundStart()
        {
            Plugin.instance.splitViewCreated = false;
            SplitViewChecks.InitSplitViewObjects(); //addSplitViewObjects
            BoolStuff.ResetEnumBools(); // resets all enum bools
            TerminalClockStuff.showTime = false; // disable clock on game restart
            ViewCommands.ResetPluginInstanceBools(); //reset view command bools
        }

        internal static void OnPlayerSpawn()
        {
            if (ConfigSettings.alwaysOnDynamic.Value)
                Plugin.instance.Terminal.StartCoroutine(AlwaysOnDynamic(Plugin.instance.Terminal));
        }

        internal static void OnStartGame()
        {
            if (!StartOfRound.Instance.inShipPhase)
            {
                if (!TerminalEvents.clockDisabledByCommand && ConfigSettings.terminalClock.Value)
                    TerminalClockStuff.showTime = true;

            }
            else
            {
                TerminalClockStuff.showTime = false;
            }


            //Cheat credits, only uncomment when testing and needing credits
            //NetHandler.Instance.SyncCreditsServerRpc(999999, Plugin.instance.Terminal.numberOfItemsInDropship);
        }

        private static void CompatibilityCheck()
        {
            if (Chainloader.PluginInfos.ContainsKey("BMX.LobbyCompatibility"))
            {
                Plugin.MoreLogs("LobbyCompatibility detected, setting appropriate Lobby Compatibility Level depending on networking status");
                Plugin.instance.LobbyCompat = true;
                BMX_LobbyCompat.SetCompat(ConfigSettings.ModNetworking.Value);
            }
            if (Chainloader.PluginInfos.ContainsKey("com.potatoepet.AdvancedCompany"))
            {
                Plugin.MoreLogs("Advanced Company detected, setting Advanced Company Compatibility options");
                Plugin.instance.CompatibilityAC = true;
                //if (ConfigSettings.ModNetworking.Value)
                //AdvancedCompanyCompat.AdvancedCompanyStuff();
            }
            if (Chainloader.PluginInfos.ContainsKey("Rozebud.FovAdjust"))
            {
                Plugin.MoreLogs("Rozebud's FovAdjust detected!");
                Plugin.instance.FovAdjust = true;
            }
            if (Chainloader.PluginInfos.ContainsKey("RickArg.lethalcompany.helmetcameras"))
            {
                Plugin.MoreLogs("Helmet Cameras by Rick Arg detected!");
                Plugin.instance.HelmetCamsMod = true;
            }
            if (Chainloader.PluginInfos.ContainsKey("SolosBodycams"))
            {
                Plugin.MoreLogs("SolosBodyCams by CapyCat (Solo) detected!");
                Plugin.instance.SolosBodyCamsMod = true;
            }
            if (Chainloader.PluginInfos.ContainsKey("Zaggy1024.OpenBodyCams"))
            {
                Plugin.MoreLogs("OpenBodyCams by Zaggy1024 detected!");
                Plugin.instance.OpenBodyCamsMod = true;
            }
            if (Chainloader.PluginInfos.ContainsKey("Zaggy1024.TwoRadarMaps"))
            {
                Plugin.MoreLogs("TwoRadarMaps by Zaggy1024 detected!");
                Plugin.instance.TwoRadarMapsMod = true;
            }
            if (Chainloader.PluginInfos.ContainsKey("com.malco.lethalcompany.moreshipupgrades")) //other mods that simply append to the help command
            {
                Plugin.MoreLogs("Lategame Upgrades by malco detected!");
                Plugin.instance.LateGameUpgrades = true;
            }
            if (Chainloader.PluginInfos.ContainsKey("darmuh.suitsTerminal"))
            {
                Plugin.MoreLogs("suitsTerminal detected!");
                Plugin.instance.suitsTerminal = true;
            }
            if (Chainloader.PluginInfos.ContainsKey("TerminalFormatter"))
            {
                Plugin.MoreLogs("Terminal Formatter by mrov detected!");
                Plugin.instance.TerminalFormatter = true;
            }
        }
    }
}
