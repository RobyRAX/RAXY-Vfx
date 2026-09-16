# Basic Setup Sample

Minimal **RAXY VFX Manager** demo aligned with Project Alice wiring: `VfxManager` + `VfxBankSO` + `VfxOwner` + spawn settings, plus a manual world-space spawn like game `HitFxManager`.

## Contents

| Asset | Role |
|-------|------|
| `Basic Setup.unity` | Playable scene (camera, light, ground, bootstrap, demo actor, world target cube) |
| `Prefabs/Sample VFX Bootstrap.prefab` | `VfxManager` singleton host |
| `Prefabs/Sample Vfx Demo Actor.prefab` | Capsule unit with `VfxOwner`, `NamedTransformSet` (`Hand`), and `SampleVfxDemoController` |
| `Data/Sample Vfx Bank.asset` | `VfxBankSO` with direct prefab entry `sample_burst` |
| `Vfx/Sample Burst.prefab` | `ParticleSystem` + `VfxInstance` |
| `Scripts/SampleVfxDemoController.cs` | Keyboard + Odin debug buttons |

### Spawn settings in the bank

| Id | Type | What it shows |
|----|------|----------------|
| `hand_burst` | `VfxSpawnPoint` → `Hand` | Same pattern as combat timeline `VFX` tag → `Request_SpawnVfx(id)` |
| `foot_burst` | Relative `Vector3` | One-shot burst offset below the unit |
| `aura_loop` | `VfxSpawnPoint` + `registerToOwner` | Tracked VFX; toggle with key **3** or inspector |

## Import

1. Add `com.raxy.vfx` to the project (Git URL or `file:` path).
2. Package Manager → **RAXY VFX Manager** → **Samples** → **Import** Basic Setup.
3. Open `Assets/Samples/RAXY VFX Manager/<version>/Basic Setup/Basic Setup.unity`.
4. Enter Play Mode.

## Play Mode controls

| Input | Action |
|-------|--------|
| **1** | `hand_burst` via `VfxOwner.Request_SpawnVfx` |
| **2** | `foot_burst` |
| **3** | Spawn / toggle tracked `aura_loop` |
| **4** | Manual `VfxSpawnRequest` at **World Burst Target** cube (hit-FX style) |

Inspector buttons on **Sample Vfx Demo Controller** mirror the same actions.

## Notes

- Uses **direct prefab** references only (no Addressables setup).
- In Project Alice, `UnitCombatEventSequencerBase` calls `Request_SpawnVfx` with the spawn setting id from attack timelines.
- Game-specific `VisualBank`, `HitFxManager`, and combat banks stay in your game project.
