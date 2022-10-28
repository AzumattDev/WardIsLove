using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using WardIsLove.Util.DiscordMessenger;
using HarmonyLib;
using PieceManager;
using ServerSync;
using Steamworks;
using UnityEngine;
using WardIsLove.Extensions;
using WardIsLove.PatchClasses;
using WardIsLove.Util;
using WardIsLove.Util.UI;

namespace WardIsLove
{
    [BepInPlugin(HGUID, ModName, version)]
    [BepInIncompatibility("Azumatt.BetterWards")]
    //[BepInDependency("org.bepinex.plugins.guilds", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("org.bepinex.plugins.groups", BepInDependency.DependencyFlags.SoftDependency)]
    public partial class WardIsLovePlugin : BaseUnityPlugin
    {
        public enum WardBehaviorEnums
        {
            Default = 0,
            NoMonsters = 1,
        }

        public enum WardGUIFeedbackEnums
        {
            Feedback = 0,
            Bug = 1,
            Idea = 2
        }

        public enum WardInteractBehaviorEnums
        {
            Default = 0,
            OwnerOnly = 1,
            Everyone = 2,
            Group = 3
        }

        public enum WardModelTypes
        {
            Thor = 0,
            Loki = 1,
            Hel = 2,
            Default = 3,
            BetterWard = 4,
            BetterWardType2 = 5,
            BetterWardType3 = 6,
            BetterWardType4 = 7,
        }

        public enum WardDamageTypes
        {
            Normal,
            Frost,
            Poison,
            Fire,
            Lightning,
            Spirit,
            Stagger
        }

        public const string version = "3.0.5";
        public const string ModName = "WardIsLove";
        internal const string Author = "Azumatt";
        internal const string HGUID = Author + "." + "WardIsLove";
        private const string HarmonyGUID = "Harmony." + Author + "." + ModName;
        private static string ConfigFileName = HGUID + ".cfg";
        private static string ConfigFileFullPath = Paths.ConfigPath + Path.DirectorySeparatorChar + ConfigFileName;
        public static string ConnectionError = "";
        public static bool IsUpToDate;
        public static bool ValidServer = false;
        public static bool Admin = false;
        public static bool Raidable = false;
        public static bool fInit = false;
        public static int EffectTick = 0;
        public static BuildPiece Thorward;
        public static GameObject LightningVFX;
        internal static string CachedID = "";
        internal static string CachedPersona = "";

        private static Sprite icon;
        public static readonly ManualLogSource WILLogger = BepInEx.Logging.Logger.CreateLogSource(ModName);
        private static WardIsLovePlugin? plugin;

        public static ConfigFile localizationFile;
        public static Dictionary<string, ConfigEntry<string>> localizedStrings = new();
        private static long _timeSave;
        public static Material ForceField = null;

        // Project Repository Info
        internal static string Repository = "https://github.com/WardIsLove/WardIsLove";

        internal static string ApiRepositoryLatestRelease =
            "https://api.github.com/repos/AzumattDev/WardIsLove/releases/latest";

        //harmony
        private static Harmony harmony;

        private static bool isInitialized = false;

        private static PieceTable? _hammer;

        private readonly ConfigSync configSync = new(ModName)
            { DisplayName = ModName, CurrentVersion = version, MinimumRequiredVersion = version };

        public static WardIsLovePlugin Instance { get; private set; }


        public void Awake()
        {
            ServerConfigLocked = config("General", "Force Server Config", true, "Force Server Config");
            _ = configSync.AddLockingConfigEntry(ServerConfigLocked);

            Instance = this;
            /* ANNOUNCEMENT IN CONFIGS */
            Announcement = config("Announcement", "Information about this config file", "",
                "The values set in this config file are the GLOBAL defaults for each ward placed. Admins and owners (when allowed) can change the individual ward configurations per ward",
                false);
            /* Streamer Mode */
            StreamerMode = config("UI", "StreamerMode", false,
                "Prevent the display of steam hover text for admins. SteamID and SteamName will be hidden for all users.",
                false);
            DisableGUI = config("UI", "DisableGUI", false,
                "Prevent the GUI option from being available, even in SinglePlayer - Gratak special request. Also disables the hover text display.");

            /* Charge */
            ChargeItem = config("Charge", "Charge Item", "Thunderstone",
                "Item needed to charge the ward. Limit is 1 item: Goes by prefab name. List here: https://github.com/Valheim-Modding/Wiki/wiki/ObjectDB-Table");
            ChargeItemAmount = config("Charge", "Charge Item Amount", 5,
                "Amount of the Item needed to charge the ward. If you set this to 0, the item is not needed and can charge without cost.");
            /* Control GUI */
            WardControl = config("Control GUI", "Ward Control", false,
                "Should ward owners have control of their ward via their own (limited) GUI interface?");
            /* General */
            WardEnabled = config("General", "WardEnabled", true, "Enable WardIsLove Configurations");
            ShowMarker = config("General", "ShowMarker", true,
                "Whether or not you want to show the area marker for wards", false);
            WardHotKey = config("General", "WardHotKey", new KeyboardShortcut(KeyCode.G),
                new ConfigDescription("Personal hotkey to toggle a ward on which you're permitted on/off",
                    new AcceptableShortcuts()), false);
            AutoClose = config("General", "AutoCloseDoors", false,
                "Whether or not you want to have doors auto close inside the ward.", false);
            WardNotify = config("General", "WardNotify", true,
                "Whether or not you want to be notified when entering and leaving a ward.", false);
            WardNotifyMessageEntry = config("General", "WardEntryMessage", "Entered {0}'s ward",
                "Entry message for ward notifications");
            WardNotifyMessageExit = config("General", "WardExitMessage", "{0} you have left the ward",
                "Exit message for ward notifications");
            WardDamageIncrease = config("General", "WardDamageIncrease", 0f,
                new ConfigDescription(
                    "Increase incoming damage to creatures in the game while they are inside the ward.\nValues are in percentage 0% - 100%.",
                    new AcceptableValueRange<float>(0, 100)));
            DisablePickup = config("General", "EnablePickup", false,
                "Allow non-permitted users to automatically picking up items inside a ward");
            AdminAutoPerm = config("General", "AdminAutoPerm", false,
                "Enable or disable the auto-permit on wards for admins", false);
            ItemStandInteraction = config("General", "ItemStandInteraction", false,
                "Allow non-permitted users to interact with item stands inside a ward");
            PortalInteraction = config("General", "PortalInteraction", false,
                "Allow non-permitted users to interact with portals inside a ward");
            NoTeleport = config("General", "NoTeleport", true,
                "Prevent non-permitted users from going through a portal inside a ward");
            PickableInteraction = config("General", "PickableInteraction", false,
                "Allow non-permitted users to interact with crops/pickables inside a ward");
            ItemInteraction = config("General", "ItemInteraction", false,
                "Allow non-permitted users to interact with items dropped inside a ward");
            DoorInteraction = config("General", "DoorInteraction", false,
                "Allow non-permitted users to interact with doors inside a ward");
            ChestInteraction = config("General", "ChestInteraction", false,
                "Allow non-permitted users to interact with chests inside a ward");
            CraftingStationInteraction = config("General", "CraftingStationInteraction", false,
                "Allow non-permitted users to interact with crafting stations inside a ward");
            SmelterInteraction = config("General", "SmeltingStationInteraction", false,
                "Allow non-permitted users to interact with Smelters inside a ward");
            BeehiveInteraction = config("General", "BeehiveInteraction", false,
                "Allow non-permitted users to interact with Beehives inside a ward");
            MaptableInteraction = config("General", "MapTableInteraction", false,
                "Allow non-permitted users to interact with MapTables inside a ward");
            SignInteraction = config("General", "SignInteraction", false,
                "Allow non-permitted users to interact with Signs inside a ward");
            WardNoDeathPen = config("General", "WardNoDeathPen", false,
                "Enables wards to provide no skill loss on death to those who have access yet die inside a ward.");
            EnableBubble = config("General", "EnableBubble", false,
                "Enables the bubble visual effect on wards");
            PushoutPlayers = config("General", "PushoutPlayers", false,
                "Prevent non-permitted users from entering the warded area. If they are already inside, you must deal with the consequences.");
            PushoutCreatures = config("General", "PushoutCreatures", false,
                "Prevent creatures from entering the warded area if the bubble is active. If they are already inside, you must deal with the consequences");
            ShipInteraction = config("General", "ShipInteraction", false,
                "Allow non-permitted users to interact with ships inside a ward");
            NoFoodDrain = config("General", "NoFoodDrain", false,
                "Prevent food loss inside ward for permitted players");
            WardDamageAmount = config("General", "WardDamageAmount", 0f,
                new ConfigDescription(
                    "Amount of damage, per tick, to creatures while they are inside the ward. Does not apply to tames\nValues are direct values, not percents."));
            WardDamageRepeatRate = config("General", "WardDamageRepeatRate", 2f,
                new ConfigDescription(
                    "Amount of seconds to wait between damage ticks."));


            /* Show Flash */
            ShowFlash = config("General", "ShowFLASH", true,
                "Show the ward flash when something is damaged");

            /* Fireplace Unlimited */
            CookingUnlimited = config("Fire", "OvenUnlimited", false,
                "Infinite oven fuel, who doesn't want this? Enable to allow your oven to never run out of fuel inside a warded area");
            BathingUnlimited = config("Fire", "BathingUnlimited", false,
                "Infinite bath fuel, who doesn't want this? Enable to allow your bath to never run out of fuel inside a warded area");
            FireplaceUnlimited = config("Fire", "FireplaceUnlimited", false,
                "Fireplaces inside a ward no longer need constant fueling\nThis might conflict with other mods that fuel fires.\nDisable this feature if you are having problems.");
            FireSources = config("Fire", "FireSources",
                "piece_walltorch,piece_sconce,piece_groundtorch,piece_groundtorch_wood,piece_groundtorch_green,piece_groundtorch_blue,piece_brazierceiling01,fire_pit,bonfire,hearth",
                "The fire sources inside a ward you want to never run out of fuel.\nUses Prefab Name.\nIf the Prefab Name contains the string, it will no longer require fuel.");
            /* Ward Range */
            WardRange = config<float>("WardRange", "WardRange", 20,
                new ConfigDescription("Range of the ward", new AcceptableValueRange<float>(0.0f, 100f)));
            /*Health Regen */
            WardPassiveHealthRegen = config("Health Regen", "WardPassiveHealthRegenValue", 0.5f,
                "How much health you should regen passively inside a ward per second");
            /* Stamina Regen */
            WardPassiveStaminaRegen = config("Stamina Regen", "WardPassiveStaminaRegenValue", 0.5f,
                "How much stamina you should regen passively inside a ward per second");
            /* PvE */
            WardPve = config("PvE", "WardPvE", false,
                "Whether or not you want PvE enabled inside a ward by default.");
            WardOnlyPerm = config("PvE", "WardOnlyPerm", false,
                "Whether or not you want PvE or PvP forced inside a ward ONLY when permitted or owner.");
            WardNotPerm = config("PvE", "WardNotPerm", false,
                "Whether or not you want PvE or PvP forced inside a ward ONLY when NOT permitted or owner.");
            /* PvP */
            WardPvP = config("PvP", "WardPvP", false,
                "Whether or not you want PvP enabled inside a ward by default.");
            PvpNotPerm = config("PvP", "CallToArmsBETA", false,
                "Call to Arms BETA, couldn't test this one alone. Report all bugs!\nToggles PvP on for non-permitted players inside a ward. Attempt to toggle on for all permitted players.");
            CtaMessage = config("PvP", "CallToArmsMessage", "Call to arms! An enemy has entered your ward!",
                "Call to Arms BETA message shown");
            /* Structures */
            WardStructures = config("Structures", "IndestructibleItems", false,
                "Whether or not you want to have indestructible structures inside a ward. If this is set to true, and items are defined damage reduction is 100%");
            ItemStructureNames = config("Structures", "Items", "ward,portal,guard,chest,gate,door,iron,stone",
                "The items inside a ward you want to make indestructible.\nUses Prefab Name.\nIf the Prefab Name contains the string, it will be indestructible. List here: https://valheim-modding.github.io/Jotunn/data/pieces/piece-list.html");
            WardDamageReduction = config("Structures", "WardDamageReduction", 0.0f,
                new ConfigDescription("Reduce incoming damage to player built structures/items",
                    new AcceptableValueRange<float>(0.0f, 100f)));
            NoWeatherDmg = config("Structures", "NoWeatherDmg", true,
                "No weather damage on structures inside wards");
            // _autoRepair = config("Structures", "AutoRepair", false, "Auto repair on structures inside wards");
            // _autoRepairTime = config("Structures", "AutoRepairTime", 3f, "Time between auto repair ticks");
            // _autoRepairAmount = config<float>("Structures", "AutoRepairAmount", 20,
            //     new ConfigDescription("Repair amount in percent", new AcceptableValueRange<float>(0.0f, 100f)));

            /* RAID PROTECTION */
            RaidProtection = config("Raid Protection", "RaidProtection", true,
                "Should offline raid protection be turned on?\nPrevents non permitted players from damaging your base if inside your ward");
            RaidablePlayersNeeded = config("Raid Protection", "RaidablePlayersNeeded", 2,
                "Minimum number of players required to be online for their warded area to be raided");
            ShowraidableMessage = config("Raid Protection", "RaidableMessageShow", true,
                "Display Raid message if not raidable", false);


            localizationFile =
                new ConfigFile(
                    Path.Combine(Paths.ConfigPath, HGUID + ".Localization.cfg"), false);

            // send log
            WILLogger.LogDebug("Loading WardIsLove configuration file");
            WILLogger.LogDebug("Starting WardIsLove-Client");
            harmony = new Harmony(HarmonyGUID);

            harmony.PatchAll();

            //check for new versions, notify client
            _ = StartCoroutine(GitHubCheck.CheckForNewVersion());

            _timeSave = DateTime.Now.Ticks;
            Local.Localize();

            _ = StartCoroutine(WardMonoscriptExt.UpdateAreas());
            WardGUI.Init();
            WardLimitServerCheck();
            //GetWebHook();
            /*new DiscordMessage()
                .SetUsername("Test Message")
                .SetAvatar("https://staticdelivery.nexusmods.com/mods/3667/images/402/402-1620654411-147438437.png")
                .SetContent("Azumatt testing YAML based discord webhook")
                .AddEmbed()
                .SetTimestamp(DateTime.Now)
                .SetAuthor("Feli", "https://staticdelivery.nexusmods.com/mods/3667/images/402/402-1620654411-147438437.png", "https://staticdelivery.nexusmods.com/mods/3667/images/402/402-1620654411-147438437.png")
                .SetTitle("Test Embed")
                .SetDescription(
                    "Modified version of DiscordMessenger. This is a test embedding with projects that use YamlDotNet.")
                .SetColor(14177041)
                .AddField("Test Field", "Test Value")
                .AddField("Test Field", "Test Value Inline", true)
                .SetFooter("Test Footer", "https://staticdelivery.nexusmods.com/mods/3667/images/402/402-1620654411-147438437.png")
                .Build()
                .SendMessageAsync(
                    "https://discord.com/api/webhooks/1013108653454266418/LWzwvOcLZwJ-QbtPq49VxJ9yMNc2sP2v17fuG8fpBGj10ZDKn6GW_AqJ3-6B8h0Ox_pj");*/

            SetupWatcher();
        }

        private static void GetWebHook()
        {
            Task.Run(async () =>
            {
                string asyncResult =
                    //await WardGUIUtil.GetAsync("https://wardislove-13a2b-default-rtdb.firebaseio.com/WardIsLove.json");
                    await WardGUIUtil.GetAsync("https://kgwebhook-default-rtdb.firebaseio.com/azumattwebhook.json");
                string link = asyncResult.Trim('"');
            });
        }

        private void SetupWatcher()
        {
            FileSystemWatcher watcher = new(Paths.ConfigPath, ConfigFileName);
            watcher.Changed += ReadConfigValues;
            watcher.Created += ReadConfigValues;
            watcher.Renamed += ReadConfigValues;
            watcher.IncludeSubdirectories = true;
            watcher.SynchronizingObject = ThreadingHelper.SynchronizingObject;
            watcher.EnableRaisingEvents = true;
        }

        private void ReadConfigValues(object sender, FileSystemEventArgs e)
        {
            if (!File.Exists(ConfigFileFullPath)) return;
            try
            {
                WILLogger.LogDebug("ReadConfigValues called");
                Config.Reload();
            }
            catch
            {
                WILLogger.LogError($"There was an issue loading your {ConfigFileName}");
                WILLogger.LogError("Please check your config entries for spelling and format!");
            }
        }

        private void Update()
        {
            if (!WardGUI.IsPanelVisible()) return;
            if (!Input.GetKey(KeyCode.Escape)) return;
            try
            {
                WardGUI.Hide();
            }
            catch
            {
                // I don't give two fucks if this fails. Attempt is all I care about.
            }
        }

        // unpatch on game close
        private void OnDestroy()
        {
            localizationFile.Save();
        }


        /*private IEnumerator DelayedRepair()
        {
            while (true)
            {
                if (!WardMonoscriptExt.WardMonoscriptsINSIDE.Any() || !ZNetScene.instance ||
                    Player.m_localPlayer == null) yield return null;
                foreach (WardMonoscript? ward in WardMonoscriptExt.WardMonoscriptsINSIDE)
                {
                    if (ward.GetAutoRepairOn())
                    {
                        List<WearNTear> allInstances = WearNTear.GetAllInstaces();
                        if (allInstances.Count > 0)
                            foreach (WearNTear instance in allInstances)
                            {
                                ZNetView instanceField = instance.m_nview;
                                if (instanceField == null ||
                                    !WardMonoscript.CheckInWardMonoscript(instance.transform.position))
                                    continue;
                                float num1 = instanceField.GetZDO().GetFloat("health");
                                if (!(num1 > 0.0) || !(num1 < (double)instance.m_health)) continue;
                                float num2 = num1 +
                                             (float)(instance.m_health * (double)ward.GetAutoRepairAmount() /
                                                     100.0);
                                if (num2 > (double)instance.m_health)
                                    num2 = instance.m_health;
                                instanceField.GetZDO().Set("health", num2);
                                instanceField.InvokeRPC(ZNetView.Everybody, "WNTHealthChanged", num2);
                            }
                    }


                    float time;
                    try
                    {
                        time = ward.GetAutoRepairTextTime();
                    }
                    catch
                    {
                        time = 5;
                    }


                    yield return new WaitForSecondsRealtime(time);
                }

                yield break;
            }
        }*/

        public static AssetBundle GetAssetBundle(string filename)
        {
            Assembly execAssembly = Assembly.GetExecutingAssembly();

            string resourceName = execAssembly.GetManifestResourceNames()
                .Single(str => str.EndsWith(filename));

            using Stream? stream = execAssembly.GetManifestResourceStream(resourceName);
            return AssetBundle.LoadFromStream(stream);
        }

        private static object GetInstanceField<T>(T instance, string fieldName)
        {
            FieldInfo field = typeof(T).GetField(fieldName,
                BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)!;
            return (object)field == null ? null : field.GetValue(instance);
        }

        [HarmonyPatch(typeof(ZNetScene), nameof(ZNetScene.Awake))]
        public static class WardIsLoveZNetPost
        {
            public static void Postfix(ZNetScene __instance)
            {
                GameObject? hammer = ZNetScene.instance.GetPrefab("Hammer");
                PieceTable? hammerTable = hammer.GetComponent<ItemDrop>().m_itemData.m_shared.m_buildPieces;
                _hammer = hammerTable;
                _hammer?.m_pieces.Remove(ZNetScene.instance.GetPrefab("guard_stone").gameObject);
                ZNetScene.instance.GetPrefab("guard_stone").GetComponent<Piece>().enabled = false;
            }
        }

        [HarmonyPatch(typeof(PrivateArea), nameof(PrivateArea.IsEnabled))]
        static class PrivateAreaIsEnabledPatch
        {
            static bool Prefix(PrivateArea __instance)
            {
                return __instance.gameObject.name != "guard_stone";
            }
        }


        [HarmonyPatch(typeof(TextInput), nameof(TextInput.IsVisible))]
        private static class INPUTPATCHforFeedback
        {
            private static void Postfix(ref bool __result)
            {
                if (WardGUI.IsPanelVisible()) __result = true;
            }
        }

        #region ConfigOptions

        public static ConfigEntry<bool> ServerConfigLocked = null!;
        public static ConfigEntry<bool> WardEnabled = null!;

        public static ConfigEntry<bool> StreamerMode = null!;
        public static ConfigEntry<bool> WardControl = null!;
        public static ConfigEntry<string> ChargeItem = null!;
        public static ConfigEntry<int> ChargeItemAmount = null!;
        public static ConfigEntry<string> Announcement = null!;
        public static ConfigEntry<KeyboardShortcut> WardHotKey = null!;
        public static ConfigEntry<float> WardRange = null!;
        public static ConfigEntry<float> WardPassiveHealthRegen = null!;
        public static ConfigEntry<float> WardPassiveStaminaRegen = null!;
        public static ConfigEntry<bool> WardOnlyPerm = null!;
        public static ConfigEntry<bool> WardNotPerm = null!;
        public static ConfigEntry<bool> PvpNotPerm = null!;
        public static ConfigEntry<string> CtaMessage = null!;
        public static ConfigEntry<bool> WardPvP = null!;
        public static ConfigEntry<bool> WardPve = null!;
        public static ConfigEntry<bool> ShowMarker = null!;
        public static ConfigEntry<bool> WardStructures = null!;
        public static ConfigEntry<bool> NoWeatherDmg = null!;
        public static ConfigEntry<string> ItemStructureNames = null!;
        public static ConfigEntry<bool> AutoClose = null!;
        public static ConfigEntry<float> WardDamageAmount = null!;
        public static ConfigEntry<float> WardDamageRepeatRate = null!;
        public static ConfigEntry<bool> WardNotify = null!;
        public static ConfigEntry<string> WardNotifyMessageEntry = null!;
        public static ConfigEntry<string> WardNotifyMessageExit = null!;
        public static ConfigEntry<bool> WardNoDeathPen = null!;
        public static ConfigEntry<bool> BathingUnlimited = null!;
        public static ConfigEntry<bool> CookingUnlimited = null!;
        public static ConfigEntry<bool> FireplaceUnlimited = null!;
        public static ConfigEntry<string> FireSources = null!;
        public static ConfigEntry<bool> DisablePickup = null!;
        public static ConfigEntry<bool> PushoutPlayers = null!;
        public static ConfigEntry<bool> PushoutCreatures = null!;
        public static ConfigEntry<bool> AdminAutoPerm = null!;
        public static ConfigEntry<bool> ItemStandInteraction = null!;
        public static ConfigEntry<bool> PortalInteraction = null!;
        public static ConfigEntry<bool> NoTeleport = null!;
        public static ConfigEntry<bool> PickableInteraction = null!;
        public static ConfigEntry<bool> ItemInteraction = null!;
        public static ConfigEntry<bool> DoorInteraction = null!;
        public static ConfigEntry<bool> ChestInteraction = null!;
        public static ConfigEntry<bool> CraftingStationInteraction = null!;
        public static ConfigEntry<bool> SignInteraction = null!;
        public static ConfigEntry<bool> SmelterInteraction = null!;
        public static ConfigEntry<bool> BeehiveInteraction = null!;
        public static ConfigEntry<bool> MaptableInteraction = null!;
        public static ConfigEntry<bool> EnableBubble = null!;
        public static ConfigEntry<float> WardDamageReduction = null!;
        public static ConfigEntry<float> WardDamageIncrease = null!;
        public static ConfigEntry<int> RaidablePlayersNeeded = null!;
        public static ConfigEntry<bool> RaidProtection = null!;
        public static ConfigEntry<bool> ShowraidableMessage = null!;
        public static ConfigEntry<bool> AutoRepair = null!;
        public static ConfigEntry<float> AutoRepairTime = null!;
        public static ConfigEntry<float> AutoRepairAmount = null!;
        public static ConfigEntry<bool> ShipInteraction = null!;
        public static ConfigEntry<bool> NoFoodDrain = null!;
        public static ConfigEntry<bool> ShowFlash = null!;
        private static ConfigEntry<int> _maxWardCountConfig = null!;
        private static ConfigEntry<int> _maxWardCountVipConfig = null!;
        public static ConfigEntry<int> MaxDaysDifferenceConfig = null!;
        public static ConfigEntry<string> ViPplayersListConfig = null!;
        public static ConfigEntry<bool> DisableGUI = null!;


        private ConfigEntry<T> config<T>(string group, string name, T value, ConfigDescription description,
            bool synchronizedSetting = true)
        {
            ConfigDescription extendedDescription =
                new(
                    description.Description +
                    (synchronizedSetting ? " [Synced with Server]" : " [Not Synced with Server]"),
                    description.AcceptableValues, description.Tags);
            ConfigEntry<T> configEntry = Config.Bind(group, name, value, extendedDescription);
            //var configEntry = Config.Bind(group, name, value, description);

            SyncedConfigEntry<T> syncedConfigEntry = configSync.AddConfigEntry(configEntry);
            syncedConfigEntry.SynchronizedConfig = synchronizedSetting;

            return configEntry;
        }

        private ConfigEntry<T> config<T>(string group, string name, T value, string description,
            bool synchronizedSetting = true)
        {
            return config(group, name, value, new ConfigDescription(description), synchronizedSetting);
        }

        private class ConfigurationManagerAttributes
        {
            public bool? Browsable = false;
        }

        class AcceptableShortcuts : AcceptableValueBase
        {
            public AcceptableShortcuts() : base(typeof(KeyboardShortcut))
            {
            }

            public override object Clamp(object value) => value;
            public override bool IsValid(object value) => true;

            public override string ToDescriptionString() =>
                "# Acceptable values: " + string.Join(", ", KeyboardShortcut.AllKeyCodes);
        }

        #endregion
    }
}