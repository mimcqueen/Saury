using Saury.Core;
using UnityEngine;

namespace Saury.Bootstrap
{
    /// <summary>
    /// Simple 3D modeled characters from primitive mesh kits.
    /// Readable as people/creatures — not sticker circles.
    /// </summary>
    public static class CharacterKit
    {
        public struct HostBuild
        {
            public GameObject root;
            public Transform mouth;
            public Transform head;
            public Transform torso;
        }

        public struct DadBuild
        {
            public GameObject root;
            public Transform visual;
        }

        public static HostBuild BuildHost(Transform parent, Vector3 worldPos)
        {
            var root = new GameObject("CPU_Host");
            root.transform.SetParent(parent, false);
            root.transform.position = worldPos;

            // Hip / seated pelvis on chair
            var hip = MeshKit.Part(root.transform, "Hip", PrimitiveType.Cube,
                new Vector3(0f, 0.15f, 0.05f), new Vector3(0.55f, 0.22f, 0.4f), SauryPalette.HostSuit);

            var torso = MeshKit.Part(root.transform, "Torso", PrimitiveType.Cube,
                new Vector3(0f, 0.55f, 0.02f), new Vector3(0.62f, 0.7f, 0.35f), SauryPalette.HostSuit);

            var shirt = MeshKit.Part(root.transform, "Shirt", PrimitiveType.Cube,
                new Vector3(0f, 0.55f, 0.16f), new Vector3(0.45f, 0.45f, 0.08f), SauryPalette.HostShirt);

            // Shoulders + arms (resting on desk)
            MeshKit.Part(root.transform, "ShoulderL", PrimitiveType.Sphere,
                new Vector3(-0.42f, 0.85f, 0f), Vector3.one * 0.18f, SauryPalette.HostSuit);
            MeshKit.Part(root.transform, "ShoulderR", PrimitiveType.Sphere,
                new Vector3(0.42f, 0.85f, 0f), Vector3.one * 0.18f, SauryPalette.HostSuit);

            MeshKit.PartRot(root.transform, "ArmL", PrimitiveType.Capsule,
                new Vector3(-0.55f, 0.45f, 0.25f), new Vector3(20f, 0f, 25f),
                new Vector3(0.14f, 0.32f, 0.14f), SauryPalette.HostSuit);
            MeshKit.PartRot(root.transform, "ArmR", PrimitiveType.Capsule,
                new Vector3(0.55f, 0.45f, 0.25f), new Vector3(20f, 0f, -25f),
                new Vector3(0.14f, 0.32f, 0.14f), SauryPalette.HostSuit);

            // Hands on desk
            MeshKit.Part(root.transform, "HandL", PrimitiveType.Sphere,
                new Vector3(-0.45f, 0.22f, 0.55f), Vector3.one * 0.14f, SauryPalette.SkinWarm);
            MeshKit.Part(root.transform, "HandR", PrimitiveType.Sphere,
                new Vector3(0.45f, 0.22f, 0.55f), Vector3.one * 0.14f, SauryPalette.SkinWarm);

            // Neck + head
            MeshKit.Part(root.transform, "Neck", PrimitiveType.Cylinder,
                new Vector3(0f, 0.95f, 0f), new Vector3(0.14f, 0.08f, 0.14f), SauryPalette.SkinWarm);

            var head = MeshKit.Part(root.transform, "Head", PrimitiveType.Sphere,
                new Vector3(0f, 1.22f, 0.02f), new Vector3(0.42f, 0.48f, 0.4f), SauryPalette.SkinWarm);

            // Hair / combover
            MeshKit.Part(head.transform, "Hair", PrimitiveType.Sphere,
                new Vector3(0.05f, 0.2f, -0.05f), new Vector3(0.95f, 0.55f, 0.9f),
                new Color(0.25f, 0.18f, 0.12f));

            // Eyes
            BuildEyePair(head.transform, 0.12f, 0.08f, 0.2f);

            // Nose
            MeshKit.Part(head.transform, "Nose", PrimitiveType.Cube,
                new Vector3(0f, -0.02f, 0.22f), new Vector3(0.08f, 0.1f, 0.12f), SauryPalette.SkinWarm);

            // Mouth flap (talk target)
            var mouth = MeshKit.Part(head.transform, "Mouth", PrimitiveType.Cube,
                new Vector3(0f, -0.14f, 0.2f), new Vector3(0.2f, 0.06f, 0.06f),
                new Color(0.45f, 0.12f, 0.15f));

            // Tie
            MeshKit.Part(root.transform, "Tie", PrimitiveType.Cube,
                new Vector3(0f, 0.5f, 0.2f), new Vector3(0.08f, 0.35f, 0.04f),
                new Color(0.15f, 0.2f, 0.45f));

            // Chair behind host
            var chair = new GameObject("HostChair");
            chair.transform.SetParent(parent, false);
            chair.transform.position = worldPos + new Vector3(0f, 0f, -0.35f);
            MeshKit.Part(chair.transform, "Seat", PrimitiveType.Cube,
                new Vector3(0f, 0.35f, 0f), new Vector3(0.7f, 0.08f, 0.7f), SauryPalette.DirtyPlastic);
            MeshKit.Part(chair.transform, "Back", PrimitiveType.Cube,
                new Vector3(0f, 0.75f, -0.3f), new Vector3(0.7f, 0.7f, 0.08f), SauryPalette.DirtyPlastic);

            return new HostBuild { root = root, mouth = mouth.transform, head = head.transform, torso = torso.transform };
        }

        public static DadBuild BuildDad(Transform parent, Vector3 localPos, CreatureId creature, Color? overrideColor = null, string name = "Dad")
        {
            var root = new GameObject(name);
            root.transform.SetParent(parent, false);
            root.transform.localPosition = localPos;

            // Physics capsule (invisible-ish collider body) — visual is child
            var bodyCol = root.AddComponent<CapsuleCollider>();
            bodyCol.height = 1.8f;
            bodyCol.radius = 0.35f;
            bodyCol.center = new Vector3(0f, 0.9f, 0f);

            var visual = new GameObject("Visual").transform;
            visual.SetParent(root.transform, false);

            Color body = overrideColor ?? SauryPalette.CreatureBody(creature);
            Color accent = Color.Lerp(body, Color.black, 0.25f);

            // Legs
            MeshKit.Part(visual, "LegL", PrimitiveType.Capsule,
                new Vector3(-0.15f, 0.4f, 0f), new Vector3(0.22f, 0.4f, 0.22f), accent);
            MeshKit.Part(visual, "LegR", PrimitiveType.Capsule,
                new Vector3(0.15f, 0.4f, 0f), new Vector3(0.22f, 0.4f, 0.22f), accent);

            // Torso shape varies by creature
            switch (creature)
            {
                case CreatureId.Blob:
                    MeshKit.Part(visual, "Torso", PrimitiveType.Sphere,
                        new Vector3(0f, 0.95f, 0f), new Vector3(0.95f, 0.85f, 0.8f), body);
                    break;
                case CreatureId.Cryptid:
                    MeshKit.Part(visual, "Torso", PrimitiveType.Cube,
                        new Vector3(0f, 1.05f, 0f), new Vector3(0.55f, 0.9f, 0.35f), body);
                    break;
                default:
                    MeshKit.Part(visual, "Torso", PrimitiveType.Capsule,
                        new Vector3(0f, 1.0f, 0f), new Vector3(0.55f, 0.45f, 0.35f), body);
                    break;
            }

            // Arms
            MeshKit.PartRot(visual, "ArmL", PrimitiveType.Capsule,
                new Vector3(-0.42f, 1.05f, 0f), new Vector3(0f, 0f, 15f),
                new Vector3(0.16f, 0.35f, 0.16f), body);
            MeshKit.PartRot(visual, "ArmR", PrimitiveType.Capsule,
                new Vector3(0.42f, 1.05f, 0f), new Vector3(0f, 0f, -15f),
                new Vector3(0.16f, 0.35f, 0.16f), body);

            // Head
            Vector3 headScale = creature switch
            {
                CreatureId.Goblin => new Vector3(0.5f, 0.45f, 0.5f),
                CreatureId.Imp => new Vector3(0.4f, 0.45f, 0.4f),
                CreatureId.Catgirl => new Vector3(0.42f, 0.42f, 0.4f),
                CreatureId.Cryptid => new Vector3(0.35f, 0.55f, 0.4f),
                CreatureId.Blob => new Vector3(0.55f, 0.45f, 0.55f),
                _ => new Vector3(0.42f, 0.45f, 0.42f)
            };
            var head = MeshKit.Part(visual, "Head", PrimitiveType.Sphere,
                new Vector3(0f, 1.55f, 0.02f), headScale, body);

            BuildEyePair(head.transform, 0.15f, 0.1f, 0.22f);

            // Creature extras
            switch (creature)
            {
                case CreatureId.Goblin:
                    MeshKit.Part(head.transform, "EarL", PrimitiveType.Sphere,
                        new Vector3(-0.55f, 0.15f, 0f), new Vector3(0.35f, 0.55f, 0.2f), body);
                    MeshKit.Part(head.transform, "EarR", PrimitiveType.Sphere,
                        new Vector3(0.55f, 0.15f, 0f), new Vector3(0.35f, 0.55f, 0.2f), body);
                    MeshKit.Part(head.transform, "Nose", PrimitiveType.Sphere,
                        new Vector3(0f, -0.05f, 0.45f), Vector3.one * 0.25f, accent);
                    break;
                case CreatureId.Catgirl:
                    MeshKit.PartRot(head.transform, "EarL", PrimitiveType.Cube,
                        new Vector3(-0.28f, 0.55f, 0f), new Vector3(0f, 0f, -20f),
                        new Vector3(0.18f, 0.35f, 0.1f), body);
                    MeshKit.PartRot(head.transform, "EarR", PrimitiveType.Cube,
                        new Vector3(0.28f, 0.55f, 0f), new Vector3(0f, 0f, 20f),
                        new Vector3(0.18f, 0.35f, 0.1f), body);
                    MeshKit.Part(visual, "Tail", PrimitiveType.Capsule,
                        new Vector3(0f, 0.7f, -0.35f), new Vector3(0.12f, 0.4f, 0.12f), accent);
                    break;
                case CreatureId.Cryptid:
                    MeshKit.Part(head.transform, "AntlerL", PrimitiveType.Cylinder,
                        new Vector3(-0.2f, 0.7f, -0.1f), new Vector3(0.06f, 0.35f, 0.06f), SauryPalette.DirtyPlastic);
                    MeshKit.Part(head.transform, "AntlerR", PrimitiveType.Cylinder,
                        new Vector3(0.2f, 0.7f, -0.1f), new Vector3(0.06f, 0.35f, 0.06f), SauryPalette.DirtyPlastic);
                    break;
                case CreatureId.Imp:
                    MeshKit.PartRot(head.transform, "HornL", PrimitiveType.Cylinder,
                        new Vector3(-0.2f, 0.55f, 0f), new Vector3(20f, 0f, -25f),
                        new Vector3(0.08f, 0.22f, 0.08f), new Color(0.9f, 0.85f, 0.7f));
                    MeshKit.PartRot(head.transform, "HornR", PrimitiveType.Cylinder,
                        new Vector3(0.2f, 0.55f, 0f), new Vector3(20f, 0f, 25f),
                        new Vector3(0.08f, 0.22f, 0.08f), new Color(0.9f, 0.85f, 0.7f));
                    break;
                case CreatureId.Human:
                    MeshKit.Part(head.transform, "Mullet", PrimitiveType.Sphere,
                        new Vector3(0f, 0.05f, -0.35f), new Vector3(0.7f, 0.5f, 0.55f),
                        new Color(0.2f, 0.15f, 0.1f));
                    MeshKit.Part(head.transform, "Hat", PrimitiveType.Cylinder,
                        new Vector3(0f, 0.45f, 0f), new Vector3(0.55f, 0.08f, 0.55f),
                        new Color(0.55f, 0.15f, 0.12f));
                    break;
                case CreatureId.Blob:
                    MeshKit.Part(head.transform, "BlobDrool", PrimitiveType.Sphere,
                        new Vector3(0.15f, -0.35f, 0.25f), Vector3.one * 0.2f, Color.Lerp(body, Color.white, 0.3f));
                    break;
            }

            // Trucker hat prop for all (deadbeat read)
            if (creature != CreatureId.Human)
            {
                MeshKit.Part(head.transform, "Cap", PrimitiveType.Cylinder,
                    new Vector3(0f, 0.4f, 0.05f), new Vector3(0.5f, 0.07f, 0.5f),
                    new Color(0.5f, 0.15f, 0.12f));
            }

            return new DadBuild { root = root, visual = visual };
        }

        public static GameObject BuildBaby(Transform parent, Color primary, Color accent, float earScale, float headSquash)
        {
            var root = new GameObject("MockBabyMesh");
            root.transform.SetParent(parent, false);

            var head = MeshKit.Part(root.transform, "Head", PrimitiveType.Sphere,
                new Vector3(0f, 0.15f, 0f), new Vector3(0.7f, 0.7f * headSquash, 0.7f), primary);
            BuildEyePair(head.transform, 0.18f, 0.12f, 0.28f, big: true);

            MeshKit.Part(head.transform, "EarL", PrimitiveType.Sphere,
                new Vector3(-0.55f, 0.05f, 0f), Vector3.one * (0.32f * earScale), accent);
            MeshKit.Part(head.transform, "EarR", PrimitiveType.Sphere,
                new Vector3(0.55f, 0.05f, 0f), Vector3.one * (0.32f * earScale), accent);

            MeshKit.Part(root.transform, "Body", PrimitiveType.Capsule,
                new Vector3(0f, -0.45f, 0f), new Vector3(0.55f, 0.32f, 0.45f),
                Color.Lerp(primary, Color.white, 0.2f));

            // Onesie feet
            MeshKit.Part(root.transform, "FootL", PrimitiveType.Sphere,
                new Vector3(-0.18f, -0.75f, 0.05f), Vector3.one * 0.22f, accent);
            MeshKit.Part(root.transform, "FootR", PrimitiveType.Sphere,
                new Vector3(0.18f, -0.75f, 0.05f), Vector3.one * 0.22f, accent);

            // Tiny trucker hat
            MeshKit.Part(head.transform, "HatBrim", PrimitiveType.Cylinder,
                new Vector3(0f, 0.4f, 0.05f), new Vector3(0.55f, 0.05f, 0.55f), accent);
            MeshKit.Part(head.transform, "HatTop", PrimitiveType.Sphere,
                new Vector3(0f, 0.5f, 0f), new Vector3(0.4f, 0.25f, 0.4f), accent);

            // Arm stubs for animatronic wave read
            MeshKit.PartRot(root.transform, "ArmL", PrimitiveType.Capsule,
                new Vector3(-0.4f, -0.25f, 0f), new Vector3(0f, 0f, 40f),
                new Vector3(0.12f, 0.2f, 0.12f), primary);
            MeshKit.PartRot(root.transform, "ArmR", PrimitiveType.Capsule,
                new Vector3(0.4f, -0.25f, 0f), new Vector3(0f, 0f, -40f),
                new Vector3(0.12f, 0.2f, 0.12f), primary);

            return root;
        }

        static void BuildEyePair(Transform head, float x, float y, float z, bool big = false)
        {
            float s = big ? 0.28f : 0.2f;
            float pupil = big ? 0.12f : 0.09f;
            for (int i = 0; i < 2; i++)
            {
                float side = i == 0 ? -1f : 1f;
                var white = MeshKit.Part(head, $"EyeWhite_{i}", PrimitiveType.Sphere,
                    new Vector3(side * x, y, z), Vector3.one * s, SauryPalette.EyeWhite, smoothness: 0.55f);
                MeshKit.Part(white.transform, "Pupil", PrimitiveType.Sphere,
                    new Vector3(0f, 0f, 0.35f), Vector3.one * (pupil / s), SauryPalette.EyeIris, smoothness: 0.4f);
            }
        }
    }
}
