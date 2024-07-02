#if TESTINGBUILD
using System;
using HarmonyLib;
using UnityEngine;

namespace WardIsLove.Util;

public class WaterVolumePatch
{
    private static int[][] coordMap;

    public static Vector3 getVertexForCoord(int x, int z, Vector3[] vertices)
    {
        x = Mathf.Clamp(x, 0, 31);
        z = Mathf.Clamp(z, 0, 31);
        return vertices[WaterVolumePatch.coordMap[x][z]];
    }

    public static void precalculateVertices(MeshFilter waterSurface)
    {
        if (WaterVolumePatch.coordMap != null)
            return;
        Vector3[] vertices = waterSurface.sharedMesh.vertices;
        WaterVolumePatch.coordMap = new int[32][];
        for (int index = 0; index < WaterVolumePatch.coordMap.Length; ++index)
            WaterVolumePatch.coordMap[index] = new int[32];
        for (int index = 0; index < vertices.Length; ++index)
        {
            if ((double)vertices[index].y == 0.0)
            {
                float num1 = vertices[index].x + 1f;
                float num2 = vertices[index].z + 1f;
                float f1 = (float)((double)num1 * 31.0 / 2.0);
                float f2 = (float)((double)num2 * 31.0 / 2.0);
                WaterVolumePatch.coordMap[Mathf.RoundToInt(f1)][Mathf.RoundToInt(f2)] = index;
            }
        }

        Debug.Log((object)"Cached water surface vertex order");
    }
}

[HarmonyPatch(typeof(WaterVolume), nameof(WaterVolume.Awake))]
static class WaterVolumeAwakePatch
{
    public static void Postfix(WaterVolume __instance)
    {
        if (__instance.GetComponent<WaterSurfaceManager>() == null)
        {
            __instance.gameObject.AddComponent<WaterSurfaceManager>();
        }
    }
}

[HarmonyPatch(typeof(WaterVolume), nameof(WaterVolume.GetWaterSurface))]
public class GetWaterSurface_Patch
{
    public static float Postfix(float __result, WaterVolume __instance, Vector3 point)
    {
        __result += getSurfaceDeviation(__instance, point);
        return __result;
    }

    private static float getSurfaceDeviation(WaterVolume volume, Vector3 point)
    {
        // Handle bubble exclusion zones
        foreach (WardBubbleExclusionZone bubble in WardBubbleExclusionZone.allInstances)
        {
            float distance = Vector3.Distance(bubble.transform.position, point);

            if (distance < bubble.getRadius())
            {
                return -1000f; // Large negative value to effectively remove water
            }
        }

        return 0.0f;
    }


    private static Vector3 interpolateX(Vector3 a, Vector3 b, Vector3 pos)
    {
        float t = Mathf.InverseLerp(a.x, b.x, pos.x);
        float y = Mathf.Lerp(a.y, b.y, t);
        if ((double)a.z != (double)b.z)
            Debug.Log((object)"interpolateX inputs have mismatched Z");
        return new Vector3(pos.x, y, a.z);
    }

    private static Vector3 interpolateZ(Vector3 a, Vector3 b, Vector3 pos)
    {
        float t = Mathf.InverseLerp(a.z, b.z, pos.z);
        float y = Mathf.Lerp(a.y, b.y, t);
        if ((double)a.x != (double)b.x)
            Debug.Log((object)"interpolateZ inputs have mismatched X");
        return new Vector3(a.x, y, pos.z);
    }
}
#endif