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

            // Yard
            MeshKit.Part(_arena, "Yard", PrimitiveType.Cube,
                new Vector3(0f, -0.05f, 0f), new Vector3(16f, 0.1f, 10f),
                new Color(0.22f, 0.36f, 0.16f), collider: true, smoothness: 0.05f);
            MeshKit.Part(_arena, "Fence", PrimitiveType.Cube,
                new Vector3(0f, 0.7f, 4.6f), new Vector3(16f, 1.4f, 0.15f), SauryPalette.DirtyWood, collider: true);
            MeshKit.Part(_arena, "TrailerHint", PrimitiveType.Cube,
                new Vector3(-6f, 1.0f, -3.5f), new Vector3(3.5f, 2.0f, 2.2f), SauryPalette.DirtyPlastic);

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
