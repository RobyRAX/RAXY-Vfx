using System.Collections;
using RAXY.VfxManager;
using Sirenix.OdinInspector;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace RAXY.VfxManager.Samples
{
    /// <summary>
    /// Minimal VFX demo: bank spawn (combat sequencer style), tracked aura, relative burst, and manual world spawn (hit FX style).
    /// </summary>
    public class SampleVfxDemoController : MonoBehaviour
    {
        public const string HandBurstId = "hand_burst";
        public const string FootBurstId = "foot_burst";
        public const string AuraLoopId = "aura_loop";

        [TitleGroup("References")]
        [SerializeField]
        VfxOwner vfxOwner;

        [TitleGroup("References")]
        [SerializeField]
        Transform worldBurstTarget;

        [TitleGroup("References")]
        [SerializeField]
        VfxInstance burstVfxPrefab;

        [TitleGroup("Debug")]
        [SerializeField]
        bool logInputHintsOnStart = true;

        bool _banksReady;

        void Awake()
        {
            if (vfxOwner == null)
                vfxOwner = GetComponent<VfxOwner>();
        }

        void Start()
        {
            StartCoroutine(WaitForBanksThenLogHints());
        }

        IEnumerator WaitForBanksThenLogHints()
        {
            while (vfxOwner != null && !vfxOwner.AreBanksReady)
                yield return null;

            _banksReady = vfxOwner != null && vfxOwner.AreBanksReady;

            if (!_banksReady)
                yield break;

            if (logInputHintsOnStart)
            {
                Debug.Log(
                    "[SampleVfxDemo] Keys: 1 = hand_burst (spawn point), 2 = foot_burst (relative), " +
                    "3 = aura_loop (tracked), 4 = world burst at target cube. " +
                    "Use inspector buttons during Play mode only.");
            }
        }

        void Update()
        {
            if (!_banksReady || vfxOwner == null)
                return;

            if (WasDigitKeyPressed(1))
                SpawnHandBurst();
            if (WasDigitKeyPressed(2))
                SpawnFootBurst();
            if (WasDigitKeyPressed(3))
                SpawnOrToggleAura();
            if (WasDigitKeyPressed(4))
                SpawnWorldBurstAtTarget();
        }

        static bool WasDigitKeyPressed(int digit)
        {
#if ENABLE_INPUT_SYSTEM
            var keyboard = Keyboard.current;
            if (keyboard == null)
                return false;

            return digit switch
            {
                1 => keyboard.digit1Key.wasPressedThisFrame,
                2 => keyboard.digit2Key.wasPressedThisFrame,
                3 => keyboard.digit3Key.wasPressedThisFrame,
                4 => keyboard.digit4Key.wasPressedThisFrame,
                _ => false
            };
#else
            if (digit < 0 || digit > 9)
                return false;

            return Input.GetKeyDown((KeyCode)((int)KeyCode.Alpha0 + digit));
#endif
        }

        [TitleGroup("Bank Spawn")]
        [Button, DisableInEditorMode]
        public void SpawnHandBurst()
        {
            if (!EnsureOwner())
                return;

            vfxOwner.Request_SpawnVfx(HandBurstId);
        }

        [TitleGroup("Bank Spawn")]
        [Button, DisableInEditorMode]
        public void SpawnFootBurst()
        {
            if (!EnsureOwner())
                return;

            vfxOwner.Request_SpawnVfx(FootBurstId);
        }

        [TitleGroup("Tracked VFX")]
        [Button, DisableInEditorMode]
        public void SpawnOrToggleAura()
        {
            if (!EnsureOwner())
                return;

            if (!vfxOwner.HasEntry(AuraLoopId))
            {
                vfxOwner.Request_SpawnVfx(AuraLoopId);
                return;
            }

            var inst = vfxOwner.GetVfx(AuraLoopId);
            if (inst != null && inst.gameObject.activeSelf)
                vfxOwner.DeactivateTrackedVfx(AuraLoopId);
            else
                vfxOwner.ActivateTrackedVfx(AuraLoopId);
        }

        [TitleGroup("Tracked VFX")]
        [Button, DisableInEditorMode]
        public void DeactivateAura()
        {
            if (vfxOwner != null && vfxOwner.HasEntry(AuraLoopId))
                vfxOwner.DeactivateTrackedVfx(AuraLoopId);
        }

        [TitleGroup("Manual World Spawn")]
        [Button, DisableInEditorMode]
        public void SpawnWorldBurstAtTarget()
        {
            if (burstVfxPrefab == null)
            {
                Debug.LogWarning("[SampleVfxDemo] Assign burstVfxPrefab (Hit-Effect_v02) on the controller.");
                return;
            }

            Transform target = worldBurstTarget != null ? worldBurstTarget : transform;
            Vector3 spawnPos = target.position;
            Quaternion spawnRot = Quaternion.identity;

            var vfxSpawnReq = new VfxSpawnRequest
            {
                owner = vfxOwner,
                reqId = burstVfxPrefab.OriginalName,
                vfxPrefab = burstVfxPrefab,
                spawnPointType = VfxSpawnRequestPointType.Vector3,
                registerToOwner = false,
                setAsChild = false,
                overrideScale = false,
                isRelativeToOwner = false,
                spawnPos = spawnPos,
                spawnRot = spawnRot.eulerAngles,
                vfxScaleOverride_Vector3 = Vector3.one,
                spawnPointEntryId = string.Empty
            };

            VfxManager.SpawnVfx(vfxSpawnReq);
        }

        bool EnsureOwner()
        {
            if (!Application.isPlaying)
            {
                Debug.LogWarning("[SampleVfxDemo] VFX demo controls only work in Play mode.");
                return false;
            }

            if (vfxOwner == null)
            {
                Debug.LogWarning("[SampleVfxDemo] VfxOwner is missing on the demo actor.");
                return false;
            }

            if (!vfxOwner.AreBanksReady)
            {
                Debug.LogWarning("[SampleVfxDemo] VfxOwner banks are still loading.");
                return false;
            }

            return true;
        }
    }
}
