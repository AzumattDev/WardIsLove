using System;
using HarmonyLib;
using UnityEngine;
using WardIsLove.Extensions;

namespace WardIsLove.Util
{
    [HarmonyPatch]
    public class DamageArea : MonoBehaviour
    {
        [SerializeField] internal WardMonoscript _area;
        private Collider _collider;
        private HitData hitdata;
        private Humanoid hum;
        private GameObject LightningStrikeVFX;
        private Transform strikelocation;

        private void Awake()
        {
            LightningStrikeVFX = ZNetScene.instance.GetPrefab("wardlightningAOE");
        }

        private void OnEnable()
        {
            try
            {
                if (_area == null) _area = GetComponentInParent<WardMonoscript>();
                //InvokeRepeating(nameof(Castlightning), 0f, 1);
            }
            catch (Exception e)
            {
                // ignored
            }
        }


        private void Castlightning()
        {
            if (!_area.IsEnabled()) return;
            if (!_area.GetBubbleOn()) return;
            if (!Player.m_localPlayer) return;
            Collider[] hitcolliders = Physics.OverlapSphere(transform.position, _area.m_radius);
            foreach (Collider hitcollider in hitcolliders)
                if (hitcollider.gameObject.GetComponent<MonsterAI>() != null)
                {
                    if (hitcollider.gameObject.GetComponent<Humanoid>().m_tamed) return;
                    try
                    {
                        Tameable? tame = hitcollider.GetComponent<Tameable>();
                        if (tame != null)
                            if (tame.GetTameness() > 0)
                                return;
                    }
                    catch (Exception e)
                    {
                        WardIsLovePlugin.WILLogger.LogInfo(e);
                    }

                    GameObject tmp = hitcollider.gameObject;
                    _collider = hitcollider;
                    hum = tmp.GetComponent<Humanoid>();
                    try
                    {
                        hitdata = new HitData
                        {
                            m_attacker = Player.m_localPlayer.GetZDOID(),
                            m_blockable = false,
                            m_damage = new HitData.DamageTypes
                            {
                                m_blunt = 0f,
                                m_chop = 0f,
                                m_lightning = 10f,
                                m_damage = 0f,
                                m_fire = 0f,
                                m_frost = 0f,
                                m_pickaxe = 0f,
                                m_pierce = 0f,
                                m_poison = 0f,
                                m_slash = 0f,
                                m_spirit = 0f
                            },
                            m_dir = Vector3.zero,
                            m_dodgeable = false,
                            m_ranged = true,
                            m_skill = Skills.SkillType.All,
                            m_backstabBonus = 0f,
                            m_hitCollider = _collider,
                            m_pushForce = 3.5f,
                            m_staggerMultiplier = 0.01f,
                            m_toolTier = 10,
                            m_statusEffect = "",
                            m_point = Vector3.zero
                        };
                    }
                    catch (Exception e)
                    {
                        WardIsLovePlugin.WILLogger.LogError(e);
                    }

                    strikelocation = hum.transform;
                    //StartCoroutine(LightningStrike());
                    Instantiate(LightningStrikeVFX, strikelocation.position, Quaternion.identity);
                    hum.ApplyDamage(hitdata, true, false, HitData.DamageModifier.Weak);
                }
        }

        /*private IEnumerator LightningStrike()
        {
            //yield return null;
            Instantiate(LightningStrikeVFX, strikelocation, false);
            hum.ApplyDamage(hitdata, true, false, HitData.DamageModifier.Weak);
            yield return new WaitForSeconds(2.5f);
        }*/
    }
}