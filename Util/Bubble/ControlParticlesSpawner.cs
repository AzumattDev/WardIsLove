using HarmonyLib;
using UnityEngine;
using WardIsLove.Extensions;

namespace WardIsLove.Util.Bubble
{
    [HarmonyPatch]
    public class ControlParticlesSpawner : MonoBehaviour
    {
        public static ParticleSystem cps = null!;
        public WardMonoscript wardMonoscript = null!;

        private void Start()
        {
            cps = GetComponent<ParticleSystem>();
            wardMonoscript = GetComponentInParent<WardMonoscript>();
        }

        private void OnCollisionEnter(Collision collision)
        {
            OnProjectileHit(collision.gameObject, wardMonoscript);
        }

        private void OnTriggerEnter(Collider collider)
        {
            OnProjectileHit(collider.gameObject, wardMonoscript);
        }

        public static void OnProjectileHit(GameObject collisionObject, WardMonoscript instance)
        {
            if (!collisionObject) return;
            Projectile component = collisionObject.GetComponent<Projectile>();
            Cinder zinder = collisionObject.GetComponent<Cinder>();
            if (instance == null) return;
            if (component != null)
            {
                if (Vector3.Distance(instance.transform.position, component.m_startPoint) > (double)instance.m_radius && (double)Vector3.Distance(instance.transform.position, component.transform.position) < (double)instance.m_radius)
                {
                    component.OnHit(null, collisionObject.transform.position, false, -collisionObject.transform.forward);
                    ZNetScene.instance.Destroy(collisionObject.gameObject);
                }
            }
            else if (zinder != null)
            {
                if ((Vector3.Distance(instance.transform.position, zinder.transform.position) < (double)instance.GetWardRadius()))
                {
                    ZNetScene.instance.Destroy(collisionObject.gameObject);
                }
            }
        }
    }
}