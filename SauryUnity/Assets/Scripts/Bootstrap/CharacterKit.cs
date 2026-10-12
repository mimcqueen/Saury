using Saury.Core;
using UnityEngine;

namespace Saury.Bootstrap
{
    /// <summary>
    /// Art-directed doll kits — connected limbs, readable silhouettes, sleazy Maury energy.
    /// Simple modeling, intentionally staged — not sterile greybox.
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

            // Chair first (behind)
            var chair = new GameObject("HostChair").transform;
            chair.SetParent(parent, false);
            chair.position = worldPos + new Vector3(0f, 0f, -0.4f);
            MeshKit.Part(chair, "Seat", PrimitiveType.Cube,
                new Vector3(0f, 0.38f, 0.05f), new Vector3(0.75f, 0.1f, 0.7f), SauryPalette.DirtyPlastic, smoothness: 0.15f);
            MeshKit.Part(chair, "Back", PrimitiveType.Cube,
                new Vector3(0f, 0.85f, -0.28f), new Vector3(0.75f, 0.85f, 0.1f), SauryPalette.DirtyPlastic, smoothness: 0.15f);
            MeshKit.Part(chair, "Cushion", PrimitiveType.Cube,
                new Vector3(0f, 0.95f, -0.22f), new Vector3(0.65f, 0.5f, 0.08f), SauryPalette.BruisedPurple, smoothness: 0.35f);

            // Connected seated body
            var hip = MeshKit.Part(root.transform, "Hip", PrimitiveType.Sphere,
                new Vector3(0f, 0.42f, 0.02f), new Vector3(0.55f, 0.28f, 0.42f), SauryPalette.HostSuit, smoothness: 0.35f);

            var torso = MeshKit.Part(root.transform, "Torso", PrimitiveType.Capsule,
                new Vector3(0f, 0.78f, 0.02f), new Vector3(0.58f, 0.38f, 0.32f), SauryPalette.HostSuit, smoothness: 0.38f);

            MeshKit.Part(root.transform, "Belly", PrimitiveType.Sphere,
                new Vector3(0f, 0.62f, 0.12f), new Vector3(0.5f, 0.35f, 0.35f), SauryPalette.HostSuitShadow, smoothness: 0.4f);

            MeshKit.Part(root.transform, "Shirt", PrimitiveType.Cube,
                new Vector3(0f, 0.8f, 0.18f), new Vector3(0.38f, 0.42f, 0.06f), SauryPalette.HostShirt, smoothness: 0.45f);

            MeshKit.Part(root.transform, "Tie", PrimitiveType.Cube,
                new Vector3(0f, 0.72f, 0.22f), new Vector3(0.07f, 0.38f, 0.04f),
                new Color(0.12f, 0.18f, 0.4f), smoothness: 0.5f);
            MeshKit.GlowPart(root.transform, "TieClip", PrimitiveType.Cube,
                new Vector3(0f, 0.62f, 0.24f), new Vector3(0.1f, 0.04f, 0.02f),
                SauryPalette.NeonAmber, SauryPalette.NeonAmber, 0.8f);

            // Shoulders → upper arm → forearm → hand (connected)
            BuildArm(root.transform, -1);
            BuildArm(root.transform, 1);

            // Neck + head (sleazy doll)
            MeshKit.Part(root.transform, "Neck", PrimitiveType.Sphere,
                new Vector3(0f, 1.12f, 0.02f), new Vector3(0.18f, 0.16f, 0.18f), SauryPalette.SkinSweat, smoothness: 0.45f);

            var head = MeshKit.Part(root.transform, "Head", PrimitiveType.Sphere,
                new Vector3(0f, 1.38f, 0.04f), new Vector3(0.4f, 0.46f, 0.38f), SauryPalette.SkinSweat, smoothness: 0.42f);

            // Bad combover
            MeshKit.Part(head.transform, "HairDome", PrimitiveType.Sphere,
                new Vector3(0.04f, 0.18f, -0.02f), new Vector3(0.92f, 0.55f, 0.88f), SauryPalette.Combover, smoothness: 0.2f);
            MeshKit.Part(head.transform, "ComboverFlap", PrimitiveType.Sphere,
                new Vector3(0.18f, 0.05f, 0.05f), new Vector3(0.45f, 0.25f, 0.5f), SauryPalette.Combover, smoothness: 0.2f);

            // Face audience (−Z)
            BuildFace(head.transform, 0.11f, 0.06f, -0.22f, big: false, irisTowardLocalZ: -1f);

            MeshKit.Part(head.transform, "Nose", PrimitiveType.Sphere,
                new Vector3(0f, -0.02f, -0.22f), new Vector3(0.12f, 0.14f, 0.16f), SauryPalette.SkinWarm, smoothness: 0.4f);

            var mouth = MeshKit.Part(head.transform, "Mouth", PrimitiveType.Cube,
                new Vector3(0f, -0.14f, -0.2f), new Vector3(0.18f, 0.05f, 0.06f),
                new Color(0.5f, 0.15f, 0.18f), smoothness: 0.35f);

            // Sweaty brow shine
            MeshKit.GlowPart(head.transform, "BrowShine", PrimitiveType.Sphere,
                new Vector3(0f, 0.12f, -0.18f), new Vector3(0.25f, 0.08f, 0.1f),
                SauryPalette.SkinSweat, Color.white, 0.35f);

            return new HostBuild { root = root, mouth = mouth.transform, head = head.transform, torso = torso.transform };
        }

        static void BuildArm(Transform root, int side)
        {
            float s = side;
            var shoulder = MeshKit.Part(root, side < 0 ? "ShoulderL" : "ShoulderR", PrimitiveType.Sphere,
                new Vector3(s * 0.38f, 1.05f, 0f), Vector3.one * 0.2f, SauryPalette.HostSuit, smoothness: 0.38f);

            // Upper arm toward desk (audience / -Z)
            var upper = MeshKit.LimbChain(root, side < 0 ? "UpperArmL" : "UpperArmR",
                new Vector3(s * 0.42f, 1.0f, -0.08f),
                new Vector3(-25f, 0f, s * 28f),
                0.38f, 0.07f, SauryPalette.HostSuit);

            // Forearm resting forward on desk top
            var fore = MeshKit.LimbChain(root, side < 0 ? "ForeArmL" : "ForeArmR",
                new Vector3(s * 0.5f, 0.62f, -0.42f),
                new Vector3(-55f, 0f, s * 8f),
                0.34f, 0.065f, SauryPalette.HostSuit);

            MeshKit.Part(root, side < 0 ? "HandL" : "HandR", PrimitiveType.Sphere,
                new Vector3(s * 0.48f, 0.38f, -0.7f), Vector3.one * 0.13f, SauryPalette.SkinWarm, smoothness: 0.4f);

            _ = shoulder; _ = upper; _ = fore;
        }

        public static DadBuild BuildDad(Transform parent, Vector3 localPos, CreatureId creature, Color? overrideColor = null, string name = "Dad")
        {
            var root = new GameObject(name);
            root.transform.SetParent(parent, false);
            root.transform.localPosition = localPos;

            var bodyCol = root.AddComponent<CapsuleCollider>();
            bodyCol.height = 1.85f;
            bodyCol.radius = 0.38f;
            bodyCol.center = new Vector3(0f, 0.92f, 0f);

            var visual = new GameObject("Visual").transform;
            visual.SetParent(root.transform, false);

            Color body = overrideColor ?? SauryPalette.CreatureBody(creature);
            Color accent = SauryPalette.CreatureAccent(creature);
            Color limb = Color.Lerp(body, Color.black, 0.12f);

            // Pelvis
            MeshKit.Part(visual, "Pelvis", PrimitiveType.Sphere,
                new Vector3(0f, 0.72f, 0f), new Vector3(0.45f, 0.28f, 0.32f), limb, smoothness: 0.3f);

            // Legs — connected chains
            MeshKit.LimbChain(visual, "ThighL", new Vector3(-0.14f, 0.7f, 0f), new Vector3(8f, 0f, 8f), 0.42f, 0.09f, limb);
            MeshKit.LimbChain(visual, "ThighR", new Vector3(0.14f, 0.7f, 0f), new Vector3(8f, 0f, -8f), 0.42f, 0.09f, limb);
            MeshKit.Part(visual, "FootL", PrimitiveType.Sphere,
                new Vector3(-0.16f, 0.08f, 0.06f), new Vector3(0.18f, 0.1f, 0.26f), accent, smoothness: 0.25f);
            MeshKit.Part(visual, "FootR", PrimitiveType.Sphere,
                new Vector3(0.16f, 0.08f, 0.06f), new Vector3(0.18f, 0.1f, 0.26f), accent, smoothness: 0.25f);

            // Torso by creature
            GameObject torso;
            switch (creature)
            {
                case CreatureId.Blob:
                    torso = MeshKit.Part(visual, "Torso", PrimitiveType.Sphere,
                        new Vector3(0f, 1.05f, 0f), new Vector3(0.95f, 0.8f, 0.75f), body, smoothness: 0.55f);
                    break;
                case CreatureId.Cryptid:
                    torso = MeshKit.Part(visual, "Torso", PrimitiveType.Capsule,
                        new Vector3(0f, 1.15f, 0f), new Vector3(0.42f, 0.5f, 0.32f), body, smoothness: 0.25f);
                    break;
                default:
                    torso = MeshKit.Part(visual, "Torso", PrimitiveType.Capsule,
                        new Vector3(0f, 1.1f, 0f), new Vector3(0.5f, 0.42f, 0.34f), body, smoothness: 0.32f);
                    break;
            }

            // Arms connected
            MeshKit.LimbChain(visual, "ArmL", new Vector3(-0.32f, 1.35f, 0f), new Vector3(15f, 0f, 35f), 0.4f, 0.075f, limb);
            MeshKit.LimbChain(visual, "ArmR", new Vector3(0.32f, 1.35f, 0f), new Vector3(15f, 0f, -35f), 0.4f, 0.075f, limb);
            MeshKit.Part(visual, "HandL", PrimitiveType.Sphere,
                new Vector3(-0.55f, 0.95f, 0.1f), Vector3.one * 0.14f, body, smoothness: 0.35f);
            MeshKit.Part(visual, "HandR", PrimitiveType.Sphere,
                new Vector3(0.55f, 0.95f, 0.1f), Vector3.one * 0.14f, body, smoothness: 0.35f);

            // Head
            Vector3 headScale = creature switch
            {
                CreatureId.Goblin => new Vector3(0.48f, 0.42f, 0.48f),
                CreatureId.Imp => new Vector3(0.38f, 0.42f, 0.38f),
                CreatureId.Catgirl => new Vector3(0.4f, 0.4f, 0.38f),
                CreatureId.Cryptid => new Vector3(0.34f, 0.52f, 0.38f),
                CreatureId.Blob => new Vector3(0.55f, 0.42f, 0.55f),
                _ => new Vector3(0.4f, 0.44f, 0.4f)
            };
            MeshKit.Part(visual, "Neck", PrimitiveType.Sphere,
                new Vector3(0f, 1.42f, 0f), new Vector3(0.16f, 0.12f, 0.16f), body, smoothness: 0.35f);
            var head = MeshKit.Part(visual, "Head", PrimitiveType.Sphere,
                new Vector3(0f, 1.62f, 0.02f), headScale, body, smoothness: 0.38f);

            BuildFace(head.transform, 0.13f, 0.08f, 0.22f, big: creature == CreatureId.Goblin || creature == CreatureId.Blob);

            ApplyCreatureExtras(visual, head.transform, creature, body, accent);

            // Shared deadbeat cap (except human who has mullet+hat)
            if (creature != CreatureId.Human)
            {
                MeshKit.Part(head.transform, "Cap", PrimitiveType.Cylinder,
                    new Vector3(0f, 0.38f, 0.04f), new Vector3(0.48f, 0.07f, 0.48f), accent, smoothness: 0.3f);
                MeshKit.Part(head.transform, "Brim", PrimitiveType.Cube,
                    new Vector3(0f, 0.32f, 0.22f), new Vector3(0.42f, 0.03f, 0.22f), accent, smoothness: 0.3f);
            }

            _ = torso;
            return new DadBuild { root = root, visual = visual };
        }

        static void ApplyCreatureExtras(Transform visual, Transform head, CreatureId creature, Color body, Color accent)
        {
            switch (creature)
            {
                case CreatureId.Human:
                    MeshKit.Part(head, "Mullet", PrimitiveType.Sphere,
                        new Vector3(0f, 0.0f, -0.32f), new Vector3(0.65f, 0.45f, 0.5f), SauryPalette.Combover, smoothness: 0.2f);
                    MeshKit.Part(head, "Hat", PrimitiveType.Cylinder,
                        new Vector3(0f, 0.42f, 0f), new Vector3(0.52f, 0.08f, 0.52f), accent, smoothness: 0.28f);
                    MeshKit.Part(head, "Brim", PrimitiveType.Cube,
                        new Vector3(0f, 0.36f, 0.24f), new Vector3(0.45f, 0.03f, 0.24f), accent, smoothness: 0.28f);
                    break;
                case CreatureId.Goblin:
                    MeshKit.Part(head, "EarL", PrimitiveType.Sphere,
                        new Vector3(-0.52f, 0.12f, 0f), new Vector3(0.28f, 0.55f, 0.18f), body, smoothness: 0.35f);
                    MeshKit.Part(head, "EarR", PrimitiveType.Sphere,
                        new Vector3(0.52f, 0.12f, 0f), new Vector3(0.28f, 0.55f, 0.18f), body, smoothness: 0.35f);
                    MeshKit.Part(head, "Nose", PrimitiveType.Sphere,
                        new Vector3(0f, -0.05f, 0.42f), Vector3.one * 0.22f, accent, smoothness: 0.4f);
                    break;
                case CreatureId.Catgirl:
                    MeshKit.PartRot(head, "EarL", PrimitiveType.Sphere,
                        new Vector3(-0.22f, 0.48f, 0f), new Vector3(0f, 0f, -18f),
                        new Vector3(0.16f, 0.32f, 0.12f), body);
                    MeshKit.PartRot(head, "EarR", PrimitiveType.Sphere,
                        new Vector3(0.22f, 0.48f, 0f), new Vector3(0f, 0f, 18f),
                        new Vector3(0.16f, 0.32f, 0.12f), body);
                    var tail = MeshKit.LimbChain(visual, "Tail", new Vector3(0f, 0.85f, -0.25f),
                        new Vector3(50f, 0f, 0f), 0.55f, 0.06f, accent);
                    _ = tail;
                    break;
                case CreatureId.Cryptid:
                    MeshKit.PartRot(head, "AntlerL", PrimitiveType.Capsule,
                        new Vector3(-0.18f, 0.55f, -0.08f), new Vector3(15f, 0f, -20f),
                        new Vector3(0.06f, 0.28f, 0.06f), SauryPalette.DirtyPlastic);
                    MeshKit.PartRot(head, "AntlerR", PrimitiveType.Capsule,
                        new Vector3(0.18f, 0.55f, -0.08f), new Vector3(15f, 0f, 20f),
                        new Vector3(0.06f, 0.28f, 0.06f), SauryPalette.DirtyPlastic);
                    break;
                case CreatureId.Imp:
                    MeshKit.PartRot(head, "HornL", PrimitiveType.Capsule,
                        new Vector3(-0.16f, 0.48f, 0f), new Vector3(25f, 0f, -30f),
                        new Vector3(0.07f, 0.2f, 0.07f), accent);
                    MeshKit.PartRot(head, "HornR", PrimitiveType.Capsule,
                        new Vector3(0.16f, 0.48f, 0f), new Vector3(25f, 0f, 30f),
                        new Vector3(0.07f, 0.2f, 0.07f), accent);
                    break;
                case CreatureId.Blob:
                    MeshKit.Part(head, "Drool", PrimitiveType.Sphere,
                        new Vector3(0.12f, -0.32f, 0.22f), Vector3.one * 0.18f,
                        Color.Lerp(body, Color.white, 0.35f), smoothness: 0.7f);
                    break;
            }
        }

        public static GameObject BuildBaby(Transform parent, Color primary, Color accent, float earScale, float headSquash)
        {
            var root = new GameObject("MockBabyMesh");
            root.transform.SetParent(parent, false);

            MeshKit.Part(root.transform, "Pelvis", PrimitiveType.Sphere,
                new Vector3(0f, -0.35f, 0f), new Vector3(0.45f, 0.28f, 0.4f),
                Color.Lerp(primary, Color.white, 0.15f), smoothness: 0.45f);

            var head = MeshKit.Part(root.transform, "Head", PrimitiveType.Sphere,
                new Vector3(0f, 0.2f, 0f), new Vector3(0.72f, 0.72f * headSquash, 0.72f), primary, smoothness: 0.48f);
            BuildFace(head.transform, 0.16f, 0.1f, 0.28f, big: true);

            MeshKit.Part(head.transform, "EarL", PrimitiveType.Sphere,
                new Vector3(-0.5f, 0.05f, 0f), Vector3.one * (0.3f * earScale), accent, smoothness: 0.4f);
            MeshKit.Part(head.transform, "EarR", PrimitiveType.Sphere,
                new Vector3(0.5f, 0.05f, 0f), Vector3.one * (0.3f * earScale), accent, smoothness: 0.4f);

            MeshKit.LimbChain(root.transform, "ArmL", new Vector3(-0.35f, -0.05f, 0f), new Vector3(0f, 0f, 50f), 0.28f, 0.06f, primary);
            MeshKit.LimbChain(root.transform, "ArmR", new Vector3(0.35f, -0.05f, 0f), new Vector3(0f, 0f, -50f), 0.28f, 0.06f, primary);

            MeshKit.Part(root.transform, "FootL", PrimitiveType.Sphere,
                new Vector3(-0.16f, -0.7f, 0.05f), Vector3.one * 0.2f, accent, smoothness: 0.4f);
            MeshKit.Part(root.transform, "FootR", PrimitiveType.Sphere,
                new Vector3(0.16f, -0.7f, 0.05f), Vector3.one * 0.2f, accent, smoothness: 0.4f);

            MeshKit.Part(head.transform, "HatBrim", PrimitiveType.Cylinder,
                new Vector3(0f, 0.42f, 0.05f), new Vector3(0.55f, 0.05f, 0.55f), accent, smoothness: 0.35f);
            MeshKit.Part(head.transform, "HatTop", PrimitiveType.Sphere,
                new Vector3(0f, 0.52f, 0f), new Vector3(0.38f, 0.22f, 0.38f), accent, smoothness: 0.35f);

            return root;
        }

        static void BuildFace(Transform head, float x, float y, float z, bool big, float irisTowardLocalZ = 1f)
        {
            float s = big ? 0.26f : 0.18f;
            float pupil = big ? 0.11f : 0.08f;
            float iz = Mathf.Sign(irisTowardLocalZ);
            for (int i = 0; i < 2; i++)
            {
                float side = i == 0 ? -1f : 1f;
                var white = MeshKit.Part(head, $"EyeWhite_{i}", PrimitiveType.Sphere,
                    new Vector3(side * x, y, z), Vector3.one * s, SauryPalette.EyeWhite, smoothness: 0.65f);
                MeshKit.Part(white.transform, "Iris", PrimitiveType.Sphere,
                    new Vector3(0f, 0f, iz * 0.32f), Vector3.one * (pupil / s), SauryPalette.EyeIris, smoothness: 0.45f);
                MeshKit.GlowPart(white.transform, "Catch", PrimitiveType.Sphere,
                    new Vector3(0.12f, 0.12f, iz * 0.45f), Vector3.one * 0.15f,
                    Color.white, new Color(1f, 0.95f, 0.8f), 1.4f);
            }
        }
    }
}
