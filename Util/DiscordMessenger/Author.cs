using System;
using HarmonyLib;
using YamlDotNet.Serialization;

namespace WardIsLove.Util.DiscordMessenger;

[HarmonyPatch]
[Serializable]
public class Author
{
    [YamlMember(Alias = "name")]
    public string Name { get; set; } = null!;

    [YamlMember(Alias = "url")]
    public string Url { get; set; } = null!;

    [YamlMember(Alias = "icon_url", ApplyNamingConventions = false)]
    public string Icon { get; set; } = null!;

    [YamlMember(Alias = "proxy_icon_url", ApplyNamingConventions = false)]
    public string ProxyIcon { get; set; } = null!;
}