using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;
using WardIsLove.Extensions;
using WardIsLove.Util.DiscordMessenger;
using WardIsLove.Util.UI;
using static WardIsLove.PatchClasses.PlayerHealthUpdatePatch;
using static WardIsLove.PatchClasses.PlayerStaminaUpdatePatch;
using static WardIsLove.PatchClasses.PushoutNonPermitted;

namespace WardIsLove.Util.Bubble
{
    [HarmonyPatch]
    public class WardEntryDetector : MonoBehaviour
    {
        public WardMonoscript m_wardEntered;
        private Coroutine? Heal;
        private Coroutine? Stamina;
        private Coroutine? DamageUpdate;

        /* Pushout Routines */
        private Coroutine? PushoutPlayersRoutine;
        private Coroutine? PushoutCreaturesRoutine;

        /* Ward Damage things */
        private HitData hitData = new HitData();
        [SerializeField] private List<Humanoid> m_character = new List<Humanoid>();
        [SerializeField] internal WardIsLovePlugin.WardDamageTypes _type;
        [SerializeField] internal Vector3 staggerDirection = new Vector3(0f, 0f, 0f);

        private void OnTriggerEnter(Collider collider)
        {
            Player component = null;
            Humanoid component2 = null;
            if (collider.GetComponent<Player>())
            {
                component = collider.GetComponent<Player>();
            }
            else if (collider.GetComponent<Humanoid>())
            {
                component2 = collider.GetComponent<Humanoid>();
            }

            if (component2 != null && Player.m_localPlayer != component2)
            {
                PushoutCreaturesRoutine = StartCoroutine(PushoutCreature(component2, m_wardEntered));
                /* Ward Damage */
                Humanoid? monsterchar = collider.gameObject.GetComponent<Humanoid>();
                m_character.Add(monsterchar);
                monsterchar.m_onDeath =
                    (Action)Delegate.Combine(monsterchar.m_onDeath,
                        new Action(delegate { RemoveFromList(monsterchar); }));

                DamageUpdate = StartCoroutine(UpdateDamage());
            }

            if (component == null || Player.m_localPlayer != component)
                return;
            if (m_wardEntered.IsEnabled() && m_wardEntered.GetWardNotificationsOn())
                Player.m_localPlayer.Message(MessageHud.MessageType.Center,
                    string.Format(m_wardEntered.GetWardEnterNotifyMessage(), m_wardEntered.GetCreatorName()));

            /* Send the player a message about the raidable status on entry as well */
            if (m_wardEntered.GetRaidProtectionOn())
                OfflineStatus.CheckOfflineStatus(m_wardEntered);

            Heal = StartCoroutine(DelayedHeal(component, m_wardEntered));
            Stamina = StartCoroutine(DelayedStaminaRegen(component, m_wardEntered));

            PushoutPlayersRoutine = StartCoroutine(PushoutPlayer(component, m_wardEntered));


            //SendWardMessage(m_wardEntered, component.GetPlayerName(), "Entered", component.GetPlayerID());
        }

        private void OnTriggerExit(Collider collider)
        {
            collider.TryGetComponent(typeof(Humanoid), out Component test);
            if (m_character.Contains((Humanoid)test))
            {
                if (DamageUpdate != null)
                {
                    StopCoroutine(DamageUpdate);
                }

                m_character.Remove((Humanoid)test);
            }

            Player component = collider.GetComponent<Player>();
            if (component == null || Player.m_localPlayer != component)
                return;
            if (m_wardEntered.IsEnabled() && m_wardEntered.GetWardNotificationsOn())
                Player.m_localPlayer.Message(MessageHud.MessageType.Center,
                    string.Format(m_wardEntered.GetWardExitNotifyMessage(), component.GetPlayerName()));
            if (Heal != null)
            {
                StopCoroutine(Heal);
            }

            if (Stamina != null)
            {
                StopCoroutine(Stamina);
            }

            if (PushoutPlayersRoutine != null)
            {
                StopCoroutine(PushoutPlayersRoutine);
            }

            if (PushoutCreaturesRoutine != null)
            {
                StopCoroutine(PushoutCreaturesRoutine);
            }

            if (DamageUpdate != null)
            {
                StopCoroutine(DamageUpdate);
            }
            //SendWardMessage(m_wardEntered, component.GetPlayerName(), "Exited", component.GetPlayerID());
        }

        private IEnumerator UpdateDamage()
        {
            /* TODO Condense this later, if possible. */
            while (true)
            {
                yield return new WaitForSeconds(WardIsLovePlugin.WardDamageRepeatRate.Value);
                switch (_type)
                {
                    case WardIsLovePlugin.WardDamageTypes.Frost:
                        foreach (Humanoid? character in m_character)
                        {
                            hitData = new HitData
                            {
                                m_point = Vector3.zero,
                            };
                            hitData.m_damage.m_frost = m_wardEntered.GetWardDamageAmount();
                            if (!character.IsTamed())
                            {
                                character.ApplyDamage(hitData, true, false);
                            }
                        }

                        break;
                    case WardIsLovePlugin.WardDamageTypes.Poison:
                        foreach (Humanoid? character in m_character)
                        {
                            hitData = new HitData
                            {
                                m_point = Vector3.zero,
                            };
                            hitData.m_damage.m_poison = m_wardEntered.GetWardDamageAmount();
                            if (!character.IsTamed())
                            {
                                character.ApplyDamage(hitData, true, false);
                            }
                        }

                        break;
                    case WardIsLovePlugin.WardDamageTypes.Fire:
                        foreach (Humanoid? character in m_character)
                        {
                            hitData = new HitData
                            {
                                m_point = Vector3.zero,
                            };
                            hitData.m_damage.m_fire = m_wardEntered.GetWardDamageAmount();
                            if (!character.IsTamed())
                            {
                                character.ApplyDamage(hitData, true, false);
                            }
                        }

                        break;
                    case WardIsLovePlugin.WardDamageTypes.Lightning:
                        foreach (Humanoid? character in m_character)
                        {
                            hitData = new HitData
                            {
                                m_point = Vector3.zero,
                            };
                            hitData.m_damage.m_lightning = m_wardEntered.GetWardDamageAmount();
                            if (!character.IsTamed())
                            {
                                character.ApplyDamage(hitData, true, false);
                            }
                        }

                        break;
                    case WardIsLovePlugin.WardDamageTypes.Spirit:
                        foreach (Humanoid? character in m_character)
                        {
                            hitData = new HitData
                            {
                                m_point = Vector3.zero,
                            };
                            hitData.m_damage.m_spirit = m_wardEntered.GetWardDamageAmount();
                            if (!character.IsTamed())
                            {
                                character.ApplyDamage(hitData, true, false);
                            }
                        }

                        break;
                    case WardIsLovePlugin.WardDamageTypes.Stagger:
                        foreach (Humanoid? character in m_character)
                        {
                            hitData = new HitData
                            {
                                m_point = Vector3.zero,
                            };
                            hitData.m_damage.m_damage = m_wardEntered.GetWardDamageAmount();
                            staggerDirection = character.transform.rotation.eulerAngles;
                            if (character.IsTamed()) continue;
                            character.AddStaggerDamage(hitData.m_damage.m_damage, staggerDirection);
                            character.ApplyDamage(hitData, true, false);
                        }

                        break;
                    case WardIsLovePlugin.WardDamageTypes.Normal:
                        foreach (Humanoid? character in m_character)
                        {
                            hitData = new HitData
                            {
                                m_point = Vector3.zero,
                            };
                            hitData.m_damage.m_blunt = m_wardEntered.GetWardDamageAmount();
                            if (!character.IsTamed())
                                character.ApplyDamage(hitData, true, false);
                        }

                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private void RemoveFromList(Humanoid character)
        {
            m_character.Remove(character);
        }

        public void SendWardMessage(WardMonoscript ward, string playerName, string detection, long playerID)
        {
            // if (string.IsNullOrWhiteSpace(m_text.text.ToString())) return;
            long playerId = Game.instance.GetPlayerProfile().m_playerID;
            string messageSent = detection == "Exited"
                ? string.Format(m_wardEntered.GetWardExitNotifyMessage(), playerName)
                : string.Format(m_wardEntered.GetWardEnterNotifyMessage(), m_wardEntered.GetCreatorName());
            new DiscordMessage()
                .SetUsername($"WardIsLove v{WardIsLovePlugin.version}")
                .SetAvatar("https://i.imgur.com/CzwaEed.png")
                .AddEmbed()
                .SetTimestamp(DateTime.Now)
                .SetTitle($"{playerName}")
                .SetDescription(
                    $"{detection} ward")
                .SetColor(15258703)
                .AddField("Ward Owner", $"{ward.GetCreatorName()}", true)
                .AddField("Permitted", $"{ward.IsPermitted(playerID)}", true)
                .AddField("Message Shown To Player", $"{messageSent}")
                .Build()
                .SendMessageAsync("");
        }
    }

    [HarmonyPatch]
    public class CollisionBubble : MonoBehaviour
    {
        private Collider COL;
        private WardMonoscript ward;

        private void Awake()
        {
            ward = GetComponentInParent<WardMonoscript>();
            COL = GetComponent<MeshCollider>();
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (ward.GetBubbleOn())
            {
                if (collision.collider.transform.root.gameObject.name.ToLower().Contains("ship") ||
                    collision.collider.transform.root.gameObject.name.ToLower().Contains("karve") ||
                    collision.collider.transform.root.gameObject.name.ToLower().Contains("raft") ||
                    collision.collider.transform.root.gameObject.name.ToLower().Contains("cart") ||
                    collision.collider.transform.root.gameObject.name.ToLower().Contains("saddle"))
                {
                    Physics.IgnoreCollision(collision.collider, COL, true);
                    Collider[]? colliders =
                        collision.collider.transform.root.gameObject.GetComponentsInChildren<Collider>();
                    foreach (Collider collider in colliders)
                    {
                        Physics.IgnoreCollision(collider, COL, true); // Have to do this, or it glitches for ships.
                    }

                    return;
                }
            }

            if (!ward.GetPushoutPlayersOn() && collision.collider == Player.m_localPlayer?.m_collider)
            {
                Physics.IgnoreCollision(collision.collider, COL, true);
                StartCoroutine(DelayedCollision(collision.collider, COL));
                return;
            }

            if (!ward.GetPushoutCreaturesOn() && collision.collider != Player.m_localPlayer?.m_collider)
            {
                Physics.IgnoreCollision(collision.collider, COL, true);
                StartCoroutine(DelayedCollision(collision.collider, COL));
                return;
            }

            if (ward.GetPushoutPlayersOn() && collision.collider == Player.m_localPlayer?.m_collider &&
                ward.IsPermitted(Player.m_localPlayer.GetPlayerID()))
            {
                Physics.IgnoreCollision(collision.collider, COL, true);
                StartCoroutine(DelayedCollision(collision.collider, COL));
            }

            if (!ward.GetPushoutCreaturesOn() || collision.collider == Player.m_localPlayer?.m_collider) return;
            if (!collision.collider.gameObject.GetComponent<Character>()) return;
            if (!collision.collider.gameObject.GetComponent<Character>().IsTamed()) return;
            Physics.IgnoreCollision(collision.collider, COL, true);
        }

        private IEnumerator DelayedCollision(Collider first, Collider second)
        {
            yield return new WaitForSecondsRealtime(1);
            if (first != null && second != null)
            {
                Physics.IgnoreCollision(first, second, false);
            }
        }
    }
}