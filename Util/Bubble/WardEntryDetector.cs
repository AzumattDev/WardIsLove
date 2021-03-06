using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;
using WardIsLove.Extensions;
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

        /* Pushout Routines */
        private Coroutine? PushoutPlayersRoutine;
        private Coroutine? PushoutCreaturesRoutine;

        /* Ward Damage things */
        private HitData hitData = new HitData();
        [SerializeField] private List<Character> m_character = new List<Character>();
        [SerializeField] internal WardIsLovePlugin.WardDamageTypes _type;
        [SerializeField] internal float DamagePerHit = 0f;
        [SerializeField] internal Vector3 staggerDirection = new Vector3(0f, 0f, 0f);

        private void OnTriggerEnter(Collider collider)
        {
            Player component = null;
            Character component2 = null;
            if (collider.GetComponent<Player>())
            {
                component = collider.GetComponent<Player>();
            }
            else if (collider.GetComponent<Character>())
            {
                component2 = collider.GetComponent<Character>();
            }

            if (component2 != null && Player.m_localPlayer != component2)
            {
                PushoutCreaturesRoutine = StartCoroutine(PushoutCreature(component2, m_wardEntered));
                /* Ward Damage */
                Character? monsterchar = collider.gameObject.GetComponent<Character>();
                m_character.Add(monsterchar);
                Humanoid? hum = collider.gameObject.GetComponent<Humanoid>();
                hum.m_onDeath =
                    (Action)Delegate.Combine(hum.m_onDeath, new Action(delegate { RemoveFromList(monsterchar); }));

                StartCoroutine(UpdateDamage());
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
            collider.TryGetComponent(typeof(Character), out Component test);
            if (m_character.Contains((Character)test))
            {
                StopCoroutine(UpdateDamage());
                m_character.Remove((Character)test);
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
            //SendWardMessage(m_wardEntered, component.GetPlayerName(), "Exited", component.GetPlayerID());
        }

        private IEnumerator UpdateDamage()
        {
            /* TODO Condense this later, if possible. */
            while (true)
            {
                switch (_type)
                {
                    case WardIsLovePlugin.WardDamageTypes.Frost:
                        yield return new WaitForSeconds(WardIsLovePlugin.WardDamageRepeatRate.Value);
                        foreach (Character? character in m_character)
                        {
                            hitData = new HitData
                            {
                                m_point = character.GetCenterPoint()
                            };
                            hitData.m_damage.m_frost = m_wardEntered.GetWardDamageAmount();
                            character.ApplyDamage(hitData, true, false);
                        }

                        break;
                    case WardIsLovePlugin.WardDamageTypes.Poison:
                        yield return new WaitForSeconds(WardIsLovePlugin.WardDamageRepeatRate.Value);
                        foreach (Character? character in m_character)
                        {
                            hitData = new HitData
                            {
                                m_point = character.GetCenterPoint()
                            };
                            hitData.m_damage.m_poison = m_wardEntered.GetWardDamageAmount();
                            character.ApplyDamage(hitData, true, false);
                        }

                        break;
                    case WardIsLovePlugin.WardDamageTypes.Fire:
                        yield return new WaitForSeconds(WardIsLovePlugin.WardDamageRepeatRate.Value);
                        foreach (Character? character in m_character)
                        {
                            hitData = new HitData
                            {
                                m_point = character.GetCenterPoint()
                            };
                            hitData.m_damage.m_fire = m_wardEntered.GetWardDamageAmount();
                            character.ApplyDamage(hitData, true, false);
                        }

                        break;
                    case WardIsLovePlugin.WardDamageTypes.Lightning:
                        yield return new WaitForSeconds(WardIsLovePlugin.WardDamageRepeatRate.Value);
                        foreach (Character? character in m_character)
                        {
                            hitData = new HitData
                            {
                                m_point = character.GetCenterPoint()
                            };
                            hitData.m_damage.m_lightning = m_wardEntered.GetWardDamageAmount();
                            character.ApplyDamage(hitData, true, false);
                        }

                        break;
                    case WardIsLovePlugin.WardDamageTypes.Spirit:
                        yield return new WaitForSeconds(WardIsLovePlugin.WardDamageRepeatRate.Value);
                        foreach (Character? character in m_character)
                        {
                            hitData = new HitData
                            {
                                m_point = character.GetCenterPoint()
                            };
                            hitData.m_damage.m_spirit = m_wardEntered.GetWardDamageAmount();
                            character.ApplyDamage(hitData, true, false);
                        }

                        break;
                    case WardIsLovePlugin.WardDamageTypes.Stagger:
                        foreach (Character? character in m_character)
                        {
                            hitData = new HitData
                            {
                                m_point = character.GetCenterPoint()
                            };
                            hitData.m_damage.m_damage = m_wardEntered.GetWardDamageAmount();
                            staggerDirection = character.transform.rotation.eulerAngles;
                            character.AddStaggerDamage(DamagePerHit, staggerDirection);
                            character.ApplyDamage(hitData, true, false);
                        }

                        yield return new WaitForSeconds(WardIsLovePlugin.WardDamageRepeatRate.Value);
                        break;
                    case WardIsLovePlugin.WardDamageTypes.Normal:
                        yield return new WaitForSeconds(WardIsLovePlugin.WardDamageRepeatRate.Value);
                        foreach (Character? character in m_character)
                        {
                            hitData = new HitData
                            {
                                m_point = character.GetCenterPoint()
                            };
                            hitData.m_damage.m_blunt = m_wardEntered.GetWardDamageAmount();
                            character.ApplyDamage(hitData, true, false);
                        }

                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            // ReSharper disable once IteratorNeverReturns
        }

        private void RemoveFromList(Character character)
        {
            m_character.Remove(character);
        }

        public void SendWardMessage(WardMonoscript ward, string playerName, string detection, long playerID)
        {
            // if (string.IsNullOrWhiteSpace(m_text.text.ToString())) return;
            long playerId = Game.instance.GetPlayerProfile().m_playerID;
            _ = Task.Run(async () =>
            {
                string asyncResult =
                    await WardGUIUtil.GetAsync("https://wardislove-13a2b-default-rtdb.firebaseio.com/WardIsLove.json");
                string link = asyncResult.Trim('"');
                string messageSent = detection == "Exited"
                    ? string.Format(m_wardEntered.GetWardExitNotifyMessage(), playerName)
                    : string.Format(m_wardEntered.GetWardEnterNotifyMessage(), m_wardEntered.GetCreatorName());
                print(link);
                string json =
                    $@"{{""username"":""WardIsLove v{WardIsLovePlugin.version}"",""avatar_url"":""https://i.imgur.com/CzwaEed.png""," +
                    $@"""embeds"":[{{""title"":""{playerName}"",""description"":""" + detection + " ward" +
                    @""",""color"":15258703,""fields"":[{""name"":""Ward Owner"",""value"":""" + ward.GetCreatorName() +
                    @""",""inline"":true},{""name"":""Permitted"",""value"":""" + ward.IsPermitted(playerID) +
                    @""",""inline"":true},{""name"":""Message Shown To Player"",""value"":""" + messageSent +
                    @""",""inline"":false}]}]}";
                WardGUIUtil.SendMSG(link, json);
            });
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