#if TESTINGBUILD
using System.Collections.Generic;
using UnityEngine;

namespace WardIsLove.Util
{
    public class WardBubbleExclusionZone : MonoBehaviour
    {
        public static HashSet<WardBubbleExclusionZone> allInstances = new HashSet<WardBubbleExclusionZone>();
        public float radius = 10f;

        private void Awake()
        {
            allInstances.Add(this);
        }

        private void OnDestroy()
        {
            allInstances.Remove(this);
        }

        public float getRadius() => this.radius;

        public void UpdateWaterMesh()
        {
            foreach (var manager in WaterSurfaceManager.managers.Values)
            {
                manager.UpdateWaterMesh(transform.position, radius);
            }
        }
    }
}
#endif