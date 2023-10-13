using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Groups;
using HarmonyLib;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WardIsLove.Extensions;
using WardIsLove.Util.UI;
using Object = UnityEngine.Object;

namespace WardIsLove.Util
{
    [HarmonyPatch]
    public class WardMonoscript : MonoBehaviour, Hoverable, Interactable
    {
        public static List<WardMonoscript> m_allAreas = new();
        public EffectList m_activateEffect = new();
        public EffectList m_addPermittedEffect = new();
        public CirclesProjector m_areaMarker;
        public GameObject m_bardWardAudio;
        public GameObject m_bubble;
        public List<WardMonoscript> m_connectedAreas = new();
        public Light m_wardLightColor;
        public ParticleSystem m_wardParticleLightColor;
        public GameObject m_connectEffect;
        public List<GameObject> m_connectionInstances = new();
        public float m_connectionUpdateTime = -1000f;
        public EffectList m_deactivateEffect = new();
        public GameObject m_enabledBurningEffect;
        public GameObject m_enabledEffect;
        public GameObject m_enabledNMAEffect;
        public bool m_flashAvailable = true;
        public EffectList m_flashEffect = new();
        public GameObject m_inRangeEffect;
        public MeshRenderer m_model;
        public MeshRenderer m_modelLoki;
        public MeshRenderer m_modelDefault;
        public MeshRenderer m_modelHel;
        public MeshRenderer m_modelBetterWard;
        public MeshRenderer m_modelBetterWard_Type2;
        public MeshRenderer m_modelBetterWard_Type3;
        public MeshRenderer m_modelBetterWard_Type4;
        public string m_name = "Ward";
        public ZNetView m_nview;
        public ZNetView m_rootObjectOverride;
        public Piece m_piece;
        public GameObject m_playerBase;
        public float m_radius;
        public float m_radiusBurning;
        public float m_radiusNMA;
        public EffectList m_removedPermittedEffect = new();
        public bool m_tempChecked;
        public float m_updateConnectionsInterval = 5f;


        public void Awake()
        {
            if (m_areaMarker)
                m_areaMarker.m_radius = this.GetWardRadius();
            m_nview = (bool)(Object)m_rootObjectOverride
                ? m_rootObjectOverride.GetComponent<ZNetView>()
                : GetComponent<ZNetView>();
            if (m_nview.GetZDO() == null)
                return;
            if (!m_nview.IsValid())
                return;
            GetComponent<WearNTear>().m_onDamaged += OnDamaged;
            m_enabledNMAEffect.GetComponent<SphereCollider>().radius = this.GetWardRadius();
            m_enabledBurningEffect.GetComponent<SphereCollider>().radius = this.GetWardRadius();

            m_radius = this.GetWardRadius();
            m_playerBase.GetComponent<SphereCollider>().radius = this.GetWardRadius();
            /* Activate NMA etc when needed */

            if (this.GetBubbleMode() == WardIsLovePlugin.WardBehaviorEnums.NoMonsters)
                m_enabledNMAEffect.SetActive(true);
            else if (this.GetBubbleMode() == WardIsLovePlugin.WardBehaviorEnums.Default)
                m_enabledNMAEffect.SetActive(false);


            m_piece = GetComponent<Piece>();
            if (m_areaMarker)
                m_areaMarker.gameObject.SetActive(false);
            m_bardWardAudio.SetActive(this.GetWardIsLoveOn());
            if (m_inRangeEffect)
                m_inRangeEffect.SetActive(false);
            m_allAreas.Add(this);

            InvokeRepeating(nameof(UpdateStatus), 0.0f, 1f);
            /* TODO Get This working again */
            //StartCoroutine(DelayRepairRoutine());
            m_nview.Register("ToggleEnabled", new Action<long, long>(RPC_ToggleEnabled));
            m_nview.Register("TogglePermitted", new Action<long, long, string>(RPC_TogglePermitted));
            m_nview.Register("FlashShield", RPC_FlashShield);
            m_nview.Register("SyncWardsMOFO", new Action<long, int>(SwapWardModel));
            m_nview.Register("WILWardLimit Reactivate",
                (long sender, bool isAdmin) =>
                {
                    if (!isAdmin)
                    {
                        m_nview.m_zdo.Set("WILLimitedWardTime", EnvMan.instance.GetCurrentDay());
                    }
                    else
                    {
                        m_nview.m_zdo.Set("WILLimitedWardTime", EnvMan.instance.GetCurrentDay() + 50000);
                    }
                });
            SwapWardModel(0, m_nview.GetZDO().GetInt("wardModelKey"));

            /* Get bubble color */

            if (m_nview.GetZDO().GetBool("wardFresh", true) == false)
            {
                int colorNumbers = m_nview.GetZDO().GetInt("wardColorCount",
                    m_bubble.GetComponent<ForceFieldController>().procedrualGradientRamp.colorKeys.Length);
                int colorAlphaNumbers = m_nview.GetZDO().GetInt("wardAlphaCount",
                    m_bubble.GetComponent<ForceFieldController>().procedrualGradientRamp.colorKeys.Length);

                WardIsLovePlugin.WILLogger.LogDebug(
                    $"Color Number values has a value of {colorNumbers}");
                Gradient gradient = new();
                GradientColorKey[] gradientKeys = new GradientColorKey[colorNumbers];
                GradientAlphaKey[] gradientAlphaKeys = new GradientAlphaKey[colorAlphaNumbers];
                for (int i = 0; i < colorNumbers; i++)
                {
                    if (ColorUtility.TryParseHtmlString(
                            m_nview.GetZDO().GetString($"wardColor{i}"), out Color color))
                    {
                        gradientKeys[i].color = color;
                        gradientKeys[i].time = m_nview.GetZDO().GetFloat($"wardColorTime{i}");
                    }
                }

                for (int i = 0; i < colorAlphaNumbers; i++)
                {
                    gradientAlphaKeys[i].alpha = m_nview.GetZDO().GetFloat($"wardAlpha{i}");
                    gradientAlphaKeys[i].time = m_nview.GetZDO().GetFloat($"wardAlphaTime{i}");
                }

                gradient.SetKeys(gradientKeys, gradientAlphaKeys);
                m_bubble.GetComponent<ForceFieldController>().procedrualGradientRamp = gradient;
                m_bubble.GetComponent<ForceFieldController>().procedrualRampColorTint =
                    gradient.colorKeys[1].color;
            }
        }

        private void SwapWardModel(long sender, int index)
        {
            for (int i = 0; i < transform.Find("new").childCount; i++)
            {
                transform.Find("new").GetChild(i).gameObject.SetActive(false);
            }

            transform.Find("new").GetChild(index).gameObject.SetActive(true);
            if (m_nview.IsOwner())
            {
                m_nview.GetZDO().Set("wardModelKey", index);
            }

            SetWardColor(index);
        }

        private void SetWardColor(int type)
        {
            switch (type)
            {
                case 0:
                    if (ColorUtility.TryParseHtmlString("#FCDFC1", out Color color))
                    {
                        m_wardLightColor.color = color;
                    }

                    break;
                case 1:
                    if (ColorUtility.TryParseHtmlString("#00AFD4", out Color color1))
                    {
                        m_wardLightColor.color = color1;
                    }

                    break;
                case 2:
                    if (ColorUtility.TryParseHtmlString("#1DB03B", out Color color2))
                    {
                        m_wardLightColor.color = color2;
                    }

                    break;
                case 3:
                    if (ColorUtility.TryParseHtmlString("#C7FCC1", out Color color3))
                    {
                        m_wardLightColor.color = color3;
                    }

                    break;
                case 4:
                    if (ColorUtility.TryParseHtmlString("#D0C1FC", out Color color4))
                    {
                        m_wardLightColor.color = color4;
                    }

                    break;
                case 5:
                    if (ColorUtility.TryParseHtmlString("#FCC3C1", out Color color5))
                    {
                        m_wardLightColor.color = color5;
                    }

                    break;
                case 6:
                    if (ColorUtility.TryParseHtmlString("#92F1F0", out Color color6))
                    {
                        m_wardLightColor.color = color6;
                    }

                    break;
                default:
                    if (ColorUtility.TryParseHtmlString("#FCDFC1", out Color colorDefault))
                    {
                        m_wardLightColor.color = colorDefault;
                    }

                    break;
            }
        }

        public void OnDestroy()
        {
            try
            {
                if (ZNetScene.instance)
                {
                    try
                    {
                        m_bubble.gameObject.SetActive(false);
                        ZNetScene.instance.Destroy(gameObject);
                    }
                    catch
                    {
                        m_bubble.gameObject.SetActive(false);
                        ZNetScene.instance.Destroy(gameObject);
                    }
                }
            }
            catch
            {
                WardIsLovePlugin.WILLogger.LogError("Couldn't destroy the bubble properly");
            }


            _ = m_allAreas.Remove(this);
        }

        public void OnDrawGizmosSelected()
        {
        }

        public string GetHoverText()
        {
            if (!m_nview.IsValid() || Player.m_localPlayer == null)
                return "";
            if (Vector3.Distance(Player.m_localPlayer.transform.position, transform.position) >= 5)
            {
                return "";
            }

            if (!m_nview.m_zdo.GetBool("WILLimitedWard")) return "";
            ShowAreaMarker();
            StringBuilder text = new(256);
            if (Input.GetKeyDown(KeyCode.DownArrow) && WardIsLovePlugin.Admin)
                if (IsEnabled())
                    SetEnabled(false);
            if (Input.GetKeyDown(KeyCode.UpArrow) && WardIsLovePlugin.Admin)
                if (!IsEnabled())
                    SetEnabled(true);
            if (m_piece.IsCreator())
            {
                if (IsEnabled())
                {
                    AppendNameText(text);
                    AdminAppend(text);
                    _ = text.Append("\n[<color=#FFFF00><b>$KEY_Use</b></color>] • $piece_guardstone_deactivate");
                }
                else
                {
                    AppendNameText(text);
                    AdminAppend(text);
                    _ = text.Append("\n[<color=#FFFF00><b>$KEY_Use</b></color>] • $piece_guardstone_activate");
                }
            }
            else if (IsPermitted(Player.m_localPlayer.GetPlayerID()))
            {
                if (IsEnabled())
                {
                    AppendNameText(text);
                    AdminAppend(text);
                    _ = text.Append("\n[<color=#FFFF00><b>" + WardIsLovePlugin.WardHotKey.Value +
                                    $"</b></color>] • $piece_guardstone_deactivate {Localization.instance.Localize("$piece_guardstone")}");
                    if (WardIsLovePlugin.WardHotKey.Value.IsDown())
                    {
                        //SetEnabled(!IsEnabled());
                        m_nview.InvokeRPC("ToggleEnabled", m_piece.GetCreator());
                        //copied code for player animation
                        Vector3 vector = transform.position - Player.m_localPlayer.transform.position;
                        vector.y = 0f;
                        vector.Normalize();
                        Player.m_localPlayer.transform.rotation = Quaternion.LookRotation(vector);
                        Player.m_localPlayer.m_zanim.SetTrigger("interact");
                    }
                }
                else
                {
                    AppendNameText(text);
                    AdminAppend(text);
                    _ = text.Append("\n[<color=#FFFF00><b>" + WardIsLovePlugin.WardHotKey.Value +
                                    $"</b></color>] • $piece_guardstone_activate {Localization.instance.Localize("$piece_guardstone")}");
                    _ = text.Append("\n[<color=#FFFF00><b>$KEY_Use</b></color>] • $piece_guardstone_remove");
                    if (WardIsLovePlugin.WardHotKey.Value.IsDown())
                    {
                        //SetEnabled(!IsEnabled());

                        m_nview.InvokeRPC("ToggleEnabled", m_piece.GetCreator());
                        //copied code for player animation
                        Vector3 vector = transform.position - Player.m_localPlayer.transform.position;
                        vector.y = 0f;
                        vector.Normalize();
                        Player.m_localPlayer.transform.rotation = Quaternion.LookRotation(vector);
                        Player.m_localPlayer.m_zanim.SetTrigger("interact");
                    }
                }
            }
            else if (IsEnabled())
            {
                AppendNameText(text);
                AdminAppend(text);
            }
            else
            {
                AppendNameText(text);
                AdminAppend(text);
                _ = text.Append(IsPermitted(Player.m_localPlayer.GetPlayerID())
                    ? "\n[<color=#FFFF00><b>$KEY_Use</b></color>] • $piece_guardstone_remove"
                    : "\n[<color=#FFFF00><b>$KEY_Use</b></color>] • $piece_guardstone_add");
            }

            AddUserList(text);
            int timeto = m_nview.m_zdo.GetInt("WILLimitedWardTime") + WardIsLovePlugin.MaxDaysDifference;
            int difference = timeto - EnvMan.instance.GetCurrentDay();
            string tf = EnvMan.instance.GetCurrentDay() >= timeto
                ? "<color=#FF0000>No</color>\n"
                : "<color=#00FF00>Yes</color>";
            string isEnabled = $"Is ward charged • {tf}";
            if (difference < 0)
            {
                difference = 0;
            }

            string daysTillExpire = $"\nDays Till Expiration • <color=#00FF00>{difference}</color>\n";
            if (EnvMan.instance.GetCurrentDay() >= timeto)
            {
                daysTillExpire = "";
            }

            string creatorText = "";
            if ((EnvMan.instance.GetCurrentDay() != m_nview.m_zdo.GetInt("WILLimitedWardTime")))
            {
                creatorText = m_piece.IsCreator() ||
                              IsPermitted(Player.m_localPlayer.GetPlayerID())
                    ? Localization.instance.Localize(
                        "[<color=#FFFF00><b>LAlt + $KEY_Use</b></color>] <color=#00FF00>Recharge Ward</color>\n\n")
                    : "";
            }

            string result = "<color=#00FFFF>" + isEnabled + daysTillExpire + "</color>" +
                            creatorText;
            return result + Localization.instance.Localize(text.ToString());
        }

        public string GetHoverName()
        {
            return m_name;
        }

        public bool Interact(Humanoid human, bool hold, bool alt)
        {
            if (hold)
                return false;
            if (Vector3.Distance(Player.m_localPlayer.transform.position, transform.position) >= 5)
            {
                return false;
            }

            Player player = human as Player;
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.Tilde))
                try
                {
                    if (WardGUI.IsPanelVisible())
                    {
                        WardGUI.Hide(); // put this here just in case.
                    }
                    else
                    {
                        if (!WardIsLovePlugin.DisableGUI.Value)
                        {
                            if (WardIsLovePlugin.Admin || (m_piece.IsCreator() && WardIsLovePlugin.WardControl.Value))
                            {
                                WardGUI.Show(this);
                            }
                        }
                    }

                    return false;
                }
                catch (Exception ex)
                {
                    WardIsLovePlugin.WILLogger.LogError($"Interact Method : {ex}");
                    return true;
                }

            if (Input.GetKey(KeyCode.LeftAlt))
            {
                if ((m_piece.IsCreator() || IsPermitted(Player.m_localPlayer.GetPlayerID())) &&
                    (EnvMan.instance.GetCurrentDay() != m_nview.m_zdo.GetInt("WILLimitedWardTime")))
                {
                    ItemDrop? item = ObjectDB.instance.GetItemPrefab(WardIsLovePlugin.ChargeItem.Value)
                        .GetComponent<ItemDrop>();
                    WardIsLovePlugin.WILLogger.LogDebug(
                        $":You have {Player.m_localPlayer.GetInventory().CountItems(item.m_itemData.m_shared.m_name)} of {item.m_itemData.m_shared.m_name}");
                    if (Player.m_localPlayer.GetInventory().CountItems(item.m_itemData.m_shared.m_name) >=
                        WardIsLovePlugin.ChargeItemAmount.Value)
                    {
                        if (WardIsLovePlugin.ChargeItemAmount.Value == 0)
                        {
                            Instantiate(ZNetScene.instance.GetPrefab("vfx_HealthUpgrade"), transform.position,
                                Quaternion.identity);
                            m_nview.InvokeRPC("WILWardLimit Reactivate", WardIsLovePlugin.Admin);
                            return true;
                        }

                        Player.m_localPlayer.GetInventory().RemoveItem(item.m_itemData.m_shared.m_name,
                            WardIsLovePlugin.ChargeItemAmount.Value);
                        Player.m_localPlayer.ShowRemovedMessage(item.m_itemData,
                            WardIsLovePlugin.ChargeItemAmount.Value);
                        Instantiate(ZNetScene.instance.GetPrefab("vfx_HealthUpgrade"), transform.position,
                            Quaternion.identity);
                        m_nview.InvokeRPC("WILWardLimit Reactivate", WardIsLovePlugin.Admin);
                    }
                    else
                    {
                        if (WardIsLovePlugin.Admin)
                        {
                            Instantiate(ZNetScene.instance.GetPrefab("vfx_HealthUpgrade"), transform.position,
                                Quaternion.identity);
                            m_nview.InvokeRPC("WILWardLimit Reactivate", WardIsLovePlugin.Admin);
                        }
                        else
                        {
                            Player.m_localPlayer.Message(MessageHud.MessageType.Center, "$msg_incompleteoffering");
                        }
                    }
                }

                return true;
            }

            if (m_piece.IsCreator())
            {
                m_nview.InvokeRPC("ToggleEnabled", player.GetPlayerID());
                return true;
            }

            if (IsPermitted(player.GetPlayerID()) && WardIsLovePlugin.WardHotKey.Value.IsDown())
            {
                //m_nview.InvokeRPC("ToggleEnabled", player.GetPlayerID());
                return true;
            }

            if (IsEnabled())
                return false;
            m_nview.ClaimOwnership();
            m_nview.InvokeRPC("TogglePermitted", player.GetPlayerID(), player.GetPlayerName());
            return true;
        }

        public bool UseItem(Humanoid user, ItemDrop.ItemData item)
        {
            return false;
        }

        public void UpdateStatus()
        {
            bool flag = IsEnabled();
            m_enabledEffect.SetActive(flag);
            m_flashAvailable = true;
            this.EmissionSetter();
            m_bardWardAudio.SetActive(this.GetWardIsLoveOn());

            // if (this.m_areaMarker)
            m_areaMarker.m_radius = this.GetWardRadius();
            m_radius = this.GetWardRadius();
            m_playerBase.GetComponent<SphereCollider>().radius = this.GetWardRadius();
            m_radiusNMA = this.GetWardRadius();
            m_radiusBurning = this.GetWardRadius();
            m_enabledNMAEffect.GetComponent<SphereCollider>().radius = this.GetWardRadius();

            m_enabledBurningEffect.GetComponent<SphereCollider>().radius = this.GetWardRadius();
            if (this.GetBubbleOn() && IsEnabled())
            {
                if (!m_bubble.activeSelf)
                    m_bubble.SetActive(true);
                //var newScale = this.m_bubble.transform.root.localScale * this.GetWardRadius() * 0.08f;
                Vector3 newScale = m_bubble.transform.root.localScale * this.GetWardRadius() * 2f;
                m_bubble.transform.localScale = newScale;
            }
            else
            {
                m_bubble.SetActive(false);
            }
            /* Activate NMA etc when needed */

            if (this.GetBubbleMode() == WardIsLovePlugin.WardBehaviorEnums.NoMonsters)
            {
                m_enabledNMAEffect.SetActive(true);
                m_enabledBurningEffect.SetActive(false);
            }

            else if (this.GetBubbleMode() == WardIsLovePlugin.WardBehaviorEnums.Default)
            {
                m_enabledNMAEffect.SetActive(false);
                m_enabledBurningEffect.SetActive(false);
            }
        }

        /*public void DelayRepair()
        {
            StartCoroutine(DelayRepairRoutine());
        }*/

        IEnumerator DelayRepairRoutine()
        {
            while (true)
            {
                float time;
                WardMonoscript? ward = null;
                if (WardIsLovePlugin.WardEnabled != null && ZNetScene.instance && WardIsLovePlugin.WardEnabled.Value)
                {
                    List<WearNTear> allInstances = WearNTear.GetAllInstances();
                    if (allInstances.Count > 0)
                    {
                        foreach (WearNTear instance in allInstances)
                        {
                            ZNetView instanceField = instance.m_nview;
                            if (instanceField == null ||
                                !CheckInWardMonoscript(instance.transform.position))
                                continue;
                            ward = WardMonoscriptExt.GetWardMonoscript(instance.transform.position);
                            if (!ward.GetAutoRepairOn() || !IsEnabled()) continue;
                            float num1 = instanceField.GetZDO().GetFloat("health");
                            if (!(num1 > 0.0) || !(num1 < (double)instance.m_health)) continue;
                            float num2 = num1 + (float)(instance.m_health * (double)ward.GetAutoRepairAmount() / 100.0);
                            if (num2 > (double)instance.m_health)
                                num2 = instance.m_health;
                            instanceField.GetZDO().Set("health", num2);
                            instanceField.InvokeRPC(ZNetView.Everybody, "WNTHealthChanged", num2);
                        }
                    }
                }

                //float time = 0;
                try
                {
                    time = ward.GetAutoRepairTextTime() + 30;
                }
                catch
                {
                    time = 30;
                }

                yield return new WaitForSecondsRealtime(time);
            }
        }

        public void AddUserList(StringBuilder text)
        {
            List<KeyValuePair<long, string>> permittedPlayers = GetPermittedPlayers();
            _ = text.Append("\n$piece_guardstone_additional • ");
            for (int index = 0; index < permittedPlayers.Count; ++index)
            {
                _ = text.Append(permittedPlayers[index].Value);
                if (index != permittedPlayers.Count - 1)
                    _ = text.Append(", ");
            }

            AddAdditionalInformation(text);
        }

        public void AdminAppend(StringBuilder text)
        {
            if (WardIsLovePlugin.Admin && !WardIsLovePlugin.StreamerMode.Value)
            {
                _ = text.Append("\n$piece_guardstone_owner • " + GetCreatorName() +
                                " <color=#FFA500><b>[Steam Info: " +
                                m_nview.GetZDO().GetString("steamName") + " " +
                                m_nview.GetZDO().GetString("steamID") + "]</b></color>");
            }
            else
            {
                _ = text.Append("\n$piece_guardstone_owner • " + GetCreatorName());
            }
        }

        public void AddAdditionalInformation(StringBuilder text)
        {
            WardIsLovePlugin.WardInteractBehaviorEnums accessMode = this.GetAccessMode();
            WardIsLovePlugin.WardBehaviorEnums bubbleMode = this.GetBubbleMode();
            HitData.DamageType damageType = this.GetDamageType();

            if (m_piece.IsCreator())
            {
                if (WardIsLovePlugin.WardControl.Value || WardIsLovePlugin.Admin)
                {
                    text.Append(Localization.instance.Localize(
                        "\n[<color=#FFFF00><b>SHIFT + $KEY_Use</b></color>] • Toggle Ward GUI"));
                }

                text.Append(Localization.instance.Localize(
                    $"\n$betterwards_accessMode $betterwards_accessMode_{accessMode}"));
                text.Append(Localization.instance.Localize(
                    $"\n$betterwards_bubbleMode $betterwards_bubbleMode_{bubbleMode}"));
                text.Append(Localization.instance.Localize(
                    $"\nRadius • {this.GetWardRadius().ToString()}"));
            }
            else
            {
                if (WardIsLovePlugin.Admin && !WardIsLovePlugin.DisableGUI.Value)
                {
                    text.Append(Localization.instance.Localize(
                        "\n[<color=#FFFF00><b>SHIFT + $KEY_Use</b></color>] • Toggle Ward GUI"));
                }

                text.Append(Localization.instance.Localize(
                    $"\n$betterwards_accessMode $betterwards_accessMode_{accessMode}"));
                text.Append(Localization.instance.Localize(
                    $"\n$betterwards_bubbleMode $betterwards_bubbleMode_{bubbleMode}"));
                text.Append(Localization.instance.Localize(
                    $"\nRadius • {this.GetWardRadius().ToString()}"));
            }
        }

        public void RemovePermitted(long playerID)
        {
            List<KeyValuePair<long, string>> permittedPlayers = GetPermittedPlayers();
            if (permittedPlayers.RemoveAll(x => x.Key == playerID) <= 0)
                return;
            SetPermittedPlayers(permittedPlayers);
            _ = m_removedPermittedEffect.Create(transform.position, transform.rotation);
        }

        public bool IsPermitted(long playerID)
        {
            bool iIsPermitted = false;
            if (m_nview.IsValid())
            {
                WardIsLovePlugin.WardInteractBehaviorEnums accessMode = this.GetAccessMode();
                foreach (KeyValuePair<long, string> permittedPlayer in GetPermittedPlayers()
                             .Where(permittedPlayer => permittedPlayer.Key == playerID))
                    iIsPermitted = true;

                if (WardIsLovePlugin.Admin && WardIsLovePlugin.AdminAutoPerm.Value)
                    return true;

                switch (accessMode)
                {
                    case WardIsLovePlugin.WardInteractBehaviorEnums.Everyone:
                    case WardIsLovePlugin.WardInteractBehaviorEnums.Default
                        when (iIsPermitted || m_piece.GetCreator() == playerID):
                    case WardIsLovePlugin.WardInteractBehaviorEnums.OwnerOnly when m_piece.GetCreator() == playerID:
                        return true;
                    case WardIsLovePlugin.WardInteractBehaviorEnums.Group:
                    {
                        if (API.IsLoaded())
                        {
                            List<KeyValuePair<long, string>> permittedPlayers = GetPermittedPlayers();
                            if (API.FindGroupMemberByPlayerId(m_piece.GetCreator()) != null)
                            {
                                return true;
                            }

                            try
                            {
                                if (permittedPlayers.Any(permittedPlayer => API.GroupPlayers()
                                        .Contains(PlayerReference.fromPlayerId(permittedPlayer.Key))))
                                {
                                    return true;
                                }
                            }
                            catch
                            {
                            }
                        }

                        break;
                    }
                    default:
                        return false;
                }
            }

            return false;
        }

        public void AddPermitted(long playerID, string playerName)
        {
            List<KeyValuePair<long, string>> permittedPlayers = GetPermittedPlayers();
            if (permittedPlayers.Any(keyValuePair => keyValuePair.Key == playerID))
            {
                return;
            }

            permittedPlayers.Add(new KeyValuePair<long, string>(playerID, playerName));
            SetPermittedPlayers(permittedPlayers);
            _ = m_addPermittedEffect.Create(transform.position, transform.rotation);
        }

        public void SetPermittedPlayers(List<KeyValuePair<long, string>> users)
        {
            m_nview.ClaimOwnership();
            m_nview.GetZDO().Set(ZDOVars.s_permitted, users.Count, false);
            for (int index = 0; index < users.Count; ++index)
            {
                KeyValuePair<long, string> user = users[index];
                m_nview.GetZDO().Set("pu_id" + index, user.Key);
                m_nview.GetZDO().Set("pu_name" + index, user.Value);
            }
        }

        public List<KeyValuePair<long, string>> GetPermittedPlayers()
        {
            List<KeyValuePair<long, string>> keyValuePairList = new();
            int num = m_nview.GetZDO().GetInt(ZDOVars.s_permitted);
            for (int index = 0; index < num; ++index)
            {
                long key = m_nview.GetZDO().GetLong("pu_id" + index);
                string str = m_nview.GetZDO().GetString("pu_name" + index);
                if (key != 0L)
                    keyValuePairList.Add(new KeyValuePair<long, string>(key, str));
            }

            return keyValuePairList;
        }

        public void RPC_TogglePermitted(long uid, long playerID, string name)
        {
            if (!m_nview.IsOwner() || IsEnabled())
                return;
            if (IsPermitted(playerID))
                RemovePermitted(playerID);
            else
                AddPermitted(playerID, name);
        }

        public void RPC_ToggleEnabled(long uid, long playerID)
        {
            if (this.GetAccessMode() == WardIsLovePlugin.WardInteractBehaviorEnums.Default)
            {
                WardIsLovePlugin.WILLogger.LogDebug($"Toggle enabled, creator is {m_nview.GetZDO().GetString("creatorName")} {m_piece.GetCreator()}");
                if (!m_nview.IsOwner() && m_piece.GetCreator() != playerID && !IsPermitted(playerID)) return;
                /*            if (!m_nview.IsOwner() || m_piece.GetCreator() != playerID || !this.IsPermitted(playerID))
                                return;*/
                SetEnabled(!IsEnabled());
                if (this.GetBubbleOn())
                {
                    if (!m_bubble.activeSelf)
                        m_bubble.SetActive(true);
                    //var newScale = this.m_bubble.transform.root.localScale * this.GetWardRadius() * 0.08f;
                    Vector3 newScale = m_bubble.transform.root.localScale * this.GetWardRadius() * 2f;
                    m_bubble.transform.localScale = newScale;
                }
                else
                {
                    m_bubble.SetActive(false);
                }
            }
            else
            {
#if DEBUG
                WardIsLovePlugin.WILLogger.LogDebug($"Toggle enabled from {playerID}  creator is {m_piece.GetCreator()}");
#endif
                if (m_nview.IsOwner() &&
                    (IsPermitted(playerID) &&
                     this.GetAccessMode() == WardIsLovePlugin.WardInteractBehaviorEnums.Default ||
                     this.GetAccessMode() == WardIsLovePlugin.WardInteractBehaviorEnums.Everyone ||
                     m_piece.IsCreator()))
                    SetEnabled(!IsEnabled());
            }
        }

        public bool IsEnabled()
        {
            return m_nview.IsValid() && m_nview.GetZDO().GetBool(ZDOVars.s_enabled) && this.WILWardLimitCheck();
        }

        public void SetEnabled(bool enabled)
        {
            //this.m_bardWardAudio.gameObject.SetActive(enabled);
            m_nview.GetZDO().Set(nameof(enabled), enabled);
            UpdateStatus();
            _ = enabled
                ? m_activateEffect.Create(transform.position, transform.rotation)
                : m_deactivateEffect.Create(transform.position, transform.rotation);
        }

        public void Setup(string name)
        {
            m_nview.GetZDO().Set(ZDOVars.s_creatorName, name);
            m_nview.GetZDO().Set("steamName", SteamFriends.GetPersonaName());
            m_nview.GetZDO().Set("steamID", SteamUser.GetSteamID().ToString());
            m_nview.GetZDO().Set("wardFresh", true);
        }

        public void PokeAllAreasInRange()
        {
            foreach (WardMonoscript allArea in m_allAreas.Where(allArea =>
                         !(allArea == this) && IsInside(allArea.transform.position, 0.0f)))
                allArea.StartInRangeEffect();
        }

        public void StartInRangeEffect()
        {
            m_inRangeEffect.SetActive(true);
            CancelInvoke("StopInRangeEffect");
            Invoke("StopInRangeEffect", 0.2f);
        }

        public void StopInRangeEffect()
        {
            m_inRangeEffect.SetActive(false);
        }

        public void PokeConnectionEffects()
        {
            List<WardMonoscript> connectedAreas = GetConnectedAreas();
            StartConnectionEffects();
            foreach (WardMonoscript WardMonoscript in connectedAreas)
                WardMonoscript.StartConnectionEffects();
        }

        public void StartConnectionEffects()
        {
            List<WardMonoscript> WardMonoscriptList = m_allAreas
                .Where(allArea => !(allArea == this) && IsInside(allArea.transform.position, 0.0f)).ToList();

            Vector3 position = transform.position + Vector3.up * 1.4f;
            if (m_connectionInstances.Count != WardMonoscriptList.Count)
            {
                StopConnectionEffects();
                for (int index = 0; index < WardMonoscriptList.Count; ++index)
                    m_connectionInstances.Add(Instantiate(m_connectEffect, position,
                        Quaternion.identity, transform));
            }

            if (m_connectionInstances.Count == 0)
                return;
            for (int index = 0; index < WardMonoscriptList.Count; ++index)
            {
                Vector3 vector3 = WardMonoscriptList[index].transform.position + Vector3.up * 1.4f - position;
                Quaternion quaternion = Quaternion.LookRotation(vector3.normalized);
                GameObject connectionInstance = m_connectionInstances[index];
                connectionInstance.transform.position = position;
                connectionInstance.transform.rotation = quaternion;
                connectionInstance.transform.localScale = new Vector3(1f, 1f, vector3.magnitude);
            }

            CancelInvoke("StopConnectionEffects");
            Invoke("StopConnectionEffects", 0.3f);
        }

        public void StopConnectionEffects()
        {
            foreach (GameObject connectionInstance in m_connectionInstances)
                Destroy(connectionInstance);
            m_connectionInstances.Clear();
        }

        public string GetCreatorName()
        {
            return m_nview.GetZDO().GetString(ZDOVars.s_creatorName);
        }

        public static bool CheckInWardMonoscript(Vector3 point, bool flash = false)
        {
            foreach (WardMonoscript allArea in m_allAreas.Where(allArea =>
                         allArea.IsEnabled() && allArea.IsInside(point, 0.0f)))
            {
                if (flash)
                    allArea.FlashShield(false);
                return true;
            }

            return false;
        }

        public static bool CheckInWardOutWard(Vector3 point, out WardMonoscript wardout, bool flash = false)
        {
            foreach (WardMonoscript allArea in m_allAreas.Where(allArea =>
                         allArea.IsEnabled() && allArea.IsInside(point, 0.0f)))
            {
                if (flash)
                    allArea.FlashShield(false);
                wardout = allArea;
                return true;
            }

            wardout = null;
            return false;
        }

        public static bool CheckAccess(Vector3 point, float radius = 0.0f, bool flash = true, bool wardCheck = false)
        {
            List<WardMonoscript> WardMonoscriptList = new();
            bool flag;
            if (wardCheck)
            {
                flag = true;
                foreach (WardMonoscript allArea in m_allAreas.Where(allArea =>
                             allArea.IsEnabled() && allArea.IsInside(point, radius) && !allArea.HaveLocalAccess()))
                {
                    flag = false;
                    WardMonoscriptList.Add(allArea);
                }
            }
            else
            {
                flag = false;
                foreach (WardMonoscript allArea in m_allAreas.Where(allArea =>
                             allArea.IsEnabled() && allArea.IsInside(point, radius)))
                    if (allArea.HaveLocalAccess())
                        flag = true;
                    else
                        WardMonoscriptList.Add(allArea);
            }

            if (flag || WardMonoscriptList.Count <= 0)
                return true;
            if (!flash) return false;
            foreach (WardMonoscript WardMonoscript in WardMonoscriptList)
                WardMonoscript.FlashShield(false);

            return false;
        }

        private bool HaveLocalAccess()
        {
            return m_piece.IsCreator() || IsPermitted(Player.m_localPlayer.GetPlayerID());
        }

        private List<WardMonoscript> GetConnectedAreas(bool forceUpdate = false)
        {
            if ((Time.time - (double)m_connectionUpdateTime > m_updateConnectionsInterval) | forceUpdate)
            {
                GetAllConnectedAreas(m_connectedAreas);
                m_connectionUpdateTime = Time.time;
            }

            return m_connectedAreas;
        }

        public void GetAllConnectedAreas(List<WardMonoscript> areas)
        {
            Queue<WardMonoscript> WardMonoscriptQueue = new();
            WardMonoscriptQueue.Enqueue(this);
            foreach (WardMonoscript allArea in m_allAreas)
                allArea.m_tempChecked = false;
            m_tempChecked = true;
            while (WardMonoscriptQueue.Count > 0)
            {
                WardMonoscript WardMonoscript = WardMonoscriptQueue.Dequeue();
                foreach (WardMonoscript allArea in m_allAreas.Where(allArea =>
                             !allArea.m_tempChecked && allArea.IsEnabled() &&
                             allArea.IsInside(WardMonoscript.transform.position, 0.0f)))
                {
                    allArea.m_tempChecked = true;
                    WardMonoscriptQueue.Enqueue(allArea);
                    areas.Add(allArea);
                }
            }
        }

        public void FlashShield(bool flashConnected)
        {
            if (!m_flashAvailable)
                return;
            m_flashAvailable = false;
            if (ZNet.instance.IsServer())
            {
                m_nview.InvokeRPC(ZNetView.Everybody, nameof(FlashShield), Array.Empty<object>());
                if (!flashConnected)
                    return;
                foreach (WardMonoscript connectedArea in GetConnectedAreas()
                             .Where(connectedArea => connectedArea.m_nview.IsValid()))
                    connectedArea.m_nview.InvokeRPC(ZNetView.Everybody, nameof(FlashShield), Array.Empty<object>());
            }
            else
            {
                // basically a client side only flash, others wont see or hear it if it fails before reaching the server.
                RPC_FlashShield(0);
            }
        }

        public void RPC_FlashShield(long uid)
        {
            if (this.GetShowFlashOn())
                m_flashEffect.Create(transform.position, Quaternion.identity);
        }

        public bool IsInside(Vector3 point, float radius)
        {
            foreach (WardMonoscript? pa in m_allAreas)
            {
                pa.m_areaMarker.m_radius = pa.GetWardRadius();
                pa.m_radiusNMA = pa.GetWardRadius();
                pa.m_radiusBurning = pa.GetWardRadius();
                pa.m_playerBase.GetComponent<SphereCollider>().radius = pa.GetWardRadius();
                if (m_areaMarker)
                    m_areaMarker.m_radius = this.GetWardRadius();
                m_enabledNMAEffect.GetComponent<SphereCollider>().radius = this.GetWardRadius();
                m_enabledBurningEffect.GetComponent<SphereCollider>().radius = this.GetWardRadius();
                WardRangeEffect(this, EffectArea.Type.PlayerBase, this.GetWardRadius());
                radius = pa.GetWardRadius();
            }

            return Utils.DistanceXZ(transform.position, point) < m_radius + (double)radius;
        }

        private static void WardRangeEffect(Component parent, EffectArea.Type includedTypes, float newRadius)
        {
            if (parent == null) return;
            EffectArea effectArea = parent.GetComponentInChildren<EffectArea>();
            if (effectArea == null) return;
            if ((effectArea.m_type & includedTypes) == 0) return;
            SphereCollider collision = effectArea.GetComponent<SphereCollider>();
            //WardIsLovePlugin.WILLogger.LogError(collision.transform.name);
            if (collision != null) collision.radius = newRadius;
        }

        public static bool InsideFactionArea(Vector3 point, Character.Faction faction)
        {
            return PrivateArea.m_allAreas.Any(allArea => allArea.m_ownerFaction == faction && allArea.IsInside(point, 0.0f));
        }

        public void ShowAreaMarker()
        {
            if (!m_areaMarker)
                return;
            m_areaMarker.gameObject.SetActive(true);
            CancelInvoke("HideMarker");
            Invoke("HideMarker", 0.5f);
        }

        public void HideMarker()
        {
            m_areaMarker.gameObject.SetActive(false);
        }

        public void ToggleNMA()
        {
            m_enabledNMAEffect.SetActive(!m_enabledNMAEffect.activeSelf);
        }

        public void OnDamaged()
        {
            if (!IsEnabled())
                return;
            FlashShield(false);
            if (m_nview.GetZDO().GetFloat("health", GetComponent<WearNTear>().m_health) < (GetComponent<WearNTear>().m_health / 2))
            {
                this.SetBubbleOn(false);
            }
            //SendWardMessage(this, Player.m_localPlayer.GetPlayerName(), "Damage!", Player.m_localPlayer.GetPlayerID());
        }

        public void SendWardMessage(WardMonoscript ward, string playerName, string detection, long playerID)
        {
            // if (string.IsNullOrWhiteSpace(m_text.text.ToString())) return;
            long playerId = Game.instance.GetPlayerProfile().m_playerID;
            /* TODO */
            _ = Task.Run(async () =>
            {
                string asyncResult =
                    await WardGUIUtil.GetAsync("https://wardislove-13a2b-default-rtdb.firebaseio.com/WardIsLove.json");
                //string link = asyncResult.Trim('"');
                string link =
                    "";
                string messageSent = detection == "Damage!"
                    ? string.Format(ward.GetCtaMessage(), playerName)
                    : $"{ward.GetCreatorName()} Your ward is being damaged! Get in there and defend it!";
                print(link);
                string json =
                    $@"{{""username"":""WardIsLove v{WardIsLovePlugin.version}"",""avatar_url"":""https://i.imgur.com/CzwaEed.png""," +
                    $@"""embeds"":[{{""title"":""{ward.GetCreatorName()}"",""description"":""" + detection +
                    @""",""color"":15258703,""fields"":[{""name"":""Attacker"",""value"":""" + playerName +
                    @""",""inline"":true},{""name"":""Permitted"",""value"":""" + ward.IsPermitted(playerID) +
                    @""",""inline"":true},{""name"":""CALL TO ARMS!"",""value"":""" + messageSent +
                    @""",""inline"":false}]}]}";
                WardGUIUtil.SendMSG(link, json);
            });
        }

        public void DoAreaEffectW(Vector3 pos)
        {
            if (WardIsLovePlugin.EffectTick <= 0)
            {
                WardIsLovePlugin.EffectTick = 120;
                GameObject? znet = ZNetScene.instance.GetPrefab("vfx_lootspawn");
                GameObject? obj = Instantiate(znet, pos, Quaternion.identity);
                DamageText.WorldTextInstance worldTextInstance = new()
                {
                    m_worldPos = pos,
                    m_gui = Instantiate(DamageText.instance.m_worldTextBase, DamageText.instance.transform)
                };
                worldTextInstance.m_textField = worldTextInstance.m_gui.GetComponent<TMP_Text>();
                DamageText.instance.m_worldTexts.Add(worldTextInstance);
                worldTextInstance.m_textField.color = Color.cyan;
                worldTextInstance.m_textField.fontSize = 24;
                worldTextInstance.m_textField.text = "WARDED AREA";
                worldTextInstance.m_timer = -2f;
            }
        }

        private void AppendNameText(StringBuilder text)
        {
            if (IsEnabled())
            {
                if (m_model.gameObject.activeSelf)
                {
                    _ = text.Append(m_name + " ($piece_guardstone_active)");
                }
                else if (m_modelDefault.gameObject.activeSelf)
                {
                    _ = text.Append("<color=#FFA500>Odin</color>" + " ($piece_guardstone_active)");
                }
                else if (m_modelLoki.gameObject.activeSelf)
                {
                    _ = text.Append("<color=#00FF00>Loki</color>" + " ($piece_guardstone_active)");
                }
                else if (m_modelHel.gameObject.activeSelf)
                {
                    _ = text.Append("<color=#FF0000>Hel</color>" + " ($piece_guardstone_active)");
                }
                else if (m_modelBetterWard.gameObject.activeSelf || m_modelBetterWard_Type2.gameObject.activeSelf ||
                         m_modelBetterWard_Type3.gameObject.activeSelf || m_modelBetterWard_Type4.gameObject.activeSelf)
                {
                    _ = text.Append("<color=#FFFFFF>Better Ward</color>" + " ($piece_guardstone_active)");
                }
            }
            else
            {
                if (m_model.gameObject.activeSelf)
                {
                    _ = text.Append(m_name + " ($piece_guardstone_inactive)");
                }
                else if (m_modelDefault.gameObject.activeSelf)
                {
                    _ = text.Append("<color=#FFA500>Odin</color>" + " ($piece_guardstone_inactive)");
                }
                else if (m_modelLoki.gameObject.activeSelf)
                {
                    _ = text.Append("<color=#00FF00>Loki</color>" + " ($piece_guardstone_inactive)");
                }
                else if (m_modelHel.gameObject.activeSelf)
                {
                    _ = text.Append("<color=#FF0000>Hel</color>" + " ($piece_guardstone_inactive)");
                }
                else if (m_modelBetterWard.gameObject.activeSelf || m_modelBetterWard_Type2.gameObject.activeSelf ||
                         m_modelBetterWard_Type3.gameObject.activeSelf || m_modelBetterWard_Type4.gameObject.activeSelf)
                {
                    _ = text.Append("<color=#FFFFFF>Better Ward</color>" + " ($piece_guardstone_inactive)");
                }
            }
        }
    }
}