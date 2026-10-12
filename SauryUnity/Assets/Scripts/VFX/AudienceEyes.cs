using Saury.Bootstrap;
using UnityEngine;

namespace Saury.VFX
{
    /// <summary>Many creepy 3D eye pairs in the crushed-black audience — Liar's Bar intimacy.</summary>
    public class AudienceEyes : MonoBehaviour
    {
        Transform[] _pairs;

        public void Build(Transform parent, int pairs = 28)
        {
            _pairs = new Transform[pairs];
            for (int i = 0; i < pairs; i++)
            {
                int row = i / 7;
                int col = i % 7;
                float x = -4.5f + col * 1.35f + Random.Range(-0.18f, 0.18f);
                float y = 0.75f + row * 0.55f + Random.Range(-0.1f, 0.1f);
                float z = -5.5f - row * 0.5f + Random.Range(-0.15f, 0.15f);

                var pair = new GameObject($"EyePair_{i}").transform;
                pair.SetParent(parent, false);
                pair.localPosition = new Vector3(x, y, z);
                // Slight random yaw toward stage
                pair.localRotation = Quaternion.Euler(Random.Range(-8f, 8f), Random.Range(-25f, 25f), 0f);

                // Barely-there skull mass (almost invisible)
                MeshKit.Part(pair, "SkullHint", PrimitiveType.Sphere,
                    new Vector3(0f, 0f, -0.1f), new Vector3(0.5f, 0.26f, 0.18f),
                    new Color(0.03f, 0.025f, 0.03f), smoothness: 0.05f);

                for (int e = 0; e < 2; e++)
                {
                    float side = e == 0 ? -0.13f : 0.13f;
                    var white = MeshKit.Part(pair, $"White_{e}", PrimitiveType.Sphere,
                        new Vector3(side, 0f, 0.04f), new Vector3(0.15f, 0.1f, 0.08f),
                        SauryPalette.EyeWhite, smoothness: 0.7f);
                    MeshKit.Part(white.transform, "Iris", PrimitiveType.Sphere,
                        new Vector3(0f, 0f, 0.32f), Vector3.one * 0.42f, SauryPalette.EyeIris, smoothness: 0.4f);
                    MeshKit.GlowPart(white.transform, "Catch", PrimitiveType.Sphere,
                        new Vector3(0.12f, 0.1f, 0.48f), Vector3.one * 0.16f,
                        Color.white, new Color(1f, 0.92f, 0.65f), 2.2f);
                }

                _pairs[i] = pair;
            }
        }

        void Update()
        {
            if (_pairs == null) return;
            foreach (var p in _pairs)
            {
                if (p == null) continue;
                float n = Mathf.PerlinNoise(p.GetInstanceID() * 0.013f, Time.time * 0.55f);
                float sy = n > 0.86f ? 0.12f : 1f;
                // Occasional slow look drift
                float yaw = Mathf.Sin(Time.time * 0.4f + p.GetInstanceID()) * 6f;
                p.localScale = new Vector3(1f, sy, 1f);
                var e = p.localEulerAngles;
                p.localRotation = Quaternion.Euler(e.x, yaw, e.z);
            }
        }
    }
}
