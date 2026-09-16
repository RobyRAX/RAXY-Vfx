# Changelog

## [1.0.11] - 2026-03-16

### Removed

- Basic Setup: **Sample VFX Bootstrap** prefab and scene instance (`VfxManager` lazy host only).

### Changed

- Basic Setup scene: Main Camera Y rotation ~17.79° (matches sample authoring).

## [1.0.10] - 2026-03-16

### Changed

- `VfxEntry`: prefab data lives under nested `vfxPrefabProvider` (composition over inheriting `AddressableAssetProviderGameObject`).
- **Sample Vfx Bank** YAML and editor wiring updated for the new layout.

### Fixed

- **Sample Vfx Bank**: `directAsset` references use root prefab file IDs (`Claw` / `Hit-Effect_v02`) so entries resolve in the Inspector after import.

## [1.0.9] - 2026-03-16

### Fixed

- `VfxEntry`: Odin **Prefab** field for direct (non-addressable) entries so bank rows show the assigned prefab.
- Editor: auto-wire **Sample Vfx Bank** prefabs on import + menu **RAXY → VFX → Wire Sample Vfx Bank Prefabs** (fixes missing/broken refs after sample re-import or GUID remap).

## [1.0.8] - 2026-03-16

### Changed

- `VfxManager`: lazy `DontDestroyOnLoad` host (same pattern as `AddressableService`); no scene placement required.
- `BuildVfxSpawnRequest` and `SpawnVfx` are **static** APIs (`VfxManager.SpawnVfx`, etc.); removed `Singleton<T>` base.

## [1.0.7] - 2026-03-16

### Fixed

- `BuildVfxSpawnRequest`: safe prefab resolution (`VfxPrefabUtility`) so destroyed/missing refs no longer throw on `GameObject.scene`.
- `VfxOwner`: cache project prefab roots when banks load (stable spawn even if `VfxEntry.directAsset` is corrupted at runtime).
- **Basic Setup** sample: regenerate unique asset GUIDs so importing the sample into Project Alice does not collide with existing Claw/Hit VFX GUIDs (was a common cause of destroyed prefab references).

## [1.0.6] - 2026-03-16

### Fixed

- `BuildVfxSpawnRequest`: validate spawn settings, VFX entries, and prefab references before `GetComponent` (avoids `MissingReferenceException` on destroyed or missing assets).
- `VfxOwner`: load bank SOs in `Awake`; expose `AreBanksReady`; guard `Request_SpawnVfx` when manager or banks are unavailable.
- Basic Setup sample: wait for banks before key input; Odin spawn buttons disabled outside Play mode.

## [1.0.5] - 2026-03-16

### Fixed

- Basic Setup sample: digit key shortcuts use Input System when project active input handling is Input System only.

## [1.0.4] - 2026-03-16

### Fixed

- Basic Setup `Sample_DemoActor` material: correct URP Lit shader GUID for Unity 6 (was invalid → pink / InternalErrorShader).

## [1.0.3] - 2026-03-16

### Fixed

- Basic Setup: demo actor uses bundled URP Lit material (fixes pink capsule in URP projects).
- Basic Setup scene: normalize main camera rotation (fixes `QuaternionToEuler` warning on scene open).

## [1.0.2] - 2026-03-16

### Fixed

- **Basic Setup** sample VFX: replace placeholder particle prefab with Project Alice Basic Claw `Claw.prefab` and `Hit-Effect_v02.prefab` (with dependencies).

## [1.0.1] - 2026-03-16

### Added

- **Basic Setup** package sample: demo scene, `VfxBankSO`, `VfxOwner` spawn points, and `SampleVfxDemoController` (bank spawn, tracked VFX, manual world spawn).
