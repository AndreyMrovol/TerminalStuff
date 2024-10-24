using OpenLib.CoreMethods;
using OpenLib.Events;
using TerminalStuff.PluginCore;
using TerminalStuff.SpecialStuff;
using TerminalStuff.VisualCore;
using static TerminalStuff.EventSub.TerminalStart;

namespace TerminalStuff.EventSub
{
    internal class Subscribers
    {
        internal static string OriginalOtherText;
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

            OpenLib.TerminalUpdatePatch.usePatch = true;
            EventManager.SetTerminalInUse.AddListener(TerminalGeneral.OnSetTerminalInUse);

            //TeleporterStuff
            EventManager.NormalTPFound.AddListener(Teleporters.OnNormalAwake);
            EventManager.InverseTPFound.AddListener(Teleporters.OnInverseAwake);

            //GameStuff
            EventManager.GameNetworkManagerStart.AddListener(GameStuff.OnGameStart);
            EventManager.StartOfRoundStart.AddListener(GameStuff.OnStartOfRoundStart);
            EventManager.StartOfRoundStartGame.AddListener(GameStuff.OnStartGame);
            EventManager.PlayerSpawn.AddListener(GameStuff.OnPlayerSpawn);

            //PlayerStuff
            OpenLib.PlayerUpdatePatch.usePatch = true;
            EventManager.PlayerIsInShip.AddListener(AlwaysOnStuff.PlayerShipChanged);
            EventManager.PlayerIsDead.AddListener(AlwaysOnStuff.IsPlayerDead);

            //Unique
            EventManager.GetNewDisplayText.AddListener(TerminalParse.OnNewDisplayText);

            //CamEvents
            CamEvents.UpdateCamsEvent.AddListener(CamEvents.OnUpdateCamsEvent);
            CamEvents.UpdateTextures.AddListener(CamEvents.GetTextures);
        }

        internal static void OnTerminalAwake(Terminal instance)
        {
            Plugin.instance.Terminal = instance;
            Plugin.MoreLogs($"Setting Plugin.instance.Terminal");
            CacheDefaultDisplayTexts();
            FontStuff.SetCachedDefault();
            StuffForLibrary.AddCommands(); //replaced addkeywords
            AlwaysOnStuff.screenSettings ??= new(ConfigSettings.TerminalScreen.Value);
            if(Plugin.instance.CruiserTerm)
                Compatibility.CruiserTerm.CreateDenyKeyword();
        }

        internal static void CacheDefaultDisplayTexts()
        {
            if (DynamicBools.TryGetKeyword("Other", out TerminalKeyword otherWord))
            {
                if (GameStuff.oneTimeOnly)
                    otherWord.specialKeywordResult.displayText = OriginalOtherText;
                else
                    OriginalOtherText = otherWord.specialKeywordResult.displayText;
            }
            else
                Plugin.WARNING("Unable to find other command at awake!");

        }
    }
}
