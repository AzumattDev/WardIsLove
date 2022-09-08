using System;
using HarmonyLib;
using YamlDotNet.Serialization;

namespace WardIsLove.Util.DiscordMessenger;

[HarmonyPatch]
[Serializable]
public class Author
{
    [YamlMember(Alias = "name")]
    public string Name { get; set; }

    [YamlMember(Alias = "url")]
    public string Url { get; set; }

    [YamlMember(Alias = "icon_url")]
    public string Icon { get; set; }

    [YamlMember(Alias = "proxy_icon_url")]
    public string ProxyIcon { get; set; }
}