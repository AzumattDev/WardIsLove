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

        internal static string SelectedPlayerName;
        internal static string SelectedEffectArea;
        internal static string SelectedPermission;
        internal static string SelectedWardModel;
        public Dropdown EffectAreaDropdown;
        public Dropdown PermissionsDropdown;
        public Dropdown PlayerDropdown;
        public Dropdown DamageTypeDropdown;
        public Dropdown ModelTypeDropdown;
        public Text WardOwnerText;
        public Text WardPermittedText;
        public Text WardPermittedListText;

        private void Start()
        {
            PopulateEffectAreaList();
            PopulatePermissionsList();
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
            foreach (HitData.DamageType types in Enum.GetValues(typeof(HitData.DamageType)))
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
            
            ZRoutedRpc.instance.InvokeRoutedRPC(ZRoutedRpc.instance.GetServerPeerID(), "DropdownListRequest",
                new ZPackage());
            
            if (External_list.Count > 0)
            {
                PlayerDropdown.ClearOptions();
                foreach (KeyValuePair<long, DropdownData> player in External_list)
                {
                    PlayerDropdown.options.Add(new Dropdown.OptionData { text = player.Value.name });
                    WardIsLovePlugin.WILLogger.LogWarning($"Adding {player.Value.name} to dropdown.");
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
        public string name;
    }
}