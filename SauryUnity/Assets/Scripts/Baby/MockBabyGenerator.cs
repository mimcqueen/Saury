using System.Collections;
using Saury.Dad;
using UnityEngine;

namespace Saury.Baby
{
    /// <summary>
    /// Offline / no-API mock AI baby. Funny placeholders that change each win.
    /// Swap GenerateCoroutine body for a real image/mesh API later.
    /// </summary>
    public class MockBabyGenerator : MonoBehaviour
    {
        public BabyHistory History { get; private set; } = new BabyHistory();
        public GeneratedBaby LastBaby { get; private set; }

        public IEnumerator GenerateTowardWinner(DadSeed winner, float fakeLatency = 1.4f)
        {
            History.PushWinner(winner);
            string prompt = History.BuildPrompt(winner);
            yield return new WaitForSeconds(fakeLatency);

            LastBaby = GeneratedBaby.FromMock(History, winner, prompt);
            History.lastBlurb = LastBaby.roastLine;
        }
    }

    [System.Serializable]
    public class GeneratedBaby
    {
        public string title;
        public string roastLine;
        public string promptUsed;
        public Color primary;
        public Color accent;
        public float earScale;
        public float headSquash;
        public int generation;
        public bool usedMockApi = true;

        public static GeneratedBaby FromMock(BabyHistory history, DadSeed winner, string prompt)
        {
            int g = history.generation;
            var rng = new System.Random(1337 + g * 97 + (winner?.slotIndex ?? 0) * 13);
            float h = (float)((g * 0.17 + rng.NextDouble() * 0.2) % 1.0);
            Color primary = Color.HSVToRGB(h, 0.45f + (float)rng.NextDouble() * 0.25f, 0.75f);
            Color accent = Color.HSVToRGB((h + 0.35f) % 1f, 0.55f, 0.9f);

            string[] mishaps =
            {
                "DNA MACHINE hiccuped a trucker hat onto the fontanel.",
                "AI swore this was photoreal. It lied with confidence.",
                "Half goblin, half CRT static, all custody nightmare.",
                "Mom's eyes. Dad's mullet. The Host's unpaid bills.",
                "Bowling-alley return spit out THIS instead of a 10-pin.",
                "Prompt said 'cute.' Model heard 'cursed talk-show prop.'"
            };

            string nameBit = winner != null ? winner.displayName : "Nobody";
            return new GeneratedBaby
            {
                title = $"CUSTODY UNIT #{g:00} — {nameBit}'s drift",
                roastLine = mishaps[g % mishaps.Length],
                promptUsed = prompt,
                primary = primary,
                accent = accent,
                earScale = 0.8f + (float)rng.NextDouble() * 0.9f,
                headSquash = 0.75f + (float)rng.NextDouble() * 0.45f,
                generation = g,
                usedMockApi = true
            };
        }
    }
}
