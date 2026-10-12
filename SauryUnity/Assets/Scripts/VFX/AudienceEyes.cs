using Saury.Bootstrap;
using UnityEngine;

namespace Saury.VFX
{
    /// <summary>Eyes-only audience in the dark.</summary>
    public class AudienceEyes : MonoBehaviour
    {
        Transform[] _eyes;

        public void Build(Transform parent, int pairs = 18)
        {
            _eyes = new Transform[pairs];
            for (int i = 0; i < pairs; i++)
            {
                var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                go.name = $"EyePair_{i}";
                Object.Destroy(go.GetComponent<Collider>());
                go.transform.SetParent(parent, false);
                float x = -3.5f + (i % 6) * 1.2f + Random.Range(-0.15f, 0.15f);
                float y = 0.6f + (i / 6) * 0.55f + Random.Range(-0.1f, 0.1f);
                float z = -5.2f - (i / 6) * 0.4f;
                go.transform.localPosition = new Vector3(x, y, z);
                go.transform.localScale = new Vector3(0.35f, 0.12f, 0.08f);
                PrimitiveMaterials.Apply(go, new Color(0.95f, 0.92f, 0.55f));
                _eyes[i] = go.transform;
            }
        }

        void Update()
        {
            if (_eyes == null) return;
            foreach (var e in _eyes)
            {
                if (e == null) continue;
                float blink = Mathf.PerlinNoise(e.GetInstanceID() * 0.01f, Time.time * 0.7f);
                float sy = blink > 0.82f ? 0.02f : 0.12f;
                e.localScale = new Vector3(0.35f, sy, 0.08f);
            }
        }
    }
}
