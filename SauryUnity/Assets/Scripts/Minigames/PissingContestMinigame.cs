using System.Collections;
using System.Collections.Generic;
using Saury.Bootstrap;
using Saury.Dad;
using UnityEngine;
using UnityEngine.UI;

namespace Saury.Minigames
{
    /// <summary>
    /// Cartoon yellow arcs / garden-hose proxy into cans. Highest score when time's up.
    /// </summary>
    public class PissingContestMinigame : MonoBehaviour
    {
        public float duration = 45f;
        public System.Action<int /*winnerSlot*/, int[] /*places by slot order*/> onFinished;

        readonly List<DadAvatarMotor> _players = new List<DadAvatarMotor>();
        readonly Dictionary<int, float> _scores = new Dictionary<int, float>();
        Transform _arena;
        Text _hud;
        bool _running;

        public void Build(Transform parent, IReadOnlyList<DadSeed> dads, Text hud)
        {
            _hud = hud;
            _arena = new GameObject("PissingContestArena").transform;
            _arena.SetParent(parent, false);
            _arena.localPosition = new Vector3(20f, 0f, 0f);

            // Ground strip
            var ground = GameObject.CreatePrimitive(PrimitiveType.Cube);
            ground.name = "Yard";
            ground.transform.SetParent(_arena, false);
            ground.transform.localPosition = new Vector3(0f, -0.05f, 0f);
            ground.transform.localScale = new Vector3(14f, 0.1f, 8f);
            PrimitiveMaterials.Apply(ground, new Color(0.28f, 0.42f, 0.22f));

            // Cans / plant targets
            for (int i = 0; i < 5; i++)
            {
                var can = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                can.name = $"Can_{i}";
                can.transform.SetParent(_arena, false);
                can.transform.localPosition = new Vector3(-2f + i * 1.5f, 0.35f, 2.5f);
                can.transform.localScale = new Vector3(0.55f, 0.35f, 0.55f);
                PrimitiveMaterials.Apply(can, new Color(0.55f, 0.55f, 0.58f));
                can.AddComponent<TargetCan>().value = 1f + i * 0.35f;
            }

            var plant = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            plant.name = "HostsPlant";
            plant.transform.SetParent(_arena, false);
            plant.transform.localPosition = new Vector3(0f, 0.6f, 3.4f);
            plant.transform.localScale = new Vector3(0.8f, 1.1f, 0.8f);
            PrimitiveMaterials.Apply(plant, new Color(0.2f, 0.55f, 0.25f));
            plant.AddComponent<TargetCan>().value = 3.5f;

            _players.Clear();
            _scores.Clear();
            for (int i = 0; i < dads.Count; i++)
            {
                var dad = dads[i];
                var body = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                body.name = $"Dad_{dad.displayName}";
                body.transform.SetParent(_arena, false);
                body.transform.localPosition = new Vector3(-3f + i * 1.8f, 1f, -2.2f);
                PrimitiveMaterials.Apply(body, dad.bodyColor);
                var motor = body.AddComponent<DadAvatarMotor>();
                motor.ConfigureKeysForSlot(dad.slotIndex);
                motor.displayName = dad.displayName;
                var hose = body.AddComponent<CartoonHose>();
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
            _running = true;
            float t = duration;
            foreach (var p in _players) p.inputEnabled = true;

            while (t > 0f)
            {
                t -= Time.deltaTime;
                if (_hud != null)
                {
                    _hud.text = $"PISSING CONTEST  {Mathf.CeilToInt(t)}s\n" + ScoreLine() +
                                "\n<size=18>Cartoon arcs into cans / Host's plant. Space/action = spray.</size>";
                }
                yield return null;
            }

            foreach (var p in _players) p.inputEnabled = false;
            _running = false;

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
            foreach (var kv in _scores)
                parts.Add($"P{kv.Key + 1}:{kv.Value:0}");
            return string.Join("  ", parts);
        }

        int Highest()
        {
            int best = 0;
            float v = -1f;
            foreach (var kv in _scores)
            {
                if (kv.Value > v) { v = kv.Value; best = kv.Key; }
            }
            return best;
        }

        int[] BuildPlaces()
        {
            var list = new List<KeyValuePair<int, float>>(_scores);
            list.Sort((a, b) => b.Value.CompareTo(a.Value));
            var places = new int[_scores.Count];
            for (int i = 0; i < list.Count; i++)
                places[list[i].Key] = i + 1;
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
            go.transform.position = transform.position + transform.forward * 0.4f + Vector3.up * 0.2f;
            go.transform.localScale = Vector3.one * 0.18f;
            PrimitiveMaterials.Apply(go, new Color(0.95f, 0.85f, 0.2f));
            var rb = go.AddComponent<Rigidbody>();
            rb.mass = 0.05f;
            rb.useGravity = true;
            // Arc toward +Z cans with slight aim wobble
            Vector3 dir = (Vector3.forward * 2.2f + Vector3.up * 2.8f + Random.insideUnitSphere * 0.35f);
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

        void OnTriggerEnter(Collider other) => TryHit(other.gameObject);
        void OnCollisionEnter(Collision c) => TryHit(c.gameObject);

        void TryHit(GameObject other)
        {
            if (_spent) return;
            var can = other.GetComponent<TargetCan>();
            if (can == null) return;
            _spent = true;
            onHit?.Invoke(ownerSlot, can.value);
            Destroy(gameObject);
        }

        void Awake()
        {
            var col = GetComponent<SphereCollider>();
            if (col == null) col = gameObject.AddComponent<SphereCollider>();
            col.isTrigger = true;
            col.radius = 0.6f;
        }
    }
}
