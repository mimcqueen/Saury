using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Saury.Host
{
    /// <summary>CPU/NPC Host — wah + text; idle / talk pose on a modeled body.</summary>
    public class HostPresenter : MonoBehaviour
    {
        public Transform hostRoot;
        public Transform mouth;
        public Transform head;
        public Text cardText;
        public Text subtitleText;

        float _wahTimer;
        Vector3 _rootBase;
        Quaternion _headBase;

        static readonly string[] WahBits =
        {
            "wah-wah-WAH",
            "wah… wah? WAH!",
            "wuh-wahhh",
            "WAH wah wah.",
            "…wah."
        };

        void Start()
        {
            if (hostRoot != null) _rootBase = hostRoot.localPosition;
            if (head != null) _headBase = head.localRotation;
        }

        public void Say(string card, string roast = null)
        {
            if (cardText != null) cardText.text = card;
            if (subtitleText != null)
                subtitleText.text = string.IsNullOrEmpty(roast) ? RandomWah() : roast;
            _wahTimer = 1.35f;
        }

        public IEnumerator SayAndHold(string card, string roast, float hold)
        {
            Say(card, roast);
            yield return new WaitForSeconds(hold);
        }

        void Update()
        {
            // Idle breathe / slight lean
            if (hostRoot != null)
            {
                float bob = Mathf.Sin(Time.time * 1.4f) * 0.015f;
                float sway = Mathf.Sin(Time.time * 0.7f) * 0.8f;
                hostRoot.localPosition = _rootBase + new Vector3(0f, bob, 0f);
                hostRoot.localRotation = Quaternion.Euler(0f, sway, 0f);
            }

            if (head != null)
            {
                float nod = Mathf.Sin(Time.time * (_wahTimer > 0f ? 9f : 1.1f)) * (_wahTimer > 0f ? 6f : 2f);
                head.localRotation = _headBase * Quaternion.Euler(nod, Mathf.Sin(Time.time * 0.5f) * 3f, 0f);
            }

            if (mouth != null && _wahTimer > 0f)
            {
                _wahTimer -= Time.deltaTime;
                float flap = 1f + Mathf.Abs(Mathf.Sin(Time.time * 16f)) * 1.1f;
                mouth.localScale = new Vector3(1f, flap, 1f);
            }
            else if (mouth != null)
            {
                mouth.localScale = Vector3.one;
            }
        }

        static string RandomWah() => WahBits[Random.Range(0, WahBits.Length)];
    }
}
