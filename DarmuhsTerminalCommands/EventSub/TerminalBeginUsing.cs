using static TerminalStuff.TerminalEvents;
using static TerminalStuff.EventSub.TerminalStart;
using UnityEngine.InputSystem;
using OpenLib.CoreMethods;

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

            if(!ViewCommands.isVideoPlaying)
                StartofHandling.CheckNetNode(Plugin.instance.Terminal.currentNode);
        }

        internal static void StartUsingTerminalCheck(Terminal instance)
        {
            TerminalNode nextNode = null;

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
                Plugin.Spam("Syncing items between players for refund command");
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
                Plugin.Spam("disabling cams views, alwaysOnDisplay is [DISABLED]");
                SplitViewChecks.DisableSplitView("neither");
                ViewCommands.isVideoPlaying = false;

                //Always load to last node or start if alwayson disabled
                if(lastNode.displayText == "" || !ConfigSettings.CacheLastTerminalPage.Value)
                {
                    instance.LoadNewNode(instance.terminalNodes.specialNodes.ToArray()[1]);
                    nextNode = instance.terminalNodes.specialNodes.ToArray()[1];
                }
                else
                {
                    instance.LoadNewNode(lastNode);
                    nextNode = lastNode;
                }
                    

                if (lastText.Length > 0 && ConfigSettings.CacheLastTerminalPage.Value)
                    LogicHandling.SetTerminalInput(lastText);
            }
            else
            {
                Plugin.MoreLogs("Terminal is Always On, checking for active monitoring to return to.");
                if (Plugin.instance.isOnMirror || Plugin.instance.isOnCamera || Plugin.instance.isOnMap || Plugin.instance.isOnMiniCams || Plugin.instance.isOnMiniMap || Plugin.instance.isOnOverlay)
                {
                    Plugin.Spam($"[returning to camera-type node during AOD]\nMap: {Plugin.instance.isOnMap} \nCams: {Plugin.instance.isOnCamera} \nMiniMap: {Plugin.instance.isOnMiniMap} \nMiniCams: {Plugin.instance.isOnMiniCams} \nOverlay: {Plugin.instance.isOnOverlay}\nMirror: {Plugin.instance.isOnMirror}");
                    if (lastText.Length > 0 && ConfigSettings.CacheLastTerminalPage.Value)
                        LogicHandling.SetTerminalInput(lastText);

                    return;
                }
                else if(ViewCommands.isVideoPlaying && Plugin.instance.Terminal.currentNode == VideoManager.videoPlayerNode)
                {
                    Plugin.Spam($"VideoPlayer: {ViewCommands.isVideoPlaying}\nCurrently Playing: {VideoManager.currentlyPlaying}");
                    return;
                }
                else
                {
                    Plugin.Spam($"[no matching camera-type nodes during AOD]\nMap: {Plugin.instance.isOnMap} \nCams: {Plugin.instance.isOnCamera} \nMiniMap: {Plugin.instance.isOnMiniMap} \nMiniCams: {Plugin.instance.isOnMiniCams} \nOverlay: {Plugin.instance.isOnOverlay}\nMirror: {Plugin.instance.isOnMirror}\nVideoPlayer: {ViewCommands.isVideoPlaying}");

                    if (ConfigSettings.CacheLastTerminalPage.Value)
                    {
                        if (lastNode == null)
                        {
                            Plugin.WARNING("lastNode terminalNode is null, cached pages failed!");
                            return;
                        }

                        Plugin.Spam($"lastNode Length - {lastNode.displayText.Length}");

                        if (lastNode.displayText.Length < 1)
                        {
                            instance.LoadNewNode(instance.terminalNodes.specialNodes.ToArray()[1]);
                            nextNode = instance.terminalNodes.specialNodes.ToArray()[1];
                        }
                            
                        else if (!Plugin.instance.splitViewCreated && (bool)Plugin.instance.Terminal.displayingPersistentImage)
                        {
                            if (lastText.Length > 0)
                                LogicHandling.SetTerminalInput(lastText);
                        }
                        else
                        {
                            instance.LoadNewNode(lastNode);
                            nextNode = lastNode;
                        }
                            
                    }

                    if (lastText.Length > 0 && ConfigSettings.CacheLastTerminalPage.Value)
                        LogicHandling.SetTerminalInput(lastText);
                }
            }

            if (ConfigSettings.networkedNodes.Value)
            {
                if(nextNode == null)
                {
                    Plugin.WARNING("Failed to grab nextNode for server sync!");
                    return;
                }    
                Plugin.Spam("sending current node to other users");
                NetHandler.NetNodeReset(true);
                NetHandler.Instance.NodeLoadServerRpc(Plugin.instance.Terminal.topRightText.text, nextNode.name, nextNode.displayText);
            }
        }
        //end of void
    }
}
