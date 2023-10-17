using HarmonyLib;
using UnityEngine;

namespace WardIsLove.Util.Bubble
{
    [HarmonyPatch]
    public class ControlParticlesSpawner : MonoBehaviour
    {
        public static ParticleSystem cps = null!;
        public string bubbleTag = "WardIsLoveFF";

        /*private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag(bubbleTag))
            {
                WardIsLovePlugin.WILLogger.LogError($"{collision.gameObject.name}");
                */
        /*            if (!WardMonoscript.CheckInWardMonoscript(collision.transform.position))
                               {*/
        /*
                       Destroy(collision.gameObject);
                       cps.transform.position = collision.transform.position;
                       cps.Emit(12);
                       */
        /*            }*/ /*
            }
        }*/

        /*private void OnTriggerEnter(Collider collider)
        {
            if (collider.gameObject != Player.m_localPlayer.gameObject)
            {
                WardIsLovePlugin.WILLogger.LogError($"{collider.gameObject.name}");

                if (!WardMonoscript.CheckInWardMonoscript(collider.transform.position))
                {
                    Destroy(collider.gameObject);
                    cps.transform.position = collider.transform.position;
                    cps.Emit(12);
                }
            }
        }*/
    }
}