using System.Collections;
using System.Collections.Generic;
using Saury.Bootstrap;
using Saury.Core;
using Saury.Dad;
using Saury.UI;
using UnityEngine;

namespace Saury.Minigames
{
    /// <summary>Grimy PS1 bathroom arena (ref 06) — cartoon arcs into cans / Host plant.</summary>
    public class PissingContestMinigame : MonoBehaviour
    {
        public float duration = 40f;
        public System.Action<int, int[]> onFinished;

        readonly List<DadAvatarMotor> _players = new List<DadAvatarMotor>();
        readonly Dictionary<int, float> _scores = new Dictionary<int, float>();
        Transform _arena;
        ShowHud _hud;
        Transform _plantAim;

        public Vector3 ArenaWorldCenter =>
            _arena != null ? _arena.TransformPoint(new Vector3(0f, 0f, 0.5f)) : Vector3.zero;

        public void Build(Transform parent, IReadOnlyList<DadSeed> dads, ShowHud hud)
        {
            _hud = hud;
            if (DemoSettings.demoMode) duration = 32f;

            _arena = new GameObject("PissingContestArena").transform;
            _arena.SetParent(parent, false);
            _arena.localPosition = new Vector3(22f, 0f, 0f);

            BuildBathroomSet(_arena);

            // Cans along the far wall / under sinks line
            for (int i = 0; i < 5; i++)
            {
                var canRoot = new GameObject($"Can_{i}").transform;
                canRoot.SetParent(_arena, false);
                canRoot.localPosition = new Vector3(-3f + i * 1.5f, 0f, 2.4f);
                var can = MeshKit.Part(canRoot, "Body", PrimitiveType.Cylinder,
                    new Vector3(0f, 0.35f, 0f), new Vector3(0.5f, 0.35f, 0.5f),
                    new Color(0.45f, 0.48f, 0.42f), collider: true, metallic: 0.35f);
                MeshKit.Part(canRoot, "Rim", PrimitiveType.Cylinder,
                    new Vector3(0f, 0.72f, 0f), new Vector3(0.52f, 0.04f, 0.52f), SauryPalette.Metal, metallic: 0.55f);
                can.AddComponent<TargetCan>().value = 1f + i * 0.35f;
            }

            // Host's plant (bonus) — sit on a grimy toilet tank area
            var plant = new GameObject("HostsPlant").transform;
            plant.SetParent(_arena, false);
            plant.localPosition = new Vector3(0f, 0f, 3.5f);
            _plantAim = plant;
            MeshKit.Part(plant, "Pot", PrimitiveType.Cylinder,
                new Vector3(0f, 0.25f, 0f), new Vector3(0.55f, 0.25f, 0.55f),
                new Color(0.4f, 0.22f, 0.16f), collider: true);
            var foliage = MeshKit.Part(plant, "Foliage", PrimitiveType.Sphere,
                new Vector3(0f, 0.85f, 0f), new Vector3(0.9f, 1.1f, 0.9f),
                new Color(0.22f, 0.48f, 0.22f), collider: true);
            foliage.AddComponent<TargetCan>().value = 3.5f;

            _players.Clear();
            _scores.Clear();
            for (int i = 0; i < dads.Count; i++)
            {
                var dad = dads[i];
                var build = CharacterKit.BuildDad(_arena, new Vector3(-3.2f + i * 2.0f, 0f, -2.6f),
                    dad.creature, dad.bodyColor, $"Dad_{dad.displayName}");
                var rb = build.root.AddComponent<Rigidbody>();
                rb.mass = 2.2f;
                rb.linearDamping = 1.4f;
                rb.angularDamping = 2.5f;
                var motor = build.root.AddComponent<DadAvatarMotor>();
                motor.ConfigureKeysForSlot(dad.slotIndex);
                motor.displayName = dad.displayName;
                motor.aiTarget = _plantAim;
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

        static void BuildBathroomSet(Transform arena)
        {
            // Grimy tile floor (ref 06)
            MeshKit.Part(arena, "TileFloor", PrimitiveType.Cube,
                new Vector3(0f, -0.05f, 0f), new Vector3(14f, 0.1f, 11f),
                new Color(0.42f, 0.4f, 0.36f), collider: true, smoothness: 0.12f);
            // Missing tile voids
            for (int i = 0; i < 5; i++)
            {
                MeshKit.Part(arena, $"TileVoid_{i}", PrimitiveType.Cube,
                    new Vector3(-4f + i * 2.1f, -0.02f, -1.5f + (i % 2) * 2.2f),
                    new Vector3(0.7f, 0.08f, 0.7f), SauryPalette.StageBlack, smoothness: 0.05f);
            }
            // Stains
            MeshKit.Part(arena, "FloorStain", PrimitiveType.Cylinder,
                new Vector3(1.2f, 0.01f, 0.5f), new Vector3(2.4f, 0.02f, 1.6f),
                new Color(0.28f, 0.22f, 0.16f), smoothness: 0.08f);

            // Tile walls
            Color tile = new Color(0.48f, 0.45f, 0.4f);
            Color tileDirty = new Color(0.35f, 0.3f, 0.24f);
            MeshKit.Part(arena, "WallBack", PrimitiveType.Cube,
                new Vector3(0f, 1.8f, 5.2f), new Vector3(14f, 3.6f, 0.25f), tileDirty, collider: true, smoothness: 0.15f);
            MeshKit.Part(arena, "WallL", PrimitiveType.Cube,
                new Vector3(-7f, 1.8f, 0f), new Vector3(0.25f, 3.6f, 11f), tile, collider: true, smoothness: 0.15f);
            MeshKit.Part(arena, "WallR", PrimitiveType.Cube,
                new Vector3(7f, 1.8f, 0f), new Vector3(0.25f, 3.6f, 11f), tile, collider: true, smoothness: 0.15f);

            // Sink row (left wall)
            for (int i = 0; i < 3; i++)
            {
                float z = -1.5f + i * 1.6f;
                var sink = new GameObject($"Sink_{i}").transform;
                sink.SetParent(arena, false);
                sink.localPosition = new Vector3(-6.2f, 0f, z);
                MeshKit.Part(sink, "Basin", PrimitiveType.Cube,
                    new Vector3(0f, 0.95f, 0f), new Vector3(1.1f, 0.18f, 0.7f),
                    new Color(0.72f, 0.7f, 0.65f), collider: true, smoothness: 0.35f);
                MeshKit.Part(sink, "Bowl", PrimitiveType.Cylinder,
                    new Vector3(0f, 0.82f, 0.05f), new Vector3(0.55f, 0.12f, 0.4f),
                    new Color(0.55f, 0.55f, 0.52f), smoothness: 0.4f);
                MeshKit.Part(sink, "Faucet", PrimitiveType.Capsule,
                    new Vector3(0f, 1.2f, -0.2f), new Vector3(0.06f, 0.14f, 0.06f), SauryPalette.Metal, metallic: 0.6f);
                // Mirror slab
                MeshKit.Part(sink, "Mirror", PrimitiveType.Cube,
                    new Vector3(0f, 1.85f, -0.35f), new Vector3(1.0f, 0.85f, 0.04f),
                    new Color(0.25f, 0.28f, 0.3f), smoothness: 0.55f);
                MeshKit.GlowPart(sink, "MirrorGrime", PrimitiveType.Cube,
                    new Vector3(0f, 1.85f, -0.32f), new Vector3(0.85f, 0.7f, 0.02f),
                    new Color(0.15f, 0.18f, 0.16f), new Color(0.4f, 0.55f, 0.5f), 0.15f);
            }

            // Stall partition (right)
            MeshKit.Part(arena, "Stall", PrimitiveType.Cube,
                new Vector3(5.2f, 1.4f, 0.5f), new Vector3(0.12f, 2.8f, 4.5f),
                new Color(0.22f, 0.2f, 0.18f), collider: true, smoothness: 0.2f);
            MeshKit.Part(arena, "StallDoor", PrimitiveType.Cube,
                new Vector3(4.4f, 1.3f, -1.8f), new Vector3(1.2f, 2.4f, 0.08f),
                new Color(0.28f, 0.26f, 0.24f), smoothness: 0.18f);

            // Paper towel / door cues
            MeshKit.Part(arena, "Dispenser", PrimitiveType.Cube,
                new Vector3(-2f, 1.6f, 4.95f), new Vector3(0.55f, 0.7f, 0.2f),
                new Color(0.35f, 0.22f, 0.16f), smoothness: 0.2f);
            MeshKit.Part(arena, "BackDoor", PrimitiveType.Cube,
                new Vector3(2.5f, 1.5f, 5.05f), new Vector3(1.4f, 2.6f, 0.12f),
                SauryPalette.Metal, metallic: 0.45f);

            // Sickly overhead + practical spot
            var bathSpot = new GameObject("BathSpot");
            bathSpot.transform.SetParent(arena, false);
            bathSpot.transform.localPosition = new Vector3(0f, 5.2f, -1.5f);
            bathSpot.transform.LookAt(arena.TransformPoint(new Vector3(0f, 0.4f, 1f)));
            var yl = bathSpot.AddComponent<Light>();
            yl.type = LightType.Spot;
            yl.intensity = 5.2f;
            yl.range = 18f;
            yl.spotAngle = 58f;
            yl.color = new Color(0.95f, 0.88f, 0.7f);
            yl.shadows = LightShadows.Soft;

            var fill = new GameObject("BathFill");
            fill.transform.SetParent(arena, false);
            fill.transform.localPosition = new Vector3(-3f, 3.5f, 2f);
            var fl = fill.AddComponent<Light>();
            fl.type = LightType.Point;
            fl.intensity = 0.9f;
            fl.range = 10f;
            fl.color = new Color(0.55f, 0.65f, 0.55f);
        }

        public IEnumerator Run()
        {
            float t = duration;
            foreach (var p in _players) p.inputEnabled = true;
            while (t > 0f)
            {
                t -= Time.deltaTime;
                if (Input.GetKeyDown(KeyCode.F1)) t = 0f;
                if (_hud != null)
                {
                    _hud.SetTimer(t);
                    if (DemoSettings.ShowDebug)
                        _hud.SetBody($"PISSING CONTEST\n{ScoreLine()}\nSpace = spray");
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
            if (!motor.ActionHeld || _cooldown > 0f) return;
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
            // Face plant / cans roughly
            var motor = GetComponent<DadAvatarMotor>();
            if (motor != null && motor.aiTarget != null)
            {
                var to = (motor.aiTarget.position - transform.position).normalized;
                dir = to * 3.2f + Vector3.up * 2.6f + Random.insideUnitSphere * 0.4f;
            }
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
