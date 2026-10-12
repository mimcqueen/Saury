using System.Collections;
using System.Text;
using Saury.Baby;
using Saury.CameraRig;
using Saury.Dad;
using Saury.Host;
using Saury.Lobby;
using Saury.Minigames;
using Saury.UI;
using UnityEngine;

namespace Saury.Core
{
    /// <summary>
    /// Vertical slice flow:
    /// lobby → make dad → mini 1 → baby showcase → mini 2 → baby showcase → crown
    /// </summary>
    public class GameFlowController : MonoBehaviour
    {
        public LocalLobbyStub lobby = new LocalLobbyStub();
        public PlayerScoreboard scoreboard = new PlayerScoreboard();
        public GamePhase phase = GamePhase.Lobby;

        public HostPresenter host;
        public StageCameraDirector cams;
        public ShowHud hud;
        public MakeDadController makeDad;
        public MockBabyGenerator babyGen;
        public BabyShowcaseDirector showcase;
        public PissingContestMinigame pissing;
        public BbqBrawlMinigame bbq;
        public Transform worldRoot;

        int _lastWinnerSlot;
        bool _busy;

        public void KickOff()
        {
            StartCoroutine(RunShow());
        }

        IEnumerator RunShow()
        {
            // --- LOBBY ---
            phase = GamePhase.Lobby;
            lobby.OpenLocal(4);
            scoreboard.EnsureSlots(lobby.seats);
            cams.CutTo(cams.lobbyOrbit);
            host.Say("LOCAL LOBBY", $"Code {lobby.roomCode} · owner is still a dad · Host is CPU");
            hud.phaseText.text = "LOBBY";
            hud.bodyText.text = LobbyBlurb();
            RefreshStars();

            float lobbyT = 0f;
            while (lobbyT < 4.5f)
            {
                lobbyT += Time.deltaTime;
                cams.TickLobbyOrbit(Time.time);
                if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.F1)) break;
                yield return null;
            }

            // --- MAKE DAD ---
            phase = GamePhase.MakeDad;
            cams.BlendTo(cams.hostPush, 0.6f);
            host.Say("MAKE DAD", "wah — seed the DNA machine");
            hud.phaseText.text = "MAKE DAD";
            makeDad.Begin(lobby, hud.bodyText, hud.hostSubtitle);
            while (!makeDad.AllDone)
            {
                if (Input.GetKeyDown(KeyCode.F1))
                {
                    foreach (var s in lobby.seats) s.ready = true;
                }
                RefreshStars();
                yield return null;
            }
            makeDad.End();

            // Stub Mom seed into history
            babyGen.History.anchorMom = "tired Maury mom, nicotine curtains, eyes in the dark";

            // --- HUB INTRO ---
            phase = GamePhase.HubIntro;
            cams.BlendTo(cams.hostPush, 0.4f);
            hud.bodyText.text = "";
            yield return host.SayAndHold("TONIGHT ON SAURY!", "Two trash minis. One cursed custody baby.", 3.2f);

            // --- MINI 1 ---
            yield return RunMini1();
            yield return RunBabyShowcase(1);

            // --- MINI 2 ---
            yield return RunMini2();
            yield return RunBabyShowcase(2);

            // --- CROWN ---
            phase = GamePhase.Crown;
            cams.BlendTo(cams.podium, 0.5f);
            int crown = scoreboard.CrownWinnerSlot();
            var champ = lobby.seats.Find(d => d.slotIndex == crown);
            string champName = champ != null ? champ.displayName : $"P{crown + 1}";
            host.Say("CROWN OF CUSTODY", $"{champName} takes the night · screenshot the crime");
            hud.phaseText.text = "CROWN";
            hud.bodyText.text = FinalCard(champName);
            RefreshStars();
            yield return new WaitForSeconds(6f);

            phase = GamePhase.RematchPrompt;
            host.Say("REMATCH?", "Enter from hub · Esc ends play mode");
            hud.phaseText.text = "REMATCH?";
            while (!Input.GetKeyDown(KeyCode.Return) && !Input.GetKeyDown(KeyCode.F1))
                yield return null;

            // Soft rematch: restart flow
            KickOff();
        }

        IEnumerator RunMini1()
        {
            phase = GamePhase.Mini1Announce;
            cams.BlendTo(cams.hostPush, 0.35f);
            yield return host.SayAndHold("NEXT MINI", "PISSING CONTEST — cartoon arcs, cans, Host's plant", 2.8f);

            phase = GamePhase.Mini1Play;
            yield return cams.WhipTo(cams.miniWide, 0.3f);
            hud.phaseText.text = "MINI 1";
            pissing.Build(worldRoot, lobby.seats, hud.bodyText);
            int winner = 0;
            int[] places = null;
            pissing.onFinished = (w, p) => { winner = w; places = p; };
            yield return pissing.Run();
            ApplyPlaces(places);
            _lastWinnerSlot = winner;
            pissing.Teardown();
            host.Say("PODIUM", $"{NameOf(winner)} wins the hose");
            yield return new WaitForSeconds(1.5f);
        }

        IEnumerator RunMini2()
        {
            phase = GamePhase.Mini2Announce;
            cams.BlendTo(cams.hostPush, 0.35f);
            yield return host.SayAndHold("NEXT MINI", "BACKYARD BBQ BRAWL — Gang Beasts–lite grill war", 2.8f);

            phase = GamePhase.Mini2Play;
            yield return cams.WhipTo(cams.miniWide, 0.3f);
            hud.phaseText.text = "MINI 2";
            bbq.Build(worldRoot, lobby.seats, hud.bodyText);
            int winner = 0;
            int[] places = null;
            bbq.onFinished = (w, p) => { winner = w; places = p; };
            yield return bbq.Run();
            ApplyPlaces(places);
            _lastWinnerSlot = winner;
            bbq.Teardown();
            host.Say("PODIUM", $"{NameOf(winner)} owns the grill");
            yield return new WaitForSeconds(1.5f);
        }

        IEnumerator RunBabyShowcase(int index)
        {
            phase = index == 1 ? GamePhase.BabyShowcase1 : GamePhase.BabyShowcase2;
            cams.BlendTo(cams.babyLane, 0.45f);
            hud.phaseText.text = $"BABY SHOWCASE {index}";
            hud.bodyText.text = "";
            host.Say("DNA REVEAL", "wah-wah — bowling alley lights");

            var winner = lobby.seats.Find(d => d.slotIndex == _lastWinnerSlot);
            yield return babyGen.GenerateTowardWinner(winner, 1.2f);
            yield return showcase.PlayReveal(babyGen.LastBaby, 5.5f);

            // Eyes cut beat
            yield return cams.WhipTo(cams.eyesCut, 0.25f);
            host.Say("AUDIENCE", "…wah.");
            yield return new WaitForSeconds(1.2f);
            cams.BlendTo(cams.hostPush, 0.4f);
        }

        void ApplyPlaces(int[] places)
        {
            if (places == null) return;
            for (int slot = 0; slot < places.Length; slot++)
                scoreboard.Award(slot, places[slot]);
            RefreshStars();
        }

        void RefreshStars()
        {
            if (hud == null || lobby == null) return;
            var sb = new StringBuilder();
            sb.AppendLine("CUSTODY STARS");
            foreach (var d in lobby.seats)
                sb.AppendLine($"{d.displayName}: {scoreboard.Stars(d.slotIndex)}★ {(d.ready ? "" : "")}");
            hud.starsText.text = sb.ToString();
        }

        string NameOf(int slot)
        {
            var d = lobby.seats.Find(x => x.slotIndex == slot);
            return d != null ? d.displayName : $"P{slot + 1}";
        }

        string LobbyBlurb()
        {
            return "SAURY! local lobby stub\n" +
                   $"Code: {lobby.roomCode}\n" +
                   $"{lobby.seats.Count} dads seated · Host = CPU NPC\n\n" +
                   "Enter = start Make Dad early\n" +
                   "Same-PC controls later: P1 WASD  P2 Arrows  P3 IJKL  P4 TFGH";
        }

        string FinalCard(string champ)
        {
            var baby = babyGen.LastBaby;
            return $"NIGHT WINNER: {champ}\n\n" +
                   $"Final baby: {(baby != null ? baby.title : "(none)")}\n" +
                   $"{(baby != null ? baby.roastLine : "")}\n\n" +
                   "Souvenir = this still + stars + VHS chrome.\n" +
                   "Enter = rematch from hub.";
        }
    }
}
