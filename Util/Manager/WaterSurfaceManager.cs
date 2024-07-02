#if TESTINGBUILD
using System.Collections.Generic;
using UnityEngine;

namespace WardIsLove.Util
{
    public class WaterSurfaceManager : MonoBehaviour
    {
        public static Dictionary<WaterVolume, WaterSurfaceManager> managers = new Dictionary<WaterVolume, WaterSurfaceManager>();
        public WaterVolume waterVolume;
        public MeshFilter waterSurface;
        private Vector3[] originalVertices;
        private Vector3[] alteredVertices;

        private void Awake()
        {
            waterVolume = GetComponent<WaterVolume>();
            waterSurface = waterVolume.m_waterSurface.GetComponent<MeshFilter>();
            managers[waterVolume] = this;

            originalVertices = waterSurface.sharedMesh.vertices;
            alteredVertices = (Vector3[])originalVertices.Clone();
        }

        private void OnDestroy()
        {
            managers.Remove(waterVolume);
        }

        public void UpdateWaterMesh(Vector3 bubblePosition, float bubbleRadius)
        {
            for (int i = 0; i < originalVertices.Length; i++)
            {
                Vector3 worldPosition = waterSurface.transform.TransformPoint(originalVertices[i]);
                alteredVertices[i].y = Vector3.Distance(bubblePosition, worldPosition) < bubbleRadius
                    ? Mathf.Min(alteredVertices[i].y, -bubbleRadius) // Push water vertices significantly down within the exclusion zone 
                    : originalVertices[i].y; // Reset to original
            }

            waterSurface.mesh.vertices = alteredVertices;
            waterSurface.mesh.RecalculateNormals();
            waterSurface.mesh.RecalculateBounds(); // Ensure the bounds are recalculated for proper rendering
        }
    }
}
#endif