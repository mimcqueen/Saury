# Move Saury Project context → local

You can’t “teleport” this cloud Project VM onto your PC. You **can** make every local Cursor agent inherit what we established by putting the Context pack in your repo.

## What stays where

| Thing | Where it lives |
|---|---|
| This long Project chat | Cloud Project (history stays here) |
| Notes / Context files | Context tab in the Project |
| Game code | `C:\Users\mcque\dev\Saury` + https://github.com/mimcqueen/Saury |
| Who pushes git | **Only you** |

## Recommended setup

1. Keep this Project open for planning.
2. Copy from Context into your repo:

```
C:\Users\mcque\dev\Saury\
  AGENTS.md                 ← docs/local-agents.md
  docs\
    project-context.md
    game-design.md
    art-direction.md
    preferences.md
    move-to-local.md
  media\refs\               ← media/refs\
```

3. Cursor → Open Folder → `C:\Users\mcque\dev\Saury` → new local Agent → `@docs/project-context.md` + task.
4. You commit & push.

## Demo polish (separate)

Apply [demo-polish-handoff.md](demo-polish-handoff.md) (tarball/patch) locally — agents do not push.
