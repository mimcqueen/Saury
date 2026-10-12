# Saury!

**3D Maury talk-show minigame party** — Unity vertical slice.

Wrong-genre web Prompt Slam is archived; this repo ships the **Unity** hub + trash minis + bowling-alley baby showcase.

## Project

Unity project folder: [`SauryUnity/`](SauryUnity/)

| | |
|---|---|
| **Editor** | **Unity 6000.3.26f1** (Unity 6 LTS) |
| **Render** | URP (from official 3D template) |
| **Host** | CPU/NPC only — all humans play dads |
| **MVP minis** | Pissing Contest · Backyard BBQ Brawl |
| **Baby** | Mock AI gen → 3D mesh showcase (API optional later) |

## Quick start

Full guide: Project store `docs/how-to-run-unity.md` (also mirrored intent in `SauryUnity/README.md`).

### On your machine

1. Install **Unity Hub** + Editor **6000.3.26f1** (or any Unity 6 LTS close to it).
2. Hub → **Open** → select the `SauryUnity` folder.
3. Open scene `Assets/Scenes/Bootstrap.unity`.
4. Press **Play**.

Flow auto-runs: **Lobby → Make Dad → Mini 1 → Baby showcase → Mini 2 → Baby showcase → Crown**.

## Controls (same-PC stub)

| Player | Move | Action |
|--------|------|--------|
| P1 | WASD | Space |
| P2 | Arrows | Right Ctrl |
| P3 | IJKL | Right Shift |
| P4 | TFGH | Enter |

**Make Dad:** ←/→ creature · 1–5 attributes · mouse-drag morph · Enter ready · Tab next seat · **F5** ready-all  
**Dev:** **F1** skip / force-advance phase · Enter skips short lobby wait

## What works vs stubbed

| Piece | Status |
|-------|--------|
| Hub stage (desk, curtains, eyes, spotlights, VHS overlay) | Works (procedural) |
| CPU Host wah + text cards | Works |
| Local lobby stub (4 seats, room code) | Works — not Steam yet |
| Make Dad seed pack | Works (stub art) |
| Pissing Contest | Playable |
| Backyard BBQ Brawl | Playable (soft physics) |
| Camera cuts (orbit / push / whip / eyes) | Works (custom director; Cinemachine package present for later) |
| AI baby | **Mock** funny placeholders that change each win |
| Bowling-alley showcase | Works (lights, roll-out, animatronic idle) |
| Stars + crown | Works |
| Steam lobbies / real mesh API / selfie | Stubbed / deferred |

## Design source of truth

Project store: `docs/game-design.md` · `preferences.md`

## License note (cloud agents)

This cloud VM installed the Unity **Editor binary** via Unity CLI, but **Play Mode / batchmode require a Unity license login**. Open the project on a licensed Hub install to press Play.
