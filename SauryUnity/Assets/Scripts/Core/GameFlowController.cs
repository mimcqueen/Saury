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
    /// Demo flow: Lobby → Make Dad → Pissing Contest → Baby → BBQ Brawl → Baby → Crown.
    /// Demo Mode (default): clean HUD, AI seats 2–4, F1 skip. F3 toggles debug dump.
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

        public void KickOff()
        {
            StopAllCoroutines();
            StartCoroutine(RunShow());
        }

        void Update()
        {
            DemoSettings.TickHotkeys();
            if (hud != null) hud.ApplyDemoChrome();
        }

        IEnumerator RunShow()
        {
            // --- LOBBY ---
            phase = GamePhase.Lobby;
            lobby.OpenLocal(4);
            scoreboard.EnsureSlots(lobby.seats);
            cams.CutTo(cams.lobbyOrbit);
            host.Say("LOCAL LOBBY", DemoSettings.demoMode
                ? "Solo demo · P1 is you · seats 2–4 AI"
                : $"Code {lobby.roomCode} · Host is CPU");
            hud.SetPhase("LOBBY");
            hud.SetTimer(-1f);
            hud.ClearBabyChrome();
            hud.SetBody(LobbyBlurb());
            hud.SetControlHint(DemoSettings.demoMode
                ? "Enter start · F1 skip · P1 WASD+Space"
                : "Enter = start · F1 skip");
            RefreshStars();

            float lobbyWait = DemoSettings.demoMode ? 2.8f : 4.5f;
            float lobbyT = 0f;
            while (lobbyT < lobbyWait)
            {
                lobbyT += Time.deltaTime;
                cams.TickLobbyOrbit(Time.time);
                if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.F1)) break;
                yield return null;
            }

            // --- MAKE DAD ---
            phase = GamePhase.MakeDad;
            var makeCam = cams.makeDadTable != null ? cams.makeDadTable : cams.hostPush;
            cams.BlendTo(makeCam, 0.55f);
            host.Say("MAKE DAD", "wah — seed the DNA machine");
            hud.SetPhase("MAKE DAD");
            hud.SetTimer(-1f);
            hud.SetControlHint("←/→ creature · 1–5 tags · Enter ready · F5 all");
            // In demo: show make-dad panel even if body is "debug" — use body temporarily
            bool prevDebug = DemoSettings.debugHud;
            if (DemoSettings.demoMode) DemoSettings.debugHud = true;
            hud.ApplyDemoChrome();
            makeDad.Begin(lobby, hud.bodyText, hud.hostSubtitle);
            makeDad.demoAutoReadyOthers = DemoSettings.demoMode && DemoSettings.AiOtherSeats;

            float makeDadCap = DemoSettings.demoMode ? 18f : 90f;
            float makeT = 0f;
            while (!makeDad.AllDone && makeT < makeDadCap)
            {
                makeT += Time.deltaTime;
                if (Input.GetKeyDown(KeyCode.F1))
                {
                    foreach (var s in lobby.seats) s.ready = true;
                }
                RefreshStars();
                yield return null;
            }
            if (!makeDad.AllDone)
            {
                foreach (var s in lobby.seats) s.ready = true;
            }
            makeDad.End();
            DemoSettings.debugHud = prevDebug;
            hud.ApplyDemoChrome();
            hud.SetBody("");

            babyGen.History.anchorMom = "tired Maury mom, nicotine curtains, eyes in the dark";

            // --- HUB INTRO ---
            phase = GamePhase.HubIntro;
            cams.BlendTo(cams.hostPush, 0.4f);
            hud.SetPhase("TONIGHT");
            hud.SetControlHint("");
            yield return host.SayAndHold("TONIGHT ON SAURY!", "Two trash minis. One cursed custody baby.",
                DemoSettings.demoMode ? 2.2f : 3.2f);

            yield return RunMini1();
            yield return RunBabyShowcase(1);

            yield return RunMini2();
            yield return RunBabyShowcase(2);

            // --- CROWN ---
            phase = GamePhase.Crown;
            cams.BlendTo(cams.podium, 0.5f);
            int crown = scoreboard.CrownWinnerSlot();
            var champ = lobby.seats.Find(d => d.slotIndex == crown);
            string champName = champ != null ? champ.displayName : $"P{crown + 1}";
            host.Say("CROWN OF CUSTODY", $"{champName} takes the night · screenshot the crime");
            hud.SetPhase("CROWN");
            hud.SetTimer(-1f);
            hud.SetBody(FinalCard(champName));
            if (DemoSettings.demoMode)
            {
                // Show crown card briefly even in demo — use host subtitle for the punchline
                hud.hostSubtitle.text = $"Winner: {champName}";
            }
            RefreshStars();
            yield return WaitOrSkip(DemoSettings.demoMode ? 4.5f : 6f);

            phase = GamePhase.RematchPrompt;
            host.Say("REMATCH?", "Enter = rematch · Esc ends Play Mode");
            hud.SetPhase("REMATCH?");
            hud.SetControlHint("Enter rematch · F1 rematch");
            while (!Input.GetKeyDown(KeyCode.Return) && !Input.GetKeyDown(KeyCode.F1))
                yield return null;

            KickOff();
        }

        IEnumerator RunMini1()
        {
            phase = GamePhase.Mini1Announce;
            cams.BlendTo(cams.hostPush, 0.35f);
            yield return host.SayAndHold("NEXT MINI", "PISSING CONTEST — grimy bathroom cans",
                DemoSettings.demoMode ? 1.8f : 2.8f);

            phase = GamePhase.Mini1Play;
            hud.SetPhase("PISSING CONTEST");
            hud.SetControlHint("WASD move · Space spray");
            pissing.Build(worldRoot, lobby.seats, hud);
            cams.AimMiniHighAngle(pissing.ArenaWorldCenter, 7.5f);
            yield return cams.WhipTo(cams.miniWide, 0.28f);

            int winner = 0;
            int[] places = null;
            pissing.onFinished = (w, p) => { winner = w; places = p; };
            yield return pissing.Run();
            ApplyPlaces(places);
            _lastWinnerSlot = winner;
            pissing.Teardown();
            hud.SetTimer(-1f);
            host.Say("PODIUM", $"{NameOf(winner)} wins the hose");
            yield return WaitOrSkip(1.2f);
        }

        IEnumerator RunMini2()
        {
            phase = GamePhase.Mini2Announce;
            cams.BlendTo(cams.hostPush, 0.35f);
            yield return host.SayAndHold("NEXT MINI", "BACKYARD BBQ BRAWL — Gang Beasts–lite",
                DemoSettings.demoMode ? 1.8f : 2.8f);

            phase = GamePhase.Mini2Play;
            hud.SetPhase("BBQ BRAWL");
            hud.SetControlHint("WASD move · Space shove");
            bbq.Build(worldRoot, lobby.seats, hud);
            cams.AimMiniHighAngle(bbq.ArenaWorldCenter, 8f);
            yield return cams.WhipTo(cams.miniWide, 0.28f);

            int winner = 0;
            int[] places = null;
            bbq.onFinished = (w, p) => { winner = w; places = p; };
            yield return bbq.Run();
            ApplyPlaces(places);
            _lastWinnerSlot = winner;
            bbq.Teardown();
            hud.SetTimer(-1f);
            host.Say("PODIUM", $"{NameOf(winner)} owns the grill");
            yield return WaitOrSkip(1.2f);
        }

        IEnumerator RunBabyShowcase(int index)
        {
            phase = index == 1 ? GamePhase.BabyShowcase1 : GamePhase.BabyShowcase2;
            cams.BlendTo(cams.babyLane, 0.5f);
            hud.SetPhase($"BABY {index}");
            hud.SetTimer(-1f);
            hud.SetBody("");
            hud.SetControlHint("");
            host.Say("DNA REVEAL", "wah-wah — bowling alley lights");

            var winner = lobby.seats.Find(d => d.slotIndex == _lastWinnerSlot);
            yield return babyGen.GenerateTowardWinner(winner, DemoSettings.demoMode ? 0.85f : 1.2f);
            yield return showcase.PlayReveal(babyGen.LastBaby, DemoSettings.demoMode ? 4.2f : 5.5f);

            yield return cams.WhipTo(cams.eyesCut, 0.25f);
            host.Say("AUDIENCE", "…wah.");
            yield return WaitOrSkip(0.9f);
            cams.BlendTo(cams.hostPush, 0.4f);
            hud.ClearBabyChrome();
        }

        IEnumerator WaitOrSkip(float seconds)
        {
            float t = 0f;
            while (t < seconds)
            {
                t += Time.deltaTime;
                if (Input.GetKeyDown(KeyCode.F1)) yield break;
                yield return null;
            }
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
            sb.AppendLine("★ CUSTODY");
            foreach (var d in lobby.seats)
                sb.AppendLine($"{d.displayName}  {scoreboard.Stars(d.slotIndex)}");
            hud.starsText.text = sb.ToString();
        }

        string NameOf(int slot)
        {
            var d = lobby.seats.Find(x => x.slotIndex == slot);
            return d != null ? d.displayName : $"P{slot + 1}";
        }

        string LobbyBlurb()
        {
            return "SAURY! local lobby\n" +
                   $"Code: {lobby.roomCode}\n" +
                   $"{lobby.seats.Count} dads · Host = CPU\n\n" +
                   "Demo: P1 WASD+Space · AI seats 2–4\n" +
                   "F1 skip · F3 debug HUD · F5 ready-all";
        }

        string FinalCard(string champ)
        {
            var baby = babyGen.LastBaby;
            return $"NIGHT WINNER: {champ}\n\n" +
                   $"Final baby: {(baby != null ? baby.title : "(none)")}\n" +
                   $"{(baby != null ? baby.roastLine : "")}\n\n" +
                   "Enter = rematch from hub.";
        }
    }
}
