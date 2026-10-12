# Demo polish handoff — camera / HUD / solo demo

**Mitchell pushes — agents do not.** Nothing was pushed to `mimcqueen/Saury` or any remote from this pass.

Apply on your machine at `C:\Users\mcque\dev\Saury` (or wherever your Saury clone lives), then **you** commit + push to `main` when happy.

## What changed

1. **Cameras**
   - Hub / Make Dad: Buckshot-table intimacy (tighter FOV, desk-centered mounts, crushed ambient).
   - Minis: fixed **high-angle** cam that **aims at the arena** (±22) — previously looked at world origin (broken).
   - Baby: bowling pomp FOV / framing.
2. **HUD**
   - Demo Mode **default ON** — phase title, timer, custody stars, short hint; no OnGUI-style dump.
   - **F3** toggles debug body panel. **F1** skip / **F5** ready-all stay (tiny hint).
3. **Solo demo**
   - P1 = WASD+Space; seats 2–4 = simple AI chasing plant/grill.
   - Make Dad auto-ready seats 2–4; shorter auto-advances; F1 never stuck.
4. **Set polish (light)**
   - Pissing Contest → grimy bathroom (tiles, sinks, stalls) from ref 06.
   - Hub spots more Buckshot (ref 05).
   - Dads slightly floppier limbs.
5. **Docs** — [how-to-run-unity.md](how-to-run-unity.md) updated for Demo Mode + controls.

## How to apply (pick one)

### A. Patch (from vertical-slice tip)

If your tree matches `cursor/unity-vertical-slice-9e28` / recent GitHub `main` with that slice:

```bat
cd C:\Users\mcque\dev\Saury
git apply path\to\demo-polish.patch
```

Patch file: [demo-polish.patch](demo-polish.patch)

If `git apply` complains, use `--3way` or method B.

### B. Drop-in scripts (safest)

Unpack [demo-polish-scripts.tgz](demo-polish-scripts.tgz) over the repo root (preserves paths under `SauryUnity/Assets/Scripts/...` and `tools/`).

New files (must exist after unpack):

- `SauryUnity/Assets/Scripts/Core/DemoSettings.cs`
- `SauryUnity/Assets/Scripts/Camera/CamMount.cs`

Unity will generate `.meta` on import.

### Then Play

1. Open **`SauryUnity/`** in Hub Editor **6000.3.26f1**.
2. Scene `Assets/Scenes/Bootstrap.unity` → **Play**.
3. Solo path: Lobby → Make Dad (edit P1, Enter) → Piss → Baby → BBQ → Baby → Crown.
4. **F1** skip · **F3** debug HUD · **WASD+Space** in minis.

## Media

[../media/unity-slice/](../media/unity-slice/) — offline Blender stills; honest caption in that README. Live HUD stills = your Capture menu.

## Local agent commit (not pushed)

Agent branch (Cursor Origin temp remote only, **not GitHub**): `cursor/demo-camera-hud-833d`  
Base: `origin/cursor/unity-vertical-slice-9e28`  
**Do not** treat Origin push as shipping to `mimcqueen/Saury`.
