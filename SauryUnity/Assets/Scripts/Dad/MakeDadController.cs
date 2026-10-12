using System.Collections.Generic;
using Saury.Core;
using Saury.Lobby;
using UnityEngine;
using UnityEngine.UI;

namespace Saury.Dad
{
    /// <summary>Make Dad stubs — enough seed for baby gen inputs. Mouse morph summarized as hint.</summary>
    public class MakeDadController : MonoBehaviour
    {
        public LocalLobbyStub lobby;
        public Text panelText;
        public Text hintText;

        int _editSlot;
        bool _active;
        Vector2 _morph;

        static readonly string[] Hair = { "mullet", "high-and-tight", "greasy bangs", "balding island", "rat tail" };
        static readonly string[] Eyes = { "bloodshot", "shifty", "doe-eyed", "pixel squint", "glowing" };
        static readonly string[] Props = { "trucker's hat", "folding chair", "BBQ tongs", "unpaid tab", "lawnmower key" };
        static readonly string[] Energy = { "deadbeat swagger", "nervous laugh", "courtroom sweat", "grill boss", "wah victim" };

        public void Begin(LocalLobbyStub lobbyStub, Text panel, Text hint)
        {
            lobby = lobbyStub;
            panelText = panel;
            hintText = hint;
            _editSlot = 0;
            _active = true;
            _morph = Vector2.zero;
            Refresh();
        }

        public bool AllDone => lobby != null && lobby.AllReady();

        void Update()
        {
            if (!_active || lobby == null) return;
            var dad = Current();
            if (dad == null) return;

            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
                CycleCreature(dad, -1);
            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
                CycleCreature(dad, 1);
            if (Input.GetKeyDown(KeyCode.Alpha1)) dad.hairTag = Next(Hair, dad.hairTag);
            if (Input.GetKeyDown(KeyCode.Alpha2)) dad.eyesTag = Next(Eyes, dad.eyesTag);
            if (Input.GetKeyDown(KeyCode.Alpha3)) dad.propTag = Next(Props, dad.propTag);
            if (Input.GetKeyDown(KeyCode.Alpha4)) dad.energyTag = Next(Energy, dad.energyTag);
            if (Input.GetKeyDown(KeyCode.Alpha5))
                dad.realism = dad.realism == RealismStyle.SmoothTV ? RealismStyle.Pixel : RealismStyle.SmoothTV;

            // Mouse face morph → prompt hint
            if (Input.GetMouseButton(0))
            {
                _morph.x = Mathf.Clamp(_morph.x + Input.GetAxis("Mouse X") * 0.05f, -1f, 1f);
                _morph.y = Mathf.Clamp(_morph.y + Input.GetAxis("Mouse Y") * 0.05f, -1f, 1f);
                dad.morphHint = MorphToHint(_morph);
            }

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                _editSlot = (_editSlot + 1) % lobby.seats.Count;
                _morph = Vector2.zero;
            }

            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
            {
                dad.ready = true;
                if (!AllDone)
                {
                    // jump to next unready
                    for (int i = 0; i < lobby.seats.Count; i++)
                    {
                        int idx = (_editSlot + 1 + i) % lobby.seats.Count;
                        if (!lobby.seats[idx].ready) { _editSlot = idx; break; }
                    }
                }
            }

            // Dev: F5 ready-all
            if (Input.GetKeyDown(KeyCode.F5))
            {
                foreach (var s in lobby.seats) s.ready = true;
            }

            Refresh();
        }

        public void End() => _active = false;

        DadSeed Current() =>
            lobby != null && lobby.seats.Count > 0 ? lobby.seats[_editSlot] : null;

        void CycleCreature(DadSeed dad, int dir)
        {
            int v = ((int)dad.creature + dir + 6) % 6;
            dad.creature = (CreatureId)v;
            dad.bodyColor = Color.HSVToRGB(v / 6f, 0.5f, 0.8f);
        }

        void Refresh()
        {
            if (panelText == null || lobby == null) return;
            var dad = Current();
            var lines = new List<string>
            {
                "MAKE DAD — seed pack for AI baby",
                $"Room {lobby.roomCode}  (owner slot {lobby.ownerSlot + 1} still plays as dad)",
                $"Editing seat {_editSlot + 1}/{lobby.seats.Count}: {dad.displayName}",
                "",
                $"Creature ←/→ : {dad.creature}",
                $"[1] Hair: {dad.hairTag}",
                $"[2] Eyes: {dad.eyesTag}",
                $"[3] Prop: {dad.propTag}",
                $"[4] Energy: {dad.energyTag}",
                $"[5] Style: {dad.realism}",
                $"Morph (drag mouse): {dad.morphHint}",
                $"Ready: {(dad.ready ? "YES" : "no")}   Enter=ready  Tab=next seat  F5=ready all"
            };
            panelText.text = string.Join("\n", lines);
            if (hintText != null)
                hintText.text = "Selfie optional (R1) — descriptors + morph always suffice.";
        }

        static string Next(string[] opts, string cur)
        {
            int i = System.Array.IndexOf(opts, cur);
            i = (i + 1 + opts.Length) % opts.Length;
            return opts[i];
        }

        static string MorphToHint(Vector2 m)
        {
            if (m.magnitude < 0.15f) return "default mug";
            string x = m.x > 0.35f ? "wide grin" : m.x < -0.35f ? "long face" : "smushed";
            string y = m.y > 0.35f ? "high brow" : m.y < -0.35f ? "chin shelf" : "soft cheeks";
            return $"{x}, {y}";
        }
    }
}
