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

See [`SauryUnity/README.md`](SauryUnity/README.md). Project vision for local agents: [`docs/project-context.md`](docs/project-context.md) · [`AGENTS.md`](AGENTS.md).

### On your machine

1. `git pull` this repo.
2. Install **Unity Hub** + Editor **6000.3.26f1** (or any Unity 6 LTS close to it).
3. Hub → **Open** → select the `SauryUnity` folder.
4. Open scene `Assets/Scenes/Bootstrap.unity`.
5. Press **Play** (Demo Mode HUD on by default; **F3** = debug panel).

Flow: **Lobby → Make Dad → Pissing Contest → Baby → BBQ Brawl → Baby → Crown**.

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
| Hub stage (3D desk/curtains/spots/eyes) | Works — `StageKit` mesh blockout |
| CPU Host | Works — modeled primitive kit + idle/talk |
| Dad avatars | Works — 6 creature 3D kits |
| Local lobby stub | Works — not Steam yet |
| Make Dad seed pack | Works |
| Pissing Contest / BBQ Brawl | Playable 3D arenas + kit bodies |
| Baby showcase | 3D lane/cradle/emissive chase + mock baby mesh |
| AI baby API | Mock gens (change each win) |
| Steam lobbies / selfie / final art | Stubbed / deferred |

## Design source of truth

Project store: `docs/game-design.md` · `preferences.md`

## License note (cloud agents)

This cloud VM installed the Unity **Editor binary** via Unity CLI, but **Play Mode / batchmode require a Unity license login**. Open the project on a licensed Hub install to press Play.
