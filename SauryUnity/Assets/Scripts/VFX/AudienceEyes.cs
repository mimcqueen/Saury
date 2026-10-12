using Saury.Bootstrap;
using UnityEngine;

namespace Saury.VFX
{
    /// <summary>3D eye pairs in the dark audience — not billboard stickers.</summary>
    public class AudienceEyes : MonoBehaviour
    {
        Transform[] _pairs;

        public void Build(Transform parent, int pairs = 16)
        {
            _pairs = new Transform[pairs];
            for (int i = 0; i < pairs; i++)
            {
                int row = i / 6;
                int col = i % 6;
                float x = -3.8f + col * 1.45f + Random.Range(-0.12f, 0.12f);
                float y = 0.85f + row * 0.65f + Random.Range(-0.08f, 0.08f);
                float z = -5.6f - row * 0.55f + Random.Range(-0.1f, 0.1f);

                var pair = new GameObject($"EyePair_{i}").transform;
                pair.SetParent(parent, false);
                pair.localPosition = new Vector3(x, y, z);

                // Soft face shadow mass behind eyes
                MeshKit.Part(pair, "SkullHint", PrimitiveType.Sphere,
                    new Vector3(0f, 0f, -0.08f), new Vector3(0.55f, 0.28f, 0.2f),
                    new Color(0.04f, 0.03f, 0.04f));

                for (int e = 0; e < 2; e++)
                {
                    float side = e == 0 ? -0.14f : 0.14f;
                    var white = MeshKit.Part(pair, $"White_{e}", PrimitiveType.Sphere,
                        new Vector3(side, 0f, 0.05f), new Vector3(0.16f, 0.1f, 0.08f),
                        SauryPalette.EyeWhite, smoothness: 0.6f);
                    MeshKit.Part(white.transform, "Iris", PrimitiveType.Sphere,
                        new Vector3(0f, 0f, 0.35f), Vector3.one * 0.45f, SauryPalette.EyeIris);
                    // subtle emissive catchlight via small glow
                    MeshKit.GlowPart(white.transform, "Catch", PrimitiveType.Sphere,
                        new Vector3(0.15f, 0.15f, 0.5f), Vector3.one * 0.18f,
                        Color.white, new Color(1f, 0.95f, 0.7f), 1.2f);
                }

                _pairs[i] = pair;
            }
        }

        void Update()
        {
            if (_pairs == null) return;
            foreach (var p in _pairs)
            {
                if (p == null || p.childCount < 2) continue;
                // Blink by squashing the pair on Y
                float n = Mathf.PerlinNoise(p.GetInstanceID() * 0.01f, Time.time * 0.65f);
                float sy = n > 0.84f ? 0.15f : 1f;
                p.localScale = new Vector3(1f, sy, 1f);
            }
        }
    }
}
