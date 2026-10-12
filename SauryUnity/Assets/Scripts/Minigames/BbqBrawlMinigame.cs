using System.Collections;
using System.Collections.Generic;
using Saury.Bootstrap;
using Saury.Dad;
using UnityEngine;
using UnityEngine.UI;

namespace Saury.Minigames
{
    /// <summary>
    /// Soft ragdoll food-fight around a flaming grill. Last dad near the grill / most control wins.
    /// </summary>
    public class BbqBrawlMinigame : MonoBehaviour
    {
        public float duration = 50f;
        public System.Action<int, int[]> onFinished;

        readonly List<DadAvatarMotor> _players = new List<DadAvatarMotor>();
        readonly Dictionary<int, float> _control = new Dictionary<int, float>();
        Transform _arena;
        Text _hud;
        Transform _grill;

        public void Build(Transform parent, IReadOnlyList<DadSeed> dads, Text hud)
        {
            _hud = hud;
            _arena = new GameObject("BBQBrawlArena").transform;
            _arena.SetParent(parent, false);
            _arena.localPosition = new Vector3(-20f, 0f, 0f);

            var ground = GameObject.CreatePrimitive(PrimitiveType.Cube);
            ground.transform.SetParent(_arena, false);
            ground.transform.localPosition = new Vector3(0f, -0.05f, 0f);
            ground.transform.localScale = new Vector3(12f, 0.1f, 12f);
            PrimitiveMaterials.Apply(ground, new Color(0.35f, 0.28f, 0.18f));

            _grill = GameObject.CreatePrimitive(PrimitiveType.Cylinder).transform;
            _grill.name = "Grill";
            _grill.SetParent(_arena, false);
            _grill.localPosition = new Vector3(0f, 0.45f, 0f);
            _grill.localScale = new Vector3(1.6f, 0.45f, 1.6f);
            PrimitiveMaterials.Apply(_grill.gameObject, new Color(0.15f, 0.15f, 0.16f));

            var flame = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            flame.name = "Flame";
            Object.Destroy(flame.GetComponent<Collider>());
            flame.transform.SetParent(_grill, false);
            flame.transform.localPosition = new Vector3(0f, 1.2f, 0f);
            flame.transform.localScale = new Vector3(0.7f, 1.1f, 0.7f);
            PrimitiveMaterials.Apply(flame, new Color(1f, 0.45f, 0.1f));
            flame.AddComponent<FlameWobble>();

            // Props: tongs / brats / folding chairs as physics toys
            SpawnProp("Chair", PrimitiveType.Cube, new Vector3(2.5f, 0.6f, -1f), new Vector3(1.2f, 1.1f, 0.15f), new Color(0.55f, 0.55f, 0.5f), 3f);
            SpawnProp("Brat", PrimitiveType.Capsule, new Vector3(-1.5f, 0.4f, 1.5f), new Vector3(0.25f, 0.45f, 0.25f), new Color(0.7f, 0.35f, 0.25f), 0.4f);
            SpawnProp("Tongs", PrimitiveType.Cube, new Vector3(1.2f, 0.3f, 2f), new Vector3(0.15f, 0.15f, 1.1f), new Color(0.6f, 0.6f, 0.65f), 0.8f);
            SpawnProp("Cooler", PrimitiveType.Cube, new Vector3(-3f, 0.5f, -2f), new Vector3(1.4f, 1f, 0.9f), new Color(0.2f, 0.35f, 0.7f), 6f);

            _players.Clear();
            _control.Clear();
            for (int i = 0; i < dads.Count; i++)
            {
                var dad = dads[i];
                var body = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                body.name = $"Brawler_{dad.displayName}";
                body.transform.SetParent(_arena, false);
                float ang = i * Mathf.PI * 2f / dads.Count;
                body.transform.localPosition = new Vector3(Mathf.Cos(ang) * 4f, 1.1f, Mathf.Sin(ang) * 4f);
                PrimitiveMaterials.Apply(body, dad.bodyColor);
                var motor = body.AddComponent<DadAvatarMotor>();
                motor.ConfigureKeysForSlot(dad.slotIndex);
                motor.displayName = dad.displayName;
                motor.moveForce = 32f;
                body.AddComponent<SoftGrab>().Configure(motor);
                _players.Add(motor);
                _control[dad.slotIndex] = 0f;
            }
        }

        void SpawnProp(string name, PrimitiveType type, Vector3 pos, Vector3 scale, Color color, float mass)
        {
            var go = GameObject.CreatePrimitive(type);
            go.name = name;
            go.transform.SetParent(_arena, false);
            go.transform.localPosition = pos;
            go.transform.localScale = scale;
            PrimitiveMaterials.Apply(go, color);
            var rb = go.AddComponent<Rigidbody>();
            rb.mass = mass;
            rb.linearDamping = 0.8f;
        }

        public IEnumerator Run()
        {
            float t = duration;
            foreach (var p in _players) p.inputEnabled = true;

            while (t > 0f)
            {
                t -= Time.deltaTime;
                // Grill control: accumulate time near grill while upright-ish
                foreach (var p in _players)
                {
                    if (p.eliminated) continue;
                    float dist = Vector3.Distance(p.transform.position, _grill.position);
                    if (dist < 2.2f && Vector3.Dot(p.transform.up, Vector3.up) > 0.35f)
                        _control[p.slot] += Time.deltaTime;
                }

                // Mild flame hazard — knock back if too close too long (comedy, not hard)
                foreach (var p in _players)
                {
                    float dist = Vector3.Distance(p.transform.position, _grill.position);
                    if (dist < 0.95f)
                    {
                        var rb = p.GetComponent<Rigidbody>();
                        if (rb != null)
                            rb.AddExplosionForce(6f, _grill.position, 2f, 0.4f, ForceMode.VelocityChange);
                    }
                }

                if (_hud != null)
                {
                    _hud.text = $"BACKYARD BBQ BRAWL  {Mathf.CeilToInt(t)}s\n" + ControlLine() +
                                "\n<size=18>Gang Beasts–lite: shove for the grill. Action = hop/bump.</size>";
                }
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

        string ControlLine()
        {
            var parts = new List<string>();
            foreach (var kv in _control)
                parts.Add($"P{kv.Key + 1}:{kv.Value:0.0}s");
            return string.Join("  ", parts);
        }

        int Highest()
        {
            int best = 0;
            float v = -1f;
            foreach (var kv in _control)
                if (kv.Value > v) { v = kv.Value; best = kv.Key; }
            return best;
        }

        int[] BuildPlaces()
        {
            var list = new List<KeyValuePair<int, float>>(_control);
            list.Sort((a, b) => b.Value.CompareTo(a.Value));
            var places = new int[_control.Count];
            for (int i = 0; i < list.Count; i++)
                places[list[i].Key] = i + 1;
            return places;
        }
    }

    public class FlameWobble : MonoBehaviour
    {
        Vector3 _base;
        void Start() => _base = transform.localScale;
        void Update()
        {
            float s = 1f + Mathf.Sin(Time.time * 9f) * 0.15f;
            transform.localScale = new Vector3(_base.x * s, _base.y * (2f - s), _base.z * s);
            transform.Rotate(0f, 40f * Time.deltaTime, 0f);
        }
    }

    public class SoftGrab : MonoBehaviour
    {
        DadAvatarMotor _motor;
        float _cd;

        public void Configure(DadAvatarMotor m) => _motor = m;

        void Update()
        {
            _cd -= Time.deltaTime;
            if (_motor == null || !_motor.inputEnabled) return;
            if (!Input.GetKeyDown(_motor.action) || _cd > 0f) return;
            _cd = 0.4f;
            // Impulse shove nearby rigidbodies — big grabs, low skill floor
            var cols = Physics.OverlapSphere(transform.position + transform.forward * 0.6f, 1.1f);
            foreach (var c in cols)
            {
                if (c.attachedRigidbody == null || c.attachedRigidbody.gameObject == gameObject) continue;
                c.attachedRigidbody.AddForce((c.transform.position - transform.position).normalized * 9f + Vector3.up * 3f, ForceMode.VelocityChange);
            }
        }
    }
}
