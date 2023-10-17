using System;
using HarmonyLib;
using YamlDotNet.Serialization;

namespace WardIsLove.Util.DiscordMessenger;

[HarmonyPatch]
[Serializable]
public class Video
{
    [YamlMember(Alias = "url")] public string Url { get; set; } = null!;

    [YamlMember(Alias = "proxy_url", ApplyNamingConventions = false)] public string ProxyVideo { get; set; } = null!;

    [YamlMember(Alias = "width")] public string Width { get; set; } = null!;

    [YamlMember(Alias = "height")] public string Height { get; set; } = null!;
}