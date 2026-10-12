using Saury.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Saury.UI
{
    /// <summary>
    /// Minimal TV-show chrome — phase title, timer, custody stars, short control hint.
    /// Not a console dump. Debug body panel only when DemoSettings.ShowDebug.
    /// </summary>
    public class ShowHud : MonoBehaviour
    {
        public Text phaseText;
        public Text timerText;
        public Text hostCard;
        public Text hostSubtitle;
        public Text bodyText;
        public Text babyTitle;
        public Text babyRoast;
        public Text babyPrompt;
        public Text starsText;
        public Text controlsText;
        public Text debugHint;

        public static ShowHud Create(Transform canvasRoot)
        {
            var go = new GameObject("ShowHud");
            go.transform.SetParent(canvasRoot, false);
            var hud = go.AddComponent<ShowHud>();

            // Top bar: phase (L) · host card (C) · timer + stars (R)
            hud.phaseText = MakeText(go.transform, "Phase", 20, TextAnchor.UpperLeft, new Vector2(28, -22), new Vector2(360, 28),
                new Color(0.95f, 0.88f, 0.7f, 0.9f));
            hud.hostCard = MakeText(go.transform, "HostCard", 32, TextAnchor.UpperCenter, new Vector2(0, -28), new Vector2(820, 40),
                new Color(1f, 0.92f, 0.75f, 0.95f));
            hud.hostSubtitle = MakeText(go.transform, "HostSub", 16, TextAnchor.UpperCenter, new Vector2(0, -68), new Vector2(720, 24),
                new Color(0.85f, 0.8f, 0.7f, 0.75f));
            hud.timerText = MakeText(go.transform, "Timer", 28, TextAnchor.UpperRight, new Vector2(-28, -22), new Vector2(120, 36),
                new Color(0.95f, 0.9f, 0.55f, 0.95f));
            hud.starsText = MakeText(go.transform, "Stars", 15, TextAnchor.UpperRight, new Vector2(-28, -58), new Vector2(260, 90),
                new Color(0.92f, 0.88f, 0.7f, 0.8f));

            // Debug / make-dad detail — hidden in demo mode
            hud.bodyText = MakeText(go.transform, "Body", 16, TextAnchor.LowerLeft, new Vector2(28, 48), new Vector2(480, 240),
                new Color(0.9f, 0.86f, 0.76f, 0.82f));

            // Baby chrome — lower center, sparse
            hud.babyTitle = MakeText(go.transform, "BabyTitle", 24, TextAnchor.LowerCenter, new Vector2(0, 96), new Vector2(860, 32),
                new Color(1f, 0.85f, 0.9f, 0.95f));
            hud.babyRoast = MakeText(go.transform, "BabyRoast", 16, TextAnchor.LowerCenter, new Vector2(0, 68), new Vector2(860, 26),
                new Color(0.9f, 0.82f, 0.75f, 0.88f));
            hud.babyPrompt = MakeText(go.transform, "BabyPrompt", 11, TextAnchor.LowerCenter, new Vector2(0, 46), new Vector2(920, 20),
                new Color(0.6f, 0.55f, 0.5f, 0.45f));

            hud.controlsText = MakeText(go.transform, "Controls", 13, TextAnchor.LowerRight, new Vector2(-28, 18), new Vector2(460, 26),
                new Color(0.72f, 0.7f, 0.62f, 0.55f));
            hud.debugHint = MakeText(go.transform, "DebugHint", 11, TextAnchor.LowerLeft, new Vector2(28, 14), new Vector2(360, 18),
                new Color(0.55f, 0.55f, 0.5f, 0.35f));

            hud.hostCard.alignment = TextAnchor.UpperCenter;
            hud.hostSubtitle.alignment = TextAnchor.UpperCenter;
            hud.timerText.alignment = TextAnchor.UpperRight;
            hud.babyTitle.alignment = TextAnchor.LowerCenter;
            hud.babyRoast.alignment = TextAnchor.LowerCenter;
            hud.babyPrompt.alignment = TextAnchor.LowerCenter;
            hud.starsText.alignment = TextAnchor.UpperRight;
            hud.controlsText.alignment = TextAnchor.LowerRight;

            hud.ApplyDemoChrome();
            return hud;
        }

        public void ApplyDemoChrome()
        {
            bool dbg = DemoSettings.ShowDebug;
            if (bodyText != null) bodyText.gameObject.SetActive(dbg);
            if (babyPrompt != null) babyPrompt.gameObject.SetActive(dbg);
            if (debugHint != null)
            {
                debugHint.gameObject.SetActive(true);
                debugHint.text = dbg
                    ? "DEBUG · F3 demo · F1 skip · F5 ready-all"
                    : "Demo · F1 skip · F3 debug";
            }
            if (controlsText != null && string.IsNullOrEmpty(controlsText.text))
                controlsText.text = "P1 WASD + Space";
        }

        public void SetTimer(float secondsOrNeg)
        {
            if (timerText == null) return;
            if (secondsOrNeg < 0f)
            {
                timerText.text = "";
                return;
            }
            timerText.text = Mathf.CeilToInt(secondsOrNeg).ToString();
        }

        public void SetPhase(string title) { if (phaseText != null) phaseText.text = title; }

        public void SetControlHint(string hint)
        {
            if (controlsText != null) controlsText.text = hint ?? "";
        }

        public void ClearBabyChrome()
        {
            if (babyTitle != null) babyTitle.text = "";
            if (babyRoast != null) babyRoast.text = "";
            if (babyPrompt != null) babyPrompt.text = "";
        }

        public void SetBody(string text)
        {
            if (bodyText == null) return;
            bodyText.text = text ?? "";
            bodyText.gameObject.SetActive(DemoSettings.ShowDebug && !string.IsNullOrEmpty(text));
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
