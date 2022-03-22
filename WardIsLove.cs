using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using ServerSync;
using UnityEngine;
using WardIsLove.Extensions;
using WardIsLove.PatchClasses;
using WardIsLove.Util;
using WardIsLove.Util.UI;

namespace WardIsLove
{
    [BepInPlugin(HGUIDLower, ModName, version)]
    [BepInIncompatibility("azumatt.BetterWards")]
    [BepInDependency("org.bepinex.plugins.guilds", BepInDependency.DependencyFlags.SoftDependency)]
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
            Everyone = 2
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

        public const string version = "2.3.4";
        public const string ModName = "WardIsLove";
        internal const string Author = "Azumatt";
        internal const string HGUID = Author + "." + "WardIsLove";
        internal const string HGUIDLower = "azumatt.WardIsLove";
        private const string HarmonyGUID = "Harmony." + Author + "." + ModName;
        private static string ConfigFileName = HGUIDLower + ".cfg";
        private static string ConfigFileFullPath = Paths.ConfigPath + Path.DirectorySeparatorChar + ConfigFileName;
        public static string ConnectionError = "";
        public static bool IsUpToDate;
        public static bool ValidServer = false;
        public static bool Admin = false;
        public static bool Raidable = false;
        public static bool fInit = false;
        public static int EffectTick = 0;
        public static GameObject Thorward;
        public static GameObject LightningVFX;

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

        private string hashCheck = "";
        public static WardIsLovePlugin Instance { get; private set; }


        public void Awake()
        {
            _serverConfigLocked = config("General", "Force Server Config", true, "Force Server Config");
            _ = configSync.AddLockingConfigEntry(_serverConfigLocked);


            /* ANNOUNCEMENT IN CONFIGS */
            _announcement = config("Announcement", "Information about this config file", "",
                "The values set in this config file are the GLOBAL defaults for each ward placed. Admins and owners (when allowed) can change the individual ward configurations per ward",
                false);
            /* Charge */
            _chargeItem = config("Charge", "Charge Item", "Thunderstone",
                "Item needed to charge the ward. Limit is 1 item: Goes by prefab name. List here: https://github.com/Valheim-Modding/Wiki/wiki/ObjectDB-Table");
            _chargeItemAmount = config("Charge", "Charge Item Amount", 5,
                "Amount of the Item needed to charge the ward. If you set this to 0, the item is not needed and can charge without cost.");
            /* Control GUI */
            _wardControl = config("Control GUI", "Ward Control", false, "Should ward owners have control of their ward via their own (limited) GUI interface?");
            /* General */
            _wardEnabled = config("General", "WardEnabled", true, "Enable WardIsLove Configurations");
            _showMarker = config("General", "ShowMarker", true,
                "Whether or not you want to show the area marker for wards", false);
            _wardHotKey = config("General", "WardHotKey", KeyCode.G,
                "Personal hotkey to toggle a ward on which you're permitted on/off", false);
            _autoClose = config("General", "AutoCloseDoors", false,
                "Whether or not you want to have doors auto close inside the ward.", false);
            _wardNotify = config("General", "WardNotify", true,
                "Whether or not you want to be notified when entering and leaving a ward.", false);
            _wardNotifyMessageEntry = config("General", "WardEntryMessage", "Entered {0}'s ward",
                "Entry message for ward notifications");
            _wardNotifyMessageExit = config("General", "WardExitMessage", "{0} you have left the ward",
                "Exit message for ward notifications");
            _wardDamageIncrease = config("General", "WardDamageIncrease", 0f,
                new ConfigDescription(
                    "Increase incoming damage to creatures in the game while they are inside the ward.\nValues are in percentage 0% - 100%.",
                    new AcceptableValueRange<float>(0, 100)));
            _disablePickup = config("General", "EnablePickup", false,
                "Allow non-permitted users to automatically picking up items inside a ward");
            _adminAutoPerm = config("General", "AdminAutoPerm", false,
                "Enable or disable the auto-permit on wards for admins", false);
            _itemStandInteraction = config("General", "ItemStandInteraction", false,
                "Allow non-permitted users to interact with item stands inside a ward");
            _portalInteraction = config("General", "PortalInteraction", false,
                "Allow non-permitted users to interact with portals inside a ward");
            _noTeleport = config("General", "NoTeleport", true,
                "Prevent non-permitted users from going through a portal inside a ward");
            _pickableInteraction = config("General", "PickableInteraction", false,
                "Allow non-permitted users to interact with crops/pickables inside a ward");
            _itemInteraction = config("General", "ItemInteraction", false,
                "Allow non-permitted users to interact with items dropped inside a ward");
            _doorInteraction = config("General", "DoorInteraction", false,
                "Allow non-permitted users to interact with doors inside a ward");
            _chestInteraction = config("General", "ChestInteraction", false,
                "Allow non-permitted users to interact with chests inside a ward");
            _craftingStationInteraction = config("General", "CraftingStationInteraction", false,
                "Allow non-permitted users to interact with crafting stations inside a ward");
            _smelterInteraction = config("General", "SmeltingStationInteraction", false,
                "Allow non-permitted users to interact with Smelters inside a ward");
            _beehiveInteraction = config("General", "BeehiveInteraction", false,
                "Allow non-permitted users to interact with Beehives inside a ward");
            _maptableInteraction = config("General", "MapTableInteraction", false,
                "Allow non-permitted users to interact with MapTables inside a ward");
            _signInteraction = config("General", "SignInteraction", false,
                "Allow non-permitted users to interact with Signs inside a ward");
            _wardNoDeathPen = config("General", "WardNoDeathPen", false,
                "Enables wards to provide no skill loss on death to those who have access yet die inside a ward.");
            _enableBubble = config("General", "EnableBubble", false,
                "Enables the bubble visual effect on wards");
            _pushoutPlayers = config("General", "PushoutPlayers", false,
                "Prevent non-permitted users from entering the warded area. If they are already inside, you must deal with the consequences.");
            _pushoutCreatures = config("General", "PushoutCreatures", false,
                "Prevent creatures from entering the warded area. If they are already inside, you must deal with the consequences");
            _shipInteraction = config("General", "ShipInteraction", false,
                "Allow non-permitted users to interact with ships inside a ward");
            _noFoodDrain = config("General", "NoFoodDrain", false,
                "Prevent food loss inside ward for permitted players");
            /*_wardDamageAmount = config("General", "WardDamageAmount", 0f,
                new ConfigDescription(
                    "Amount of damage, per tick, to creatures while they are inside the ward. Does not apply to tames\nValues are in percentage 0% - XXXX%.",
                    new AcceptableValueRange<float>(0f, 1000f)));*/


            /* Show Flash */
            _showFlash = config("General", "ShowFLASH", true,
                "Show the ward flash when something is damaged");

            /* Fireplace Unlimited */
            _cookingUnlimited = config("Fire", "OvenUnlimited", false,
                "Infinite oven fuel, who doesn't want this? Enable to allow your oven to never run out of fuel inside a warded area");
            _bathingUnlimited = config("Fire", "BathingUnlimited", false,
                "Infinite bath fuel, who doesn't want this? Enable to allow your bath to never run out of fuel inside a warded area");
            _fireplaceUnlimited = config("Fire", "FireplaceUnlimited", false,
                "Fireplaces inside a ward no longer need constant fueling\nThis might conflict with other mods that fuel fires.\nDisable this feature if you are having problems.");
            _fireSources = config("Fire", "FireSources",
                "piece_walltorch,piece_sconce,piece_groundtorch,piece_groundtorch_wood,piece_groundtorch_green,piece_groundtorch_blue,piece_brazierceiling01,fire_pit,bonfire,hearth",
                "The fire sources inside a ward you want to never run out of fuel.\nUses Prefab Name.\nIf the Prefab Name contains the string, it will no longer require fuel.");
            //* Config Requirements *//
            /* Thor Ward */
            _thorwardItemReqs = config("ThorWard", "ItemReqs ThorWard",
                "Silver,SurtlingCore,TrophyAbomination,Thunderstone",
                "The items required to craft the Ward.\nUses Prefab Name. Limit of 4 items\nFull List here: https://github.com/Valheim-Modding/Wiki/wiki/ObjectDB-Table");
            _thorwardReco = config("ThorWard", "ItemRecovery ThorWard", "true,true,true,true",
                "Should the item be recoverable or not?\nGoes in order from left to right");
            _thorwardItemAmou = config("ThorWard", "ItemAmounts ThorWard", "15,30,1,1",
                "Amount of each item required to make the Thor Ward\nGoes in order from left to right");
            /* Ward Range */
            _wardRange = config<float>("WardRange", "WardRange", 20,
                new ConfigDescription("Range of the ward", new AcceptableValueRange<float>(0.0f, 100f)));
            /*Health Regen */
            _wardPassiveHealthRegen = config("Health Regen", "WardPassiveHealthRegenValue", 0.5f,
                "How much health you should regen passively inside a ward per second");
            /* Stamina Regen */
            _wardPassiveStaminaRegen = config("Stamina Regen", "WardPassiveStaminaRegenValue", 0.5f,
                "How much stamina you should regen passively inside a ward per second");
            /* PvE */
            _wardPve = config("PvE", "WardPvE", false,
                "Whether or not you want PvE enabled inside a ward by default.");
            _wardOnlyPerm = config("PvE", "WardOnlyPerm", false,
                "Whether or not you want PvE or PvP forced inside a ward ONLY when permitted or owner.");
            _wardNotPerm = config("PvE", "WardNotPerm", false,
                "Whether or not you want PvE or PvP forced inside a ward ONLY when NOT permitted or owner.");
            /* PvP */
            _wardPvP = config("PvP", "WardPvP", false,
                "Whether or not you want PvP enabled inside a ward by default.");
            _pvpNotPerm = config("PvP", "CallToArmsBETA", false,
                "Call to Arms BETA, couldn't test this one alone. Report all bugs!\nToggles PvP on for non-permitted players inside a ward. Attempt to toggle on for all permitted players.");
            _ctaMessage = config("PvP", "CallToArmsMessage", "Call to arms! An enemy has entered your ward!",
                "Call to Arms BETA message shown");
            /* Structures */
            _wardStructures = config("Structures", "IndestructibleItems", false,
                "Whether or not you want to have indestructible structures inside a ward. If this is set to true, and items are defined damage reduction is 100%");
            _itemStructureNames = config("Structures", "Items", "ward,portal,guard,chest,gate,door,iron,stone",
                "The items inside a ward you want to make indestructible.\nUses Prefab Name.\nIf the Prefab Name contains the string, it will be indestructible. List here: https://valheim-modding.github.io/Jotunn/data/pieces/piece-list.html");
            _wardDamageReduction = config("Structures", "WardDamageReduction", 0.0f,
                new ConfigDescription("Reduce incoming damage to player built structures/items",
                    new AcceptableValueRange<float>(0.0f, 100f)));
            _noWeatherDmg = config("Structures", "NoWeatherDmg", true,
                "No weather damage on structures inside wards");
            // _autoRepair = config("Structures", "AutoRepair", false, "Auto repair on structures inside wards");
            // _autoRepairTime = config("Structures", "AutoRepairTime", 3f, "Time between auto repair ticks");
            // _autoRepairAmount = config<float>("Structures", "AutoRepairAmount", 20,
            //     new ConfigDescription("Repair amount in percent", new AcceptableValueRange<float>(0.0f, 100f)));

            /* RAID PROTECTION */
            _raidProtection = config("Raid Protection", "RaidProtection", true,
                "Should offline raid protection be turned on?\nPrevents non permitted players from damaging your base if inside your ward");
            _raidablePlayersNeeded = config("Raid Protection", "RaidablePlayersNeeded", 2,
                "Minimum number of players required to be online for their warded area to be raided");
            _showraidableMessage = config("Raid Protection", "RaidableMessageShow", true,
                "Display Raid message if not raidable", false);


            localizationFile =
                new ConfigFile(
                    Path.Combine(Paths.ConfigPath, HGUIDLower + ".Localization.cfg"), false);

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
            SetupWatcher();
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
           // harmony.UnpatchSelf();
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


        public static void FinalInit(ZNetScene __instance)
        {
            if (!__instance || _thorwardItemReqs.Value.Length <= 0) return;

            // Update Recipe for controller users...
            if (!fInit) RecipeFunction.UpdateRecipeFinal();
            Piece thorWard = Thorward.GetComponent<Piece>();
            Recipe? tr = ObjectDB.instance.m_recipes.Find(rt => rt.name == thorWard.name);
            _ = ObjectDB.instance.m_recipes.Remove(tr);
        }

        [HarmonyPatch(typeof(ZNetScene), nameof(ZNetScene.Awake))]
        public static class WardIsLoveZNetScenePrefix
        {
            public static void Prefix(ZNetScene __instance)
            {
                if (__instance.m_prefabs is not { Count: > 0 }) return;
                __instance.m_prefabs.Add(Thorward);
                __instance.m_prefabs.Add(LightningVFX);
            }
        }

        [HarmonyPatch(typeof(ZNetScene), nameof(ZNetScene.Awake))]
        public static class WardIsLoveZNetPost
        {
            public static void Postfix(ZNetScene __instance)
            {
                GameObject? hammer = ZNetScene.instance.GetPrefab("Hammer");
                PieceTable? hammerTable = hammer.GetComponent<ItemDrop>().m_itemData.m_shared.m_buildPieces;
                _hammer = hammerTable;

                if (_hammer is not null && _hammer.m_pieces.Contains(Thorward)) return;
                _hammer?.m_pieces.Add(Thorward);
                _hammer?.m_pieces.Remove(ZNetScene.instance.GetPrefab("guard_stone").gameObject);
                ZNetScene.instance.GetPrefab("guard_stone").GetComponent<Piece>().enabled = false;
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

        public static ConfigEntry<bool>? _serverConfigLocked;
        public static ConfigEntry<bool>? _wardEnabled;

        public static ConfigEntry<bool>? _wardControl;
        public static ConfigEntry<string>? _chargeItem;
        public static ConfigEntry<int>? _chargeItemAmount;
        public static ConfigEntry<string>? _announcement;
        public static ConfigEntry<KeyCode>? _wardHotKey;
        public static ConfigEntry<bool>? _wardRangeEnabled;
        public static ConfigEntry<float>? _wardRange;
        public static ConfigEntry<float>? _wardPassiveHealthRegen;
        public static ConfigEntry<float>? _wardPassiveStaminaRegen;
        public static ConfigEntry<bool>? _wardOnlyPerm;
        public static ConfigEntry<bool>? _wardNotPerm;
        public static ConfigEntry<bool>? _pvpNotPerm;
        public static ConfigEntry<string>? _ctaMessage;
        public static ConfigEntry<bool>? _wardPvP;
        public static ConfigEntry<bool>? _wardPve;
        public static ConfigEntry<bool>? _showMarker;
        public static ConfigEntry<bool>? _wardStructures;
        public static ConfigEntry<bool>? _noWeatherDmg;
        public static ConfigEntry<string>? _itemStructureNames;
        public static ConfigEntry<bool>? _autoClose;
        public static ConfigEntry<float>? _wardDamageAmount;
        public static ConfigEntry<bool>? _wardNotify;
        public static ConfigEntry<string>? _wardNotifyMessageEntry;
        public static ConfigEntry<string>? _wardNotifyMessageExit;
        public static ConfigEntry<bool>? _wardNoDeathPen;
        public static ConfigEntry<bool>? _bathingUnlimited;
        public static ConfigEntry<bool>? _cookingUnlimited;
        public static ConfigEntry<bool>? _fireplaceUnlimited;
        public static ConfigEntry<string>? _fireSources;
        public static ConfigEntry<bool>? _disablePickup;
        public static ConfigEntry<bool>? _pushoutPlayers;
        public static ConfigEntry<bool>? _pushoutCreatures;
        public static ConfigEntry<bool>? _adminAutoPerm;
        public static ConfigEntry<bool>? _itemStandInteraction;
        public static ConfigEntry<bool>? _portalInteraction;
        public static ConfigEntry<bool>? _noTeleport;
        public static ConfigEntry<bool>? _pickableInteraction;
        public static ConfigEntry<bool>? _itemInteraction;
        public static ConfigEntry<bool>? _doorInteraction;
        public static ConfigEntry<bool>? _chestInteraction;
        public static ConfigEntry<bool>? _craftingStationInteraction;
        public static ConfigEntry<bool>? _signInteraction;
        public static ConfigEntry<bool>? _smelterInteraction;
        public static ConfigEntry<bool>? _beehiveInteraction;
        public static ConfigEntry<bool>? _maptableInteraction;
        public static ConfigEntry<bool>? _enableBubble;
        public static ConfigEntry<float>? _wardDamageReduction;
        public static ConfigEntry<float>? _wardDamageIncrease;
        public static ConfigEntry<int>? _raidablePlayersNeeded;
        public static ConfigEntry<bool>? _raidProtection;
        public static ConfigEntry<bool>? _showraidableMessage;
        public static ConfigEntry<bool>? _autoRepair;
        public static ConfigEntry<float>? _autoRepairTime;
        public static ConfigEntry<float>? _autoRepairAmount;
        public static ConfigEntry<string>? _thorwardItemReqs;
        public static ConfigEntry<string>? _thorwardReco;
        public static ConfigEntry<string>? _thorwardItemAmou;
        public static ConfigEntry<string>? _defaultwardItemReqs;
        public static ConfigEntry<string>? _defaultwardReco;
        public static ConfigEntry<string>? _defaultwardItemAmou;
        public static ConfigEntry<bool>? _shipInteraction;
        public static ConfigEntry<bool>? _noFoodDrain;
        public static ConfigEntry<bool>? _showFlash;
        private static ConfigEntry<int>? MaxWardCountConfig;
        private static ConfigEntry<int>? MaxWardCountVIPConfig;
        public static ConfigEntry<int>? MaxDaysDifferenceConfig;
        public static ConfigEntry<string>? VIPplayersListConfig;


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

        #endregion
    }
}
