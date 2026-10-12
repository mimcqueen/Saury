# Saury! — Game Design

**Primary:** 3D Maury talk-show **minigame battle** party game — Mario Party / Pummel Party chaos on a Liar’s Bar–intimate stage. Steam / PC couch or simple Steam lobbies.

**Wrong-genre archive:** Slim web **Prompt Slam** felt like a drinking / write-and-vote night — not the friend-slop **minigame-filled battle** Mitchell wants. Salvage only where noted.

> **Locked (carry forward)** — detail in [preferences.md](../preferences.md)  
> Creatures ×6 · Eerie dingy stage · No VO (wah + text) · Eyes-only audience · Spotlights · Full VHS · Smooth/Pixel · Mouse face morph · **Unity** · Q1 local/simple lobbies · Q3 Gang Beasts–lite · Deadbeat minis · **Host = CPU/NPC; all humans play dads** · **R1 selfie optional** · **R2 3D mesh baby + bowling-alley pomp** · AI baby on win (Q2)  
>
> Agents: **no git push** to Mitchell’s GitHub. Agent Store / patches only.

Also see: [art-direction.md](art-direction.md) · [minigame-brainstorm.md](minigame-brainstorm.md)

---

## Why this will be fun tonight

- [ ] **The baby is the star** — every round win regenerates a cursed AI → 3D kid; mishaps are the joke.
- [ ] You’re in a **pissing contest / BBQ brawl** between showcases — minis feed the morph.
- [ ] The **NPC Host** slam-zooms the bowling-alley baby reveal after each win.
- [ ] Cameras cut around a cramped, dingy stage like **Liar’s Bar** / Buckshot intimacy.
- [ ] Easy-chaos; bad players still get laughs.
- [ ] Night ends with a **never-seen-before custody screenshot**.
- [ ] ~25–35 min; rematch from the hub.

---

## 1. Pitch

**Saury!** is a cursed daytime talk show in a black-box studio. Deadbeat creature dads smash through trailer-park minigames while a wah-mouthed **CPU Host** runs the circus. The **most fun part is the baby**: each win feeds avatar + descriptors into AI generation → a **3D mesh infant** rolls out with bowling-alley pomp. Lean into funny AI mishaps. Winner takes the fever-dream screenshot home.

---

## 2. Platform & setup

| | Default |
|---|---|
| **Platform** | **Steam / PC** — **Q1 LOCKED:** local / same-PC + **simple Steam lobbies** first |
| **Players** | 3–6 ideal (4 sweet). **Everyone is a playing dad** |
| **Show Host** | **CPU/NPC only** — announces minis, baby showcase, advances phases |
| **Lobby owner** | Technical only (Start / listen-server PC). **Still plays as a dad** |
| **Engine** | **Unity** (Mitchell OK; prefer Play Mode on his licensed local Hub) |
| **Session** | ~25–35 min |
| **Phones** | Later — out of MVP |

---

## 3. Engine — Unity

Unity LTS + Cinemachine + PhysX + Steamworks. Reasons: Liar’s Bar camera blends, Gang Beasts–lite joints, Steam glue, additive mini scenes. Godot = alt only if forced.

---

## 4. Core loop — Maury Party

```
Lobby → Make Dad (seed pack + 3D doll)
→ Mom Forge → Hub seat-in → Seed baby
→ SHOW BLOCK × N:
     NPC Host → announce mini → camera whip
     → Minigame 30–90s → podium
     → Stars + AI baby regen toward winner → 3D bowling-alley showcase
→ Host crown → Souvenir → Rematch
```

**Teach line:** “Win the dumb game. The baby mutates into you. Screenshot the crime.”

### Win loop

1. Minis pay **custody stars** (1st=3, 2nd=2, 3rd=1). Night winner = most stars.  
2. **On each mini win (Q2):** regenerate baby toward winner (AI + history) → **3D mesh showcase** with bowling-alley lights / roll-out / animatronic idle (**R2**).  
3. Souvenir = final baby hero still + stars + VHS chrome.

No bio-dad deduction, no Step-Dad, no Prompt Slam spine, no human Host seat.

---

## 5. Hub fantasy

Cramped talk-show set — desk, nicotine curtains, crushed blacks, **eyes-only audience**, hard spotlights, **full VHS**. Visual north star: Mitchell’s refs + [art-direction.md](art-direction.md) (Buckshot table intimacy, GB softbody chaos, horror-bathroom piss contest).

**NPC Host:** Mouth flaps + text + Charlie Brown wah. Roasts, deals next mini, emcees baby reveal, crowns. Advances on timer/ready — lobby owner only handles session tech.

**Cameras (Liar’s Bar bar):** slow orbit lobby · push-in Host card · whip into arena · podium return · bowling-alley baby roll-out · eye cutaways.

Difficulty: **not crazy hard** — wide hurtboxes, short rounds.

---

## 6. Make Dad + AI → 3D baby

### Make Dad → gen seed

| Input | Required? |
|---|---|
| Creature base (6 locked) | Yes |
| Attribute chips → text descriptors | Yes |
| Smooth / Pixel (+ style chips) | Yes |
| Mouse face morph → prompt hints + doll | Yes (default OK) |
| Selfie / Steam avatar | **Optional (R1)** — consent, session-ephemeral |
| Catchphrase | Optional |

Hub **3D doll** for minis. Baby = **AI → 3D mesh** on stage.

### Mom Forge

One-slot-per-player + **fight-wheel** (~60–75s). Mom descriptors = stable maternal half of every baby prompt.

### Per-win evolution

`BabyHistory`: `anchorMom` + `winnerStack` + up to **3 sticky traitLocks** + capped `chaosSalt` + `prevImage`/prev mesh ref at soft img2img strength ~0.35–0.55. Drift toward winners; never identical; mishaps welcome.

### Pipeline

```
Seeds + history → filtered prompt → gen (image and/or mesh)
→ Unity skinned/textured infant (or image→mesh bootstrap)
→ Bowling-alley showcase (lights, chute/elevator, animatronic wiggle)
```

**Target:** 3D animated baby pomp. Billboard/CRT = interim bootstrap only. Mock fallback if offline/rate-limited.

### Failure modes

Offline → funny mock doll + stickers · Rate limit → Host “developing…” · Selfie privacy opt-in · Filters must not neuter deadbeat gag · Gen timeout ~10s then fallback.

---

## 7. Minigame slate — deadbeat battles

**Tone:** White-trash / trailer-park / Maury deadbeat chaos — friend-slop funny, not a lecture. Steam-shippable (cartoon pee / hose proxy OK).

**MVP pair:**

1. **Pissing Contest** — aim skill, horror-bathroom energy  
2. **Backyard BBQ Brawl** — Gang Beasts–lite physics  

**Physics (Q3):** Soft Gang Beasts–lite — big grabs, short rounds.

Full expanded slate + “next 4–6” → **[minigame-brainstorm.md](minigame-brainstorm.md)**.

Core eight (from prior lock + brainstorm):

| # | Name | Win |
|---|---|---|
| 1 | Pissing Contest | Highest aim score |
| 2 | Backyard BBQ Brawl | Last standing / grill control |
| 3 | Lawnmower Derby | First finish / most laps |
| 4 | Child Support Dash | Most envelopes banked |
| 5 | Trailer Park King | Most time on picnic table |
| 6 | Beer Pong Physics | Most sinks / clear cups |
| 7 | Couch Cushion Claim | Hold sticky recliner |
| 8 | Unpaid Tab Tabernacle | Highest paid / least debt |

---

## 8. MVP vs stretch

### MVP vertical slice

1. Steam-ready PC lobby (local / simple lobbies).  
2. Hub stage + NPC Host + VHS + 2–3 Cinemachine cams.  
3. Make Dad (base + parts + morph + realism).  
4. Mom Forge (or stub).  
5. **Pissing Contest** + **BBQ Brawl**.  
6. AI → **3D baby** + bowling-alley showcase + mock fallback.  
7. Stars + crown + souvenir.

### Stretch

Full mini slate · fancier mesh-gen · richer Host deck · stronger optional selfie refs · online polish · phones later.

---

## 9. Prototype archive — Slim Prompt Slam

Wrong genre. Do not rebuild as spine. Salvage: creatures, Mom fight-wheel, baby-on-win fantasy, VHS/eerie, realism, morph, Steam/PC, no-VO wah.

---

## 10. Locked decisions

| ID | Lock |
|---|---|
| **Q1** | Local + simple Steam lobbies first |
| **Q2** | Baby changes on win via AI regen toward winner |
| **Q3** | Soft Gang Beasts–lite physics |
| **R1** | Selfie optional |
| **R2** | 3D mesh baby + bowling-alley pomp (not billboard-as-target) |
| **R3** | Show Host = CPU/NPC; all humans play dads |

Deferred: Quiplash-primary, bio-dad, Step-Dad, phone clients, Detective Night, human Host player, compositor-as-primary.

---

*Restored 2026-10-12 after Agent Store wipe. Build hub + 2 minis + 3D bowling-alley baby.*
