using System.Collections;
using Saury.Bootstrap;
using UnityEngine;
using UnityEngine.UI;

namespace Saury.Baby
{
    /// <summary>Kitschy bowling-alley baby pomp — wood lane, gutters, return, chase lights, cradle roll-out.</summary>
    public class BabyShowcaseDirector : MonoBehaviour
    {
        public Transform laneRoot;
        public Transform cradle;
        public Light[] chaseLights;
        public Text titleText;
        public Text roastText;
        public Text promptText;

        GameObject _babyMesh;
        Light _heroSpot;

        public void EnsureLane(Transform parent)
        {
            if (laneRoot != null) return;
            laneRoot = new GameObject("BowlingBabyLane").transform;
            laneRoot.SetParent(parent, false);
            laneRoot.localPosition = new Vector3(0f, 0f, 7.0f);

            // Oiled wood lane
            MeshKit.Part(laneRoot, "Lane", PrimitiveType.Cube,
                new Vector3(0f, 0.06f, 0.2f), new Vector3(1.7f, 0.1f, 9.5f), SauryPalette.LaneWood, collider: true, smoothness: 0.55f);
            MeshKit.Part(laneRoot, "LaneOil", PrimitiveType.Cube,
                new Vector3(0f, 0.12f, 0.2f), new Vector3(1.5f, 0.02f, 8.5f), SauryPalette.LaneOil, smoothness: 0.75f);

            // Arrow chevrons
            for (int i = 0; i < 5; i++)
            {
                MeshKit.Part(laneRoot, $"Arrow_{i}", PrimitiveType.Cube,
                    new Vector3(0f, 0.13f, -2.5f + i * 1.1f), new Vector3(0.35f - i * 0.04f, 0.02f, 0.25f),
                    new Color(0.15f, 0.08f, 0.05f), smoothness: 0.2f);
            }

            MeshKit.Part(laneRoot, "GutterL", PrimitiveType.Cube,
                new Vector3(-1.1f, 0.0f, 0.2f), new Vector3(0.4f, 0.18f, 9.5f), SauryPalette.DirtyPlastic, smoothness: 0.25f);
            MeshKit.Part(laneRoot, "GutterR", PrimitiveType.Cube,
                new Vector3(1.1f, 0.0f, 0.2f), new Vector3(0.4f, 0.18f, 9.5f), SauryPalette.DirtyPlastic, smoothness: 0.25f);

            // Ball return / curtain hood
            MeshKit.Part(laneRoot, "ReturnHood", PrimitiveType.Cube,
                new Vector3(0f, 1.2f, -4.4f), new Vector3(2.6f, 2.2f, 1.0f), SauryPalette.StageBlack, smoothness: 0.08f);
            MeshKit.Part(laneRoot, "ReturnChrome", PrimitiveType.Cube,
                new Vector3(0f, 0.55f, -3.85f), new Vector3(1.6f, 0.12f, 0.15f), SauryPalette.Metal, metallic: 0.7f, smoothness: 0.6f);
            MeshKit.GlowPart(laneRoot, "ReturnMouth", PrimitiveType.Cube,
                new Vector3(0f, 0.7f, -3.75f), new Vector3(1.35f, 0.95f, 0.08f),
                new Color(0.08f, 0.1f, 0.12f), SauryPalette.NeonCyan, 2.2f);

            // Showcase plinth / pin deck
            MeshKit.Part(laneRoot, "Plinth", PrimitiveType.Cylinder,
                new Vector3(0f, 0.28f, 2.8f), new Vector3(1.35f, 0.28f, 1.35f), SauryPalette.DirtyWood, collider: true, smoothness: 0.3f);
            MeshKit.GlowPart(laneRoot, "PlinthRing", PrimitiveType.Cylinder,
                new Vector3(0f, 0.56f, 2.8f), new Vector3(1.2f, 0.03f, 1.2f),
                SauryPalette.Metal, SauryPalette.NeonAmber, 1.5f);

            // Cradle elevator
            cradle = new GameObject("CradleElevator").transform;
            cradle.SetParent(laneRoot, false);
            cradle.localPosition = new Vector3(0f, -1.3f, -3.6f);
            MeshKit.Part(cradle, "Basket", PrimitiveType.Cube,
                new Vector3(0f, 0f, 0f), new Vector3(0.95f, 0.18f, 0.75f), SauryPalette.DirtyPlastic, smoothness: 0.3f);
            MeshKit.Part(cradle, "RailL", PrimitiveType.Capsule,
                new Vector3(-0.42f, 0.28f, 0f), new Vector3(0.06f, 0.28f, 0.06f), SauryPalette.Metal, metallic: 0.55f);
            MeshKit.Part(cradle, "RailR", PrimitiveType.Capsule,
                new Vector3(0.42f, 0.28f, 0f), new Vector3(0.06f, 0.28f, 0.06f), SauryPalette.Metal, metallic: 0.55f);
            MeshKit.Part(cradle, "Pillow", PrimitiveType.Sphere,
                new Vector3(0f, 0.12f, 0f), new Vector3(0.7f, 0.15f, 0.5f), SauryPalette.BruisedPurple, smoothness: 0.45f);

            // Chase light bar fixtures
            chaseLights = new Light[8];
            for (int i = 0; i < chaseLights.Length; i++)
            {
                float x = -1.15f + (i % 4) * 0.75f;
                float z = -2.4f + (i / 4) * 4.2f;
                Color col = Color.HSVToRGB((i / 8f + 0.9f) % 1f, 0.8f, 1f);

                var fixture = new GameObject($"ChaseFixture_{i}");
                fixture.transform.SetParent(laneRoot, false);
                fixture.transform.localPosition = new Vector3(x, 2.45f, z);
                fixture.transform.localRotation = Quaternion.Euler(80f, 0f, 0f);

                MeshKit.Part(fixture.transform, "Housing", PrimitiveType.Cube,
                    Vector3.zero, new Vector3(0.32f, 0.18f, 0.4f), SauryPalette.Metal, metallic: 0.65f);
                MeshKit.GlowPart(fixture.transform, "Bulb", PrimitiveType.Sphere,
                    new Vector3(0f, -0.12f, 0f), Vector3.one * 0.2f, col, col, 5f);

                var light = fixture.AddComponent<Light>();
                light.type = LightType.Spot;
                light.range = 10f;
                light.spotAngle = 40f;
                light.intensity = 0f;
                light.color = col;
                chaseLights[i] = light;
            }

            // Hero spot for reveal hold
            var hero = new GameObject("BabyHeroSpot");
            hero.transform.SetParent(laneRoot, false);
            hero.transform.localPosition = new Vector3(0.5f, 3.2f, 1.0f);
            hero.transform.LookAt(laneRoot.TransformPoint(new Vector3(0f, 1.2f, 2.8f)));
            _heroSpot = hero.AddComponent<Light>();
            _heroSpot.type = LightType.Spot;
            _heroSpot.color = new Color(1f, 0.9f, 0.75f);
            _heroSpot.intensity = 0f;
            _heroSpot.range = 12f;
            _heroSpot.spotAngle = 35f;
            _heroSpot.shadows = LightShadows.Soft;
        }

        public IEnumerator PlayReveal(GeneratedBaby baby, float hold = 5.5f)
        {
            if (titleText != null) titleText.text = "GENERATING CUSTODY…";
            if (roastText != null) roastText.text = "DNA MACHINE whirrrrs…";
            if (promptText != null) promptText.text = baby.promptUsed;

            // Dip house-ish: chase only
            if (_heroSpot != null) _heroSpot.intensity = 0f;
            yield return StartCoroutine(ChaseLights(1.35f));

            if (_babyMesh != null) Destroy(_babyMesh);
            _babyMesh = CharacterKit.BuildBaby(cradle, baby.primary, baby.accent, baby.earScale, baby.headSquash);
            _babyMesh.transform.localPosition = new Vector3(0f, 0.5f, 0f);
            cradle.localPosition = new Vector3(0f, -1.3f, -3.6f);

            if (titleText != null) titleText.text = baby.title;

            float t = 0f;
            while (t < 1.85f)
            {
                t += Time.deltaTime;
                float u = Mathf.SmoothStep(0f, 1f, t / 1.85f);
                cradle.localPosition = Vector3.Lerp(new Vector3(0f, -1.3f, -3.6f), new Vector3(0f, 0.95f, 2.6f), u);
                cradle.localRotation = Quaternion.Euler(0f, u * 380f, Mathf.Sin(u * Mathf.PI) * 12f);
                if (_heroSpot != null) _heroSpot.intensity = Mathf.Lerp(0f, 6.5f, u);
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
                if (l != null) l.intensity = 0.6f;
        }

        IEnumerator ChaseLights(float dur)
        {
            float t = 0f;
            while (t < dur)
            {
                t += Time.deltaTime;
                int on = Mathf.FloorToInt(Time.time * 12f) % chaseLights.Length;
                for (int i = 0; i < chaseLights.Length; i++)
                {
                    if (chaseLights[i] == null) continue;
                    chaseLights[i].intensity = i == on || i == (on + 4) % chaseLights.Length ? 8.5f : 0.15f;
                }
                yield return null;
            }
        }

        void AnimatronicIdle()
        {
            if (_babyMesh == null) return;
            float wiggle = Mathf.Sin(Time.time * 5.5f) * 12f;
            _babyMesh.transform.localRotation = Quaternion.Euler(wiggle * 0.3f, wiggle, 0f);
            float wave = (Mathf.Sin(Time.time * 3.4f) + 1f) * 0.5f;
            _babyMesh.transform.localScale = Vector3.one * (0.96f + wave * 0.07f);
            var arm = _babyMesh.transform.Find("ArmR");
            if (arm != null)
                arm.localRotation = Quaternion.Euler(0f, 0f, -50f + Mathf.Sin(Time.time * 6.5f) * 40f);
        }
    }
}
