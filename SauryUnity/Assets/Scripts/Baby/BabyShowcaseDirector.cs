using System.Collections;
using Saury.Bootstrap;
using UnityEngine;
using UnityEngine.UI;

namespace Saury.Baby
{
    /// <summary>Bowling-alley theatrical baby reveal: lights, roll-out, animatronic idle.</summary>
    public class BabyShowcaseDirector : MonoBehaviour
    {
        public Transform laneRoot;
        public Transform cradle;
        public Light[] chaseLights;
        public Text titleText;
        public Text roastText;
        public Text promptText;

        GameObject _babyMesh;
        GeneratedBaby _current;

        public void EnsureLane(Transform parent)
        {
            if (laneRoot != null) return;
            laneRoot = new GameObject("BowlingBabyLane").transform;
            laneRoot.SetParent(parent, false);
            laneRoot.localPosition = new Vector3(0f, 0f, 6.5f);

            var gutter = GameObject.CreatePrimitive(PrimitiveType.Cube);
            gutter.name = "Lane";
            gutter.transform.SetParent(laneRoot, false);
            gutter.transform.localPosition = new Vector3(0f, 0.05f, 0f);
            gutter.transform.localScale = new Vector3(2.2f, 0.1f, 8f);
            PrimitiveMaterials.Apply(gutter, new Color(0.45f, 0.25f, 0.18f));

            cradle = new GameObject("CradleElevator").transform;
            cradle.SetParent(laneRoot, false);
            cradle.localPosition = new Vector3(0f, -1.5f, -3.2f);

            chaseLights = new Light[6];
            for (int i = 0; i < chaseLights.Length; i++)
            {
                var go = new GameObject($"ChaseLight_{i}");
                go.transform.SetParent(laneRoot, false);
                go.transform.localPosition = new Vector3(-1.2f + (i % 3) * 1.2f, 2.2f, -2f + (i / 3) * 3f);
                var light = go.AddComponent<Light>();
                light.type = LightType.Spot;
                light.range = 8f;
                light.spotAngle = 45f;
                light.intensity = 0f;
                light.color = Color.HSVToRGB(i / 6f, 0.7f, 1f);
                chaseLights[i] = light;
            }
        }

        public IEnumerator PlayReveal(GeneratedBaby baby, float hold = 5.5f)
        {
            _current = baby;
            if (titleText != null) titleText.text = "GENERATING CUSTODY…";
            if (roastText != null) roastText.text = "DNA MACHINE whirrrrs…";
            if (promptText != null) promptText.text = baby.promptUsed;

            // House dip + chase
            yield return StartCoroutine(ChaseLights(1.2f));

            SpawnOrUpdateMesh(baby);
            cradle.localPosition = new Vector3(0f, -1.5f, -3.2f);

            if (titleText != null) titleText.text = baby.title;

            // Roll-out / elevate
            float t = 0f;
            while (t < 1.6f)
            {
                t += Time.deltaTime;
                float u = Mathf.SmoothStep(0f, 1f, t / 1.6f);
                cradle.localPosition = Vector3.Lerp(new Vector3(0f, -1.5f, -3.2f), new Vector3(0f, 0.9f, 0.4f), u);
                cradle.localRotation = Quaternion.Euler(0f, u * 360f, Mathf.Sin(u * Mathf.PI) * 8f);
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
                if (l != null) l.intensity = 0.4f;
        }

        IEnumerator ChaseLights(float dur)
        {
            float t = 0f;
            while (t < dur)
            {
                t += Time.deltaTime;
                int on = Mathf.FloorToInt(Time.time * 10f) % chaseLights.Length;
                for (int i = 0; i < chaseLights.Length; i++)
                {
                    if (chaseLights[i] == null) continue;
                    chaseLights[i].intensity = i == on ? 6f : 0.2f;
                }
                yield return null;
            }
        }

        void SpawnOrUpdateMesh(GeneratedBaby baby)
        {
            if (_babyMesh != null) Destroy(_babyMesh);
            _babyMesh = new GameObject("MockBabyMesh");
            _babyMesh.transform.SetParent(cradle, false);
            _babyMesh.transform.localPosition = Vector3.zero;

            var head = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            head.transform.SetParent(_babyMesh.transform, false);
            head.transform.localScale = new Vector3(1f, baby.headSquash, 1f) * 0.7f;
            PrimitiveMaterials.Apply(head, baby.primary);
            Object.Destroy(head.GetComponent<Collider>());

            for (int i = 0; i < 2; i++)
            {
                var ear = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                ear.transform.SetParent(head.transform, false);
                ear.transform.localPosition = new Vector3(i == 0 ? -0.55f : 0.55f, 0.1f, 0f);
                ear.transform.localScale = Vector3.one * (0.35f * baby.earScale);
                PrimitiveMaterials.Apply(ear, baby.accent);
                Object.Destroy(ear.GetComponent<Collider>());
            }

            var body = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            body.transform.SetParent(_babyMesh.transform, false);
            body.transform.localPosition = new Vector3(0f, -0.55f, 0f);
            body.transform.localScale = new Vector3(0.55f, 0.35f, 0.45f);
            PrimitiveMaterials.Apply(body, Color.Lerp(baby.primary, Color.white, 0.25f));
            Object.Destroy(body.GetComponent<Collider>());

            // Sticker prop that changes per gen
            var hat = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            hat.transform.SetParent(head.transform, false);
            hat.transform.localPosition = new Vector3(0f, 0.45f, 0f);
            hat.transform.localScale = new Vector3(0.55f, 0.08f, 0.55f);
            PrimitiveMaterials.Apply(hat, baby.accent);
            Object.Destroy(hat.GetComponent<Collider>());
        }

        void AnimatronicIdle()
        {
            if (_babyMesh == null) return;
            float wiggle = Mathf.Sin(Time.time * 5f) * 8f;
            _babyMesh.transform.localRotation = Quaternion.Euler(wiggle * 0.3f, wiggle, 0f);
            float wave = (Mathf.Sin(Time.time * 3.5f) + 1f) * 0.5f;
            _babyMesh.transform.localScale = Vector3.one * (0.95f + wave * 0.08f);
        }
    }
}
