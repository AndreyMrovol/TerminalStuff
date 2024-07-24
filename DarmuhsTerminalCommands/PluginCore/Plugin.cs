using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using OpenLib.ConfigManager;
using System;
using System.Collections.Generic;
using System.Reflection;
using TerminalStuff.EventSub;
using TerminalStuff.PluginCore;
using UnityEngine;
using UnityEngine.UI;
using static OpenLib.ConfigManager.ConfigSetup;


namespace TerminalStuff
{
    [BepInPlugin("darmuh.TerminalStuff", "darmuhsTerminalStuff", (PluginInfo.PLUGIN_VERSION))]
    [BepInDependency("darmuh.OpenLib", "0.1.2")] //hard dependency for my library
    [BepInDependency("Rozebud.FovAdjust", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("Zaggy1024.OpenBodyCams", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("Zaggy1024.TwoRadarMaps", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("darmuh.suitsTerminal", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("TerminalFormatter", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("BMX.LobbyCompatibility", BepInDependency.DependencyFlags.SoftDependency)]


    public class Plugin : BaseUnityPlugin
    {
        public static Plugin instance;
        public static class PluginInfo
        {
            public const string PLUGIN_GUID = "darmuh.TerminalStuff";
            public const string PLUGIN_NAME = "darmuhsTerminalStuff";
            public const string PLUGIN_VERSION = "3.3.1";
        }

        internal static ManualLogSource Log;

        //Compatibility
        public bool LobbyCompat = false;
        public bool CompatibilityAC = false;
        public bool LateGameUpgrades = false;
        public bool FovAdjust = false;
        public bool HelmetCamsMod = false;
        public bool SolosBodyCamsMod = false;
        public bool OpenBodyCamsMod = false;
        public bool TwoRadarMapsMod = false;
        public bool suitsTerminal = false;
        public bool TerminalFormatter = false;

        //public stuff for instance
        public bool radarNonPlayer = false;
        public bool isOnMirror = false;
        public bool isOnCamera = false;
        public bool isOnMap = false;
        public bool isOnOverlay = false;
        public bool isOnMiniMap = false;
        public bool isOnMiniCams = false;
        public bool activeCam = false;
        public bool splitViewCreated = false;
        public bool escapeConfirmation = false;

        //flashlight stuff
        public bool fSuccess = false;
        public bool hSuccess = false;

        //AutoComplete
        internal bool removeTab = false;

        internal Terminal Terminal;
        internal static List<TerminalNode> Allnodes = [];
        //internal static ShipTeleporter NormalTP;
        //internal static ShipTeleporter InverseTP;
        //internal ManualCameraRenderer MapScreen;


        public RawImage rawImage1;
        public RawImage rawImage2;
        public RenderTexture renderTexturePub;
        public Canvas terminalCanvas;
        public Vector2 originalTopSize;
        public Vector2 originalTopPosition;
        public Vector2 originalBottomSize;
        public Vector2 originalBottomPosition;
        //public GameObject myNetworkPrefab;

        private void Awake()
        {
            instance = this;
            Log = base.Logger;
            Log.LogInfo((object)$"{PluginInfo.PLUGIN_NAME} is loaded with version {PluginInfo.PLUGIN_VERSION}!");
            Log.LogInfo((object)"--------[Now with more Quality of Life!]---------");
            StuffForLibrary.Init();
            ConfigSettings.BindConfigSettings();
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
            //Addkeywords used to be here
            VideoManager.Load();
            Subscribers.Subscribe();
            Config.ConfigReloaded += OnConfigReloaded;
            Config.SettingChanged += OnSettingChanged;

            //start of networking stuff

            var types = Assembly.GetExecutingAssembly().GetTypes();
            foreach (var type in types)
            {
                var methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                foreach (var method in methods)
                {
                    var attributes = method.GetCustomAttributes(typeof(RuntimeInitializeOnLoadMethodAttribute), false);
                    if (attributes.Length > 0)
                    {
                        method.Invoke(null, null);
                    }
                }
            }

            //end of networking stuff
        }

        internal void OnSettingChanged(object sender, SettingChangedEventArgs settingChangedArg)
        {
            Spam("CONFIG SETTING CHANGE EVENT");

            if (settingChangedArg.ChangedSetting == null)
                return;

            if( ConfigMisc.CheckChangedConfigSetting(defaultManaged, settingChangedArg.ChangedSetting) || ConfigMisc.CheckChangedConfigSetting(ConfigSettings.TerminalStuffBools, settingChangedArg.ChangedSetting))
            {
                Spam("managed bools have been modified!!");
            }
        }

        internal void OnConfigReloaded(object sender, EventArgs e)
        {
            Spam("Config has been reloaded!");
            StuffForLibrary.ManualCommands();
            NetworkingCheck(ConfigSettings.ModNetworking.Value, instance.Config, defaultManaged);
            ReadConfigAndAssignValues(instance.Config, defaultManaged);
            ReadConfigAndAssignValues(instance.Config, ConfigSettings.TerminalStuffBools);
        }

        internal static void MoreLogs(string message)
        {
            if (ConfigSettings.extensiveLogging.Value)
                Log.LogInfo(message);
            else
                return;
        }

        internal static void Spam(string message)
        {
            if (ConfigSettings.developerLogging.Value)
                Log.LogDebug(message);
            else
                return;
        }

        internal static void ERROR(string message)
        {
            Log.LogError(message);
        }

        internal static void WARNING(string message)
        {
            Log.LogWarning(message);
        }
    }

}