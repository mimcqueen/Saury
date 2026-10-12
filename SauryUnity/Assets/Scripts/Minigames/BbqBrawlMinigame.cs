using System.Collections;
using System.Collections.Generic;
using Saury.Bootstrap;
using Saury.Core;
using Saury.Dad;
using Saury.UI;
using UnityEngine;

namespace Saury.Minigames
{
    /// <summary>3D backyard brawl around a flaming grill — soft Gang Beasts–lite bodies.</summary>
    public class BbqBrawlMinigame : MonoBehaviour
    {
        public float duration = 45f;
        public System.Action<int, int[]> onFinished;

        readonly List<DadAvatarMotor> _players = new List<DadAvatarMotor>();
        readonly Dictionary<int, float> _control = new Dictionary<int, float>();
        Transform _arena;
        ShowHud _hud;
        Transform _grill;

        public Vector3 ArenaWorldCenter =>
            _arena != null ? _arena.position : Vector3.zero;

        public void Build(Transform parent, IReadOnlyList<DadSeed> dads, ShowHud hud)
        {
            _hud = hud;
            if (DemoSettings.demoMode) duration = 35f;

            _arena = new GameObject("BBQBrawlArena").transform;
            _arena.SetParent(parent, false);
            _arena.localPosition = new Vector3(-22f, 0f, 0f);

            MeshKit.Part(_arena, "Patio", PrimitiveType.Cube,
                new Vector3(0f, -0.05f, 0f), new Vector3(14f, 0.1f, 14f),
                new Color(0.3f, 0.24f, 0.16f), collider: true, smoothness: 0.08f);
            MeshKit.Part(_arena, "Stain", PrimitiveType.Cylinder,
                new Vector3(1.5f, 0.01f, -1f), new Vector3(2.2f, 0.02f, 1.4f), SauryPalette.FloorStain, smoothness: 0.05f);

            MeshKit.Part(_arena, "House", PrimitiveType.Cube,
                new Vector3(0f, 1.6f, 6.2f), new Vector3(11f, 3.2f, 0.45f),
                new Color(0.5f, 0.38f, 0.28f), collider: true, smoothness: 0.15f);
            MeshKit.Part(_arena, "Door", PrimitiveType.Cube,
                new Vector3(0f, 1.1f, 5.95f), new Vector3(1.1f, 2.1f, 0.1f), SauryPalette.DirtyWood, smoothness: 0.2f);
            MeshKit.GlowPart(_arena, "PorchLight", PrimitiveType.Sphere,
                new Vector3(0f, 2.5f, 5.85f), Vector3.one * 0.25f,
                SauryPalette.NeonAmber, SauryPalette.NeonAmber, 2.5f);
            MeshKit.Part(_arena, "BushL", PrimitiveType.Sphere,
                new Vector3(-3.5f, 0.6f, 5.5f), new Vector3(1.4f, 1.1f, 0.8f), SauryPalette.GrassDead, smoothness: 0.2f);
            MeshKit.Part(_arena, "BushR", PrimitiveType.Sphere,
                new Vector3(3.5f, 0.55f, 5.5f), new Vector3(1.3f, 1.0f, 0.8f), SauryPalette.GrassDead, smoothness: 0.2f);
            var patioSpot = new GameObject("PatioSpot");
            patioSpot.transform.SetParent(_arena, false);
            patioSpot.transform.localPosition = new Vector3(2f, 5.5f, -3.5f);
            patioSpot.transform.LookAt(_arena.TransformPoint(Vector3.zero));
            var pl = patioSpot.AddComponent<Light>();
            pl.type = LightType.Spot; pl.intensity = 5.0f; pl.range = 18f; pl.spotAngle = 55f;
            pl.color = new Color(1f, 0.85f, 0.65f); pl.shadows = LightShadows.Soft;

            _grill = new GameObject("Grill").transform;
            _grill.SetParent(_arena, false);
            _grill.localPosition = Vector3.zero;
            MeshKit.Part(_grill, "Body", PrimitiveType.Cylinder,
                new Vector3(0f, 0.45f, 0f), new Vector3(1.5f, 0.4f, 1.5f),
                SauryPalette.GrillBlack, collider: true, metallic: 0.4f);
            MeshKit.Part(_grill, "Lid", PrimitiveType.Sphere,
                new Vector3(0f, 0.9f, 0f), new Vector3(1.4f, 0.55f, 1.4f),
                SauryPalette.GrillBlack, metallic: 0.45f);
            var flame = MeshKit.GlowPart(_grill, "Flame", PrimitiveType.Sphere,
                new Vector3(0f, 1.15f, 0f), new Vector3(0.7f, 1.0f, 0.7f),
                SauryPalette.Flame, SauryPalette.Flame, 3.5f);
            flame.AddComponent<FlameWobble>();
            var fireLight = _grill.gameObject.AddComponent<Light>();
            fireLight.type = LightType.Point;
            fireLight.color = SauryPalette.Flame;
            fireLight.range = 7f;
            fireLight.intensity = 2.8f;

            SpawnProp("Chair", PrimitiveType.Cube, new Vector3(2.8f, 0.55f, -1.2f), new Vector3(1.1f, 1.0f, 0.12f), SauryPalette.DirtyPlastic, 3f);
            SpawnProp("Brat", PrimitiveType.Capsule, new Vector3(-1.6f, 0.35f, 1.6f), new Vector3(0.22f, 0.4f, 0.22f), new Color(0.7f, 0.35f, 0.25f), 0.4f);
            SpawnProp("Tongs", PrimitiveType.Cube, new Vector3(1.3f, 0.25f, 2.1f), new Vector3(0.12f, 0.12f, 1.1f), SauryPalette.Metal, 0.8f);
            SpawnProp("Cooler", PrimitiveType.Cube, new Vector3(-3.2f, 0.5f, -2.2f), new Vector3(1.4f, 1.0f, 0.9f), new Color(0.18f, 0.32f, 0.65f), 6f);
            SpawnProp("Ketchup", PrimitiveType.Cylinder, new Vector3(0.8f, 0.35f, -2f), new Vector3(0.2f, 0.3f, 0.2f), new Color(0.7f, 0.1f, 0.12f), 0.3f);

            _players.Clear();
            _control.Clear();
            for (int i = 0; i < dads.Count; i++)
            {
                var dad = dads[i];
                float ang = i * Mathf.PI * 2f / dads.Count;
                var build = CharacterKit.BuildDad(_arena,
                    new Vector3(Mathf.Cos(ang) * 4.2f, 0f, Mathf.Sin(ang) * 4.2f),
                    dad.creature, dad.bodyColor, $"Brawler_{dad.displayName}");
                var rb = build.root.AddComponent<Rigidbody>();
                rb.mass = 2.4f;
                rb.linearDamping = 1.2f;
                rb.angularDamping = 2.2f;
                var motor = build.root.AddComponent<DadAvatarMotor>();
                motor.ConfigureKeysForSlot(dad.slotIndex);
                motor.displayName = dad.displayName;
                motor.moveForce = 32f;
                motor.aiTarget = _grill;
                build.root.AddComponent<SoftGrab>().Configure(motor);
                _players.Add(motor);
                _control[dad.slotIndex] = 0f;
            }
        }

        void SpawnProp(string name, PrimitiveType type, Vector3 pos, Vector3 scale, Color color, float mass)
        {
            var go = MeshKit.Part(_arena, name, type, pos, scale, color, collider: true, metallic: name == "Tongs" ? 0.6f : 0.05f);
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
                if (Input.GetKeyDown(KeyCode.F1)) t = 0f;
                foreach (var p in _players)
                {
                    if (p.eliminated) continue;
                    float dist = Vector3.Distance(p.transform.position, _grill.position);
                    if (dist < 2.3f && Vector3.Dot(p.transform.up, Vector3.up) > 0.35f)
                        _control[p.slot] += Time.deltaTime;
                }
                foreach (var p in _players)
                {
                    float dist = Vector3.Distance(p.transform.position, _grill.position);
                    if (dist < 1.0f)
                    {
                        var rb = p.GetComponent<Rigidbody>();
                        if (rb != null)
                            rb.AddExplosionForce(6.5f, _grill.position, 2.2f, 0.45f, ForceMode.VelocityChange);
                    }
                }
                if (_hud != null)
                {
                    _hud.SetTimer(t);
                    if (DemoSettings.ShowDebug)
                        _hud.SetBody($"BBQ BRAWL\n{ControlLine()}\nSpace = shove · hold grill");
                }
                yield return null;
            }
            foreach (var p in _players) p.inputEnabled = false;
            onFinished?.Invoke(Highest(), BuildPlaces());
        }

        public void Teardown()
        {
            if (_arena != null) Destroy(_arena.gameObject);
            _players.Clear();
        }

        string ControlLine()
        {
            var parts = new List<string>();
            foreach (var kv in _control) parts.Add($"P{kv.Key + 1}:{kv.Value:0.0}s");
            return string.Join("  ", parts);
        }

        int Highest()
        {
            int best = 0; float v = -1f;
            foreach (var kv in _control) if (kv.Value > v) { v = kv.Value; best = kv.Key; }
            return best;
        }

        int[] BuildPlaces()
        {
            var list = new List<KeyValuePair<int, float>>(_control);
            list.Sort((a, b) => b.Value.CompareTo(a.Value));
            var places = new int[_control.Count];
            for (int i = 0; i < list.Count; i++) places[list[i].Key] = i + 1;
            return places;
        }
    }

    public class FlameWobble : MonoBehaviour
    {
        Vector3 _base;
        void Start() => _base = transform.localScale;
        void Update()
        {
            float s = 1f + Mathf.Sin(Time.time * 9f) * 0.18f;
            transform.localScale = new Vector3(_base.x * s, _base.y * (2f - s), _base.z * s);
            transform.Rotate(0f, 50f * Time.deltaTime, 0f);
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
            bool press = _motor.aiControlled ? _motor.ActionDown : Input.GetKeyDown(_motor.action);
            if (!press || _cd > 0f) return;
            _cd = 0.4f;
            var cols = Physics.OverlapSphere(transform.position + transform.forward * 0.7f + Vector3.up * 0.8f, 1.15f);
            foreach (var c in cols)
            {
                if (c.attachedRigidbody == null || c.attachedRigidbody.gameObject == gameObject) continue;
                c.attachedRigidbody.AddForce((c.transform.position - transform.position).normalized * 9f + Vector3.up * 3f, ForceMode.VelocityChange);
            }
        }
    }
}
