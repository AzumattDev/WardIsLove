using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using WardIsLove.Extensions;
using WardIsLove.Util.DiscordMessenger;

namespace WardIsLove.Util.UI
{
    public class WardGUIUtil : MonoBehaviour
    {
        /* General */
        public Slider m_wardradius;
        public Toggle m_nofooddrain;
        public Toggle m_autoclosedoors;
        public Toggle m_wardnotifyToggle;
        public InputField m_enternotifyMessage;
        public InputField m_exitnotifyMessage;
        public Toggle m_weatherDmgToggle;
        public Toggle m_nodeathpen;
        public Toggle m_showFlashToggle;
        public Toggle m_showMarkerToggle;
        public Slider m_healthboost;
        public Slider m_staminaboost;
        public Toggle m_bathingunlimited;
        public Toggle m_cookingunlimited;
        public Toggle m_fireplaceunlimited;
        public InputField m_firesourceList;

        public Toggle m_pveEnforceToggle;
        public Toggle m_pvpEnforceToggle;
        public Toggle m_onlyPerm;
        public Toggle m_notPerm;
        public InputField m_ctaMessage;

        /* Access */
        public Toggle m_bubbleToggle;
        public Toggle m_autopickup;
        public Toggle m_itemstandInteractToggle;
        public Toggle m_portalInteractToggle;
        public Toggle m_itemInteractToggle;
        public Toggle m_doorInteractToggle;
        public Toggle m_chestInteractToggle;
        public Toggle m_shipInteractToggle;
        public Toggle m_signInteractToggle;
        public Toggle m_craftingStationInteractToggle;
        public Toggle m_smelterInteractToggle;
        public Toggle m_beehiveInteractToggle;
        public Toggle m_maptableInteractToggle;
        public Toggle m_noteleportToggle;
        public Toggle m_pickableInteractToggle;
        public Toggle m_pushoutPlayers;
        public Toggle m_pushoutCreatures;
        public Dropdown m_effectarea;
        public Dropdown m_permissions;
        public Dropdown m_PlayerDropdown;
        public GameObject m_GradientPicker;
        public GameObject m_ColorPicker;


        /* Additional */

        public Dropdown m_damageType;
        public Dropdown m_modelType;
        public InputField m_damageAmount;
        public Toggle m_indestructibleToggle;
        public InputField m_indestructibleList;
        public Slider m_creatureDamageIncrease;
        public Slider m_structDamageReduction;
        public Toggle m_autoRepairToggle;
        public Slider m_autoRepairAmount;
        public InputField m_autoRepairTime;
        public Toggle m_raidProtectionToggle;
        public InputField m_raidPlayersNeeded;

        public Toggle m_wardIsLove;

        /* Feedback */
        public Text m_subject;
        public Text m_text;
        public Button m_sendButton;
        public Dropdown m_feebackType;

        private void Start()
        {
            ResetWardGUI();
        }

        private void OnGUI()
        {
            PopWardGui();
        }

        public void ResetWardGUI()
        {
            /* General */

            /* TODO FLUSH OUT ALL OPTIONS HERE */
            if (WardGUI.PassInWardMonoscriptToGui() != null)
            {
                WardMonoscript paArea = WardGUI.PassInWardMonoscriptToGui();
                m_wardradius.value = paArea.GetWardRadius();
            }
            else
            {
                m_wardradius.value = 25;
            }

            m_nofooddrain.isOn = false;
            m_autoclosedoors.isOn = false;
            m_wardnotifyToggle.isOn = false;
            m_enternotifyMessage.text = "";
            m_exitnotifyMessage.text = "";
            m_weatherDmgToggle.isOn = false;
            m_nodeathpen.isOn = false;
            m_healthboost.value = 0;
            m_staminaboost.value = 0;
            m_fireplaceunlimited.isOn = false;
            m_bathingunlimited.isOn = false;
            m_cookingunlimited.isOn = false;
            m_firesourceList.text = WardIsLovePlugin.FireSources.Value;
            m_pvpEnforceToggle.isOn = false;
            m_pveEnforceToggle.isOn = false;
            m_onlyPerm.isOn = false;
            m_notPerm.isOn = false;
            m_ctaMessage.text = WardIsLovePlugin.CtaMessage.Value;

            /* Access */
            m_autopickup.isOn = false;
            m_pushoutPlayers.isOn = false;
            m_pushoutCreatures.isOn = false;
            m_bubbleToggle.isOn = false;
            m_noteleportToggle.isOn = false;
            m_itemstandInteractToggle.isOn = false;
            m_portalInteractToggle.isOn = false;
            m_pickableInteractToggle.isOn = false;
            m_itemInteractToggle.isOn = false;
            m_doorInteractToggle.isOn = false;
            m_chestInteractToggle.isOn = false;
            m_shipInteractToggle.isOn = false;
            m_signInteractToggle.isOn = false;
            m_craftingStationInteractToggle.isOn = false;
            m_smelterInteractToggle.isOn = false;
            m_beehiveInteractToggle.isOn = false;
            m_maptableInteractToggle.isOn = false;
            m_showFlashToggle.isOn = false;

            /* Additional */
            m_damageAmount.text = WardIsLovePlugin.WardDamageAmount.Value.ToString();
            m_indestructibleToggle.isOn = false;
            m_indestructibleList.text = WardIsLovePlugin.ItemStructureNames.Value;
            m_creatureDamageIncrease.value = 0;
            m_structDamageReduction.value = 0;
            m_wardIsLove.isOn = false;
            /*m_autoRepairToggle.isOn = false;
            m_autoRepairAmount.value = 0;
            m_autoRepairTime.text = WardIsLovePlugin._autoRepairTime.Value.ToString();*/
            m_raidProtectionToggle.isOn = false;
            m_raidPlayersNeeded.text = WardIsLovePlugin.RaidablePlayersNeeded.Value.ToString();
        }

        private void Show()
        {
        }

        public void Hide()
        {
            WardGUI.ShowCursor(false);
            WardGUI.wardGUI.SetActive(false);
            WardGUI.wardGUINoAdmin.SetActive(false);
        }

        public void PopWardGui()
        {
            WardMonoscript paArea = WardGUI.PassInWardMonoscriptToGui();
            m_wardradius.value = paArea.GetWardRadius();
            m_nofooddrain.isOn = paArea.GetNoFoodDrainOn();
            m_autoclosedoors.isOn = paArea.GetAutoCloseDoorsOn();
            m_wardnotifyToggle.isOn = paArea.GetWardNotificationsOn();
            //m_enternotifyMessage.text = paArea.GetWardEnterNotifyMessage();
            //m_exitnotifyMessage.text = paArea.GetWardExitNotifyMessage();
            m_weatherDmgToggle.isOn = paArea.GetWeatherDmgOn();
            m_nodeathpen.isOn = paArea.GetNoDeathPenOn();
            m_showFlashToggle.isOn = paArea.GetShowFlashOn();
            m_healthboost.value = paArea.GetHealthBoost();
            m_staminaboost.value = paArea.GetStaminaBoost();
            m_fireplaceunlimited.isOn = paArea.GetFireplaceUnlimOn();
            m_cookingunlimited.isOn = paArea.GetCookingUnlimOn();
            m_bathingunlimited.isOn = paArea.GetBathingUnlimOn();
            //m_firesourceList.text = paArea.GetFireplaceList();
            m_pvpEnforceToggle.isOn = paArea.GetPvpOn();
            m_pveEnforceToggle.isOn = paArea.GetPveOn();
            m_onlyPerm.isOn = paArea.GetOnlyPermOn();
            m_notPerm.isOn = paArea.GetNotPermOn();
            m_portalInteractToggle.isOn = paArea.GetPortalInteractOn();
            m_chestInteractToggle.isOn = paArea.GetChestInteractOn();
            m_doorInteractToggle.isOn = paArea.GetDoorInteractOn();
            m_itemInteractToggle.isOn = paArea.GetItemInteractOn();
            m_shipInteractToggle.isOn = paArea.GetShipInteractOn();
            m_signInteractToggle.isOn = paArea.GetSignInteractOn();
            m_craftingStationInteractToggle.isOn = paArea.GetCraftingStationInteractOn();
            m_smelterInteractToggle.isOn = paArea.GetSmelterInteractOn();
            m_beehiveInteractToggle.isOn = paArea.GetBeehiveInteractOn();
            m_maptableInteractToggle.isOn = paArea.GetMapTableInteractOn();
            m_itemstandInteractToggle.isOn = paArea.GetItemStandInteractOn();
            m_pickableInteractToggle.isOn = paArea.GetPickableInteractOn();
            m_autopickup.isOn = paArea.GetAutoPickupOn();
            //m_ctaMessage.text = paArea.GetCtaMessage();
            //m_damageType.value = (int)paArea.GetDamageType();
            m_pushoutPlayers.isOn = paArea.GetPushoutPlayersOn();
            m_pushoutCreatures.isOn = paArea.GetPushoutCreaturesOn();
            m_bubbleToggle.isOn = paArea.GetBubbleOn();
            m_noteleportToggle.isOn = paArea.GetNoTeleportOn();
            //m_damageAmount.text = paArea.GetWardDamageAmount();
            m_indestructibleToggle.isOn = paArea.GetIndestructibleOn();
            //m_indestructibleList.text = paArea.GetIndestructList();
            m_creatureDamageIncrease.value = paArea.GetCreatureDamageIncrease();
            m_structDamageReduction.value = paArea.GetStructDamageReduc();
            /*m_autoRepairToggle.isOn = paArea.GetAutoRepairOn();
            m_autoRepairAmount.value = paArea.GetAutoRepairAmount();*/
            //m_autoRepairTime.text = paArea.GetAutoRepairTextTime().ToString();
            m_raidProtectionToggle.isOn = paArea.GetRaidProtectionOn();
            m_wardIsLove.isOn = paArea.GetWardIsLoveOn();
            // m_raidPlayersNeeded.text = paArea.GetRaidProtectionPlayerNeeded().ToString();
        }

        public void WardRadiusSlider()
        {
            WardMonoscript paArea = WardGUI.PassInWardMonoscriptToGui();
            paArea.SetWardRadius(m_wardradius.value);
        }

        public void StaminaSlider()
        {
            WardMonoscript paArea = WardGUI.PassInWardMonoscriptToGui();
            paArea.SetStaminaBoost(m_staminaboost.value);
        }

        public void HealthSlider()
        {
            WardMonoscript paArea = WardGUI.PassInWardMonoscriptToGui();
            paArea.SetHealthBoost(m_healthboost.value);
        }

        public void EffectAreaCyleMode()
        {
            WardMonoscript paarea = WardGUI.PassInWardMonoscriptToGui();
            paarea.SetBubbleMode((WardIsLovePlugin.WardBehaviorEnums)m_effectarea.value);
        }

        public void PermissionsCyleMode()
        {
            WardMonoscript paarea = WardGUI.PassInWardMonoscriptToGui();
            paarea.SetAccessMode((WardIsLovePlugin.WardInteractBehaviorEnums)m_permissions.value);
        }

        public void AutoPickupToggle()
        {
            WardMonoscript paArea = WardGUI.PassInWardMonoscriptToGui();
            paArea.SetAutoPickupOnOn(m_autopickup.isOn);
        }

        public void AutoCloseToggle()
        {
            WardMonoscript paArea = WardGUI.PassInWardMonoscriptToGui();
            paArea.SetAutoCloseDoorsOn(m_autoclosedoors.isOn);
        }

        public void FireplaceUnlimToggle()
        {
            WardMonoscript paArea = WardGUI.PassInWardMonoscriptToGui();
            paArea.SetFireplaceUnlimOn(m_fireplaceunlimited.isOn);
        }

        public void BathingUnlimToggle()
        {
            WardMonoscript paArea = WardGUI.PassInWardMonoscriptToGui();
            paArea.SetBathingUnlimOn(m_bathingunlimited.isOn);
        }

        public void CookingUnlimToggle()
        {
            WardMonoscript paArea = WardGUI.PassInWardMonoscriptToGui();
            paArea.SetCookingUnlimOn(m_cookingunlimited.isOn);
        }

        public void NoDeathPenToggle()
        {
            WardMonoscript paArea = WardGUI.PassInWardMonoscriptToGui();
            paArea.SetNoDeathPenOn(m_nodeathpen.isOn);
        }

        public void NoFoodDrainToggle()
        {
            WardMonoscript paArea = WardGUI.PassInWardMonoscriptToGui();
            paArea.SetNoFoodDrainOn(m_nofooddrain.isOn);
        }

        public void PushoutPlayersToggle()
        {
            WardMonoscript paArea = WardGUI.PassInWardMonoscriptToGui();
            paArea.SetPushoutPlayersOn(m_pushoutPlayers.isOn);
        }

        public void PushoutCreaturesToggle()
        {
            WardMonoscript paArea = WardGUI.PassInWardMonoscriptToGui();
            paArea.SetPushoutCreaturesOn(m_pushoutCreatures.isOn);
        }

        public void BubbleToggle()
        {
            WardMonoscript paArea = WardGUI.PassInWardMonoscriptToGui();
            paArea.SetBubbleOn(m_bubbleToggle.isOn);
        }

        public void WeatherDmgToggle()
        {
            WardMonoscript paArea = WardGUI.PassInWardMonoscriptToGui();
            paArea.SetWeatherDmgOn(m_weatherDmgToggle.isOn);
        }

        public void ShowFlashToggle()
        {
            WardMonoscript paArea = WardGUI.PassInWardMonoscriptToGui();
            paArea.SetShowFlashOn(m_showFlashToggle.isOn);
        }

        public void ShowMarkerToggle()
        {
            WardMonoscript paArea = WardGUI.PassInWardMonoscriptToGui();
            paArea.SetShowMarkerOn(m_showMarkerToggle.isOn);
        }

        public void NoTeleportToggle()
        {
            WardMonoscript paArea = WardGUI.PassInWardMonoscriptToGui();
            paArea.SetNoTeleportOn(m_noteleportToggle.isOn);
        }

        public void SendClick()
        {
            // if (string.IsNullOrWhiteSpace(m_text.text.ToString())) return;
            long playerId = Game.instance.GetPlayerProfile().m_playerID;
            string playername = Player.m_localPlayer.GetPlayerName();

            string Escaper(string StrIn)
            {
                return StrIn.Replace("\"", "\\\"");
            }
            
            new DiscordMessage()
                .SetUsername($"WardIsLove v{WardIsLovePlugin.version}")
                .SetAvatar("https://staticdelivery.nexusmods.com/mods/3667/images/402/402-1620654411-147438437.png")
                .AddEmbed()
                .SetTimestamp(DateTime.Now)
                .SetAuthor($"{Escaper((playername))}")
                .SetTitle("Subject")
                .SetDescription($"{Escaper(m_subject.text)}")
                .SetColor(15258703)
                .AddField("Feedback Type", $"{WardGUI.FeedbackDropdownValue}")
                .AddField("Message", $"{Escaper(m_text.text)}")
                .Build()
                .SendMessageAsync(
                    "redacted");
            Hide();
        }

        public void JoinMyDiscord()
        {
            _ = Process.Start("https://discord.gg/KNvbUjWTxr");
        }

        public void JoinOurDiscord()
        {
            _ = Process.Start("https://discord.gg/nsEYWab3AY");
        }

        public void AddPermitted()
        {
            WardMonoscript paArea = WardGUI.PassInWardMonoscriptToGui();
            Dictionary<long, DropdownData> playerList = DropdownPopulate.External_list;
            KeyValuePair<long, DropdownData> playerInfo =
                playerList.FirstOrDefault(t => t.Value.name == DropdownPopulate.SelectedPlayerName);
            //var playerInfo = playerList.Find(pID => pID.m_name == DropdownPopulate.SelectedPlayerName);
            List<KeyValuePair<long, string>> permittedPlayers = paArea.GetPermittedPlayers();

            if (permittedPlayers.Any(keyValuePair => keyValuePair.Key == playerInfo.Value.id))
            {
                return;
            }

            permittedPlayers.Add(new KeyValuePair<long, string>(playerInfo.Value.id, playerInfo.Value.name));
            paArea.SetPermittedPlayers(permittedPlayers);
            _ = paArea.m_addPermittedEffect.Create(transform.position, transform.rotation);
        }

        public void RemovePermitted()
        {
            WardMonoscript paArea = WardGUI.PassInWardMonoscriptToGui();
            Dictionary<long, DropdownData> playerList = DropdownPopulate.External_list;
            KeyValuePair<long, DropdownData> playerInfo =
                playerList.FirstOrDefault(t => t.Value.name == DropdownPopulate.SelectedPlayerName);
            List<KeyValuePair<long, string>> permittedPlayers = paArea.GetPermittedPlayers();
            if (permittedPlayers.RemoveAll(x => x.Key == playerInfo.Value.id) <= 0)
                return;
            paArea.SetPermittedPlayers(permittedPlayers);
            _ = paArea.m_removedPermittedEffect.Create(transform.position, transform.rotation);
        }

        public void SetWardNotificationsOn()
        {
            WardMonoscript paArea = WardGUI.PassInWardMonoscriptToGui();
            paArea.SetWardNotificationsOn(m_wardnotifyToggle.isOn);
        }

        public void WardEnterNotifyMessage()
        {
            WardMonoscript paArea = WardGUI.PassInWardMonoscriptToGui();
            paArea.SetWardEnterNotifyMessage(m_enternotifyMessage.text);
        }

        public void WardExitNotifyMessage()
        {
            WardMonoscript paArea = WardGUI.PassInWardMonoscriptToGui();
            paArea.SetWardExitNotifyMessage(m_exitnotifyMessage.text);
        }

        public void FireplaceListSet()
        {
            WardMonoscript pa = WardGUI.PassInWardMonoscriptToGui();
            pa.SetFireplaceList(m_firesourceList.text);
        }

        public void SetWardModelType()
        {
            WardMonoscript pa = WardGUI.PassInWardMonoscriptToGui();
            pa.m_nview.InvokeRPC(ZNetView.Everybody, "SyncWardsMOFO", m_modelType.value);
        }

        public void SetWardDamageType()
        {
            WardMonoscript pa = WardGUI.PassInWardMonoscriptToGui();
            pa.SetDamageType((WardIsLovePlugin.WardDamageTypes)m_damageType.value);
        }

        public void SetWardDamageAmount()
        {
            WardMonoscript pa = WardGUI.PassInWardMonoscriptToGui();
            pa.SetWardDamageAmount(float.Parse(m_damageAmount.text));
        }

        public void SetIndestructibleOn()
        {
            WardMonoscript pa = WardGUI.PassInWardMonoscriptToGui();
            pa.SetIndestructibleOn(m_indestructibleToggle.isOn);
        }

        public void SetIndestructList()
        {
            WardMonoscript pa = WardGUI.PassInWardMonoscriptToGui();
            pa.SetIndestructList(m_indestructibleList.text);
        }

        public void SetCreatureDamageIncrease()
        {
            WardMonoscript pa = WardGUI.PassInWardMonoscriptToGui();
            pa.SetCreatureDamageIncrease(m_creatureDamageIncrease.value);
        }

        public void SetStructDamageReduc()
        {
            WardMonoscript pa = WardGUI.PassInWardMonoscriptToGui();
            pa.SetStructDamageReduc(m_structDamageReduction.value);
        }

        public void ItemstandInteractToggle()
        {
            WardMonoscript paArea = WardGUI.PassInWardMonoscriptToGui();
            paArea.SetItemStandInteractOn(m_itemstandInteractToggle.isOn);
        }

        public void PortalInteractToggle()
        {
            WardMonoscript paArea = WardGUI.PassInWardMonoscriptToGui();
            paArea.SetPortalInteractOn(m_portalInteractToggle.isOn);
        }

        public void PickableInteractToggle()
        {
            WardMonoscript paArea = WardGUI.PassInWardMonoscriptToGui();
            paArea.SetPickableInteractOn(m_pickableInteractToggle.isOn);
        }

        public void ItemInteractToggle()
        {
            WardMonoscript paArea = WardGUI.PassInWardMonoscriptToGui();
            paArea.SetItemInteractOn(m_itemInteractToggle.isOn);
        }

        public void DoorInteractToggle()
        {
            WardMonoscript paArea = WardGUI.PassInWardMonoscriptToGui();
            paArea.SetDoorInteractOn(m_doorInteractToggle.isOn);
        }

        public void ChestInteractToggle()
        {
            WardMonoscript paArea = WardGUI.PassInWardMonoscriptToGui();
            paArea.SetChestInteractOn(m_chestInteractToggle.isOn);
        }

        public void ShipInteractToggle()
        {
            WardMonoscript paArea = WardGUI.PassInWardMonoscriptToGui();
            paArea.SetShipInteractOn(m_shipInteractToggle.isOn);
        }

        public void SignInteractToggle()
        {
            WardMonoscript paArea = WardGUI.PassInWardMonoscriptToGui();
            paArea.SetSignInteractOn(m_signInteractToggle.isOn);
        }

        public void CraftingStationInteractToggle()
        {
            WardMonoscript paArea = WardGUI.PassInWardMonoscriptToGui();
            paArea.SetCraftingStationInteractOn(m_craftingStationInteractToggle.isOn);
        }

        public void SmelterInteractToggle()
        {
            WardMonoscript paArea = WardGUI.PassInWardMonoscriptToGui();
            paArea.SetSmelterInteractOn(m_smelterInteractToggle.isOn);
        }

        public void BeehiveInteractToggle()
        {
            WardMonoscript paArea = WardGUI.PassInWardMonoscriptToGui();
            paArea.SetBeehiveInteractOn(m_beehiveInteractToggle.isOn);
        }

        public void MapTableInteractToggle()
        {
            WardMonoscript paArea = WardGUI.PassInWardMonoscriptToGui();
            paArea.SetMapTableInteractOn(m_maptableInteractToggle.isOn);
        }

        public void PvPToggle()
        {
            WardMonoscript paArea = WardGUI.PassInWardMonoscriptToGui();
            paArea.SetPvpOn(m_pvpEnforceToggle.isOn);
        }

        public void PvEToggle()
        {
            WardMonoscript paArea = WardGUI.PassInWardMonoscriptToGui();
            paArea.SetPveOn(m_pveEnforceToggle.isOn);
        }

        public void OnlyPermToggle()
        {
            WardMonoscript paArea = WardGUI.PassInWardMonoscriptToGui();
            paArea.SetOnlyPermOn(m_onlyPerm.isOn);
        }

        public void NotPermToggle()
        {
            WardMonoscript paArea = WardGUI.PassInWardMonoscriptToGui();
            paArea.SetNotPermOn(m_notPerm.isOn);
        }

        public void CTAMessage()
        {
            WardMonoscript paArea = WardGUI.PassInWardMonoscriptToGui();
            paArea.SetCtaMessage(m_ctaMessage.text);
        }

        public void AutoRepairToggle()
        {
            WardMonoscript paArea = WardGUI.PassInWardMonoscriptToGui();
            paArea.SetAutoRepairOn(m_autoRepairToggle.isOn);
        }

        public void AutoRepairSlider()
        {
            WardMonoscript paArea = WardGUI.PassInWardMonoscriptToGui();
            paArea.SetAutoRepairAmount(m_autoRepairAmount.value);
        }

        public void AutoRepairTextTime()
        {
            WardMonoscript paArea = WardGUI.PassInWardMonoscriptToGui();
            paArea.SetAutoRepairTextTime(float.Parse(m_autoRepairTime.text));
        }

        public void RaidProtectionToggle()
        {
            WardMonoscript paArea = WardGUI.PassInWardMonoscriptToGui();
            paArea.SetRaidProtectionOn(m_raidProtectionToggle.isOn);
        }

        public void RaidProtectionPlayerNeeded()
        {
            WardMonoscript paArea = WardGUI.PassInWardMonoscriptToGui();
            paArea.SetRaidProtectionPlayerNeeded(int.Parse(m_raidPlayersNeeded.text));
        }

        public void WardIsLoveToggle()
        {
            WardMonoscript paArea = WardGUI.PassInWardMonoscriptToGui();
            paArea.SetWardIsLoveOn(m_wardIsLove.isOn);
        }

        public static void SendMSG(string link, string message)
        {
            if (!Uri.TryCreate(link, UriKind.Absolute, out Uri outUri) ||
                outUri.Scheme != Uri.UriSchemeHttp && outUri.Scheme != Uri.UriSchemeHttps) return;
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(link);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            using (StreamWriter streamWriter = new(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(message);
            }

            _ = httpWebRequest.GetResponseAsync();
        }

        public static async Task<string> GetAsync(string uri)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            using HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync();
            using Stream stream = response.GetResponseStream();
            using StreamReader reader = new(stream);
            return await reader.ReadToEndAsync();
        }

        public void ChooseColorButtonClick()
        {
            m_GradientPicker.SetActive(true);
            m_ColorPicker.SetActive(true);
            WardMonoscript paArea = WardGUI.PassInWardMonoscriptToGui();
            GradientPicker.Create(paArea.m_bubble.GetComponent<ForceFieldController>().procedrualGradientRamp,
                "Choose the Gradient", BubbleSetGradient, BubbleGradientFinished);
            /*ColorPicker.Create(paArea.m_bubble.GetComponent<ForceFieldController>().procedrualRampColorTint,
                "Choose the bubble Color", BubbleSetColor, BubbleColorFinished, true);*/
            paArea.m_nview.m_zdo.Set("wardFresh", false);
            m_GradientPicker.SetActive(true);
            m_ColorPicker.SetActive(true);
        }

        private void BubbleSetColor(Color currentColor)
        {
            WardMonoscript paArea = WardGUI.PassInWardMonoscriptToGui();
            paArea.m_bubble.GetComponent<ForceFieldController>().procedrualGradientRamp.colorKeys[0].color =
                currentColor;
            paArea.m_bubble.GetComponent<ForceFieldController>().procedrualGradientRamp.colorKeys[1].color =
                currentColor;
            paArea.m_bubble.GetComponent<ForceFieldController>().procedrualGradientRamp.colorKeys[2].color =
                currentColor;
            GradientColorKey[]? t = paArea.m_bubble.GetComponent<ForceFieldController>().procedrualGradientRamp
                .colorKeys;
            paArea.m_bubble.GetComponent<ForceFieldController>().procedrualRampColorTint = currentColor;
        }

        private void BubbleColorFinished(Color finishedColor)
        {
            WardIsLovePlugin.WILLogger.LogDebug("You chose the color " + ColorUtility.ToHtmlStringRGBA(finishedColor));
            WardMonoscript paArea = WardGUI.PassInWardMonoscriptToGui();
            paArea.m_nview.GetZDO().Set("wardColorCount",
                paArea.m_bubble.GetComponent<ForceFieldController>().procedrualGradientRamp.colorKeys.Length);

            int i = 0;
            foreach (GradientColorKey colorKey in paArea.m_bubble.GetComponent<ForceFieldController>()
                         .procedrualGradientRamp.colorKeys)
            {
                paArea.m_nview.GetZDO().Set($"wardColor{i}", "#" + ColorUtility.ToHtmlStringRGBA(colorKey.color));
                paArea.m_nview.GetZDO().Set($"wardColorTime{i}", colorKey.time);
                ++i;
            }

            i = 0;
            foreach (GradientAlphaKey alphaKey in paArea.m_bubble.GetComponent<ForceFieldController>()
                         .procedrualGradientRamp.alphaKeys)
            {
                paArea.m_nview.GetZDO().Set($"wardAlpha{i}", alphaKey.alpha);
                paArea.m_nview.GetZDO().Set($"wardAlphaTime{i}", alphaKey.time);
                ++i;
            }

            paArea.m_nview.GetZDO().Set("wardAlphaCount",
                paArea.m_bubble.GetComponent<ForceFieldController>().procedrualGradientRamp.alphaKeys.Length);
            m_GradientPicker.SetActive(false);
            m_ColorPicker.SetActive(false);
        }

        private void BubbleSetGradient(Gradient currentGradient)
        {
            WardMonoscript paArea = WardGUI.PassInWardMonoscriptToGui();
            paArea.m_bubble.GetComponent<ForceFieldController>().procedrualGradientRamp = currentGradient;
            paArea.m_bubble.GetComponent<ForceFieldController>().procedrualRampColorTint =
                currentGradient.colorKeys[1].color;


            WardIsLovePlugin.WILLogger.LogDebug($"Color key length being set to  {currentGradient.colorKeys.Length}");
        }


        private void BubbleGradientFinished(Gradient finishedGradient)
        {
            WardIsLovePlugin.WILLogger.LogDebug("You chose a Gradient with " + finishedGradient.colorKeys.Length +
                                                " Color keys");
            WardMonoscript paArea = WardGUI.PassInWardMonoscriptToGui();
            paArea.m_nview.GetZDO().Set("wardColorCount", finishedGradient.colorKeys.Length);

            int i = 0;
            foreach (GradientColorKey colorKey in finishedGradient.colorKeys)
            {
                paArea.m_nview.GetZDO().Set($"wardColor{i}", "#" + ColorUtility.ToHtmlStringRGBA(colorKey.color));
                paArea.m_nview.GetZDO().Set($"wardColorTime{i}", colorKey.time);
                ++i;
            }

            i = 0;
            foreach (GradientAlphaKey alphaKey in finishedGradient.alphaKeys)
            {
                paArea.m_nview.GetZDO().Set($"wardAlpha{i}", alphaKey.alpha);
                paArea.m_nview.GetZDO().Set($"wardAlphaTime{i}", alphaKey.time);
                ++i;
            }

            paArea.m_nview.GetZDO().Set("wardAlphaCount", finishedGradient.alphaKeys.Length);
            m_GradientPicker.SetActive(false);
            m_ColorPicker.SetActive(false);
        }
    }
}