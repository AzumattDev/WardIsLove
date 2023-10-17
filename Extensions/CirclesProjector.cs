using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace WardIsLove.Extensions
{
    [HarmonyPatch]
    public class CirclesProjector : MonoBehaviour
    {
        public float m_radius = 5f;
        public int m_nrOfSegments = 20;
        public GameObject? m_prefab;
        public LayerMask m_mask;
        public List<GameObject?> m_segments = new();

        public void Start()
        {
            CreateSegments();
        }

        public void Update()
        {
            CreateSegments();
            float num = 6.283185f / m_segments.Count;
            for (int index = 0; index < m_segments.Count; ++index)
            {
                float f = (float)(index * (double)num + Time.time * 0.100000001490116);
                Vector3 vector3 = transform.position +
                                  new Vector3(Mathf.Sin(f) * m_radius, 0.0f, Mathf.Cos(f) * m_radius);
                GameObject? segment = m_segments[index];
                if (Physics.Raycast(vector3 + Vector3.up * 500f, Vector3.down, out RaycastHit hitInfo, 1000f, m_mask.value))
                    vector3.y = hitInfo.point.y;
                if (segment != null) segment.transform.position = vector3;
            }

            for (int index = 0; index < m_segments.Count; ++index)
            {
                GameObject? segment = m_segments[index];
                GameObject? mSegment = index == 0 ? m_segments[m_segments.Count - 1] : m_segments[index - 1];
                if (mSegment != null)
                {
                    Vector3 normalized =
                        (((index == m_segments.Count - 1 ? m_segments[0] : m_segments[index + 1])!).transform.position -
                         mSegment.transform.position).normalized;
                    if (segment != null) segment.transform.rotation = Quaternion.LookRotation(normalized, Vector3.up);
                }
            }
        }

        public void CreateSegments()
        {
            if (m_segments.Count == m_nrOfSegments)
                return;
            foreach (GameObject? segment in m_segments)
                Destroy(segment);
            m_segments.Clear();
            for (int index = 0; index < m_nrOfSegments; ++index)
                m_segments.Add(Instantiate(m_prefab, transform.position, Quaternion.identity, transform));
        }
    }
}