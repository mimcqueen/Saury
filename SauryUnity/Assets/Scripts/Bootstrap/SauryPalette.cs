using UnityEngine;

namespace Saury.Bootstrap
{
    /// <summary>Eerie dingy Maury palette — nicotine, bruise, dirty plastic, crushed blacks.</summary>
    public static class SauryPalette
    {
        public static readonly Color StageBlack = new Color(0.02f, 0.018f, 0.022f);
        public static readonly Color Floor = new Color(0.16f, 0.12f, 0.09f);
        public static readonly Color FloorStain = new Color(0.12f, 0.08f, 0.06f);
        public static readonly Color NicotineCurtain = new Color(0.48f, 0.32f, 0.16f);
        public static readonly Color CurtainShadow = new Color(0.22f, 0.12f, 0.07f);
        public static readonly Color DirtyWood = new Color(0.32f, 0.2f, 0.12f);
        public static readonly Color Laminate = new Color(0.18f, 0.12f, 0.09f);
        public static readonly Color DirtyPlastic = new Color(0.38f, 0.34f, 0.3f);
        public static readonly Color HostSuit = new Color(0.38f, 0.08f, 0.1f);
        public static readonly Color HostSuitShadow = new Color(0.22f, 0.04f, 0.06f);
        public static readonly Color HostShirt = new Color(0.82f, 0.74f, 0.58f);
        public static readonly Color SkinWarm = new Color(0.86f, 0.68f, 0.52f);
        public static readonly Color SkinSweat = new Color(0.9f, 0.74f, 0.58f);
        public static readonly Color SkinSickly = new Color(0.7f, 0.78f, 0.58f);
        public static readonly Color Combover = new Color(0.18f, 0.12f, 0.08f);
        public static readonly Color EyeWhite = new Color(0.95f, 0.9f, 0.75f);
        public static readonly Color EyeIris = new Color(0.12f, 0.08f, 0.05f);
        public static readonly Color LaneWood = new Color(0.55f, 0.32f, 0.16f);
        public static readonly Color LaneOil = new Color(0.62f, 0.4f, 0.2f);
        public static readonly Color Metal = new Color(0.42f, 0.42f, 0.44f);
        public static readonly Color PeeYellow = new Color(0.92f, 0.82f, 0.22f);
        public static readonly Color GrillBlack = new Color(0.1f, 0.1f, 0.11f);
        public static readonly Color Flame = new Color(1f, 0.4f, 0.08f);
        public static readonly Color NeonPink = new Color(1f, 0.28f, 0.5f);
        public static readonly Color NeonAmber = new Color(1f, 0.7f, 0.25f);
        public static readonly Color NeonCyan = new Color(0.25f, 0.9f, 0.95f);
        public static readonly Color BruisedPurple = new Color(0.35f, 0.22f, 0.32f);
        public static readonly Color GrassDead = new Color(0.28f, 0.38f, 0.16f);
        public static readonly Color Chainlink = new Color(0.5f, 0.52f, 0.48f);

        public static Color CreatureBody(Core.CreatureId id) => id switch
        {
            Core.CreatureId.Human => new Color(0.8f, 0.62f, 0.48f),
            Core.CreatureId.Goblin => new Color(0.4f, 0.58f, 0.28f),
            Core.CreatureId.Catgirl => new Color(0.88f, 0.58f, 0.48f),
            Core.CreatureId.Cryptid => new Color(0.32f, 0.36f, 0.42f),
            Core.CreatureId.Blob => new Color(0.5f, 0.7f, 0.52f),
            Core.CreatureId.Imp => new Color(0.7f, 0.26f, 0.22f),
            _ => DirtyPlastic
        };

        public static Color CreatureAccent(Core.CreatureId id) => id switch
        {
            Core.CreatureId.Human => new Color(0.55f, 0.18f, 0.14f),
            Core.CreatureId.Goblin => new Color(0.25f, 0.4f, 0.18f),
            Core.CreatureId.Catgirl => new Color(0.95f, 0.75f, 0.8f),
            Core.CreatureId.Cryptid => new Color(0.55f, 0.5f, 0.35f),
            Core.CreatureId.Blob => new Color(0.7f, 0.85f, 0.7f),
            Core.CreatureId.Imp => new Color(0.95f, 0.85f, 0.55f),
            _ => DirtyPlastic
        };
    }
}
