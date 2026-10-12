using System;
using System.Collections.Generic;
using System.Text;
using Saury.Dad;

namespace Saury.Baby
{
    [Serializable]
    public class BabyHistory
    {
        public string anchorMom = "tired trailer-park mom, nicotine curtains, Maury stage lighting";
        public List<string> winnerStack = new List<string>();
        public List<string> traitLocks = new List<string>();
        public string chaosSalt = "tiny trucker hat";
        public string lastBlurb = "";
        public int generation;

        public void PushWinner(DadSeed winner)
        {
            if (winner == null) return;
            winnerStack.Add($"{winner.displayName}:{winner.creature}");
            MergeTraits(winner.DescriptorList());
            generation++;
            chaosSalt = PickSalt(generation + winner.slotIndex);
        }

        void MergeTraits(List<string> incoming)
        {
            if (incoming == null || incoming.Count == 0) return;
            int take = Math.Min(2, incoming.Count);
            for (int i = 0; i < take; i++)
            {
                string t = incoming[i];
                if (string.IsNullOrWhiteSpace(t)) continue;
                if (traitLocks.Contains(t)) continue;
                traitLocks.Add(t);
                if (traitLocks.Count > 3)
                    traitLocks.RemoveAt(0);
            }
        }

        public string BuildPrompt(DadSeed winner)
        {
            var sb = new StringBuilder();
            sb.Append("cursed cute talk-show infant, ");
            sb.Append(anchorMom);
            sb.Append(", ");
            if (traitLocks.Count > 0)
            {
                sb.Append(string.Join(", ", traitLocks));
                sb.Append(", ");
            }
            if (winner != null)
            {
                sb.Append("drifting toward ");
                sb.Append(winner.PromptTraits());
                sb.Append(", ");
            }
            sb.Append(chaosSalt);
            sb.Append(", bowling-alley spotlight, VHS still");
            return sb.ToString();
        }

        static string PickSalt(int n)
        {
            string[] salts =
            {
                "tiny trucker hat",
                "spilled BBQ sauce cheek",
                "goblin ears",
                "pixel mullet",
                "one missing sock",
                "animatronic wave",
                "crt scanline freckles",
                "folding-chair crown"
            };
            return salts[Math.Abs(n) % salts.Length];
        }
    }
}
