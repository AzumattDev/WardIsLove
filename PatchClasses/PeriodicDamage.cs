using HarmonyLib;
using WardIsLove.Extensions;
using WardIsLove.Util;

namespace WardIsLove.PatchClasses
{
    [HarmonyPatch]
    public class PeriodicDamage
    {
        private static int tick;

        [HarmonyPatch(typeof(Character), nameof(Character.FixedUpdate))]
        [HarmonyPostfix]
        private static void Postfix(Character __instance)
        {
            try
            {
                if (!WardIsLovePlugin._wardEnabled.Value) return;
                if (tick > 0) tick--;
                int damage;
                HitData hit = new();

                if (tick <= 0 && !__instance.IsPlayer() &&
                    WardMonoscript.CheckInWardMonoscript(__instance.transform.position))
                {
                    WardMonoscript ward = WardMonoscriptExt.GetWardMonoscript(__instance.transform.position);
                    _ = int.TryParse(ward.GetWardDamageAmount().ToString(), out damage);
                    tick = 100;
                    /*if (ward.GetDamageType() == HitData.DamageType.Fire)
                    {
                        hit.m_damage.m_fire = (float)damage * 0.01f;
                    }

                    if (ward.GetDamageType() == HitData.DamageType.Lightning)
                    {
                        hit.m_damage.m_lightning = (float)damage * 0.01f;
                    }
                    if (ward.GetDamageType() == HitData.DamageType.Chop)
                    {
                        hit.m_damage.m_lightning = (float)damage * 0.01f;
                    }*/


                    foreach (Character? character in Character.m_characters)
                    {
                        if (!character.IsPlayer() && !character.IsTamed() &&
                            WardMonoscript.CheckInWardMonoscript(character.transform.position))
                        {
                            if (ward.GetDamageType() == HitData.DamageType.Fire) hit.m_damage.m_fire = damage * 0.01f;

                            if (ward.GetDamageType() == HitData.DamageType.Lightning)
                                hit.m_damage.m_lightning = damage * 0.01f;
                            if (ward.GetDamageType() == HitData.DamageType.Chop)
                            {
                                hit.m_damage.m_fire = damage * 0.01f;
                                character.AddFireDamage(damage * 0.01f);
                            }
                        }

                        character.ApplyDamage(hit, true, true);
                    }
                }
            }
            catch
            {
            }
        }
    }
}