# Saury! — Preferences

Short index of locked / preferred product choices. Detail: [docs/game-design.md](docs/game-design.md).

## 2026-10-12 — Visual / stage locks (still hold)

1. **Creature set:** Human, Goblin, Catgirl, Cryptid, Blob, Imp (exactly six).
2. **Tone:** Eerie dingy TV/stage — not candy party UI.
3. **VO:** No real voice synthesis — mouth flaps + text + Charlie Brown wah.
4. **Audience:** Eyes only in the dark; heavy spotlights.
5. **VHS post (P4):** Full VHS pastiche on the show output.
6. **Realism:** Per-dad Smooth vs Pixel (feeds gen style chips + hub doll).
7. **Face morph:** Mouse freeform morph on Make Dad — also summarized into baby gen prompts.
8. **Platform:** Steam / PC party MVP; phones later.
9. **Mom readable-kid bias:** Prompts should keep a Mom-anchored infant read (old G1 spirit); slot-compositor is not the primary pipeline.

## 2026-10-12 — ARCHIVED: compositor-only baby / no-AI

**Demoted.** Earlier “compositor-only MVP — no selfie/airbrush/AI” is **superseded**. Compositor / local collage remains **offline mock fallback** only. Primary fantasy is **AI → 3D mesh baby** (see R2).

## 2026-10-12 — HARD PIVOT: 3D minigame party (locked)

1. Primary game = **minigame-filled battle** (Mario Party / Pummel Party bar) — not Quiplash-primary.
2. **3D Maury hub** + **Liar’s Bar–like cameras**.
3. **Host** = CPU/NPC (wah + text; no real VO) — announces minis, slam-zooms baby, advances phases.
4. Difficulty: easy-chaos.
5. Physics brawls: **Gang Beasts–lite** (**Q3**).
6. Engine: **Unity** (Mitchell OK).

## 2026-10-12 — Minigame tone: deadbeat-dad battles

White-trash / deadbeat-dad activity chaos (pissing contest, BBQ brawl, lawnmower derby, etc.) — friend-slop funny, not a lecture. Steam-shippable cartoon proxies OK. MVP pair: **Pissing Contest** + **Backyard BBQ Brawl**.

## 2026-10-12 — Pivot Q1–Q3 LOCKED

1. **Q1:** Local / same-PC + **simple Steam lobbies** first — not full online polish day one.
2. **Q2:** Baby changes on win — **AI regen/evolve toward winner** (not stars-only; not slot-paint primary).
3. **Q3:** Soft **Gang Beasts–lite** physics.

## 2026-10-12 — AI baby generation (first-class)

Mitchell: **most fun part = baby morphing.** Avatar + descriptors → generate a **new baby each round won**; showcase after every win; lean into **funny AI mishaps**; never looks the same. History: img2img drift + sticky trait locks (design §6).

## 2026-10-12 — R1 / R2 LOCKED

1. **R1:** Selfie **optional** (opt-in); descriptors + doll always suffice. Privacy consent required if used.
2. **R2:** On-stage fantasy = **3D mesh baby** with **bowling-alley pomp** (theatrical lights, roll-out / return-machine energy, animatronic showcase). Gen may be image→textured/skinned doll or AI mesh + mock fallback; **billboard-only is interim bootstrap only**, not the target.

## 2026-10-12 — No host player (LOCKED / R3)

**Show Host = CPU/NPC.** **Everyone is a playing dad** — no asymmetric “you’re the host, you don’t play” seat. Steam/listen-server still has a technical **lobby owner** (Start / whose PC hosts); that person **still plays as a dad** and is not the on-stage Host character.

## 2026-10-12 — Unity on Mitchell’s machine

Mitchell has **Unity installed locally (licensed Hub)** as of 2026-10-12 — prefer **Play Mode / captures on his machine**; cloud VMs often lack a Unity license.

## 2026-10-12 — GitHub source of truth

Canonical code repo: [https://github.com/mimcqueen/Saury](https://github.com/mimcqueen/Saury) (`main` = Unity vertical slice). Prefer this over the Cursor Origin temp remote for local clone / private worker.

## 2026-10-12 — Git: Mitchell only pushes (LOCKED)

**Agents must never `git push`** to mimcqueen/Saury or any remote. Only Mitchell pushes. Prefer edits on his local machine via private worker when available; otherwise deliver patches/files in Context for him to apply — do not force him to pull agent commits.

## 2026-10-12 — How Mitchell steers quality

Mitchell Play-tests locally in Unity; greybox “idea is there but rough” is not done. Prefer **reference images**, **one north-star focus per pass**, and explicit **wrong/meh/close** grades on atmosphere / characters / UI / fun. Don’t expand mechanics while the stage still looks like debug scaffolding.
