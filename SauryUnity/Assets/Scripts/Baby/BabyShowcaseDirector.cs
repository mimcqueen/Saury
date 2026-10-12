using System.Collections;
using Saury.Bootstrap;
using UnityEngine;
using UnityEngine.UI;

namespace Saury.Baby
{
    /// <summary>Bowling-alley theatrical baby reveal with real 3D lane, cradle, emissive fixtures.</summary>
    public class BabyShowcaseDirector : MonoBehaviour
    {
        public Transform laneRoot;
        public Transform cradle;
        public Light[] chaseLights;
        public Text titleText;
        public Text roastText;
        public Text promptText;

        GameObject _babyMesh;

        public void EnsureLane(Transform parent)
        {
            if (laneRoot != null) return;
            laneRoot = new GameObject("BowlingBabyLane").transform;
            laneRoot.SetParent(parent, false);
            laneRoot.localPosition = new Vector3(0f, 0f, 6.8f);

            // Lane bed + gutters
            MeshKit.Part(laneRoot, "Lane", PrimitiveType.Cube,
                new Vector3(0f, 0.05f, 0f), new Vector3(1.8f, 0.1f, 9f), SauryPalette.LaneWood, collider: true, smoothness: 0.4f);
            MeshKit.Part(laneRoot, "GutterL", PrimitiveType.Cube,
                new Vector3(-1.15f, 0.02f, 0f), new Vector3(0.35f, 0.12f, 9f), SauryPalette.DirtyPlastic);
            MeshKit.Part(laneRoot, "GutterR", PrimitiveType.Cube,
                new Vector3(1.15f, 0.02f, 0f), new Vector3(0.35f, 0.12f, 9f), SauryPalette.DirtyPlastic);

            // Return chute / curtain hood at back
            MeshKit.Part(laneRoot, "ReturnHood", PrimitiveType.Cube,
                new Vector3(0f, 1.1f, -4.2f), new Vector3(2.4f, 2.0f, 0.8f), SauryPalette.StageBlack);
            MeshKit.GlowPart(laneRoot, "ReturnMouth", PrimitiveType.Cube,
                new Vector3(0f, 0.55f, -3.7f), new Vector3(1.4f, 0.9f, 0.1f),
                new Color(0.1f, 0.1f, 0.12f), SauryPalette.NeonCyan, 1.5f);

            // Pin deck / showcase plinth
            MeshKit.Part(laneRoot, "Plinth", PrimitiveType.Cylinder,
                new Vector3(0f, 0.25f, 2.5f), new Vector3(1.2f, 0.25f, 1.2f), SauryPalette.DirtyWood, collider: true);

            // Cradle elevator (baby rides this)
            cradle = new GameObject("CradleElevator").transform;
            cradle.SetParent(laneRoot, false);
            cradle.localPosition = new Vector3(0f, -1.2f, -3.5f);
            MeshKit.Part(cradle, "Basket", PrimitiveType.Cube,
                new Vector3(0f, 0f, 0f), new Vector3(0.9f, 0.2f, 0.7f), SauryPalette.DirtyPlastic);
            MeshKit.Part(cradle, "RailL", PrimitiveType.Cube,
                new Vector3(-0.4f, 0.25f, 0f), new Vector3(0.06f, 0.4f, 0.65f), SauryPalette.Metal, metallic: 0.5f);
            MeshKit.Part(cradle, "RailR", PrimitiveType.Cube,
                new Vector3(0.4f, 0.25f, 0f), new Vector3(0.06f, 0.4f, 0.65f), SauryPalette.Metal, metallic: 0.5f);

            // Chase light fixtures — emissive meshes + real Lights
            chaseLights = new Light[6];
            for (int i = 0; i < chaseLights.Length; i++)
            {
                float x = -1.0f + (i % 3) * 1.0f;
                float z = -2.2f + (i / 3) * 3.5f;
                Color col = Color.HSVToRGB(i / 6f, 0.75f, 1f);

                var fixture = new GameObject($"ChaseFixture_{i}");
                fixture.transform.SetParent(laneRoot, false);
                fixture.transform.localPosition = new Vector3(x, 2.35f, z);

                MeshKit.Part(fixture.transform, "Housing", PrimitiveType.Cube,
                    Vector3.zero, new Vector3(0.35f, 0.2f, 0.45f), SauryPalette.Metal, metallic: 0.6f);
                MeshKit.GlowPart(fixture.transform, "Bulb", PrimitiveType.Sphere,
                    new Vector3(0f, -0.15f, 0f), Vector3.one * 0.22f, col, col, 4f);

                var light = fixture.AddComponent<Light>();
                light.type = LightType.Spot;
                light.range = 9f;
                light.spotAngle = 48f;
                light.intensity = 0f;
                light.color = col;
                fixture.transform.localRotation = Quaternion.Euler(75f, 0f, 0f);
                chaseLights[i] = light;
            }
        }

        public IEnumerator PlayReveal(GeneratedBaby baby, float hold = 5.5f)
        {
            if (titleText != null) titleText.text = "GENERATING CUSTODY…";
            if (roastText != null) roastText.text = "DNA MACHINE whirrrrs…";
            if (promptText != null) promptText.text = baby.promptUsed;

            yield return StartCoroutine(ChaseLights(1.2f));

            if (_babyMesh != null) Destroy(_babyMesh);
            _babyMesh = CharacterKit.BuildBaby(cradle, baby.primary, baby.accent, baby.earScale, baby.headSquash);
            _babyMesh.transform.localPosition = new Vector3(0f, 0.45f, 0f);
            cradle.localPosition = new Vector3(0f, -1.2f, -3.5f);

            if (titleText != null) titleText.text = baby.title;

            float t = 0f;
            while (t < 1.7f)
            {
                t += Time.deltaTime;
                float u = Mathf.SmoothStep(0f, 1f, t / 1.7f);
                cradle.localPosition = Vector3.Lerp(new Vector3(0f, -1.2f, -3.5f), new Vector3(0f, 0.85f, 2.3f), u);
                cradle.localRotation = Quaternion.Euler(0f, u * 360f, Mathf.Sin(u * Mathf.PI) * 10f);
                yield return null;
            }

            if (roastText != null) roastText.text = baby.roastLine + (baby.usedMockApi ? "  [MOCK GEN]" : "");

            float holdT = hold;
            while (holdT > 0f)
            {
                holdT -= Time.deltaTime;
                AnimatronicIdle();
                yield return null;
            }

            foreach (var l in chaseLights)
                if (l != null) l.intensity = 0.55f;
        }

        IEnumerator ChaseLights(float dur)
        {
            float t = 0f;
            while (t < dur)
            {
                t += Time.deltaTime;
                int on = Mathf.FloorToInt(Time.time * 11f) % chaseLights.Length;
                for (int i = 0; i < chaseLights.Length; i++)
                {
                    if (chaseLights[i] == null) continue;
                    chaseLights[i].intensity = i == on ? 7.5f : 0.25f;
                }
                yield return null;
            }
        }

        void AnimatronicIdle()
        {
            if (_babyMesh == null) return;
            float wiggle = Mathf.Sin(Time.time * 5.2f) * 10f;
            _babyMesh.transform.localRotation = Quaternion.Euler(wiggle * 0.25f, wiggle, 0f);
            float wave = (Mathf.Sin(Time.time * 3.2f) + 1f) * 0.5f;
            _babyMesh.transform.localScale = Vector3.one * (0.95f + wave * 0.08f);
            // Wave an arm if present
            var arm = _babyMesh.transform.Find("ArmR");
            if (arm != null)
                arm.localRotation = Quaternion.Euler(0f, 0f, -40f + Mathf.Sin(Time.time * 6f) * 35f);
        }
    }
}
