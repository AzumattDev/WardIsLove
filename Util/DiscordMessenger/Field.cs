using System;
using HarmonyLib;
using YamlDotNet.Serialization;

namespace WardIsLove.Util.DiscordMessenger;

[HarmonyPatch]
[Serializable]
public class Field
{
    [YamlMember(Alias = "name")] public string Key { get; set; } = null!;

    [YamlMember(Alias = "value")] public string Value { get; set; } = null!;

    [YamlMember(Alias = "inline")] public bool Inline { get; set; }
}