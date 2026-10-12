using UnityEngine;

namespace Saury.Bootstrap
{
    /// <summary>Dingy VHS-adjacent material look: dirty plastics, nicotine yellows, crushed blacks.</summary>
    public static class SauryPalette
    {
        public static readonly Color StageBlack = new Color(0.07f, 0.06f, 0.07f);
        public static readonly Color Floor = new Color(0.14f, 0.11f, 0.10f);
        public static readonly Color NicotineCurtain = new Color(0.42f, 0.28f, 0.14f);
        public static readonly Color DirtyWood = new Color(0.28f, 0.18f, 0.11f);
        public static readonly Color DirtyPlastic = new Color(0.32f, 0.30f, 0.28f);
        public static readonly Color HostSuit = new Color(0.45f, 0.12f, 0.14f);
        public static readonly Color HostShirt = new Color(0.78f, 0.72f, 0.62f);
        public static readonly Color SkinWarm = new Color(0.82f, 0.68f, 0.55f);
        public static readonly Color SkinSickly = new Color(0.72f, 0.78f, 0.62f);
        public static readonly Color EyeWhite = new Color(0.92f, 0.90f, 0.78f);
        public static readonly Color EyeIris = new Color(0.15f, 0.12f, 0.08f);
        public static readonly Color LaneWood = new Color(0.48f, 0.28f, 0.16f);
        public static readonly Color Metal = new Color(0.45f, 0.46f, 0.48f);
        public static readonly Color PeeYellow = new Color(0.92f, 0.82f, 0.22f);
        public static readonly Color GrillBlack = new Color(0.12f, 0.12f, 0.13f);
        public static readonly Color Flame = new Color(1f, 0.42f, 0.08f);
        public static readonly Color NeonPink = new Color(1f, 0.35f, 0.55f);
        public static readonly Color NeonCyan = new Color(0.25f, 0.85f, 0.95f);

        public static Color CreatureBody(Core.CreatureId id) => id switch
        {
            Core.CreatureId.Human => new Color(0.78f, 0.62f, 0.48f),
            Core.CreatureId.Goblin => new Color(0.42f, 0.62f, 0.32f),
            Core.CreatureId.Catgirl => new Color(0.85f, 0.55f, 0.45f),
            Core.CreatureId.Cryptid => new Color(0.35f, 0.38f, 0.45f),
            Core.CreatureId.Blob => new Color(0.55f, 0.72f, 0.55f),
            Core.CreatureId.Imp => new Color(0.72f, 0.28f, 0.22f),
            _ => DirtyPlastic
        };
    }
}
