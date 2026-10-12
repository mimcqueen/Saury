using UnityEngine;
using UnityEngine.UI;

namespace Saury.UI
{
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

            hud.phaseText = MakeText(go.transform, "Phase", 28, TextAnchor.UpperLeft, new Vector2(20, -20), new Vector2(800, 40));
            hud.hostCard = MakeText(go.transform, "HostCard", 44, TextAnchor.UpperCenter, new Vector2(0, -70), new Vector2(1100, 60));
            hud.hostSubtitle = MakeText(go.transform, "HostSub", 22, TextAnchor.UpperCenter, new Vector2(0, -130), new Vector2(1000, 40));
            hud.bodyText = MakeText(go.transform, "Body", 24, TextAnchor.MiddleLeft, new Vector2(40, -40), new Vector2(900, 520));
            hud.babyTitle = MakeText(go.transform, "BabyTitle", 32, TextAnchor.LowerCenter, new Vector2(0, 140), new Vector2(1000, 40));
            hud.babyRoast = MakeText(go.transform, "BabyRoast", 22, TextAnchor.LowerCenter, new Vector2(0, 100), new Vector2(1000, 36));
            hud.babyPrompt = MakeText(go.transform, "BabyPrompt", 14, TextAnchor.LowerCenter, new Vector2(0, 60), new Vector2(1100, 40));
            hud.starsText = MakeText(go.transform, "Stars", 20, TextAnchor.UpperRight, new Vector2(-20, -20), new Vector2(420, 120));
            hud.controlsText = MakeText(go.transform, "Controls", 16, TextAnchor.LowerLeft, new Vector2(20, 20), new Vector2(700, 40));

            hud.hostCard.alignment = TextAnchor.UpperCenter;
            hud.hostSubtitle.alignment = TextAnchor.UpperCenter;
            hud.babyTitle.alignment = TextAnchor.LowerCenter;
            hud.babyRoast.alignment = TextAnchor.LowerCenter;
            hud.babyPrompt.alignment = TextAnchor.LowerCenter;
            hud.starsText.alignment = TextAnchor.UpperRight;

            hud.controlsText.text = "Esc=quit play mode tip · F1=skip phase (dev)";
            return hud;
        }

        static Text MakeText(Transform parent, string name, int size, TextAnchor anchor, Vector2 pos, Vector2 sizeDelta)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var rt = go.AddComponent<RectTransform>();
            rt.anchorMin = anchorMin(anchor);
            rt.anchorMax = anchorMin(anchor);
            rt.pivot = pivot(anchor);
            rt.anchoredPosition = pos;
            rt.sizeDelta = sizeDelta;
            var text = go.AddComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (text.font == null)
                text.font = Font.CreateDynamicFontFromOSFont("Arial", size);
            text.fontSize = size;
            text.color = new Color(0.95f, 0.9f, 0.75f);
            text.alignment = anchor;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            text.text = "";
            return text;
        }

        static Vector2 anchorMin(TextAnchor a) => a switch
        {
            TextAnchor.UpperLeft => new Vector2(0, 1),
            TextAnchor.UpperRight => new Vector2(1, 1),
            TextAnchor.UpperCenter => new Vector2(0.5f, 1),
            TextAnchor.LowerLeft => new Vector2(0, 0),
            TextAnchor.LowerCenter => new Vector2(0.5f, 0),
            TextAnchor.MiddleLeft => new Vector2(0, 0.5f),
            _ => new Vector2(0.5f, 0.5f)
        };

        static Vector2 pivot(TextAnchor a) => anchorMin(a);
    }
}
