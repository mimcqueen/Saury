using UnityEngine;

namespace Saury.Bootstrap
{
    /// <summary>Maury hub set dressing — real 3D scenery, not UI diagrams.</summary>
    public static class StageKit
    {
        public static void BuildHubSet(Transform root)
        {
            // Stage floor with slight lip
            MeshKit.Part(root, "StageFloor", PrimitiveType.Cube,
                new Vector3(0f, -0.1f, 0.5f), new Vector3(16f, 0.2f, 14f), SauryPalette.Floor, collider: true, smoothness: 0.08f);

            MeshKit.Part(root, "StageLip", PrimitiveType.Cube,
                new Vector3(0f, 0.05f, -5.5f), new Vector3(16f, 0.3f, 0.4f), SauryPalette.DirtyWood, collider: true);

            // Nicotine curtains (pleated as stacked panels)
            for (int i = 0; i < 7; i++)
            {
                float x = -6f + i * 2f;
                MeshKit.Part(root, $"CurtainPanel_{i}", PrimitiveType.Cube,
                    new Vector3(x, 2.6f, 5.6f), new Vector3(1.85f, 5.2f, 0.12f),
                    Color.Lerp(SauryPalette.NicotineCurtain, SauryPalette.StageBlack, (i % 2) * 0.15f),
                    smoothness: 0.12f);
            }

            // Side walls / crushed blacks
            MeshKit.Part(root, "WallL", PrimitiveType.Cube,
                new Vector3(-8.2f, 2.2f, 0f), new Vector3(0.3f, 5f, 14f), SauryPalette.StageBlack);
            MeshKit.Part(root, "WallR", PrimitiveType.Cube,
                new Vector3(8.2f, 2.2f, 0f), new Vector3(0.3f, 5f, 14f), SauryPalette.StageBlack);

            // Ceiling grid strip
            MeshKit.Part(root, "Ceiling", PrimitiveType.Cube,
                new Vector3(0f, 5.2f, 0.5f), new Vector3(16f, 0.15f, 14f),
                new Color(0.05f, 0.05f, 0.06f));

            // Host desk — layered wood + laminate + mic
            var desk = new GameObject("HostDesk").transform;
            desk.SetParent(root, false);
            desk.position = new Vector3(0f, 0f, 2.15f);
            MeshKit.Part(desk, "DeskBody", PrimitiveType.Cube,
                new Vector3(0f, 0.55f, 0f), new Vector3(2.6f, 1.0f, 1.1f), SauryPalette.DirtyWood, collider: true);
            MeshKit.Part(desk, "DeskTop", PrimitiveType.Cube,
                new Vector3(0f, 1.08f, 0.05f), new Vector3(2.7f, 0.08f, 1.2f),
                new Color(0.2f, 0.14f, 0.1f), collider: true, smoothness: 0.35f);
            MeshKit.Part(desk, "Nameplate", PrimitiveType.Cube,
                new Vector3(0f, 0.7f, 0.55f), new Vector3(0.9f, 0.25f, 0.06f), SauryPalette.DirtyPlastic);
            MeshKit.Part(desk, "MicBase", PrimitiveType.Cylinder,
                new Vector3(0.55f, 1.15f, 0.15f), new Vector3(0.12f, 0.04f, 0.12f), SauryPalette.Metal, metallic: 0.6f);
            MeshKit.PartRot(desk, "MicArm", PrimitiveType.Cylinder,
                new Vector3(0.55f, 1.35f, 0.05f), new Vector3(25f, 0f, 0f),
                new Vector3(0.04f, 0.22f, 0.04f), SauryPalette.Metal);
            MeshKit.Part(desk, "MicHead", PrimitiveType.Sphere,
                new Vector3(0.55f, 1.5f, -0.1f), Vector3.one * 0.12f, SauryPalette.Metal, metallic: 0.7f);

            // SAURY logo light box behind host
            var logo = MeshKit.GlowPart(root, "LogoBox", PrimitiveType.Cube,
                new Vector3(0f, 3.6f, 5.35f), new Vector3(3.2f, 0.7f, 0.15f),
                new Color(0.25f, 0.08f, 0.1f), SauryPalette.NeonPink, 1.8f);

            // Practical spotlights as hanging fixtures + Light components
            BuildPracticalSpot(root, "SpotKey", new Vector3(1.8f, 4.6f, -1.5f), new Vector3(0f, 1.2f, 2.2f),
                new Color(1f, 0.92f, 0.75f), 4.2f, 50f);
            BuildPracticalSpot(root, "SpotFill", new Vector3(-2.2f, 4.4f, -0.5f), new Vector3(0f, 1.4f, 2.2f),
                new Color(0.7f, 0.75f, 0.95f), 2.0f, 55f);
            BuildPracticalSpot(root, "SpotRim", new Vector3(0f, 4.5f, 4.0f), new Vector3(0f, 1.5f, 2.5f),
                new Color(1f, 0.7f, 0.5f), 1.6f, 40f);

            // Dad stools / marks
            for (int i = 0; i < 4; i++)
            {
                var stool = new GameObject($"DadMark_{i}").transform;
                stool.SetParent(root, false);
                stool.position = new Vector3(-2.6f + i * 1.7f, 0f, -1.4f);
                MeshKit.Part(stool, "Seat", PrimitiveType.Cylinder,
                    new Vector3(0f, 0.45f, 0f), new Vector3(0.55f, 0.06f, 0.55f), SauryPalette.DirtyPlastic, collider: true);
                MeshKit.Part(stool, "Pole", PrimitiveType.Cylinder,
                    new Vector3(0f, 0.22f, 0f), new Vector3(0.08f, 0.22f, 0.08f), SauryPalette.Metal, metallic: 0.5f);
                MeshKit.Part(stool, "Base", PrimitiveType.Cylinder,
                    new Vector3(0f, 0.04f, 0f), new Vector3(0.4f, 0.04f, 0.4f), SauryPalette.Metal, metallic: 0.5f);
            }

            // Audience riser platform in the dark
            MeshKit.Part(root, "AudienceRiser", PrimitiveType.Cube,
                new Vector3(0f, 0.15f, -6.2f), new Vector3(12f, 0.4f, 2.5f), SauryPalette.StageBlack, collider: true);
        }

        public static void BuildPracticalSpot(Transform root, string name, Vector3 fixturePos, Vector3 aimAt, Color color, float intensity, float angle)
        {
            var go = new GameObject(name);
            go.transform.SetParent(root, false);
            go.transform.position = fixturePos;
            go.transform.LookAt(aimAt);

            // Fixture housing
            MeshKit.Part(go.transform, "Can", PrimitiveType.Cylinder,
                Vector3.zero, new Vector3(0.35f, 0.25f, 0.35f), SauryPalette.Metal, metallic: 0.65f);
            MeshKit.GlowPart(go.transform, "Lens", PrimitiveType.Sphere,
                new Vector3(0f, 0f, 0.2f), Vector3.one * 0.28f, color, color, 2.2f);
            MeshKit.Part(go.transform, "Arm", PrimitiveType.Cylinder,
                new Vector3(0f, 0.35f, 0f), new Vector3(0.06f, 0.25f, 0.06f), SauryPalette.Metal);

            var light = go.AddComponent<Light>();
            light.type = LightType.Spot;
            light.color = color;
            light.intensity = intensity;
            light.range = 18f;
            light.spotAngle = angle;
            light.shadows = LightShadows.Soft;
        }
    }
}
