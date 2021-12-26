using Guilds;
using HarmonyLib;
using UnityEngine;
using WardIsLove.Extensions;
using WardIsLove.Util;

namespace WardIsLove.PatchClasses
{
    /// Push away player when inside ward
    [HarmonyPatch]
    public class Pushout
    {
        /* Testing YAML */
        /*        [HarmonyPatch(typeof(FejdStartup), nameof(FejdStartup.Start))]
                [HarmonyPostfix]
                public static void YAMLTEST_Postfix(FejdStartup __instance)
                {
                    try
                    {
                        var localTestClan = new Clan()
                        {
                            ClanSettings = new ClanSettings()
                            {
                                Name = "New Clan Name",
                                Description = "Coolest Clan ever?",
                                Leader = 123456789,
                                Level = 10,
                                Setting1 = "Setting 1",
                                Setting2 = "Setting 2"
                            },
                            Members = new List<Member?>()
                            {
                                new()
                                {
                                    SteamId = 123456789,
                                    Name = "Its Me Matt",
                                    Rank = "10",
                                    LastOnline = DateTime.Parse(DateTime.Now.ToString("MM-dd-yyyy HH:mm:ss tt")),
                                    PlayerId = 123456789
                                },
                                new()
                                {
                                    SteamId = 987654321,
                                    Name = "Nik",
                                    Rank = "15",
                                    LastOnline = DateTime.Parse(DateTime.Now.ToString("MM-dd-yyyy HH:mm:ss tt")),
                                    PlayerId = 987654321
                                }
                            }

                        };
                        var serializer = new SerializerBuilder()
                            .WithNamingConvention(CamelCaseNamingConvention.Instance)
                            .Build();
                        var yaml = serializer.Serialize(localTestClan);
                        //System.Console.WriteLine(yaml);
                        File.WriteAllText(Paths.PluginPath + "/Azumatt-WardIsLove/generatedFile.yml", yaml);

                        var deserializer = new DeserializerBuilder()
                            .WithNamingConvention(UnderscoredNamingConvention.Instance)
                            .Build();




                        var yml = File.ReadAllText(Paths.PluginPath + "/Azumatt-WardIsLove/generatedFile.yml");
                        // Setup the input
                        var input = new StringReader(yml);

                        // Load the stream
                        var yam = new YamlStream();
                        yam.Load(input);

                        // Examine the stream
                        var mapping =
                            (YamlMappingNode)yam.Documents[0].RootNode;


                        WILLogger.LogError(mapping);

                        // List all the items
                        var items = (YamlSequenceNode)mapping.Children[new YamlScalarNode("items")];
                        foreach (YamlMappingNode item in items)
                        {
                            WILLogger.LogError(
                                $"{item.Children[new YamlScalarNode("part_no")]}\t{item.Children[new YamlScalarNode("descrip")]}");
                        }
                    }
                    catch { }
                }*/

        [HarmonyPatch(typeof(Player), nameof(Player.Update))]
        [HarmonyPostfix]
        public static void Postfix_SetLocalPlayer(Player __instance)
        {
            /*            if (WardIsLovePlugin.guildAssembly != null)
                    {*/
            if (Player.m_localPlayer == null) return;
            Guild? guild = API.CreateGuild("AzumattGuild");
            API.AddPlayerToGuild(PlayerReference.fromPlayer(Player.m_localPlayer), guild);
            API.SaveGuild(guild);
            /*}*/
        }

        [HarmonyPatch(typeof(Player), nameof(Player.Update))]
        [HarmonyPostfix]
        public static void Postfix(Player __instance)
        {
            try
            {
                if (WardMonoscriptExt.WardMonoscriptsINSIDE == null) return;
                foreach (WardMonoscript? ward in WardMonoscriptExt.WardMonoscriptsINSIDE)
                {
                    if (!ward.IsEnabled()) continue;
                    if (!ward.GetPushoutPlayersOn() ||
                        ward.m_piece.GetCreator() == Game.instance.GetPlayerProfile().GetPlayerID() ||
                        ward.IsPermitted(Game.instance.GetPlayerProfile().GetPlayerID())) continue;
                    Vector3 position = __instance.transform.position;
                    Vector3 dir = (position - ward.transform.position).normalized;
                    position += dir * 0.15f;
                    __instance.transform.position = position;
                }
            }
            catch
            {
                // ignored
            }
        }

        /* Use character here instead of Humanoid to also pushout deer */
        [HarmonyPatch(typeof(Character), nameof(Character.FixedUpdate))]
        [HarmonyPostfix]
        public static void Postfix(Character __instance)
        {
            try
            {
                if (!__instance || __instance.IsDead()) return;
                if (!Player.m_localPlayer) return;
                if (WardMonoscriptExt.WardCharacterINSIDE == null) return;

                foreach (WardMonoscript? ward in WardMonoscriptExt.WardCharacterINSIDE)
                {
                    if (!ward.IsEnabled()) continue;
                    if (__instance.IsPlayer() || __instance.IsTamed()) continue;
                    if (!WardMonoscript.CheckInWardMonoscript(__instance.transform.position)) continue;
                    if (!ward.GetPushoutCreaturesOn()) continue;
                    Vector3 position = __instance.transform.position;
                    Vector3 dir = (position - ward.transform.position).normalized;
                    position += dir * 0.15f;
                    __instance.transform.position = position;
                }
            }
            catch
            {
                // ignored
            }
        }
    }
}