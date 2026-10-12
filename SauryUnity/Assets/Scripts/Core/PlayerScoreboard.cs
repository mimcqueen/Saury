using System.Collections.Generic;
using Saury.Dad;

namespace Saury.Core
{
    [System.Serializable]
    public class PlayerScoreboard
    {
        readonly Dictionary<int, int> _stars = new Dictionary<int, int>();

        public void EnsureSlots(IReadOnlyList<DadSeed> dads)
        {
            foreach (var d in dads)
            {
                if (!_stars.ContainsKey(d.slotIndex))
                    _stars[d.slotIndex] = 0;
            }
        }

        public void Award(int slot, int place)
        {
            int add = place switch
            {
                1 => 3,
                2 => 2,
                3 => 1,
                _ => 0
            };
            if (!_stars.ContainsKey(slot)) _stars[slot] = 0;
            _stars[slot] += add;
        }

        public int Stars(int slot) => _stars.TryGetValue(slot, out var s) ? s : 0;

        public int CrownWinnerSlot()
        {
            int bestSlot = 0;
            int best = -1;
            foreach (var kv in _stars)
            {
                if (kv.Value > best)
                {
                    best = kv.Value;
                    bestSlot = kv.Key;
                }
            }
            return bestSlot;
        }
    }
}
