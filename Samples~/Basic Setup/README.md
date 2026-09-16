# Basic Setup Sample

Minimal **RAXY VFX Manager** demo aligned with Project Alice wiring: `VfxManager` + `VfxBankSO` + `VfxOwner` + spawn settings, plus a manual world-space spawn like game `HitFxManager`.

VFX prefabs are copied from **Project Alice — Basic Claw weapon** (`Claw.prefab`, `Hit-Effect_v02.prefab`) with materials, shaders, and textures so the sample looks like real combat VFX.

## Contents

| Asset | Role |
|-------|------|
| `Basic Setup.unity` | Playable scene (camera, light, ground, bootstrap, demo actor, world target cube) |
| `Prefabs/Sample VFX Bootstrap.prefab` | `VfxManager` singleton host |
| `Prefabs/Sample Vfx Demo Actor.prefab` | Capsule unit with `VfxOwner`, `NamedTransformSet` (`Hand`), and `SampleVfxDemoController` |
| `Data/Sample Vfx Bank.asset` | `VfxBankSO` with `claw_slash` + `hit_impact` |
| `Vfx/Claw Effect/...` | Claw slash prefab + dependencies |
| `Vfx/HitEffect/...` | Hit impact prefab + dependencies |
| `Scripts/SampleVfxDemoController.cs` | Keyboard + Odin debug buttons |

### Bank entries and spawn settings

| `vfxId` | Prefab | Spawn setting |
|---------|--------|----------------|
| `claw_slash` | `Claw.prefab` | `hand_burst`, `aura_loop` (tracked) |
| `hit_impact` | `Hit-Effect_v02.prefab` | `foot_burst`; manual world burst (key **4**) |

| Spawn id | Type | What it shows |
|----------|------|----------------|
| `hand_burst` | `VfxSpawnPoint` → `Hand` | Combat timeline `VFX` tag → `Request_SpawnVfx(id)` |
| `foot_burst` | Relative `Vector3` | Hit-style burst below the unit |
| `aura_loop` | `VfxSpawnPoint` + `registerToOwner` | Tracked claw at hand; toggle with **3** |

## Import

1. Add `com.raxy.vfx` to the project (Git URL or `file:` path).
2. Package Manager → **RAXY VFX Manager** → **Samples** → **Import** Basic Setup.
3. Open `Assets/Samples/RAXY VFX Manager/<version>/Basic Setup/Basic Setup.unity`.
4. Enter Play Mode.

## Play Mode controls

| Input | Action |
|-------|--------|
| **1** | `hand_burst` (claw slash at `Hand`) |
| **2** | `foot_burst` (hit impact, relative) |
| **3** | Spawn / toggle tracked `aura_loop` (claw) |
| **4** | Manual `VfxSpawnRequest` at **World Burst Target** cube (`Hit-Effect_v02`) |

Inspector buttons on **Sample Vfx Demo Controller** mirror the same actions.

## Notes

- Uses **direct prefab** references only (no Addressables setup).
- Requires **URP** (material uses `Universal Render Pipeline/Lit`).
- Keyboard shortcuts use **Input System** when active (`ENABLE_INPUT_SYSTEM`); legacy `Input` is used only if the old input manager is enabled.
- In Project Alice, `UnitCombatEventSequencerBase` calls `Request_SpawnVfx` with the spawn setting id from attack timelines.
- Game-specific `VisualBank`, `HitFxManager`, and combat banks stay in your game project.
