namespace TerminalStuff
{
    internal class SpecialConfirmationLogic
    {

        internal static string RestartAsk()
        {
            string displayText = "Restart Lobby?\n\n\n\n\n\n\n\n\n\n\n\nPlease CONFIRM or DENY.\n";
            return displayText;
        }

        internal static string RestartDeny()
        {
            string displayText = $"Restart lobby cancelled...\n\n";
            return displayText;
        }

        internal static string RestartAction()
        {
            if (!StartOfRound.Instance.inShipPhase)
            {
                string displayText = "This can only be done in orbit...\n\n";
                return displayText;
            }
            else if (!GameNetworkManager.Instance.localPlayerController.isHostPlayerObject)
            {
                string displayText = "Only the host can do this...\r\n";
                return displayText;
            }

            else
            {
                string displayText = "Restart lobby confirmed, getting new ship...\n\n";
                NetHandler.Instance.QuickRestartServerRpc();
                Plugin.MoreLogs("restarting lobby");
                return displayText;
            }

        }

    }
}
