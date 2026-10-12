using System;
using System.Collections.Generic;
using Saury.Core;
using UnityEngine;

namespace Saury.Dad
{
    [Serializable]
    public class DadSeed
    {
        public int slotIndex;
        public string displayName = "Dad";
        public CreatureId creature = CreatureId.Goblin;
        public RealismStyle realism = RealismStyle.SmoothTV;
        public string hairTag = "mullet";
        public string eyesTag = "bloodshot";
        public string propTag = "trucker's hat";
        public string energyTag = "deadbeat swagger";
        public string morphHint = "smushed grin";
        public string catchphrase = "I ain't the dad!";
        public Color bodyColor = new Color(0.55f, 0.72f, 0.42f);
        public bool ready;

        public List<string> DescriptorList()
        {
            return new List<string>
            {
                creature.ToString().ToLowerInvariant(),
                hairTag,
                eyesTag,
                propTag,
                energyTag,
                morphHint,
                realism == RealismStyle.Pixel ? "pixel TV baby" : "smooth daytime TV"
            };
        }

        public string PromptTraits()
        {
            return string.Join(", ", DescriptorList());
        }
    }
}
