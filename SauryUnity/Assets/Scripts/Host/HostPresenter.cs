using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Saury.Host
{
    /// <summary>CPU/NPC Host — wah + text cards, never a player seat.</summary>
    public class HostPresenter : MonoBehaviour
    {
        public Transform hostRoot;
        public Transform mouth;
        public Text cardText;
        public Text subtitleText;

        float _wahTimer;
        string _pendingCard = "WELCOME TO SAURY!";

        static readonly string[] WahBits =
        {
            "wah-wah-WAH",
            "wah… wah? WAH!",
            "wuh-wahhh",
            "WAH wah wah.",
            "…wah."
        };

        public void Say(string card, string roast = null)
        {
            _pendingCard = card;
            if (cardText != null) cardText.text = card;
            if (subtitleText != null)
                subtitleText.text = string.IsNullOrEmpty(roast) ? RandomWah() : roast;
            _wahTimer = 1.2f;
        }

        public IEnumerator SayAndHold(string card, string roast, float hold)
        {
            Say(card, roast);
            yield return new WaitForSeconds(hold);
        }

        void Update()
        {
            if (mouth != null && _wahTimer > 0f)
            {
                _wahTimer -= Time.deltaTime;
                float flap = 1f + Mathf.Abs(Mathf.Sin(Time.time * 18f)) * 0.55f;
                mouth.localScale = new Vector3(1f, flap, 1f);
            }
            else if (mouth != null)
            {
                mouth.localScale = Vector3.one;
            }

            if (hostRoot != null)
            {
                float bob = Mathf.Sin(Time.time * 1.6f) * 0.03f;
                hostRoot.localPosition = new Vector3(0f, bob, 0f);
            }
        }

        static string RandomWah() => WahBits[Random.Range(0, WahBits.Length)];
    }
}
