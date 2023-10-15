using BepInEx.Configuration;
using HarmonyLib;

namespace WardIsLove.PatchClasses
{
    [HarmonyPatch]
    public class Local
    {
        public static void Localize()
        {

            /* Hover Text */
            _ = LocalizeWord("betterwards_accessMode", "Access • ");
            _ = LocalizeWord("betterwards_accessMode_OwnerOnly", "Owner Only");
            _ = LocalizeWord("betterwards_accessMode_Guild", "Guild Members");
            _ = LocalizeWord("betterwards_accessMode_Group", "Group Members");
            _ = LocalizeWord("betterwards_accessMode_Everyone", "Everyone");
            _ = LocalizeWord("betterwards_accessMode_Default", "Default");

            _ = LocalizeWord("betterwards_bubbleMode", "Bubble Mode • ");
            _ = LocalizeWord("betterwards_bubbleMode_Default", "Default (Creature Spawn Blocker)");
            _ = LocalizeWord("betterwards_bubbleMode_NoMonsters", "No Monster Area");

            /* Ward Menu */
            _ = LocalizeWord("wardmenu_title", "Ward Config Menu");

            /* Ward GUI General Tab */
            _ = LocalizeWord("wardmenu_bubbletoggle", "Toggle Bubble");
            _ = LocalizeWord("wardmenu_weatherdmg", "No Weather Damage");
            _ = LocalizeWord("wardmenu_wardradius", "Ward Radius");
            _ = LocalizeWord("wardmenu_health", "Health Regen Amount");
            _ = LocalizeWord("wardmenu_stamina", "Stamina Regen Amount");
            _ = LocalizeWord("wardmenu_autopickup", "Auto-Pickup");
            _ = LocalizeWord("wardmenu_autoclosedoors", "Auto Close Doors");
            _ = LocalizeWord("wardmenu_fireplaceunlimited", "Unlimited Fireplace Fuel");
            _ = LocalizeWord("wardmenu_bathingunlimited", "Unlimited HotTub Fuel");
            _ = LocalizeWord("wardmenu_cookingunlimited", "Unlimited Cooking Fuel");
            _ = LocalizeWord("wardmenu_nodeathpen", "No Death Penalty");
            _ = LocalizeWord("wardmenu_nofooddrain", "No Food Drain");
            //_ = LocalizeWord("wardmenu_pushout", "Pushout");
            _ = LocalizeWord("wardmenu_showflash", "Show Ward Flash");
            _ = LocalizeWord("wardmenu_showmarker", "Show Ward Marker");
            _ = LocalizeWord("wardmenu_noteleport", "No Teleport");
            _ = LocalizeWord("wardmenu_joinmydiscord", "Join my Discord");
            _ = LocalizeWord("wardmenu_joinmyteamsdiscord", "Join My Teams Discord");
            _ = LocalizeWord("wardmenu_wardnotifications", "Ward Notify");
            _ = LocalizeWord("wardmenu_exitnotifymessage", "Exit Notification Message");
            _ = LocalizeWord("wardmenu_firesourcelist", "Firesource List");
            _ = LocalizeWord("wardmenu_pve", "PvE");
            _ = LocalizeWord("wardmenu_onlyperm", "Only Permitted");
            _ = LocalizeWord("wardmenu_notperm", "Not-Permitted");


            /*Ward GUI Tabs*/
            _ = LocalizeWord("wardtab_general", "General");
            _ = LocalizeWord("wardtab_additional", "Additional");
            _ = LocalizeWord("wardtab_feedback", "Feedback");
            _ = LocalizeWord("wardtab_access", "Access");
            _ = LocalizeWord("wardmenu_chestinteract", "Chest Interaction");
            _ = LocalizeWord("wardmenu_doorinteract", "Door Interaction");
            _ = LocalizeWord("wardmenu_pushoutplayers", "Pushout Players");
            _ = LocalizeWord("wardmenu_pushoutcreatures", "Pushout Creatures");
            _ = LocalizeWord("wardmenu_pickableinteract", "Pickable Protection");


            /* Ward Tab Additional */
            _ = LocalizeWord("wardmenu_damageamount", "Damage Amount");
            _ = LocalizeWord("wardmenu_indestructiblestruct", "Protect Stuctures");
            _ = LocalizeWord("wardmenu_indestructibles", "Structure List");
            _ = LocalizeWord("wardmenu_damagereduction", "Damage Reduction");
            _ = LocalizeWord("wardmenu_creaturedamageincrease", "Creature Damage Increase");
            _ = LocalizeWord("wardmenu_autorepairamount", "Auto Repair Amount");

            _ = LocalizeWord("wardmenu_removePermitted", "Remove Player");
            _ = LocalizeWord("wardmenu_addPermitted", "Add Player");
            _ = LocalizeWord("wardmenu_permittedlist", "Permitted List");
            _ = LocalizeWord("wardmenu_playerlist", "Player List");
            _ = LocalizeWord("permitted_list", "Permitted List");
            _ = LocalizeWord("wardmenu_notifymessage", "Notify Message");
            _ = LocalizeWord("wardmenu_itemstandinteract", "Itemstand Interaction");
            _ = LocalizeWord("wardmenu_portalinteract", "Portal Interaction");
            _ = LocalizeWord("wardmenu_iteminteract", "Item Interaction");
            _ = LocalizeWord("wardmenu_shipinteract", "Ship Interaction");
            _ = LocalizeWord("wardmenu_signinteract", "Sign Interaction");
            _ = LocalizeWord("wardmenu_craftingstationinteract", "Crafting Station Interaction");
            _ = LocalizeWord("wardmenu_smelterinteract", "Smelter Interaction");
            _ = LocalizeWord("wardmenu_beehiveinteract", "Beehive Interaction");
            _ = LocalizeWord("wardmenu_maptableinteract", "MapTable Interaction");
            _ = LocalizeWord("wardmenu_pvp", "PvP");
            _ = LocalizeWord("wardmenu_pvponlyperm", "PvP Only Permitted");
            _ = LocalizeWord("wardmenu_ctamessage", "Call to Arms Message");
            _ = LocalizeWord("wardmenu_autorepair", "Auto Repair");
            _ = LocalizeWord("wardmenu_autorepairtime", "Repair Time");
            _ = LocalizeWord("wardmenu_raidprotection", "Raid Protection");
            _ = LocalizeWord("wardmenu_raidplayersneeded", "Players Needed");

            /* Ward Tab Feedback */
            _ = LocalizeWord("wardmenu_optionfeedback", "<color=#FFFF00>Feedback</color>");
            _ = LocalizeWord("wardmenu_optionbug", "<color=#FF0000>Bug</color>");
            _ = LocalizeWord("wardmenu_optionidea", "<color=#00FF00>Idea</color>");

            _ = LocalizeWord("wardmenu_wardislove", "Ward Is Love?");

            /* Thor ward */
            _ = LocalizeWord("piece_thorward", "<color=#00FFFF>Thor</color>");
            _ = LocalizeWord("piece_thorward_description", "The power of Thor stored in order to protect you.");
        }

        private static string LocalizeWord(string key, string val)
        {
            if (WardIsLovePlugin.localizedStrings.ContainsKey(key)) return $"${key}";
            Localization? loc = Localization.instance;
            string? langSection = loc.GetSelectedLanguage();
            ConfigEntry<string>? configEntry = WardIsLovePlugin.localizationFile.Bind(langSection, key, val);
            Localization.instance.AddWord(key, configEntry.Value);
            WardIsLovePlugin.localizedStrings.Add(key, configEntry);

            return $"${key}";
        }
    }
}