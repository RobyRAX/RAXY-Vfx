# RAXY VFX Manager

RAXY VFX Manager provides a modular visual effects spawning system for Unity projects: VFX banks, spawn settings, pooling integration, and owner tracking.

## Features

- **VfxManager** — spawn VFX from `VfxSpawnRequest` with pooling or instantiate fallback
- **IVfxBank / VfxBankSO** — addressable VFX prefab lookup with spawn settings
- **VfxBankOverrideSO** — per-context bank overrides
- **VfxOwner** — track spawned VFX on units with spawn points
- **VfxInstance** — runtime VFX instance with poolable integration

## Setup

1. Add `VfxManager` to your bootstrap scene.
2. Create `VfxBankSO` (or override banks) with addressable VFX entries and spawn settings.
3. Attach `VfxOwner` to units that spawn or own VFX.
4. Build requests via `VfxManager.BuildVfxSpawnRequest()` or manually, then call `SpawnVfx()`.

## Samples

Package Manager → **RAXY VFX Manager** → **Samples** → import **Basic Setup**.

- Scene `Basic Setup.unity` with `VfxManager`, a demo unit (`VfxOwner` + spawn points), and a world target cube.
- Demonstrates bank spawn (`Request_SpawnVfx`), tracked aura toggle, relative `Vector3` spawn, and manual world-space spawn (same pattern as game hit FX).
- Uses direct prefab references only (no Addressables setup required).

See `Samples~/Basic Setup/README.md` after import.

## Dependencies

- **RAXY Core** (`com.raxy.core`) — addressable asset providers
- **RAXY Pooling** (`com.raxy.pooling`) — pooled VFX spawn via `ObjectPoolService`
- **RAXY Utility** (`com.raxy.utility`) — `Singleton`, gameplay spawn point helpers
- **Unity Addressables** — VFX prefab references
- **Odin Inspector** (project plugin) — editor attributes; runtime works without Odin if attributes are stripped

## Notes

Game-specific visual banks, hit FX managers, and combat VFX wiring should live in your project, not in this package.
