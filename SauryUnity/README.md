# Saury! — Unity Vertical Slice

Unity **6000.3.26f1** (Unity 6 LTS) · URP · PC / Steam couch party

## Open in Unity Hub

1. Install [Unity Hub](https://unity.com/download).
2. Install Editor **6000.3.26f1** (Hub → Installs → Install Editor → Archive / Unity 6 LTS).
   - Modules: **Linux/Mac/Windows Build Support** for your OS is enough for Play Mode.
3. **Open** → choose this folder (`SauryUnity`).
4. First open imports packages (URP, Input System, Cinemachine, uGUI). Wait for compile.
5. Open **`Assets/Scenes/Bootstrap.unity`**.
6. Press **Play**.

`SauryBootstrap` auto-builds the Maury hub and starts the show flow if the scene is empty/template debris.

Menu: **Saury → Open Bootstrap Scene** / **Play Vertical Slice Info**.

## Play path

```
Lobby → Make Dad → Hub intro
  → Pissing Contest → Baby bowling-alley showcase
  → Backyard BBQ Brawl → Baby showcase
  → Crown → Rematch (Enter)
```

## Controls

### Make Dad
- **← / →** cycle creature (Human, Goblin, Catgirl, Cryptid, Blob, Imp)
- **1–4** cycle hair / eyes / prop / energy tags
- **5** Smooth TV ↔ Pixel
- **Mouse drag** face morph → prompt hint
- **Tab** next seat · **Enter** mark ready · **F5** ready all

### Minigames (same-PC hotseat)
| Slot | Move | Action (spray / hop-grab) |
|------|------|---------------------------|
| P1 | WASD | Space |
| P2 | Arrow keys | Right Ctrl |
| P3 | IJKL | Right Shift |
| P4 | TFGH | Enter |

### Flow helpers
- **Enter** during lobby — start early  
- **F1** — force ready / skip short waits (dev)

## Architecture (slice)

| Folder | Role |
|--------|------|
| `Scripts/Bootstrap` | Procedural stage + auto-boot |
| `Scripts/Core` | Phase flow, stars |
| `Scripts/Host` | CPU Host NPC (wah + cards) |
| `Scripts/Lobby` | Local lobby stub |
| `Scripts/Dad` | Make Dad seed pack |
| `Scripts/Minigames` | Pissing Contest, BBQ Brawl, soft motors |
| `Scripts/Baby` | Mock AI gen + bowling-alley showcase |
| `Scripts/Camera` | Liar’s Bar–ish cuts |
| `Scripts/VFX` | VHS overlay, audience eyes |

## Mock AI baby

No API key required. `MockBabyGenerator` builds a new cursed mesh + roast line each win from Dad seed + Mom anchor + trait locks + chaos salt. Swap the coroutine body later for a real image/mesh API.

## 3D blockout

Procedural mesh kits (not flat UI mockups):

- `Scripts/Bootstrap/StageKit.cs` — hub set, practical spots  
- `Scripts/Bootstrap/CharacterKit.cs` — Host + 6 creature dads + baby  
- Minigame arenas build real 3D yards/patios with kit dads  

Capture live Game view: Play → **Saury → Capture 3D Slice Screenshots**.

## Known cloud-agent blocker

Headless VMs can install the Editor via Unity CLI (`unity install 6000.3.26f1`) but **cannot enter Play Mode without a Unity account license**. Develop/play on a licensed desktop Hub install.
