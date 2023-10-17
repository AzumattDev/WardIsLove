using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WardIsLove.Util.UI
{
    public class DropdownPopulate : MonoBehaviour
    {
        public static Dictionary<long, DropdownData> External_list = new();

        private static Dictionary<string, string> EffectAreaDict = new()
        {
            {
                "NoMonsters",
                "No Monsters"
            },
            {
                "ComfyZone", "Comfy Zone"
            },
            {
                "Default", "Default"
            }
        };

        internal static string SelectedPlayerName = null!;
        internal static string SelectedEffectArea = null!;
        internal static string SelectedPermission = null!;
        internal static string SelectedWardModel = null!;
        public Dropdown EffectAreaDropdown = null!;
        public Dropdown PermissionsDropdown = null!;
        public Dropdown PlayerDropdown = null!;
        public Dropdown DamageTypeDropdown = null!;
        public Dropdown ModelTypeDropdown = null!;
        public Text WardOwnerText = null!;
        public Text WardPermittedText = null!;
        public Text WardPermittedListText = null!;

        private void Start()
        {
            PopulateEffectAreaList();
            PopulatePermissionsList();
            PopulateDamageTypes();
            PopulatePlayerList();
            PopulateModelTypes();
        }

        private void OnGUI()
        {
            SelectedPlayerName = PlayerDropdown.options[PlayerDropdown.value].text;
            SelectedEffectArea = EffectAreaDropdown.options[EffectAreaDropdown.value].text;
            SelectedPermission = PermissionsDropdown.options[PermissionsDropdown.value].text;
            SelectedWardModel = ModelTypeDropdown.options[ModelTypeDropdown.value].text;
            PopulateOwnerText();
            PopulatePermittedText();
        }

        private void PopulateEffectAreaList()
        {
            EffectAreaDropdown.ClearOptions();
            foreach (WardIsLovePlugin.WardBehaviorEnums en in
                     Enum.GetValues(typeof(WardIsLovePlugin.WardBehaviorEnums)))
                EffectAreaDropdown.options.Add(new Dropdown.OptionData { text = EffectAreaDict[en.ToString()] });
        }

        private void PopulatePermissionsList()
        {
            PermissionsDropdown.ClearOptions();
            foreach (WardIsLovePlugin.WardInteractBehaviorEnums en in
                     Enum.GetValues(typeof(WardIsLovePlugin.WardInteractBehaviorEnums)))
                PermissionsDropdown.options.Add(new Dropdown.OptionData { text = en.ToString() });
        }

        private void PopulateDamageTypes()
        {
            DamageTypeDropdown.ClearOptions();
            HitData hit = new();
            foreach (WardIsLovePlugin.WardDamageTypes types in Enum.GetValues(typeof(WardIsLovePlugin.WardDamageTypes)))
                DamageTypeDropdown.options.Add(new Dropdown.OptionData { text = types.ToString() });
        }

        private void PopulateModelTypes()
        {
            ModelTypeDropdown.ClearOptions();

            foreach (WardIsLovePlugin.WardModelTypes en in
                     Enum.GetValues(typeof(WardIsLovePlugin.WardModelTypes)))
                ModelTypeDropdown.options.Add(new Dropdown.OptionData { text = en.ToString() });
        }

        public void PopulatePlayerList()
        {
            if (!PlayerDropdown || !ZNet.instance || !Player.m_localPlayer) return;

            ZRoutedRpc.instance.InvokeRoutedRPC(ZRoutedRpc.instance.GetServerPeerID(), "WILDropdownListRequest",
                new ZPackage());

            if (External_list.Count > 0)
            {
                PlayerDropdown.ClearOptions();
                foreach (KeyValuePair<long, DropdownData> player in External_list)
                {
                    PlayerDropdown.options.Add(new Dropdown.OptionData { text = player.Value.name });
                    WardIsLovePlugin.WILLogger.LogDebug(
                        $"Adding {player.Value.name} to dropdown. Player has an ID of {player.Value.id}");
                }
            }
        }

        private void PopulateOwnerText()
        {
            WardMonoscript paArea = WardGUI.PassInWardMonoscriptToGui();
            WardOwnerText.text = Localization.instance.Localize($"$piece_guardstone_owner: {paArea.GetCreatorName()}");
        }

        private void PopulatePermittedText()
        {
            WardMonoscript paArea = WardGUI.PassInWardMonoscriptToGui();
            string wardPermittedText = "";
            string wardPermittedListText = "";
            List<KeyValuePair<long, string>> permittedPlayers = paArea.GetPermittedPlayers();
            wardPermittedText += Localization.instance.Localize("$piece_guardstone_additional: ");
            for (int index = 0; index < permittedPlayers.Count; ++index)
            {
                wardPermittedText += permittedPlayers[index].Value;
                wardPermittedListText += permittedPlayers[index].Value;
                if (index != permittedPlayers.Count - 1)
                {
                    wardPermittedText += ", ";
                    wardPermittedListText += Environment.NewLine + " ";
                }
            }

            WardPermittedText.text = Localization.instance.Localize($"{wardPermittedText}");
            WardPermittedListText.text = Localization.instance.Localize($"{wardPermittedListText}");
        }
    }

    public class DropdownData
    {
        public long id;
        public string name = null!;
    }
}