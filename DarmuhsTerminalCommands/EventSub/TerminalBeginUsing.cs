﻿using static TerminalStuff.TerminalEvents;
using static TerminalStuff.EventSub.TerminalStart;
using static OpenLib.ConfigManager.ConfigSetup;
using UnityEngine.InputSystem;

namespace TerminalStuff.EventSub
{
    internal class TerminalBeginUsing
    {
        public static void OnTerminalBeginUse()
        {
            Plugin.MoreLogs("Start Using Terminal Postfix");
            
            if (Plugin.instance.Terminal == null)
            {
                Plugin.ERROR("FATAL ERROR: Terminal Instance is NULL");
                return;
            }

                
            StartUsingTerminalCheck(Plugin.instance.Terminal);

            if (StartOfRound.Instance.localPlayerController != null)
                ShouldLockPlayerCamera(false, StartOfRound.Instance.localPlayerController);

            if (Plugin.instance.Terminal.currentNode == null)
            {
                Plugin.WARNING("WARNING: currentNode is NULL, loading home page node");
                Plugin.instance.Terminal.LoadNewNode(Plugin.instance.Terminal.terminalNodes.specialNodes.ToArray()[1]);
            }
                

            if (ConfigSettings.TerminalStuffMain.specialListNum.ContainsKey(Plugin.instance.Terminal.currentNode))
                return;

            StartofHandling.CheckNetNode(Plugin.instance.Terminal.currentNode);
        }

        internal static void StartUsingTerminalCheck(Terminal instance)
        {

            if (ConfigSettings.TerminalAutoComplete.Value)
            {
                if (Plugin.instance.removeTab)
                {
                    Plugin.Spam("tab is disabled to quit terminal");
                    instance.playerActions.m_Movement_OpenMenu.Disable();
                    instance.playerActions.m_Movement_OpenMenu.ApplyBindingOverride(new InputBinding { path = "<Keyboard>/tab", overridePath = "" });
                    instance.playerActions.m_Movement_OpenMenu.Enable();
                    HUDManager.Instance.ChangeControlTip(0, "Quit terminal : [Esc]", true);
                }

            }

            //refund init
            if (ConfigSettings.terminalRefund.Value && ConfigSettings.ModNetworking.Value)
            {
                Plugin.MoreLogs("Syncing items between players for refund command");
                NetHandler.Instance.SyncDropShipServerRpc();
            }

            //walkie functions
            if (ConfigSettings.walkieTerm.Value)
            {
                Plugin.Spam("Starting TalkinTerm Coroutine");
                instance.StartCoroutine(WalkieTerm.TalkinTerm());
            }

            if (ConfigSettings.terminalShortcuts.Value && ShortcutBindings.keyActions.Count > 0)
            {
                Plugin.Spam("Listening for shortcuts");
                instance.StartCoroutine(ShortcutBindings.TerminalShortCuts());
            }

            //AlwaysOn Functions
            if (!alwaysOnDisplay)
            {
                Plugin.Spam("disabling cams views");
                SplitViewChecks.DisableSplitView("neither");
                ViewCommands.isVideoPlaying = false;

                //Always load to start if alwayson disabled
                instance.LoadNewNode(instance.terminalNodes.specialNodes.ToArray()[1]);
            }
            else
            {
                Plugin.MoreLogs("Terminal is Always On, checking for active monitoring to return to.");
                if (Plugin.instance.isOnMirror || Plugin.instance.isOnCamera || Plugin.instance.isOnMap || Plugin.instance.isOnMiniCams || Plugin.instance.isOnMiniMap || Plugin.instance.isOnOverlay)
                {
                    int nodeNum = StartofHandling.FindViewIntByString();
                    if (nodeNum == -1)
                        return;

                    TerminalNode returnNode = StartofHandling.FindViewNode(nodeNum);
                    
                    if (returnNode == null)
                        return;

                    instance.LoadNewNode(returnNode);
                    Plugin.Spam($"[returning to camera-type node during AOD]\nMap: {Plugin.instance.isOnMap} \nCams: {Plugin.instance.isOnCamera} \nMiniMap: {Plugin.instance.isOnMiniMap} \nMiniCams: {Plugin.instance.isOnMiniCams} \nOverlay: {Plugin.instance.isOnOverlay}\nMirror: {Plugin.instance.isOnMirror}");
                    return;
                }
                else
                {
                    Plugin.Spam($"[no matching camera-type nodes during AOD]\nMap: {Plugin.instance.isOnMap} \nCams: {Plugin.instance.isOnCamera} \nMiniMap: {Plugin.instance.isOnMiniMap} \nMiniCams: {Plugin.instance.isOnMiniCams} \nOverlay: {Plugin.instance.isOnOverlay}\nMirror: {Plugin.instance.isOnMirror}");
                    instance.LoadNewNode(instance.terminalNodes.specialNodes.ToArray()[1]);
                    return;
                }
            }
        }
        //end of void
    }
}
