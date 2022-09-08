using System;
using HarmonyLib;
using YamlDotNet.Serialization;

namespace WardIsLove.Util.DiscordMessenger;

[HarmonyPatch]
[Serializable]
public class Video
{
    [YamlMember(Alias = "url")] public string Url { get; set; }

    [YamlMember(Alias = "proxy_url")] public string ProxyVideo { get; set; }

    [YamlMember(Alias = "width")] public string Width { get; set; }

    [YamlMember(Alias = "height")] public string Height { get; set; }
}