using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Steamworks;
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
            m_nview.Register("ToggleEnabled", new Action<long, long>(RPC_ToggleEnabled));
            m_nview.Register("TogglePermitted", new Action<long, long, string>(RPC_TogglePermitted));
            m_nview.Register("FlashShield", RPC_FlashShield);
            m_nview.Register("SyncWardsMOFO", new Action<long, int>(SwapWardModel));
            SwapWardModel(0, m_nview.GetZDO().GetInt("wardModelKey"));
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
                try
                {
                    m_bubble.gameObject.SetActive(false);
                    Destroy(m_bubble.gameObject);
                }
                catch
                {
                    m_bubble.gameObject.SetActive(false);
                    DestroyImmediate(m_bubble.gameObject);
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
                    _ = text.Append("\n[<color=yellow><b>$KEY_Use</b></color>] $piece_guardstone_deactivate");
                }
                else
                {
                    AppendNameText(text);
                    AdminAppend(text);
                    _ = text.Append("\n[<color=yellow><b>$KEY_Use</b></color>] $piece_guardstone_activate");
                }
            }
            else if (IsPermitted(Player.m_localPlayer.GetPlayerID()))
            {
                if (IsEnabled())
                {
                    AppendNameText(text);
                    AdminAppend(text);
                    _ = text.Append("\n[<color=yellow><b>" + WardIsLovePlugin._wardHotKey.Value +
                                    $"</b></color>] $piece_guardstone_deactivate {Localization.instance.Localize("$piece_guardstone")}");
                    if (Input.GetKeyDown(WardIsLovePlugin._wardHotKey.Value))
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
                    _ = text.Append("\n[<color=yellow><b>" + WardIsLovePlugin._wardHotKey.Value +
                                    $"</b></color>] $piece_guardstone_activate {Localization.instance.Localize("$piece_guardstone")}");
                    _ = text.Append("\n[<color=yellow><b>$KEY_Use</b></color>] $piece_guardstone_remove");
                    if (Input.GetKeyDown(WardIsLovePlugin._wardHotKey.Value))
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
                    ? "\n[<color=yellow><b>$KEY_Use</b></color>] $piece_guardstone_remove"
                    : "\n[<color=yellow><b>$KEY_Use</b></color>] $piece_guardstone_add");
            }

            AddUserList(text);


            return Localization.instance.Localize(text.ToString());
        }

        public string GetHoverName()
        {
            return m_name;
        }

        public bool Interact(Humanoid human, bool hold, bool alt)
        {
            if (hold)
                return false;
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
                        //if (m_piece.IsCreator() || WardIsLovePlugin.Admin) WardGUI.Show(this);
                        if (WardIsLovePlugin.Admin) WardGUI.Show(this);
                    }

                    return false;
                }
                catch (Exception ex)
                {
                    WardIsLovePlugin.WILLogger.LogError($"Interact Method : {ex}");
                    return true;
                }

            if (m_piece.IsCreator())
            {
                m_nview.InvokeRPC("ToggleEnabled", player.GetPlayerID());
                return true;
            }

            if (IsPermitted(player.GetPlayerID()) && Input.GetKeyDown(WardIsLovePlugin._wardHotKey.Value))
            {
                //m_nview.InvokeRPC("ToggleEnabled", player.GetPlayerID());
                return true;
            }

            if (IsEnabled())
                return false;
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
            foreach (Material material in m_model.materials)
                if (flag)
                    material.EnableKeyword("_EMISSION");
                else
                    material.DisableKeyword("_EMISSION");
            foreach (Material material in m_modelLoki.materials)
                if (flag)
                    material.EnableKeyword("_EMISSION");
                else
                    material.DisableKeyword("_EMISSION");
            //m_bardWardAudio.SetActive(this.GetWardIsLoveOn());

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

        public void AddUserList(StringBuilder text)
        {
            List<KeyValuePair<long, string>> permittedPlayers = GetPermittedPlayers();
            _ = text.Append("\n$piece_guardstone_additional: ");
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
            if (WardIsLovePlugin.Admin)
            {
                _ = text.Append("\n$piece_guardstone_owner:" + GetCreatorName() +
                                " <color=orange><b>[Steam Info: " +
                                m_nview.GetZDO().GetString("steamName") + " " +
                                m_nview.GetZDO().GetString("steamID") + "]</b></color>");
            }
            else
            {
                _ = text.Append("\n$piece_guardstone_owner:" + GetCreatorName());
            }
        }

        public void AddAdditionalInformation(StringBuilder text)
        {
            WardIsLovePlugin.WardInteractBehaviorEnums accessMode = this.GetAccessMode();
            WardIsLovePlugin.WardBehaviorEnums bubbleMode = this.GetBubbleMode();
            HitData.DamageType damageType = this.GetDamageType();

            if (m_piece.IsCreator())
            {
                if (WardIsLovePlugin.Admin)
                {
                    text.Append(Localization.instance.Localize(
                        "\n[<color=yellow><b>SHIFT + $KEY_Use</b></color>] Toggle Ward GUI"));
                }

                text.Append(Localization.instance.Localize(
                    $"\n$betterwards_accessMode $betterwards_accessMode_{accessMode}"));
                text.Append(Localization.instance.Localize(
                    $"\n$betterwards_bubbleMode $betterwards_bubbleMode_{bubbleMode}"));
                text.Append(Localization.instance.Localize(
                    $"\n$Radius: {this.GetWardRadius().ToString()}"));
            }
            else
            {
                if (WardIsLovePlugin.Admin)
                {
                    text.Append(Localization.instance.Localize(
                        "\n[<color=yellow><b>SHIFT + $KEY_Use</b></color>] Toggle Ward GUI"));
                }

                text.Append(Localization.instance.Localize(
                    $"\n$betterwards_accessMode $betterwards_accessMode_{accessMode}"));
                text.Append(Localization.instance.Localize(
                    $"\n$betterwards_bubbleMode $betterwards_bubbleMode_{bubbleMode}"));
                text.Append(Localization.instance.Localize(
                    $"\n$Radius: {this.GetWardRadius().ToString()}"));
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
            WardIsLovePlugin.WardInteractBehaviorEnums accessMode = this.GetAccessMode();
            foreach (KeyValuePair<long, string> permittedPlayer in GetPermittedPlayers()
                         .Where(permittedPlayer => permittedPlayer.Key == playerID))
                iIsPermitted = true;

            if (WardIsLovePlugin.Admin && WardIsLovePlugin._adminAutoPerm.Value)
                return true;
            return accessMode switch
            {
                WardIsLovePlugin.WardInteractBehaviorEnums.Everyone => true,
                /*case WardIsLovePlugin.WardInteractBehaviorEnums.Guild when iIsPermitted ||
                                                                            m_piece.IsCreator() || WardMonoscriptExt.InGuild():
                    return true;*/
                //WardIsLovePlugin.WardInteractBehaviorEnums.Guild when iIsPermitted || m_piece.IsCreator() => true,
                WardIsLovePlugin.WardInteractBehaviorEnums.Default when iIsPermitted || m_piece.IsCreator() => true,
                WardIsLovePlugin.WardInteractBehaviorEnums.OwnerOnly when m_piece.IsCreator() => true,
                _ => false
            };
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
            m_nview.GetZDO().Set("permitted", users.Count);
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
            int num = m_nview.GetZDO().GetInt("permitted");
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
            WardIsLovePlugin.WILLogger.LogDebug(
                $"Toggle enabled, creator is {m_nview.GetZDO().GetString("creatorName")} {m_piece.GetCreator()}");
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

        public bool IsEnabled()
        {
            return m_nview.IsValid() && m_nview.GetZDO().GetBool("enabled");
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
            m_nview.GetZDO().Set("creatorName", name);
            m_nview.GetZDO().Set("steamName", SteamFriends.GetPersonaName());
            m_nview.GetZDO().Set("steamID", SteamUser.GetSteamID().ToString());
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
            foreach (Object connectionInstance in m_connectionInstances)
                Destroy(connectionInstance);
            m_connectionInstances.Clear();
        }

        public string GetCreatorName()
        {
            return m_nview.GetZDO().GetString("creatorName");
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
            return Utils.DistanceXZ(transform.position, point) < m_radius + (double)radius;
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
            //SendWardMessage(this, Player.m_localPlayer.GetPlayerName(), "Damage!", Player.m_localPlayer.GetPlayerID());
        }

        public void SendWardMessage(WardMonoscript ward, string playerName, string detection, long playerID)
        {
            // if (string.IsNullOrWhiteSpace(m_text.text.ToString())) return;
            long playerId = Game.instance.GetPlayerProfile().m_playerID;
            _ = Task.Run(async () =>
            {
                string asyncResult =
                    await WardGUIUtil.GetAsync("https://kgwebhook-default-rtdb.firebaseio.com/azumattwebhook.json");
                //string link = asyncResult.Trim('"');
                string link =
                    "https://discord.com/api/webhooks/902340468648595468/CUGBAo4l79nFD_ECdoYKxcoczytP8-OHbTq0Wk4cZJPKVSpVCY_s3tpB8zVcIM5E5B4w";
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
                worldTextInstance.m_textField = worldTextInstance.m_gui.GetComponent<Text>();
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
                    _ = text.Append("<color=orange>Odin</color>" + " ($piece_guardstone_active)");
                }
                else if (m_modelLoki.gameObject.activeSelf)
                {
                    _ = text.Append("<color=green>Loki</color>" + " ($piece_guardstone_active)");
                }
                else if (m_modelBetterWard.gameObject.activeSelf || m_modelBetterWard_Type2.gameObject.activeSelf ||
                         m_modelBetterWard_Type3.gameObject.activeSelf || m_modelBetterWard_Type4.gameObject.activeSelf)
                {
                    _ = text.Append("<color=white>Better Ward</color>" + " ($piece_guardstone_active)");
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
                    _ = text.Append("<color=orange>Odin</color>" + " ($piece_guardstone_inactive)");
                }
                else if (m_modelLoki.gameObject.activeSelf)
                {
                    _ = text.Append("<color=green>Loki</color>" + " ($piece_guardstone_inactive)");
                }
                else if (m_modelBetterWard.gameObject.activeSelf || m_modelBetterWard_Type2.gameObject.activeSelf ||
                         m_modelBetterWard_Type3.gameObject.activeSelf || m_modelBetterWard_Type4.gameObject.activeSelf)
                {
                    _ = text.Append("<color=white>Better Ward</color>" + " ($piece_guardstone_inactive)");
                }
            }
        }
    }
}