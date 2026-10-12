using UnityEngine;

namespace Saury.Bootstrap
{
    /// <summary>
    /// Eerie dingy Maury set — Liar's Bar intimacy: spots crush the world, blacks everywhere else.
    /// </summary>
    public static class StageKit
    {
        public static void BuildHubSet(Transform root)
        {
            // Crushed black void floor + stained stage oval
            MeshKit.Part(root, "VoidFloor", PrimitiveType.Cube,
                new Vector3(0f, -0.15f, 0.2f), new Vector3(28f, 0.1f, 24f), SauryPalette.StageBlack, collider: true, smoothness: 0.05f);

            MeshKit.Part(root, "StageFloor", PrimitiveType.Cylinder,
                new Vector3(0f, -0.02f, 0.8f), new Vector3(11f, 0.08f, 9f), SauryPalette.Floor, collider: true, smoothness: 0.1f);

            // Stains / scuffs
            for (int i = 0; i < 6; i++)
            {
                float ang = i * 60f * Mathf.Deg2Rad;
                MeshKit.Part(root, $"Stain_{i}", PrimitiveType.Cylinder,
                    new Vector3(Mathf.Cos(ang) * 2.2f, 0.01f, Mathf.Sin(ang) * 1.6f + 0.5f),
                    new Vector3(0.9f + i * 0.1f, 0.02f, 0.5f), SauryPalette.FloorStain, smoothness: 0.08f);
            }

            MeshKit.Part(root, "StageLip", PrimitiveType.Cube,
                new Vector3(0f, 0.08f, -4.2f), new Vector3(12f, 0.35f, 0.35f), SauryPalette.DirtyWood, collider: true);

            // Nicotine curtains — overlapping folds into crushed black
            for (int i = 0; i < 9; i++)
            {
                float x = -7f + i * 1.75f;
                float fold = (i % 2) * 0.15f;
                Color c = Color.Lerp(SauryPalette.NicotineCurtain, SauryPalette.CurtainShadow, (i % 3) * 0.25f);
                MeshKit.Part(root, $"Curtain_{i}", PrimitiveType.Cube,
                    new Vector3(x, 2.7f, 5.5f + fold), new Vector3(1.7f, 5.4f, 0.18f + fold * 0.3f), c, smoothness: 0.1f);
            }
            // Dark header / proscenium
            MeshKit.Part(root, "Proscenium", PrimitiveType.Cube,
                new Vector3(0f, 5.0f, 5.2f), new Vector3(16f, 0.6f, 0.5f), SauryPalette.StageBlack, smoothness: 0.05f);

            MeshKit.Part(root, "WallL", PrimitiveType.Cube,
                new Vector3(-8.5f, 2.4f, 0f), new Vector3(0.4f, 5.5f, 16f), SauryPalette.StageBlack);
            MeshKit.Part(root, "WallR", PrimitiveType.Cube,
                new Vector3(8.5f, 2.4f, 0f), new Vector3(0.4f, 5.5f, 16f), SauryPalette.StageBlack);
            MeshKit.Part(root, "Ceiling", PrimitiveType.Cube,
                new Vector3(0f, 5.4f, 0.5f), new Vector3(18f, 0.2f, 16f), SauryPalette.StageBlack);

            BuildHostDesk(root);
            BuildLogo(root);
            BuildPracticalSpots(root);
            BuildDadMarks(root);

            // Audience riser — pure black mass
            MeshKit.Part(root, "AudienceRiser", PrimitiveType.Cube,
                new Vector3(0f, 0.2f, -6.4f), new Vector3(14f, 0.5f, 2.8f), SauryPalette.StageBlack, collider: true);
        }

        static void BuildHostDesk(Transform root)
        {
            var desk = new GameObject("HostDesk").transform;
            desk.SetParent(root, false);
            desk.position = new Vector3(0f, 0f, 2.2f);

            MeshKit.Part(desk, "DeskBody", PrimitiveType.Cube,
                new Vector3(0f, 0.55f, 0f), new Vector3(2.8f, 1.05f, 1.15f), SauryPalette.DirtyWood, collider: true, smoothness: 0.15f);
            MeshKit.Part(desk, "DeskTop", PrimitiveType.Cube,
                new Vector3(0f, 1.1f, 0.05f), new Vector3(2.95f, 0.07f, 1.25f), SauryPalette.Laminate, collider: true, smoothness: 0.45f);
            // Front panel faces audience (-Z)
            MeshKit.GlowPart(desk, "FrontGlow", PrimitiveType.Cube,
                new Vector3(0f, 0.55f, -0.58f), new Vector3(2.6f, 0.1f, 0.05f),
                SauryPalette.BruisedPurple, SauryPalette.NeonPink, 0.9f);

            MeshKit.Part(desk, "Nameplate", PrimitiveType.Cube,
                new Vector3(0f, 0.72f, -0.56f), new Vector3(1.05f, 0.3f, 0.06f), SauryPalette.DirtyPlastic, smoothness: 0.25f);
            MeshKit.GlowPart(desk, "NameGlow", PrimitiveType.Cube,
                new Vector3(0f, 0.72f, -0.6f), new Vector3(0.9f, 0.14f, 0.03f),
                new Color(0.2f, 0.05f, 0.08f), SauryPalette.NeonAmber, 1.2f);

            // Mic boom toward audience
            MeshKit.Part(desk, "MicBase", PrimitiveType.Cylinder,
                new Vector3(0.55f, 1.15f, -0.2f), new Vector3(0.14f, 0.04f, 0.14f), SauryPalette.Metal, metallic: 0.65f);
            MeshKit.PartRot(desk, "MicArm", PrimitiveType.Capsule,
                new Vector3(0.55f, 1.38f, -0.35f), new Vector3(50f, 0f, 0f),
                new Vector3(0.045f, 0.2f, 0.045f), SauryPalette.Metal, metallic: 0.6f);
            MeshKit.Part(desk, "MicHead", PrimitiveType.Sphere,
                new Vector3(0.55f, 1.62f, -0.5f), Vector3.one * 0.15f, SauryPalette.Metal, metallic: 0.7f, smoothness: 0.55f);

            // Tiny CRT / cue monitor
            MeshKit.Part(desk, "CRT", PrimitiveType.Cube,
                new Vector3(-0.85f, 1.35f, 0.15f), new Vector3(0.55f, 0.4f, 0.35f), SauryPalette.DirtyPlastic, smoothness: 0.2f);
            MeshKit.GlowPart(desk, "CRTScreen", PrimitiveType.Cube,
                new Vector3(-0.85f, 1.35f, -0.05f), new Vector3(0.42f, 0.28f, 0.05f),
                new Color(0.1f, 0.2f, 0.15f), new Color(0.3f, 0.9f, 0.45f), 1.6f);

            // Coffee / ashtray props
            MeshKit.Part(desk, "Mug", PrimitiveType.Cylinder,
                new Vector3(-0.45f, 1.18f, -0.25f), new Vector3(0.12f, 0.1f, 0.12f), SauryPalette.DirtyPlastic);
            MeshKit.Part(desk, "Ashtray", PrimitiveType.Cylinder,
                new Vector3(0.15f, 1.14f, -0.3f), new Vector3(0.16f, 0.03f, 0.16f), SauryPalette.Metal, metallic: 0.5f);
        }

        static void BuildLogo(Transform root)
        {
            MeshKit.Part(root, "LogoFrame", PrimitiveType.Cube,
                new Vector3(0f, 3.75f, 5.25f), new Vector3(3.6f, 0.95f, 0.2f), SauryPalette.StageBlack, smoothness: 0.1f);
            MeshKit.GlowPart(root, "LogoBox", PrimitiveType.Cube,
                new Vector3(0f, 3.75f, 5.18f), new Vector3(3.2f, 0.7f, 0.12f),
                new Color(0.2f, 0.05f, 0.08f), SauryPalette.NeonPink, 2.4f);
            // Fake letter blocks
            for (int i = 0; i < 5; i++)
            {
                MeshKit.GlowPart(root, $"LogoLetter_{i}", PrimitiveType.Cube,
                    new Vector3(-1.2f + i * 0.6f, 3.75f, 5.1f), new Vector3(0.35f, 0.4f, 0.05f),
                    SauryPalette.NeonAmber, SauryPalette.NeonAmber, 1.8f);
            }
        }

        static void BuildPracticalSpots(Transform root)
        {
            // Buckshot ref 05 — hanging practical cans, subject lit, crushed blacks
            BuildPracticalSpot(root, "SpotKey", new Vector3(1.4f, 4.4f, 0.2f), new Vector3(0f, 1.55f, 2.7f),
                new Color(1f, 0.88f, 0.62f), 7.2f, 34f);
            BuildPracticalSpot(root, "SpotKey2", new Vector3(-1.3f, 4.35f, 0.4f), new Vector3(0f, 1.45f, 2.5f),
                new Color(1f, 0.78f, 0.5f), 4.8f, 30f);
            BuildPracticalSpot(root, "SpotDad", new Vector3(-1.8f, 4.2f, -2.2f), new Vector3(0f, 1.0f, -1.3f),
                new Color(1f, 0.82f, 0.58f), 2.4f, 40f);
            BuildPracticalSpot(root, "SpotRim", new Vector3(0f, 4.5f, 4.4f), new Vector3(0f, 1.6f, 2.6f),
                new Color(1f, 0.45f, 0.32f), 1.4f, 28f);
        }

        public static void BuildPracticalSpot(Transform root, string name, Vector3 fixturePos, Vector3 aimAt, Color color, float intensity, float angle)
        {
            var go = new GameObject(name);
            go.transform.SetParent(root, false);
            go.transform.position = fixturePos;
            go.transform.LookAt(aimAt);

            MeshKit.Part(go.transform, "Yoke", PrimitiveType.Cube,
                new Vector3(0f, 0.2f, 0f), new Vector3(0.08f, 0.45f, 0.08f), SauryPalette.Metal, metallic: 0.6f);
            MeshKit.Part(go.transform, "Can", PrimitiveType.Cylinder,
                Vector3.zero, new Vector3(0.4f, 0.28f, 0.4f), SauryPalette.Metal, metallic: 0.65f);
            MeshKit.GlowPart(go.transform, "Lens", PrimitiveType.Sphere,
                new Vector3(0f, 0f, 0.22f), Vector3.one * 0.3f, color, color, 2.8f);

            var light = go.AddComponent<Light>();
            light.type = LightType.Spot;
            light.color = color;
            light.intensity = intensity;
            light.range = 16f;
            light.spotAngle = angle;
            light.innerSpotAngle = angle * 0.45f;
            light.shadows = LightShadows.Soft;
            light.shadowStrength = 0.95f;
        }

        static void BuildDadMarks(Transform root)
        {
            for (int i = 0; i < 4; i++)
            {
                var stool = new GameObject($"DadMark_{i}").transform;
                stool.SetParent(root, false);
                stool.position = new Vector3(-2.7f + i * 1.8f, 0f, -1.5f);
                MeshKit.Part(stool, "Base", PrimitiveType.Cylinder,
                    new Vector3(0f, 0.04f, 0f), new Vector3(0.45f, 0.04f, 0.45f), SauryPalette.Metal, metallic: 0.55f);
                MeshKit.Part(stool, "Pole", PrimitiveType.Cylinder,
                    new Vector3(0f, 0.25f, 0f), new Vector3(0.07f, 0.22f, 0.07f), SauryPalette.Metal, metallic: 0.55f);
                MeshKit.Part(stool, "Seat", PrimitiveType.Cylinder,
                    new Vector3(0f, 0.48f, 0f), new Vector3(0.55f, 0.06f, 0.55f), SauryPalette.DirtyPlastic, collider: true, smoothness: 0.2f);
                // Floor mark circle under stool (spotlight pool hint)
                MeshKit.GlowPart(stool, "MarkGlow", PrimitiveType.Cylinder,
                    new Vector3(0f, 0.015f, 0f), new Vector3(0.7f, 0.01f, 0.7f),
                    SauryPalette.Floor, new Color(1f, 0.85f, 0.55f), 0.25f);
            }
        }
    }
}
