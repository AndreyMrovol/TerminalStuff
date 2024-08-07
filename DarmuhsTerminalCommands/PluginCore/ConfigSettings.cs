using BepInEx.Configuration;
using static OpenLib.ConfigManager.ConfigSetup;
using static OpenLib.Common.CommonTerminal;
using OpenLib.ConfigManager;
using System.Collections.Generic;
using OpenLib.CoreMethods;
using System;

namespace TerminalStuff
{
    public static class ConfigSettings
    {
        public static List<ManagedConfig> TerminalStuffBools = [];
        public static MainListing TerminalStuffMain;

        //keybinds
        public static ConfigEntry<string> walkieTermKey { get; internal set; }
        public static ConfigEntry<string> walkieTermMB { get; internal set; }
        public static ConfigEntry<string> keyActionsConfig { get; internal set; }

        //cams special
        public static ConfigEntry<bool> camsUseDetectedMods { get; internal set; }
        public static ConfigEntry<string> obcResolutionMirror {  get; internal set; }
        public static ConfigEntry<string> obcResolutionBodyCam { get; internal set; }

        //establish commands that can be turned on or off here
        public static ConfigEntry<bool> terminalShortcutCommands { get; internal set; }
        public static ConfigEntry<bool> ModNetworking { get; internal set; }
        public static ConfigEntry<bool> networkedNodes { get; internal set; } //enable or disable networked terminal nodes (beta)
        public static ConfigEntry<bool> terminalClock { get; internal set; } //Clock object itself
        public static ConfigEntry<bool> extensiveLogging { get; internal set; }
        public static ConfigEntry<bool> developerLogging { get; internal set; }
        public static ConfigEntry<bool> walkieTerm { get; internal set; } //Use walkie at terminal function
        public static ConfigEntry<bool> terminalShortcuts { get; internal set; } //adds the bind keyword and the enumerator checking for shortcuts
        public static ConfigEntry<bool> terminalLobby { get; internal set; } //lobby name command
        public static ConfigEntry<bool> terminalCams { get; internal set; } //cams command
        public static ConfigEntry<bool> terminalQuit { get; internal set; } //quit command
        public static ConfigEntry<bool> terminalClear { get; internal set; } //clear command
        public static ConfigEntry<bool> terminalLoot { get; internal set; } //loot command
        public static ConfigEntry<bool> terminalVideo { get; internal set; } //video command
        public static ConfigEntry<bool> terminalHeal { get; internal set; } //heal command
        public static ConfigEntry<bool> terminalFov { get; internal set; } //Fov command
        public static ConfigEntry<bool> terminalGamble { get; internal set; } //Gamble command
        public static ConfigEntry<bool> terminalLever { get; internal set; } //Lever command
        public static ConfigEntry<bool> terminalDanger { get; internal set; } //Danger command
        public static ConfigEntry<bool> terminalVitals { get; internal set; } //Vitals command
        public static ConfigEntry<bool> terminalBioScan { get; internal set; } //BioScan command
        public static ConfigEntry<bool> terminalBioScanPatch { get; internal set; } //BioScan Upgrade command
        public static ConfigEntry<bool> terminalVitalsUpgrade { get; internal set; } //Vitals Upgrade command
        public static ConfigEntry<bool> terminalTP { get; internal set; } //Teleporter command
        public static ConfigEntry<bool> terminalITP { get; internal set; } //Inverse Teleporter command
        public static ConfigEntry<bool> terminalMods { get; internal set; } //Modlist command
        public static ConfigEntry<bool> terminalKick { get; internal set; } //Kick command (host only)
        public static ConfigEntry<bool> terminalFcolor { get; internal set; } //Flashlight color command
        public static ConfigEntry<bool> terminalMap { get; internal set; } //Map shortcut
        public static ConfigEntry<bool> terminalMinimap { get; internal set; } //Minimap command
        public static ConfigEntry<bool> terminalMinicams { get; internal set; } //Minicams command
        public static ConfigEntry<bool> terminalOverlay { get; internal set; } //Overlay cams command
        public static ConfigEntry<bool> terminalDoor { get; internal set; } //Door Toggle command
        public static ConfigEntry<bool> terminalLights { get; internal set; } //Light Toggle command
        public static ConfigEntry<bool> terminalScolor { get; internal set; } //Light colors command
        public static ConfigEntry<bool> terminalAlwaysOn { get; internal set; } //AlwaysOn command
        public static ConfigEntry<bool> terminalLink { get; internal set; } //Link command
        public static ConfigEntry<bool> terminalLink2 { get; internal set; } //Link2 command
        public static ConfigEntry<bool> terminalRandomSuit { get; internal set; } //RandomSuit command
        public static ConfigEntry<bool> terminalClockCommand { get; internal set; } //toggle clock command
        public static ConfigEntry<bool> terminalListItems { get; internal set; } //List Items Command
        public static ConfigEntry<bool> terminalLootDetail { get; internal set; } //List Scrap Command
        public static ConfigEntry<bool> terminalMirror { get; internal set; } //mirror command
        public static ConfigEntry<bool> terminalRefund { get; internal set; } //refund command
        public static ConfigEntry<bool> terminalRestart { get; internal set; } //restart command
        public static ConfigEntry<bool> terminalPrevious { get; internal set; } //previous switch command
        public static ConfigEntry<bool> terminalRouteRandom { get; internal set; } // route random command
        public static ConfigEntry<bool> terminalRefreshCustomization { get; internal set; } //refresh customization command
        public static ConfigEntry<bool> terminalRadarZoom { get; internal set; }


        //features
        public static ConfigEntry<bool> terminalPurchasePacks { get; internal set; } // purchase packs feature



        //Strings for display messages
        public static ConfigEntry<bool> canOpenDoorInSpace { get; internal set; } //bool to allow for opening door in space
        public static ConfigEntry<string> doorOpenString { get; internal set; } //Door String
        public static ConfigEntry<string> doorCloseString { get; internal set; } //Door String
        public static ConfigEntry<string> doorSpaceString { get; internal set; } //Door String
        public static ConfigEntry<string> quitString { get; internal set; } //Quit String
        public static ConfigEntry<string> leverString { get; internal set; } //Lever String
        public static ConfigEntry<string> videoStartString { get; internal set; } //lol, start video string
        public static ConfigEntry<string> videoStopString { get; internal set; } //lol, stop video string
        public static ConfigEntry<string> tpMessageString { get; internal set; } //TP Message String
        public static ConfigEntry<string> itpMessageString { get; internal set; } //TP Message String
        public static ConfigEntry<string> vitalsPoorString { get; internal set; } //Vitals can't afford string
        public static ConfigEntry<string> vitalsUpgradePoor { get; internal set; } //Vitals Upgrade can't afford string
        public static ConfigEntry<string> healIsFullString { get; internal set; } //full health string
        public static ConfigEntry<string> healString { get; internal set; } //healing player string
        public static ConfigEntry<string> camString { get; internal set; } //Cameras on string
        public static ConfigEntry<string> camString2 { get; internal set; } //Cameras off string
        public static ConfigEntry<string> mapString { get; internal set; } //map on string
        public static ConfigEntry<string> mapString2 { get; internal set; } //map off string
        public static ConfigEntry<string> ovString { get; internal set; } //overlay on string
        public static ConfigEntry<string> ovString2 { get; internal set; } //overlay off string
        public static ConfigEntry<string> mmString { get; internal set; } //minimap on string
        public static ConfigEntry<string> mmString2 { get; internal set; } //minimap off string
        public static ConfigEntry<string> mcString { get; internal set; } //minicam on string
        public static ConfigEntry<string> mcString2 { get; internal set; } //minicam off string


        //Cost configs
        public static ConfigEntry<int> vitalsCost { get; internal set; } //Cost of Vitals Command
        public static ConfigEntry<int> vitalsUpgradeCost { get; internal set; } //Cost of Vitals Upgrade Command
        public static ConfigEntry<int> bioScanUpgradeCost { get; internal set; } //Cost of Enemy Scan Upgrade Command
        public static ConfigEntry<int> bioScanScanCost { get; internal set; } //Cost of Enemy Scan Command

        //Other config items
        public static ConfigEntry<int> gambleMinimum { get; internal set; } //Minimum amount of credits needed to gamble
        public static ConfigEntry<bool> gamblePityMode { get; internal set; } //enable or disable pity for gamblers
        public static ConfigEntry<int> gamblePityCredits { get; internal set; } //Pity Credits for losers
        public static ConfigEntry<string> gamblePoorString { get; internal set; } //gamble credits too low string
        public static ConfigEntry<string> videoFolderPath { get; internal set; } //Specify a different folder with videos
        public static ConfigEntry<bool> videoSync { get; internal set; } //Should videos be synced between players (good for AOD)
        public static ConfigEntry<bool> leverConfirmOverride { get; internal set; } //disable confirmation check for lever
        public static ConfigEntry<bool> restartConfirmOverride { get; internal set; } //disable confirmation check for lever
        public static ConfigEntry<bool> camsNeverHide { get; internal set; }
        public static ConfigEntry<string> defaultCamsView { get; internal set; }
        public static ConfigEntry<int> ovOpacity { get; internal set; } //Opacity Percentage for Overlay Cams View
        public static ConfigEntry<string> customLink { get; internal set; }
        public static ConfigEntry<string> customLink2 { get; internal set; }
        public static ConfigEntry<string> customLinkHint { get; internal set; }
        public static ConfigEntry<string> customLink2Hint { get; internal set; }
        public static ConfigEntry<string> homeLine1 { get; internal set; }
        public static ConfigEntry<string> homeLine2 { get; internal set; }
        public static ConfigEntry<string> homeLine3 { get; internal set; }
        public static ConfigEntry<string> homeHelpLines { get; internal set; }
        public static ConfigEntry<string> homeTextArt { get; internal set; }
        public static ConfigEntry<bool> alwaysOnAtStart { get; internal set; }
        public static ConfigEntry<bool> alwaysOnDynamic { get; internal set; }
        public static ConfigEntry<bool> alwaysOnWhileDead { get; internal set; }
        public static ConfigEntry<int> aodOffDelay { get; internal set; }
        public static ConfigEntry<string> routeRandomBannedWeather { get; internal set; }
        public static ConfigEntry<int> routeRandomCost { get; internal set; }
        public static ConfigEntry<string> purchasePackCommands { get; internal set; }

        //keyword strings (terminalapi)
        public static ConfigEntry<string> alwaysOnKeywords { get; internal set; } //string to match keyword
        public static ConfigEntry<string> minimapKeywords { get; internal set; }
        public static ConfigEntry<string> minicamsKeywords { get; internal set; }
        public static ConfigEntry<string> overlayKeywords { get; internal set; }
        public static ConfigEntry<string> doorKeywords { get; internal set; }
        public static ConfigEntry<string> lightsKeywords { get; internal set; }
        public static ConfigEntry<string> modsKeywords { get; internal set; }
        public static ConfigEntry<string> tpKeywords { get; internal set; }
        public static ConfigEntry<string> itpKeywords { get; internal set; }
        public static ConfigEntry<string> quitKeywords { get; internal set; }
        public static ConfigEntry<string> videoKeywords { get; internal set; }
        public static ConfigEntry<string> clearKeywords { get; internal set; }
        public static ConfigEntry<string> dangerKeywords { get; internal set; }
        public static ConfigEntry<string> healKeywords { get; internal set; }
        public static ConfigEntry<string> lootKeywords { get; internal set; }
        public static ConfigEntry<string> camsKeywords { get; internal set; }
        public static ConfigEntry<string> mapKeywords { get; internal set; }
        public static ConfigEntry<string> mirrorKeywords { get; internal set; }
        public static ConfigEntry<string> randomSuitKeywords { get; internal set; }
        public static ConfigEntry<string> clockKeywords { get; internal set; }
        public static ConfigEntry<string> ListItemsKeywords { get; internal set; } //List Items Command
        public static ConfigEntry<string> ListScrapKeywords { get; internal set; } //List Scrap Command
        public static ConfigEntry<string> randomRouteKeywords { get; internal set; }
        public static ConfigEntry<string> lobbyKeywords { get; internal set; } //show lobby name keywords
        public static ConfigEntry<string> refreshcustomizationKWs { get; internal set; }
        public static ConfigEntry<string> radarZoomKWs { get; internal set; }


        //terminal patcher keywords
        public static ConfigEntry<string> fcolorKeyword { get; internal set; }
        public static ConfigEntry<string> gambleKeyword { get; internal set; }
        public static ConfigEntry<string> leverKeywords { get; internal set; }
        public static ConfigEntry<string> scolorKeyword { get; internal set; }
        public static ConfigEntry<string> linkKeyword { get; internal set; }
        public static ConfigEntry<string> link2Keyword { get; internal set; }

        //Quality of Life features
        public static ConfigEntry<bool> LockCameraInTerminal { get; internal set; }
        public static ConfigEntry<string> TerminalLightBehaviour { get; internal set; }
        public static ConfigEntry<bool> TerminalAutoComplete { get; internal set; }
        public static ConfigEntry<string> TerminalAutoCompleteKey { get; internal set; }
        public static ConfigEntry<int> TerminalAutoCompleteMaxCount { get; internal set; }
        public static ConfigEntry<bool> TerminalHistory {  get; internal set; }
        public static ConfigEntry<int> TerminalHistoryMaxCount { get; internal set; }
        public static ConfigEntry<bool> TerminalConflictResolution { get; internal set; }
        public static ConfigEntry<float> TerminalRadarDefaultZoom { get; internal set; }
        public static ConfigEntry<string> TerminalFillEmptyText {  get; internal set; }  

        //Terminal Customization
        public static ConfigEntry<bool> TerminalCustomization { get; internal set; }
        public static ConfigEntry<string> TerminalColor { get; internal set; }
        public static ConfigEntry<string> TerminalButtonsColor { get; internal set; }
        public static ConfigEntry<string> TerminalKeyboardColor { get; internal set; }
        public static ConfigEntry<string> TerminalTextColor { get; internal set; }
        public static ConfigEntry<string> TerminalMoneyColor { get; internal set; }
        public static ConfigEntry<string> TerminalMoneyBGColor { get; internal set; }
        public static ConfigEntry<float> TerminalMoneyBGAlpha { get; internal set; }
        public static ConfigEntry<string> TerminalCaretColor { get; internal set; }
        public static ConfigEntry<string> TerminalScrollbarColor { get; internal set; }
        public static ConfigEntry<string> TerminalScrollBGColor { get; internal set; }
        public static ConfigEntry<string> TerminalClockColor { get; internal set; }
        public static ConfigEntry<string> TerminalLightColor { get; internal set; }
        public static ConfigEntry<bool> TerminalCustomBG { get; internal set; }
        public static ConfigEntry<string> TerminalCustomBGColor { get; internal set; }
        public static ConfigEntry<float> TerminalCustomBGAlpha { get; internal set; }

        //font stuff
        public static ConfigEntry<string> CustomFontPath { get; internal set; }
        public static ConfigEntry<string> CustomFontName { get; internal set; }
        public static ConfigEntry<int> CustomFontSizeMain { get; internal set; }
        public static ConfigEntry<int> CustomFontSizeMoney { get; internal set; }
        public static ConfigEntry<int> CustomFontSizeClock { get; internal set; }


        public static void BindConfigSettings()
        {
            //Network Configs
            ModNetworking = MakeBool(Plugin.instance.Config, "Networking", "ModNetworking", true, "Disable this if you want to disable networking and use this mod as a Client-sided mod");
            networkedNodes = MakeBool(Plugin.instance.Config, "Networking", "networkedNodes", false, "Enable networked Always-On Display & displaying synced terminal nodes");

            AddManagedBool(networkedNodes, defaultManaged, true, "", "");

            terminalClock = MakeBool(Plugin.instance.Config, "Quality of Life", "terminalClock", false, "Enable or Disable the terminalClock feature in it's entirety");
            walkieTerm = MakeBool(Plugin.instance.Config, "Quality of Life", "walkieTerm", true, "Enable or Disable the ability to use a walkie from your inventory at the terminal (vanilla method still works)");
            terminalShortcuts = MakeBool(Plugin.instance.Config, "Quality of Life", "terminalShortcuts", true, "Enable this for the ability to bind commands to any valid key (also enables the \"bind\" keyword.");
            extensiveLogging = MakeBool(Plugin.instance.Config, "Debug", "extensiveLogging", false, "Enable or Disable extensive logging for this mod.");
            developerLogging = MakeBool(Plugin.instance.Config, "Debug", "developerLogging", false, "Enable or Disable developer logging for this mod. (this will fill your log file FAST)");
            keyActionsConfig = MakeString(Plugin.instance.Config,"Quality of Life", "keyActionsConfig", "", "Stored keybinds, don't modify this unless you know what you're doing!");
            purchasePackCommands = MakeString(Plugin.instance.Config,"Comfort Configuration", "purchasePackCommands", "Essentials:pro,shov,walkie;PortalPack:teleporter,inverse", "List of purchase pack commands to create. Format is command:item1,item2,etc.;next command:item1,item2");

            Plugin.Spam("network configs section done");

            //keybinds
            walkieTermKey = MakeString(Plugin.instance.Config,"Quality of Life", "walkieTermKey", "LeftAlt", "Key used to activate your walkie while at the terminal, see here for valid key names https://docs.unity3d.com/Packages/com.unity.inputsystem@1.0/api/UnityEngine.InputSystem.Key.html");
            walkieTermMB = MakeString(Plugin.instance.Config,"Quality of Life", "walkieTermMB", "Left", "Mousebutton used to activate your walkie while at the terminal, see here for valid button names https://docs.unity3d.com/Packages/com.unity.inputsystem@1.3/api/UnityEngine.InputSystem.LowLevel.MouseButton.html");

            Plugin.Spam("keybind configs section done");

            //Cams Mod Config
            camsUseDetectedMods = MakeBool(Plugin.instance.Config, "Extras Configuration", "camsUseDetectedMods", true, "With this enabled, this mod will detect if another mod that adds player cams is enabled and use the mod's camera for all cams commands. Currently detects the following: Helmet Cameras by Rick Arg, Body Cameras by Solo, OpenBodyCams by ");

            //override configs
            leverConfirmOverride = MakeBool(Plugin.instance.Config, "Controls Configuration", "leverConfirmOverride", false, "Setting this to true will disable the confirmation check for the <lever> command.");
            restartConfirmOverride = MakeBool(Plugin.instance.Config, "Controls Configuration", "restartConfirmOverride", false, "Setting this to true will disable the confirmation check for the <restart> command.");

            //Keyword configs (multiple per config item)
            alwaysOnKeywords = MakeString(Plugin.instance.Config, "Custom Keywords", "alwaysOnKeywords", "alwayson;always on", "This semi-colon separated list is all keywords that can be used in terminal to return <alwayson> command");
            camsKeywords = MakeString(Plugin.instance.Config, "Custom Keywords", "camsKeywords", "cameras; show cams; cams", "This semi-colon separated list is all keywords that can be used in terminal to return <cams> command");
            mapKeywords = MakeString(Plugin.instance.Config, "Custom Keywords", "mapKeywords", "show map; map; view monitor", "Additional This semi-colon separated list is all keywords that can be used in terminal to return <map> command");
            minimapKeywords = MakeString(Plugin.instance.Config, "Custom Keywords", "minimapKeywords", "minimap; show minimap", "This semi-colon separated list is all keywords that can be used in terminal to return <minimap> command.");
            minicamsKeywords = MakeString(Plugin.instance.Config, "Custom Keywords", "minicamsKeywords", "minicams; show minicams", "This semi-colon separated list is all keywords that can be used in terminal to return <minicams> command");
            overlayKeywords = MakeString(Plugin.instance.Config, "Custom Keywords", "overlayKeywords", "overlay; show overlay", "This semi-colon separated list is all keywords that can be used in terminal to return <overlay> command");
            mirrorKeywords = MakeString(Plugin.instance.Config, "Custom Keywords", "mirrorKeywords", "mirror; reflection; show mirror", "This semi-colon separated list is all keywords that can be used in terminal to return <cams> command");
            doorKeywords = MakeString(Plugin.instance.Config, "Custom Keywords", "doorKeywords", "door; toggle door", "This semi-colon separated list is all keywords that can be used in terminal to return <door> command");
            lightsKeywords = MakeString(Plugin.instance.Config, "Custom Keywords", "lightsKeywords", "lights; toggle lights", "This semi-colon separated list is all keywords that can be used in terminal to return <lights> command");
            modsKeywords = MakeString(Plugin.instance.Config, "Custom Keywords", "modsKeywords", "modlist; mods; show mods", "This semi-colon separated list is all keywords that can be used in terminal to return <mods> command");
            tpKeywords = MakeString(Plugin.instance.Config, "Custom Keywords", "tpKeywords", "tp; use teleporter; teleport", "This semi-colon separated list is all keywords that can be used in terminal to return <tp> command");
            itpKeywords = MakeString(Plugin.instance.Config, "Custom Keywords", "itpKeywords", "itp; use inverse; inverse", "This semi-colon separated list is all keywords that can be used in terminal to return <itp> command");
            quitKeywords = MakeString(Plugin.instance.Config, "Custom Keywords", "quitKeywords", "quit;exit;leave", "This semi-colon separated list is all keywords that can be used in terminal to return <quit> command");
            videoKeywords = MakeString(Plugin.instance.Config, "Custom Keywords", "videoKeywords", "lol; play video; lolxd; hahaha", "This semi-colon separated list is all keywords that can be used in terminal to return <video> command");
            clearKeywords = MakeString(Plugin.instance.Config, "Custom Keywords", "clearKeywords", "clear;wipe;clean", "This semi-colon separated list is all keywords that can be used in terminal to return <clear> command");
            dangerKeywords = MakeString(Plugin.instance.Config, "Custom Keywords", "dangerKeywords", "danger;hazard;show danger; show hazard", "This semi-colon separated list is all keywords that can be used in terminal to return <danger> command");
            healKeywords = MakeString(Plugin.instance.Config, "Custom Keywords", "healKeywords", "healme; heal me; heal; medic", "This semi-colon separated list is all keywords that can be used in terminal to return <heal> command");
            lootKeywords = MakeString(Plugin.instance.Config, "Custom Keywords", "lootKeywords", "loot; shiploot; show loot", "This semi-colon separated list is all keywords that can be used in terminal to return <loot> command");
            randomSuitKeywords = MakeString(Plugin.instance.Config, "Custom Keywords", "randomSuitKeywords", "randomsuit; random suit", "This semi-colon separated list is all keywords that can be used in terminal to return <randomsuit> command");
            clockKeywords = MakeString(Plugin.instance.Config, "Custom Keywords", "clockKeywords", "clock; show clock; time; show time", "This semi-colon separated list is all keywords that can be used in terminal to toggle Terminal Clock display");
            ListItemsKeywords = MakeString(Plugin.instance.Config, "Custom Keywords", "ListItemsKeywords", "show items; get items; listitem", "This semi-colon separated list is all keywords that can be used in terminal to return <itemlist> command");
            ListScrapKeywords = MakeString(Plugin.instance.Config, "Custom Keywords", "ListScrapKeywords", "loot detail; listloost", "This semi-colon separated list is all keywords that can be used in terminal to return <lootlist> command");
            randomRouteKeywords = MakeString(Plugin.instance.Config, "Custom Keywords", "randomRouteKeywords", "route random; random moon", "This semi-colon separated list is all keywords that can be used in terminal to return <randomRoute> command");
            lobbyKeywords = MakeString(Plugin.instance.Config, "Custom Keywords", "lobbyKeywords", "lobby; show lobby; lobby name; show lobby name", "This semi-colon separated list is all keywords that can be used in terminal to return <lobby> command");
            refreshcustomizationKWs = MakeString(Plugin.instance.Config, "Custom Keywords", "refreshcustomizationKWs", "refresh colors; paintme; customize", "This semi-colon separated list is all keywords that can be used to run the terminalRefreshCustomization command");
            radarZoomKWs = MakeString(Plugin.instance.Config, "Custom Keywords", "radarZoomKWs", "zoom; enhance; radar zoom", "This semi-colon separated list is all keywords that can be used in terminal to return <lootlist> command");

            Plugin.Spam("keyword configs section done");

            //terminal patcher keywords
            fcolorKeyword = MakeString(Plugin.instance.Config, "Custom Keywords", "fcolorKeyword", "fcolor", "Set the keyword that can be used in terminal to return <fcolor> command");
            gambleKeyword = MakeString(Plugin.instance.Config, "Custom Keywords", "gambleKeyword", "gamble", "Set the keyword that that can be used in terminal to return <gamble> command");
            leverKeywords = MakeString(Plugin.instance.Config, "Custom Keywords", "leverKeywords", "lever", "This semi-colon separated list is all keywords that can be used in terminal to return <lever> command");
            scolorKeyword = MakeString(Plugin.instance.Config, "Custom Keywords", "scolorKeyword", "scolor", "Set the keyword that that can be used in terminal to return <scolor> command");
            linkKeyword = MakeString(Plugin.instance.Config, "Custom Keywords", "linkKeyword", "link", "Set the keyword that that can be used in terminal to return <link> command");
            link2Keyword = MakeString(Plugin.instance.Config, "Custom Keywords", "link2Keyword", "link2", "Set the keyword that that can be used in terminal to return <link2> command");

            Plugin.Spam("special keyword configs section done");

            routeRandomBannedWeather = MakeString(Plugin.instance.Config, "Fun Configuration", "routeRandomBannedWeather", "Eclipsed;Flooded;Foggy", "This semi-colon separated list will be used to exclude moons from the route random command");
            routeRandomCost = MakeClampedInt(Plugin.instance.Config, "Fun Configuration", "routeRandomCost", 100, "Flat rate for running the route random command to get a random moon...", 0, 99999);


            //Cost configs
            vitalsCost = MakeInt(Plugin.instance.Config, "Upgrades", "vitalsCost", 10, "Credits cost to run Vitals Command each time it's run.");
            vitalsUpgradeCost = MakeInt(Plugin.instance.Config, "Upgrades", "vitalsUpgradeCost", 200, "Credits cost to upgrade Vitals command to not cost credits anymore.");
            bioScanUpgradeCost = MakeInt(Plugin.instance.Config, "Upgrades", "bioScanUpgradeCost", 300, "Credits cost to upgrade Bioscan command to provide detailed information on scanned enemies.");
            bioScanScanCost = MakeInt(Plugin.instance.Config, "Upgrades", "bioScanScanCost", 15, "Credits cost to run Bioscan command each time it's run. (scans for enemy information)");

            Plugin.Spam("cost configs section done");

            //------------------------------------------------MANAGED BOOLS START------------------------------------------------//

            terminalShortcutCommands = MakeBool(Plugin.instance.Config, "Comfort Commands (On/Off)", "terminalShortcutCommands", true, "Enable or disable shortCut commands (dependent on terminalShortcuts)");

            terminalLobby = MakeBool(Plugin.instance.Config, "Comfort Commands (On/Off)", "terminalLobby", true, "Check for the current lobby name");
            AddManagedBool(terminalLobby, defaultManaged, false, "COMFORT", lobbyKeywords, MoreCommands.GetLobbyName);

            terminalQuit = MakeBool(Plugin.instance.Config, "Comfort Commands (On/Off)", "terminalQuit", true, "Command to quit terminal");
            
            AddManagedBool(terminalQuit, defaultManaged, false, "COMFORT", quitKeywords, TerminalEvents.QuitTerminalCommand);

            terminalClear = MakeBool(Plugin.instance.Config, "Comfort Commands (On/Off)", "terminalClear", true, "Command to clear terminal text");

            AddManagedBool(terminalClear, defaultManaged, false, "COMFORT", clearKeywords, ClearText);

            terminalLoot = MakeBool(Plugin.instance.Config, "Extras Commands (On/Off)", "terminalLoot", true, "Command to show total onboard loot value"); 
            AddManagedBool(terminalLoot, defaultManaged, false, "EXTRAS", lootKeywords, AllTheLootStuff.GetLootSimple, 0, false);

            terminalHeal = MakeBool(Plugin.instance.Config, "Comfort Commands (On/Off)", "terminalHeal", true, "Command to heal yourself");
            AddManagedBool(terminalHeal, defaultManaged, false, "COMFORT", healKeywords, MoreCommands.HealCommand);

            terminalFov = MakeBool(Plugin.instance.Config, "Comfort Commands (On/Off)", "terminalFov", true, "Command to change your FOV");
            AddManagedBool(terminalFov, defaultManaged, false, "COMFORT", "fov", DynamicCommands.FovPrompt, 1, true, DynamicCommands.FovConfirm, DynamicCommands.FovDeny, "", "", "fov");

            terminalGamble = MakeBool(Plugin.instance.Config, "Fun Commands (On/Off)", "terminalGamble", true, "Command to gamble your credits, by percentage"); 
            AddManagedBool(terminalGamble, defaultManaged, true, "FUN", gambleKeyword, GambaCommands.Ask2Gamble, 1, true, GambaCommands.GambleConfirm, GambaCommands.GambleDeny, "", "", gambleKeyword.Value);

            if (leverConfirmOverride.Value)
            {
                terminalLever = MakeBool(Plugin.instance.Config, "Controls Commands (On/Off)", "terminalLever", true, "Pull the lever from terminal");
                AddManagedBool(terminalLever, defaultManaged, false, "CONTROLS", leverKeywords, ShipControls.LeverControlCommand);
            }
            else
            {
                terminalLever = MakeBool(Plugin.instance.Config, "Controls Commands (On/Off)", "terminalLever", true, "Pull the lever from terminal");
                AddManagedBool(terminalLever, defaultManaged, false, "CONTROLS", leverKeywords, ShipControls.AskLever, 1, true, ShipControls.LeverControlCommand, ShipControls.DenyLever);
            }

            if (restartConfirmOverride.Value)
            {
                terminalRestart = MakeBool(Plugin.instance.Config, "Controls Commands (On/Off)", "terminalRestart", true, "Command to restart the lobby (skips firing sequence)");
                AddManagedBool(terminalRestart, defaultManaged, true, "CONTROLS", "restart", SpecialConfirmationLogic.RestartAction);
            }
            else
            {
                terminalRestart = MakeBool(Plugin.instance.Config, "Controls Commands (On/Off)", "terminalRestart", true, "Command to restart the lobby (skips firing sequence)");
                AddManagedBool(terminalRestart, defaultManaged, true, "CONTROLS", "restart", SpecialConfirmationLogic.RestartAsk, 1, true, SpecialConfirmationLogic.RestartAction, SpecialConfirmationLogic.RestartDeny);
            }

            terminalDanger = MakeBool(Plugin.instance.Config, "Controls Commands (On/Off)", "terminalDanger", true, "Check moon danger level");
            AddManagedBool(terminalDanger, defaultManaged, false, "CONTROLS", dangerKeywords, MoreCommands.DangerCommand);

            terminalVitals = MakeBool(Plugin.instance.Config, "Extras Commands (On/Off)", "terminalVitals", true, "Scan player being monitored for their vitals");
            AddManagedBool(terminalVitals, defaultManaged, true, "EXTRAS", "vitals", CostCommands.VitalsCommand);

            terminalBioScan = MakeBool(Plugin.instance.Config, "Extras Commands (On/Off)", "terminalBioScan", true, "Scan for \"non-employee\" lifeforms.");
            AddManagedBool(terminalBioScan, defaultManaged, true, "EXTRAS", "bioscan", CostCommands.BioscanCommand);

            //----------------------------------upgrade managed bools----------------------------------//

            terminalBioScanPatch = MakeBool(Plugin.instance.Config, "Extras Commands (On/Off)", "terminalBioScanPatch", true, "Purchase-able upgrade patch to bioscan command. The command will provide more precise information after the upgrade.");
            AddManagedBool(terminalBioScanPatch, defaultManaged, true, "EXTRAS", "bioscanpatch", CostCommands.AskBioscanUpgrade, 2, true, CostCommands.PerformBioscanUpgrade, null, "", "You have opted out of purchasing the BioScanner 2.0 Upgrade Patch.\n\n", "", -1, "", "", bioScanUpgradeCost.Value, "BioscanPatch", true, 1);

            terminalVitalsUpgrade = MakeBool(Plugin.instance.Config, "Extras Commands (On/Off)", "terminalVitalsUpgrade", true, "Purchase-able upgrade to vitals command to make the cost of each vitals scan free!");
            AddManagedBool(terminalVitalsUpgrade, defaultManaged, true, "EXTRAS", "vitalspatch", CostCommands.AskVitalsUpgrade, 2, true, CostCommands.PerformVitalsUpgrade, null, "", "You have opted out of purchasing the Vitals Scanner Upgrade.\n\n", "", -1, "   ", "", vitalsUpgradeCost.Value, "VitalsPatch", true, 1);

            //----------------------------------upgrade managed bools----------------------------------//


            terminalMods = MakeBool(Plugin.instance.Config, "Comfort Commands (On/Off)", "terminalMods", true, "Command to see your active mods");
            AddManagedBool(terminalMods, defaultManaged, false, "COMFORT", modsKeywords, MoreCommands.ModListCommand);

            terminalKick = MakeBool(Plugin.instance.Config, "Comfort Commands (On/Off)", "terminalKick", false, "Enables kick command for host.");
            AddManagedBool(terminalKick, defaultManaged, false, "COMFORT", "kick", AdminCommands.KickPlayersAsk, 1, true, AdminCommands.KickPlayerConfirm, AdminCommands.KickPlayerDeny);

            terminalFcolor = MakeBool(Plugin.instance.Config, "Fun Commands (On/Off)", "terminalFcolor", true, "Command to change flashlight color.");
            AddManagedBool(terminalFcolor, defaultManaged, true, "FUN", fcolorKeyword, ColorCommands.FlashColorBase, 0, true, null, null, "", "", fcolorKeyword.Value);

            terminalScolor = MakeBool(Plugin.instance.Config, "Fun Commands (On/Off)", "terminalScolor", true, "Command to change ship lights colors.");
            AddManagedBool(terminalScolor, defaultManaged, true, "FUN", scolorKeyword, ColorCommands.ShipColorBase, 0, true, null, null, "", "", scolorKeyword.Value);

            terminalDoor = MakeBool(Plugin.instance.Config, "Controls Commands (On/Off)", "terminalDoor", true, "Command to open/close the ship door.");
            AddManagedBool(terminalDoor, defaultManaged, false, "CONTROLS", doorKeywords, ShipControls.BasicDoorCommand);

            terminalLights = MakeBool(Plugin.instance.Config, "Controls Commands (On/Off)", "terminalLights", true, "Command to toggle the ship lights");
            AddManagedBool(terminalLights, defaultManaged, false, "CONTROLS", lightsKeywords, ShipControls.BasicLightsCommand);

            //----------------------------------termview managed bools----------------------------------//

            terminalCams = MakeBool(Plugin.instance.Config, "Extras Commands (On/Off)", "terminalCams", true, "Command to toggle displaying cameras in terminal");
            AddManagedBool(terminalCams, TerminalStuffBools, false, "EXTRAS", camsKeywords, ViewCommands.TermCamsEvent, 0, true, null, null, "", "", "cams", 1, "ViewInsideShipCam 1");

            terminalVideo = MakeBool(Plugin.instance.Config, "Fun Commands (On/Off)", "terminalVideo", true, "Play a video from the videoFolderPath folder <video>");
            AddManagedBool(terminalVideo, TerminalStuffBools, false, "FUN", videoKeywords, ViewCommands.LolVideoPlayerEvent, 0, true, null, null, "", "", "lol", 0, "darmuh's videoPlayer");

            terminalMap = MakeBool(Plugin.instance.Config, "Extras Commands (On/Off)", "terminalMap", true, "Command to toggle displaying radar in the terminal");
            AddManagedBool(terminalMap, TerminalStuffBools, false, "EXTRAS", mapKeywords, ViewCommands.TermMapEvent, 0, true, null, null, "", "", "map", 5, "ViewInsideShipCam 1");

            terminalMinimap = MakeBool(Plugin.instance.Config, "Extras Commands (On/Off)", "terminalMinimap", true, "Command to toggle displaying radar/cam minimap view in the terminal");
            AddManagedBool(terminalMinimap, TerminalStuffBools, false, "EXTRAS", minimapKeywords, ViewCommands.MiniMapTermEvent, 0, true, null, null, "", "", "minimap", 3, "ViewInsideShipCam 1");

            terminalMinicams = MakeBool(Plugin.instance.Config, "Extras Commands (On/Off)", "terminalMinicams", true, "Command to toggle displaying radar/cam minicams view in the terminal");
            AddManagedBool(terminalMinicams, TerminalStuffBools, false, "EXTRAS", minicamsKeywords, ViewCommands.MiniCamsTermEvent, 0, true, null, null, "", "", "minicams", 4, "ViewInsideShipCam 1");

            terminalOverlay = MakeBool(Plugin.instance.Config, "Extras Commands (On/Off)", "terminalOverlay", true, "Command to toggle displaying radar/cam overlay view in the terminal");
            AddManagedBool(terminalOverlay, TerminalStuffBools, false, "EXTRAS", overlayKeywords, ViewCommands.OverlayTermEvent, 0, true, null, null, "", "", "overlay", 2, "ViewInsideShipCam 1");

            terminalMirror = MakeBool(Plugin.instance.Config, "Extras Commands (On/Off)", "terminalMirror", true, "Command to toggle displaying a Mirror Cam in the terminal");
            AddManagedBool(terminalMirror, TerminalStuffBools, false, "EXTRAS", mirrorKeywords, ViewCommands.MirrorEvent, 0, true, null, null, "", "", "mirror", 6, "terminalStuff Mirror");

            //----------------------------------termview managed bools----------------------------------//

            terminalAlwaysOn = MakeBool(Plugin.instance.Config, "Comfort Commands (On/Off)", "terminalAlwaysOn", true, $"Command to toggle Always-On Display");
            AddManagedBool(terminalAlwaysOn, defaultManaged, false, "COMFORT", alwaysOnKeywords, MoreCommands.AlwaysOnDisplay);

            terminalLink = MakeBool(Plugin.instance.Config, "Extras Commands (On/Off)", "terminalLink", true, "Command to link to an external web-page");
            AddManagedBool(terminalLink, defaultManaged, false, "EXTRAS", linkKeyword, MoreCommands.FirstLinkAsk, 1, true, MoreCommands.FirstLinkDo, MoreCommands.FirstLinkDeny);

            terminalLink2 = MakeBool(Plugin.instance.Config, "Extras Commands (On/Off)", "terminalLink2", false, "Command to link to a second external web-page");
            AddManagedBool(terminalLink2, defaultManaged, false, "EXTRAS", link2Keyword, MoreCommands.SecondLinkAsk, 1, true, MoreCommands.SecondLinkDo, MoreCommands.SecondLinkDeny);

            terminalRandomSuit = MakeBool(Plugin.instance.Config, "Fun Commands (On/Off)", "terminalRandomSuit", true, "Command to switch your suit from a random one off the rack");
            AddManagedBool(terminalRandomSuit, defaultManaged, false, "FUN", randomSuitKeywords, TerminalEvents.RandomSuit);

            terminalClockCommand = MakeBool(Plugin.instance.Config, "Controls Commands (On/Off)", "terminalClockCommand", false, "Command to toggle the Terminal Clock off/on");
            AddManagedBool(terminalClockCommand, defaultManaged, false, "CONTROLS", clockKeywords, TerminalEvents.ClockToggle);

            terminalListItems = MakeBool(Plugin.instance.Config, "Extras Commands (On/Off)", "terminalListItems", true, "Command to list all non-scrap & not currently held items on the ship");
            AddManagedBool(terminalListItems, defaultManaged, false, "EXTRAS", ListItemsKeywords, MoreCommands.GetItemsOnShip);

            terminalLootDetail = MakeBool(Plugin.instance.Config, "Extras Commands (On/Off)", "terminalLootDetail", true, "Command to display an extensive list of all scrap on the ship");
            AddManagedBool(terminalLootDetail, defaultManaged, false, "EXTRAS", ListScrapKeywords, AllTheLootStuff.DetailedLootCommand);

            terminalRefund = MakeBool(Plugin.instance.Config, "Extras Commands (On/Off)", "terminalRefund", true, "Command to cancel an undelivered order and get your credits back");
            AddManagedBool(terminalRefund, defaultManaged, true, "EXTRAS", "refund", CostCommands.GetRefund);

            terminalPrevious = MakeBool(Plugin.instance.Config, "Extras Commands (On/Off)", "terminalPrevious", true, "Command to switch back to previous radar target");
            AddManagedBool(terminalPrevious, defaultManaged, false, "EXTRAS", "previous", ViewCommands.HandlePreviousSwitchEvent);

            terminalRouteRandom = MakeBool(Plugin.instance.Config, "Fun Commands (On/Off)", "terminalRouteRandom", true, "Command to route to a random planet");
            AddManagedBool(terminalRouteRandom, defaultManaged, true, "FUN", randomRouteKeywords, LevelCommands.RouteRandomCommand);

            terminalRouteRandom = MakeBool(Plugin.instance.Config, "Fun Commands (On/Off)", "terminalRouteRandom", true, "Command to route to a random planet");
            AddManagedBool(terminalRouteRandom, defaultManaged, true, "FUN", randomRouteKeywords, LevelCommands.RouteRandomCommand);

            terminalRefreshCustomization = MakeBool(Plugin.instance.Config, "Fun Commands (On/Off)", "terminalRefreshCustomization", false, "Command to reload the Terminal Customization settings (this will not disable any already applied customizations)");
            AddManagedBool(terminalRefreshCustomization, defaultManaged, false, "FUN", refreshcustomizationKWs, TerminalEvents.RefreshCustomizationCommand);
            terminalRadarZoom = MakeBool(Plugin.instance.Config, "Controls Commands (On/Off)", "terminalRadarZoom", true, "Command to cycle through various radar zoom levels.");
            AddManagedBool(terminalRadarZoom, defaultManaged, false, "CONTROLS", radarZoomKWs, ViewCommands.RadarZoomEvent, 0, true, null, null, "", "", "radarZoom");

            //------------------------------------------------MANAGED BOOLS END------------------------------------------------//

            //NOT MANAGED BOOLS THAT ARE COMMANDS, DEFINE THESE COMMANDS LATER THAN TERMINAL AWAKE
            terminalTP = MakeBool(Plugin.instance.Config, "Controls Commands (On/Off)", "terminalTP", true, "Command to Activate Teleporter <TP>");
            terminalITP = MakeBool(Plugin.instance.Config, "Controls Commands (On/Off)", "terminalITP", true, "Command to Activate Inverse Teleporter <ITP>");
            terminalPurchasePacks = MakeBool(Plugin.instance.Config, "Comfort Commands (On/Off)", "terminalPurchasePacks", true, "Use [purchasePackCommands] to create purchase packs that contain multiple store items in one run of the command");
            //NOT MANAGED BOOLS THAT ARE COMMANDS, DEFINE THESE COMMANDS LATER THAN TERMINAL AWAKE

            //String Configs
            doorOpenString = MakeString(Plugin.instance.Config,"Controls Configuration", "doorOpenString", "Opening door.", "Message returned on door (open) command.");
            doorCloseString = MakeString(Plugin.instance.Config,"Controls Configuration", "doorCloseString", "Closing door.", "Message returned on door (close) command.");
            doorSpaceString = MakeString(Plugin.instance.Config,"Controls Configuration", "doorSpaceString", "Can't open doors in space.", "Message returned on door (inSpace) command.");
            canOpenDoorInSpace = MakeBool(Plugin.instance.Config,"Controls Configuration", "canOpenDoorInSpace", true, "Set this to true to allow for pressing the button to open the door in space. (does not change whether the door can actually be opened)");
            quitString = MakeString(Plugin.instance.Config,"Comfort Configuration", "quitString", "goodbye!", "Message returned on quit command.");
            leverString = MakeString(Plugin.instance.Config,"Controls Configuration", "leverString", "PULLING THE LEVER!!!", "Message returned on lever pull command.");
            videoStartString = MakeString(Plugin.instance.Config,"Fun Configuration", "videoStartString", "lol.", "Message displayed when first playing a video.");
            videoStopString = MakeString(Plugin.instance.Config,"Fun Configuration", "videoStopString", "No more lol.", "Message displayed if you want to end video playback early.");
            tpMessageString = MakeString(Plugin.instance.Config,"Controls Configuration", "tpMessageString", "Teleport Button pressed.", "Message returned when TP command is run.");
            itpMessageString = MakeString(Plugin.instance.Config,"Controls Configuration", "itpMessageString", "Inverse Teleport Button pressed.", "Message returned when ITP command is run.");
            vitalsPoorString = MakeString(Plugin.instance.Config,"Upgrades", "vitalsPoorString", "You can't afford to run this command.", "Message returned when you don't have enough credits to run the <Vitals> command.");
            vitalsUpgradePoor = MakeString(Plugin.instance.Config,"Upgrades", "vitalsUpgradePoor", "You can't afford to upgrade the Vitals Scanner.", "Message returned when you don't have enough credits to unlock the vitals scanner upgrade.");
            healIsFullString = MakeString(Plugin.instance.Config,"Comfort Configuration", "healIsFullString", "You are full health!", "Message returned when heal command is run and player is already full health.");
            healString = MakeString(Plugin.instance.Config,"Comfort Configuration", "healString", "The terminal healed you?!?", "Message returned when heal command is run and player is healed.");
            camString = MakeString(Plugin.instance.Config,"Fun Configuration", "camString", "(CAMS)", "Message returned when enabling Cams command (cams).");
            camString2 = MakeString(Plugin.instance.Config,"Fun Configuration", "camString2", "Cameras disabled.", "Message returned when disabling Cams command (cams).");
            mapString = MakeString(Plugin.instance.Config,"Fun Configuration", "mapString", "(MAP)", "Message returned when enabling map command (map).");
            mapString2 = MakeString(Plugin.instance.Config,"Fun Configuration", "mapString2", "Map View disabled.", "Message returned when disabling map command (map).");
            ovString = MakeString(Plugin.instance.Config,"Fun Configuration", "ovString", "(Overlay)", "Message returned when enabling Overlay command (overlay).");
            ovString2 = MakeString(Plugin.instance.Config,"Fun Configuration", "ovString2", "Overlay disabled.", "Message returned when disabling Overlay command (overlay).");
            mmString = MakeString(Plugin.instance.Config,"Fun Configuration", "mmString", "(MiniMap)", "Message returned when enabling minimap command (minimap).");
            mmString2 = MakeString(Plugin.instance.Config,"Fun Configuration", "mmString2", "MiniMap disabled.", "Message returned when disabling minimap command (minimap).");
            mcString = MakeString(Plugin.instance.Config,"Fun Configuration", "mcString", "(MiniCams)", "Message returned when enabling minicams command (minicams).");
            mcString2 = MakeString(Plugin.instance.Config,"Fun Configuration", "mcString2", "MiniCams disabled.", "Message returned when disabling minicams command (minicams).");

            customLink = MakeString(Plugin.instance.Config,"Extras Configuration", "customLink", "https://thunderstore.io/c/lethal-company/p/darmuh/darmuhsTerminalStuff/", "URL to send players to when using the \"link\" command.");
            customLinkHint = MakeString(Plugin.instance.Config,"Extras Configuration", "customLinkHint", "Go to a specific web page.", "Hint given to players in extras menu for \"link\" command.");
            customLink2 = MakeString(Plugin.instance.Config,"Extras Configuration", "customLink2", "https://github.com/darmuh/TerminalStuff", "URL to send players to when using the second \"link\" command.");
            customLink2Hint = MakeString(Plugin.instance.Config,"Extras Configuration", "customLink2Hint", "Go to a specific web page.", "Hint given to players in extras menu for \"link\" command.");

            //Other configs
            gambleMinimum = Plugin.instance.Config.Bind<int>("Fun Configuration", "gambleMinimum", 0, "Credits needed to start gambling, 0 means you can gamble everything.");
            gamblePityMode = MakeBool(Plugin.instance.Config,"Fun Configuration", "gamblePityMode", false, "Enable Gamble Pity Mode, which gives credits back to those who lose everything.");
            gamblePityCredits = Plugin.instance.Config.Bind<int>("Fun Configuration", "gamblePityCredits", 10, "If Gamble Pity Mode is enabled, specify how much Pity Credits are given to losers. (Max: 60)");
            gamblePoorString = MakeString(Plugin.instance.Config,"Fun Configuration", "gamblePoorString", "You don't meet the minimum credits requirement to gamble.", "Message returned when your credits is less than the gambleMinimum set.");
            videoFolderPath = MakeString(Plugin.instance.Config,"Fun Configuration", "videoFolderPath", "darmuh-darmuhsTerminalStuff", "Folder name where videos will be pulled from, needs to be in BepInEx/plugins");
            videoSync = MakeBool(Plugin.instance.Config,"Fun Configuration", "videoSync", true, "When networking is enabled, this setting will sync videos being played on the terminal for all players whose terminal screen is on.");
            obcResolutionMirror = MakeString(Plugin.instance.Config,"Extras Configuration", "obcResolutionMirror", "1000; 700", "Set the resolution of the Mirror Camera created with OpenBodyCams for darmuhsTerminalStuff");
            obcResolutionBodyCam = MakeString(Plugin.instance.Config,"Extras Configuration", "obcResolutionBodyCam", "1000; 700", "Set the resolution of the Body Camera created with OpenBodyCams for darmuhsTerminalStuff");
            camsNeverHide = MakeBool(Plugin.instance.Config,"Extras Configuration", "camsNeverHide", false, "Setting this to true will make it so no command will ever auto-hide any cams command.");
            defaultCamsView = Plugin.instance.Config.Bind("Extras Configuration", "defaultCamsView", "cams", new ConfigDescription("Set the default view switch commands will use when nothing is active.", new AcceptableValueList<string>("map", "cams", "minimap", "minicams", "overlay")));
            ovOpacity = Plugin.instance.Config.Bind("Extras Configuration", "ovOpacity", 10, new ConfigDescription("Opacity percentage for Overlay View.", new AcceptableValueRange<int>(0, 100)));
            alwaysOnAtStart = MakeBool(Plugin.instance.Config, "Quality of Life", "alwaysOnAtStart", true, "Setting this to true will set <alwayson> to enabled at launch.");
            alwaysOnDynamic = MakeBool(Plugin.instance.Config, "Quality of Life", "alwaysOnDynamic", true, "Setting this to true will disable the terminal screen whenever you are not on the ship when alwayson is enabled.");
            alwaysOnWhileDead = MakeBool(Plugin.instance.Config, "Quality of Life", "alwaysOnWhileDead", false, "Set this to true if you wish to keep the screen on after death.");
            aodOffDelay = MakeClampedInt(Plugin.instance.Config, "Quality of Life", "aodOffDelay", -1, "Set this to delay turning the terminal screen off by this many seconds after leaving the ship.", -1, 30);


            //homescreen lines
            homeLine1 = MakeString(Plugin.instance.Config,"Terminal Customization", "homeline1", "Welcome to the FORTUNE-9 OS PLUS", "First line of the home command (startup screen)");
            homeLine2 = MakeString(Plugin.instance.Config,"Terminal Customization", "homeline2", "\tUpgraded by Employee: <color=#e6b800>darmuh</color>", "Second line of the home command (startup screen)");
            homeLine3 = MakeString(Plugin.instance.Config,"Terminal Customization", "homeline3", "Have a wonderful [currentDay]!", "Last line of the home command (startup screen)");
            homeHelpLines = MakeString(Plugin.instance.Config,"Terminal Customization", "homeHelpLines", ">>Type \"Help\" for a list of commands.\r\n>>Type <color=#b300b3>\"More\"</color> for a menu of darmuh's commands.\r\n", "these two lines should generally be used to point to menus of other usable commands. Can also be expanded to more than two lines by using \"\\r\\n\" to indicate a new line");
            
            homeTextArt = MakeString(Plugin.instance.Config,"Terminal Customization", "homeTextArt", "[leadingSpacex4][leadingSpace]<color=#e6b800>^^      .-=-=-=-.  ^^\r\n ^^        (`-=-=-=-=-`)         ^^\r\n         (`-=-=-=-=-=-=-`)  ^^         ^^\r\n   ^^   (`-=-=-=-=-=-=-=-`)   ^^          \r\n       ( `-=-=-=-(@)-=-=-` )      ^^\r\n       (`-=-=-=-=-=-=-=-=-`)  ^^          \r\n       (`-=-=-=-=-=-=-=-=-`)  ^^\r\n        (`-=-=-=-=-=-=-=-`)          ^^\r\n         (`-=-=-=-=-=-=-`)  ^^            \r\n           (`-=-=-=-=-`)\r\n            `-=-=-=-=-`</color>", "ASCII Art goes here");

            //Quality of Life Stuff
            LockCameraInTerminal = MakeBool(Plugin.instance.Config,"Quality of Life", "LockCameraInTerminal", false, "Enable this to lock the player camera to the terminal when it is in use.");
            TerminalLightBehaviour = MakeClampedString(Plugin.instance.Config, "Quality of Life", "TerminalLightBehaviour", "nochange", "Use this config item to change how the terminal light behaves. Options are 'nochange' which keeps vanilla behaviour, 'disable' which disables this light whenever you use it, and 'alwayson' which will keep the light on as long as the screen is on from alwayson", new AcceptableValueList<string>("nochange", "disable", "alwayson"));
            TerminalHistory = MakeBool(Plugin.instance.Config,"Quality of Life", "TerminalHistory", false, "(Requires terminalShortcuts feature to function) With this feature enabled, uparrow and downarrow will cycle through a list of previously used commands.");
            TerminalHistoryMaxCount = MakeClampedInt(Plugin.instance.Config, "Quality of Life", "TerminalHistoryMaxCount", 9, "Max amount of previous commands to save in TerminalHistory list.", 3, 50);
            TerminalAutoComplete = MakeBool(Plugin.instance.Config,"Quality of Life", "TerminalAutoComplete", false, "(Requires terminalShortcuts feature to function) With this feature enabled, tab key will cycle through a list of matching commands to the current input.");
            TerminalAutoCompleteKey = MakeString(Plugin.instance.Config, "Quality of Life", "TerminalAutoCompleteKey", "Tab", "Key used to activate TerminalAutoComplete feature https://docs.unity3d.com/Packages/com.unity.inputsystem@1.0/api/UnityEngine.InputSystem.Key.html");
            TerminalAutoCompleteMaxCount = MakeClampedInt(Plugin.instance.Config, "Quality of Life", "TerminalAutoCompleteMaxCount", 5, "Max amount of matching commands to store before disabling autocomplete.", 3, 50);
            TerminalConflictResolution = MakeBool(Plugin.instance.Config, "Quality of Life", "TerminalConflictResolution", false, "With this feature enabled, terminal command input will be weighted for conflict resolution using the Levenshtein algorithm.");
            TerminalRadarDefaultZoom = MakeClampedFloat(Plugin.instance.Config, "Quality of Life", "TerminalRadarDefaultZoom", 20f, "The default level zoom for the radar. The lower the number the more zoomed in you'll be.", 5f, 30f);
            TerminalFillEmptyText = MakeClampedString(Plugin.instance.Config, "Quality of Life", "TerminalFillEmptyText", "nochange", "AutoFill any node with empty space depending on your desired formatting", new AcceptableValueList<string>("nochange", "fillbottom", "textmiddle", "textbottom"));


            //Terminal Customization
            TerminalCustomization = MakeBool(Plugin.instance.Config, "Terminal Customization", "TerminalCustomization", false, "Enable or Disable terminal color customizations");
            TerminalColor = MakeString(Plugin.instance.Config, "Terminal Customization", "TerminalColor", "#666633", "This changes the color of the physical terminal");
            TerminalButtonsColor = MakeString(Plugin.instance.Config, "Terminal Customization", "TerminalButtonsColor", "#9900ff", "This changes the color of the physical buttons on the terminal");
            TerminalKeyboardColor = MakeString(Plugin.instance.Config, "Terminal Customization", "TerminalKeyboardColor", "#9900ff", "This changes the color of the keyboard on the terminal");
            TerminalTextColor = MakeString(Plugin.instance.Config, "Terminal Customization", "TerminalTextColor", "#ffffb3", "This changes the color of the main text in the terminal");
            TerminalMoneyColor = MakeString(Plugin.instance.Config,"Terminal Customization", "TerminalMoneyColor", "#ccffcc", "This changes the color of the current credits text in the top left of the terminal");
            TerminalMoneyBGColor = MakeString(Plugin.instance.Config, "Terminal Customization", "TerminalMoneyBGColor", "#ccffcc", "This changes the color of the current credits text in the top left of the terminal");
            TerminalMoneyBGAlpha = MakeClampedFloat(Plugin.instance.Config, "Terminal Customization", "TerminalMoneyBGAlpha", 0.1f, "This changes the transparency of the money background color.", 0f, 1f);
            TerminalCaretColor = MakeString(Plugin.instance.Config,"Terminal Customization", "TerminalCaretColor", "#9900ff", "This changes the color of the text caret in the terminal");
            TerminalScrollbarColor = MakeString(Plugin.instance.Config,"Terminal Customization", "TerminalScrollbarColor", "#9900ff", "This changes the color of the scrollbar in the terminal");
            TerminalScrollBGColor = MakeString(Plugin.instance.Config,"Terminal Customization", "TerminalScrollBGColor", "#ffffb3", "This changes the color of the background box of the scrollbar in the terminal");
            TerminalClockColor = MakeString(Plugin.instance.Config,"Terminal Customization", "TerminalClockColor", "#ccffcc", "This changes the color of the clock element that is added to the terminal");
            TerminalLightColor = MakeString(Plugin.instance.Config,"Terminal Customization", "TerminalLightColor", "#9900ff", "This changes the color of the light that shines from the terminal");
            TerminalCustomBG = MakeBool(Plugin.instance.Config, "Terminal Customization", "TerminalCustomBG", false, "Enable or Disable custom background for the terminal screen");
            TerminalCustomBGColor = MakeString(Plugin.instance.Config, "Terminal Customization", "TerminalCustomBGColor", "#9900ff", "This changes the color of the custom background for the terminal screen");
            TerminalCustomBGAlpha = MakeClampedFloat(Plugin.instance.Config, "Terminal Customization", "TerminalCustomBGAlpha", 0.08f, "This changes the transparency of the custom background for the terminal screen", 0f, 1f);

            //Font Stuff
            CustomFontPath = MakeString(Plugin.instance.Config, "Terminal Customization", "CustomFontPath", "fonts", "If you want to share a profile code that includes the font file, put it in a folder with this name in the config folder. The default example would be \"BepInEx\\config\\fonts\"");
            CustomFontName = MakeString(Plugin.instance.Config, "Terminal Customization", "CustomFontName", "", "Name of the custom font you'd like to use in the terminal, leave blank or set to \"default\" to use the normal terminal font");
            CustomFontSizeMain = MakeClampedInt(Plugin.instance.Config, "Terminal Customization", "CustomFontSizeMain", -1, "Set a custom size for your custom font (main text), leave at -1 if you wish not to change it", -1, 72);
            CustomFontSizeMoney = MakeClampedInt(Plugin.instance.Config, "Terminal Customization", "CustomFontSizeMoney", -1, "Set a custom size for your font (credits at the top left), leave at -1 if you wish not to change it", -1, 72);
            CustomFontSizeClock = MakeClampedInt(Plugin.instance.Config, "Terminal Customization", "CustomFontSizeClock", -1, "Set a custom size for your font (terminalClock), leave at -1 if you wish not to change it", -1, 72);

            PluginCore.StuffForLibrary.ManualCommands(); //add more managedbools that dont come from a specific config item

            Plugin.MoreLogs("end of config setup");

            RemoveOrphanedEntries(Plugin.instance.Config);
            NetworkingCheck(ModNetworking.Value, Plugin.instance.Config, defaultManaged);
        }

    }
}