using TerminalStuff.PluginCore;
using static OpenLib.Common.StartGame;
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
            MoreCamStuff.ResetPluginInstanceBools(); //reset view command bools

        }

        internal static void OnPlayerSpawn()
        {
            screenSettings = new(ConfigSettings.TerminalScreen.Value);
            if (screenSettings.Dynamic)
                Plugin.instance.Terminal.StartCoroutine(AlwaysOnDynamic(Plugin.instance.Terminal));

        }

        internal static void OnStartGame()
        {
            if (!StartOfRound.Instance.inShipPhase)
            {
                if (!TerminalEvents.clockDisabledByCommand && ConfigSettings.TerminalClock.Value)
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
            if (SoftCompatibility("BMX.LobbyCompatibility", ref Plugin.instance.LobbyCompat))
            {
                Plugin.MoreLogs("LobbyCompatibility detected, setting appropriate Lobby Compatibility Level depending on networking status");
                BMX_LobbyCompat.SetCompat(ConfigSettings.ModNetworking.Value);
            }
            if (SoftCompatibility("Rozebud.FovAdjust", ref Plugin.instance.FovAdjust))
            {
                Plugin.Spam("Rozebud's FovAdjust detected!");
            }
            if (SoftCompatibility("RickArg.lethalcompany.helmetcameras", ref Plugin.instance.HelmetCamsMod))
            {
                Plugin.Spam("Helmet Cameras by Rick Arg detected!");
            }
            if (SoftCompatibility("SolosBodycams", ref Plugin.instance.SolosBodyCamsMod))
            {
                Plugin.Spam("SolosBodyCams by CapyCat (Solo) detected!");
            }
            if (SoftCompatibility("Zaggy1024.OpenBodyCams", ref Plugin.instance.OpenBodyCamsMod))
            {
                Plugin.Spam("OpenBodyCams by Zaggy1024 detected!");
            }
            if (SoftCompatibility("Zaggy1024.TwoRadarMaps", ref Plugin.instance.TwoRadarMapsMod))
            {
                Plugin.Spam("TwoRadarMaps by Zaggy1024 detected!");
            }
            if (SoftCompatibility("com.malco.lethalcompany.moreshipupgrades", ref Plugin.instance.LateGameUpgrades))
            {
                Plugin.Spam("Lategame Upgrades by malco detected!");
            }
            if (SoftCompatibility("darmuh.suitsTerminal", ref Plugin.instance.suitsTerminal))
            {
                Plugin.Spam("suitsTerminal detected!");
            }
            if (SoftCompatibility("TerminalFormatter", ref Plugin.instance.TerminalFormatter))
            {
                Plugin.Spam("Terminal Formatter by mrov detected!");
            }
            if (SoftCompatibility("com.github.darmuh.LethalConstellations", ref Plugin.instance.Constellations))
            {
                Plugin.Spam("LethalConstellations detected ^.^");
            }
            if (SoftCompatibility("ShipInventory", ref Plugin.instance.ShipInventory))
                Plugin.Spam("ShipInventory compatibility enabled!");

            if (OpenLib.Plugin.instance.LethalConfig)
                OpenLib.Compat.LethalConfigSoft.AddButton("Terminal Customization", "Refresh Customizations", "Press this button to refresh all terminal customizations", "Refresh", TerminalCustomizer.TerminalCustomization);
        }
    }
}
