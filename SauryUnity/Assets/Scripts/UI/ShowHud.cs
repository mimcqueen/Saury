using UnityEngine;
using UnityEngine.UI;

namespace Saury.UI
{
    /// <summary>Thin show chrome — 3D stage stays the hero, not a UI diagram.</summary>
    public class ShowHud : MonoBehaviour
    {
        public Text phaseText;
        public Text hostCard;
        public Text hostSubtitle;
        public Text bodyText;
        public Text babyTitle;
        public Text babyRoast;
        public Text babyPrompt;
        public Text starsText;
        public Text controlsText;

        public static ShowHud Create(Transform canvasRoot)
        {
            var go = new GameObject("ShowHud");
            go.transform.SetParent(canvasRoot, false);
            var hud = go.AddComponent<ShowHud>();

            hud.phaseText = MakeText(go.transform, "Phase", 22, TextAnchor.UpperLeft, new Vector2(28, -24), new Vector2(480, 32),
                new Color(0.95f, 0.88f, 0.7f, 0.85f));
            hud.hostCard = MakeText(go.transform, "HostCard", 34, TextAnchor.UpperCenter, new Vector2(0, -36), new Vector2(900, 44),
                new Color(1f, 0.92f, 0.75f, 0.95f));
            hud.hostSubtitle = MakeText(go.transform, "HostSub", 18, TextAnchor.UpperCenter, new Vector2(0, -78), new Vector2(800, 28),
                new Color(0.85f, 0.8f, 0.7f, 0.8f));
            // Compact lower-left panel for make-dad / mini scores — not a full-screen diagram
            hud.bodyText = MakeText(go.transform, "Body", 17, TextAnchor.LowerLeft, new Vector2(28, 56), new Vector2(520, 280),
                new Color(0.92f, 0.88f, 0.78f, 0.88f));
            hud.babyTitle = MakeText(go.transform, "BabyTitle", 26, TextAnchor.LowerCenter, new Vector2(0, 110), new Vector2(900, 36),
                new Color(1f, 0.85f, 0.9f, 0.95f));
            hud.babyRoast = MakeText(go.transform, "BabyRoast", 18, TextAnchor.LowerCenter, new Vector2(0, 78), new Vector2(900, 28),
                new Color(0.9f, 0.82f, 0.75f, 0.9f));
            hud.babyPrompt = MakeText(go.transform, "BabyPrompt", 12, TextAnchor.LowerCenter, new Vector2(0, 52), new Vector2(1000, 24),
                new Color(0.65f, 0.6f, 0.55f, 0.65f));
            hud.starsText = MakeText(go.transform, "Stars", 16, TextAnchor.UpperRight, new Vector2(-28, -24), new Vector2(280, 100),
                new Color(0.95f, 0.9f, 0.7f, 0.85f));
            hud.controlsText = MakeText(go.transform, "Controls", 13, TextAnchor.LowerRight, new Vector2(-28, 20), new Vector2(420, 28),
                new Color(0.7f, 0.68f, 0.62f, 0.55f));

            hud.hostCard.alignment = TextAnchor.UpperCenter;
            hud.hostSubtitle.alignment = TextAnchor.UpperCenter;
            hud.babyTitle.alignment = TextAnchor.LowerCenter;
            hud.babyRoast.alignment = TextAnchor.LowerCenter;
            hud.babyPrompt.alignment = TextAnchor.LowerCenter;
            hud.starsText.alignment = TextAnchor.UpperRight;
            hud.controlsText.alignment = TextAnchor.LowerRight;

            hud.controlsText.text = "F1 skip · F5 ready-all";
            return hud;
        }

        static Text MakeText(Transform parent, string name, int size, TextAnchor anchor, Vector2 pos, Vector2 sizeDelta, Color color)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var rt = go.AddComponent<RectTransform>();
            rt.anchorMin = Anchor(anchor);
            rt.anchorMax = Anchor(anchor);
            rt.pivot = Anchor(anchor);
            rt.anchoredPosition = pos;
            rt.sizeDelta = sizeDelta;
            var text = go.AddComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (text.font == null)
                text.font = Font.CreateDynamicFontFromOSFont("DejaVu Sans", size);
            text.fontSize = size;
            text.color = color;
            text.alignment = anchor;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            text.raycastTarget = false;
            text.text = "";
            return text;
        }

        static Vector2 Anchor(TextAnchor a) => a switch
        {
            TextAnchor.UpperLeft => new Vector2(0, 1),
            TextAnchor.UpperRight => new Vector2(1, 1),
            TextAnchor.UpperCenter => new Vector2(0.5f, 1),
            TextAnchor.LowerLeft => new Vector2(0, 0),
            TextAnchor.LowerRight => new Vector2(1, 0),
            TextAnchor.LowerCenter => new Vector2(0.5f, 0),
            TextAnchor.MiddleLeft => new Vector2(0, 0.5f),
            _ => new Vector2(0.5f, 0.5f)
        };
    }
}
