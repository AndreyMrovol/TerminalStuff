using OpenLib.CoreMethods;
using OpenLib.Events;
using TerminalStuff.PluginCore;
using TerminalStuff.SpecialStuff;
using static TerminalStuff.EventSub.TerminalStart;

namespace TerminalStuff.EventSub
{
    internal class Subscribers
    {
        internal static void Subscribe()
        {
            EventManager.TerminalAwake.AddListener(OnTerminalAwake);
            EventManager.TerminalStart.AddListener(OnTerminalStart);
            EventManager.TerminalParseSent.AddListener(TerminalParse.OnParseSent);
            EventManager.TerminalBeginUsing.AddListener(TerminalBeginUsing.OnTerminalBeginUse);
            EventManager.TerminalLoadNewNode.AddListener(TerminalGeneral.OnLoadNode);
            EventManager.TerminalDisable.AddListener(TerminalGeneral.OnTerminalDisable);
            EventManager.TerminalLoadIfAffordable.AddListener(TerminalGeneral.OnLoadAffordable);
            EventManager.TerminalQuit.AddListener(TerminalQuit.OnTerminalQuit);
            EventManager.SetTerminalInUse.AddListener(TerminalGeneral.OnSetTerminalInUse);

            //TeleporterStuff
            EventManager.NormalTPFound.AddListener(Teleporters.OnNormalAwake);
            EventManager.InverseTPFound.AddListener(Teleporters.OnInverseAwake);

            //GameStuff
            EventManager.GameNetworkManagerStart.AddListener(GameStuff.OnGameStart);
            EventManager.StartOfRoundStart.AddListener(GameStuff.OnStartOfRoundStart);
            EventManager.StartOfRoundStartGame.AddListener(GameStuff.OnStartGame);
            EventManager.PlayerSpawn.AddListener(GameStuff.OnPlayerSpawn);

            //Unique
            EventManager.GetNewDisplayText.AddListener(TerminalParse.OnNewDisplayText);
        }

        internal static void OnTerminalAwake(Terminal instance)
        {
            Plugin.instance.Terminal = instance;
            Plugin.MoreLogs($"Setting Plugin.instance.Terminal");
            FontStuff.SetCachedDefault();
            StuffForLibrary.AddCommands(); //replaced addkeywords
            AlwaysOnStuff.screenSettings = new(ConfigSettings.TerminalScreen.Value);
        }
    }
}
