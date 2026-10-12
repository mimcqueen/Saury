using System.Collections;
using System.Collections.Generic;
using Saury.Bootstrap;
using Saury.Dad;
using UnityEngine;
using UnityEngine.UI;

namespace Saury.Minigames
{
    /// <summary>3D yard arena — cartoon arcs into cans / Host plant. Highest score wins.</summary>
    public class PissingContestMinigame : MonoBehaviour
    {
        public float duration = 45f;
        public System.Action<int, int[]> onFinished;

        readonly List<DadAvatarMotor> _players = new List<DadAvatarMotor>();
        readonly Dictionary<int, float> _scores = new Dictionary<int, float>();
        Transform _arena;
        Text _hud;

        public void Build(Transform parent, IReadOnlyList<DadSeed> dads, Text hud)
        {
            _hud = hud;
            _arena = new GameObject("PissingContestArena").transform;
            _arena.SetParent(parent, false);
            _arena.localPosition = new Vector3(22f, 0f, 0f);

            // Deadbeat backyard
            MeshKit.Part(_arena, "Yard", PrimitiveType.Cube,
                new Vector3(0f, -0.05f, 0f), new Vector3(16f, 0.1f, 10f),
                SauryPalette.GrassDead, collider: true, smoothness: 0.05f);
            // Chainlink-ish fence posts + rails
            for (int i = 0; i < 9; i++)
            {
                MeshKit.Part(_arena, $"FencePost_{i}", PrimitiveType.Cylinder,
                    new Vector3(-7f + i * 1.75f, 0.75f, 4.6f), new Vector3(0.08f, 0.75f, 0.08f),
                    SauryPalette.Chainlink, metallic: 0.55f);
            }
            MeshKit.Part(_arena, "FenceRail", PrimitiveType.Cube,
                new Vector3(0f, 1.2f, 4.6f), new Vector3(15f, 0.06f, 0.06f), SauryPalette.Chainlink, metallic: 0.5f);
            MeshKit.Part(_arena, "FenceRailLo", PrimitiveType.Cube,
                new Vector3(0f, 0.35f, 4.6f), new Vector3(15f, 0.06f, 0.06f), SauryPalette.Chainlink, metallic: 0.5f);
            // Trailer
            MeshKit.Part(_arena, "Trailer", PrimitiveType.Cube,
                new Vector3(-6.2f, 1.1f, -3.6f), new Vector3(4.2f, 2.1f, 2.4f), SauryPalette.DirtyPlastic, smoothness: 0.2f);
            MeshKit.Part(_arena, "TrailerStripe", PrimitiveType.Cube,
                new Vector3(-6.2f, 1.5f, -2.35f), new Vector3(4.0f, 0.25f, 0.05f), SauryPalette.BruisedPurple, smoothness: 0.3f);
            MeshKit.GlowPart(_arena, "TrailerWindow", PrimitiveType.Cube,
                new Vector3(-5.2f, 1.6f, -2.35f), new Vector3(0.8f, 0.55f, 0.04f),
                new Color(0.15f, 0.2f, 0.25f), SauryPalette.NeonAmber, 0.8f);
            // Lawn junk
            MeshKit.Part(_arena, "KiddiePool", PrimitiveType.Cylinder,
                new Vector3(5.5f, 0.15f, -2.5f), new Vector3(1.8f, 0.15f, 1.8f), SauryPalette.NeonCyan, smoothness: 0.4f);
            MeshKit.Part(_arena, "Tire", PrimitiveType.Cylinder,
                new Vector3(4.2f, 0.25f, 1.5f), new Vector3(0.7f, 0.2f, 0.7f), SauryPalette.StageBlack, smoothness: 0.15f);
            // Work light on yard
            var yardSpot = new GameObject("YardSpot");
            yardSpot.transform.SetParent(_arena, false);
            yardSpot.transform.localPosition = new Vector3(0f, 4.5f, -1f);
            yardSpot.transform.LookAt(_arena.TransformPoint(new Vector3(0f, 0.5f, 1f)));
            var yl = yardSpot.AddComponent<Light>();
            yl.type = LightType.Spot; yl.intensity = 4.5f; yl.range = 18f; yl.spotAngle = 55f;
            yl.color = new Color(1f, 0.92f, 0.7f); yl.shadows = LightShadows.Soft;

            // Cans
            for (int i = 0; i < 5; i++)
            {
                var canRoot = new GameObject($"Can_{i}").transform;
                canRoot.SetParent(_arena, false);
                canRoot.localPosition = new Vector3(-3f + i * 1.5f, 0f, 2.6f);
                var can = MeshKit.Part(canRoot, "Body", PrimitiveType.Cylinder,
                    new Vector3(0f, 0.35f, 0f), new Vector3(0.5f, 0.35f, 0.5f),
                    new Color(0.55f, 0.55f, 0.58f), collider: true, metallic: 0.45f);
                MeshKit.Part(canRoot, "Rim", PrimitiveType.Cylinder,
                    new Vector3(0f, 0.72f, 0f), new Vector3(0.52f, 0.04f, 0.52f), SauryPalette.Metal, metallic: 0.6f);
                can.AddComponent<TargetCan>().value = 1f + i * 0.35f;
            }

            // Host's plant
            var plant = new GameObject("HostsPlant").transform;
            plant.SetParent(_arena, false);
            plant.localPosition = new Vector3(0f, 0f, 3.6f);
            MeshKit.Part(plant, "Pot", PrimitiveType.Cylinder,
                new Vector3(0f, 0.25f, 0f), new Vector3(0.55f, 0.25f, 0.55f),
                new Color(0.45f, 0.25f, 0.18f), collider: true);
            var foliage = MeshKit.Part(plant, "Foliage", PrimitiveType.Sphere,
                new Vector3(0f, 0.85f, 0f), new Vector3(0.9f, 1.1f, 0.9f),
                new Color(0.2f, 0.55f, 0.25f), collider: true);
            foliage.AddComponent<TargetCan>().value = 3.5f;

            _players.Clear();
            _scores.Clear();
            for (int i = 0; i < dads.Count; i++)
            {
                var dad = dads[i];
                var build = CharacterKit.BuildDad(_arena, new Vector3(-3.2f + i * 2.0f, 0f, -2.4f),
                    dad.creature, dad.bodyColor, $"Dad_{dad.displayName}");
                var rb = build.root.AddComponent<Rigidbody>();
                rb.mass = 2.2f;
                rb.linearDamping = 1.4f;
                rb.angularDamping = 2.5f;
                var motor = build.root.AddComponent<DadAvatarMotor>();
                motor.ConfigureKeysForSlot(dad.slotIndex);
                motor.displayName = dad.displayName;
                var hose = build.root.AddComponent<CartoonHose>();
                hose.ownerSlot = dad.slotIndex;
                hose.onHit = (slot, pts) =>
                {
                    if (!_scores.ContainsKey(slot)) _scores[slot] = 0f;
                    _scores[slot] += pts;
                };
                _players.Add(motor);
                _scores[dad.slotIndex] = 0f;
            }
        }

        public IEnumerator Run()
        {
            float t = duration;
            foreach (var p in _players) p.inputEnabled = true;
            while (t > 0f)
            {
                t -= Time.deltaTime;
                if (_hud != null)
                    _hud.text = $"PISSING CONTEST  {Mathf.CeilToInt(t)}s\n{ScoreLine()}\nSpace/action = spray · cans + Host's plant";
                yield return null;
            }
            foreach (var p in _players) p.inputEnabled = false;
            int winner = Highest();
            onFinished?.Invoke(winner, BuildPlaces());
        }

        public void Teardown()
        {
            if (_arena != null) Destroy(_arena.gameObject);
            _players.Clear();
        }

        string ScoreLine()
        {
            var parts = new List<string>();
            foreach (var kv in _scores) parts.Add($"P{kv.Key + 1}:{kv.Value:0}");
            return string.Join("  ", parts);
        }

        int Highest()
        {
            int best = 0; float v = -1f;
            foreach (var kv in _scores) if (kv.Value > v) { v = kv.Value; best = kv.Key; }
            return best;
        }

        int[] BuildPlaces()
        {
            var list = new List<KeyValuePair<int, float>>(_scores);
            list.Sort((a, b) => b.Value.CompareTo(a.Value));
            var places = new int[_scores.Count];
            for (int i = 0; i < list.Count; i++) places[list[i].Key] = i + 1;
            return places;
        }
    }

    public class TargetCan : MonoBehaviour
    {
        public float value = 1f;
    }

    public class CartoonHose : MonoBehaviour
    {
        public int ownerSlot;
        public System.Action<int, float> onHit;
        float _cooldown;

        void Update()
        {
            _cooldown -= Time.deltaTime;
            var motor = GetComponent<DadAvatarMotor>();
            if (motor == null || !motor.inputEnabled) return;
            if (!Input.GetKey(motor.action) || _cooldown > 0f) return;
            _cooldown = 0.12f;
            Spray();
        }

        void Spray()
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.name = "PeeArc";
            go.transform.position = transform.position + Vector3.up * 1.0f + transform.forward * 0.5f;
            go.transform.localScale = Vector3.one * 0.16f;
            PrimitiveMaterials.Apply(go, SauryPalette.PeeYellow, 0.4f);
            var rb = go.AddComponent<Rigidbody>();
            rb.mass = 0.05f;
            Vector3 dir = Vector3.forward * 2.4f + Vector3.up * 2.9f + Random.insideUnitSphere * 0.35f;
            rb.AddForce(dir, ForceMode.VelocityChange);
            var arc = go.AddComponent<PeeArc>();
            arc.ownerSlot = ownerSlot;
            arc.onHit = onHit;
            Object.Destroy(go, 2.5f);
        }
    }

    public class PeeArc : MonoBehaviour
    {
        public int ownerSlot;
        public System.Action<int, float> onHit;
        bool _spent;

        void Awake()
        {
            var col = GetComponent<SphereCollider>();
            if (col == null) col = gameObject.AddComponent<SphereCollider>();
            col.isTrigger = true;
            col.radius = 0.7f;
        }

        void OnTriggerEnter(Collider other) => TryHit(other.gameObject);
        void OnCollisionEnter(Collision c) => TryHit(c.gameObject);

        void TryHit(GameObject other)
        {
            if (_spent) return;
            var can = other.GetComponent<TargetCan>() ?? other.GetComponentInParent<TargetCan>();
            if (can == null) return;
            _spent = true;
            onHit?.Invoke(ownerSlot, can.value);
            Destroy(gameObject);
        }
    }
}
