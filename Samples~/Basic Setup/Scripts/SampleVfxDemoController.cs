using RAXY.VfxManager;
using Sirenix.OdinInspector;
using UnityEngine;

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
            _banksReady = true;

            if (logInputHintsOnStart)
            {
                Debug.Log(
                    "[SampleVfxDemo] Keys: 1 = hand_burst (spawn point), 2 = foot_burst (relative), " +
                    "3 = aura_loop (tracked), 4 = world burst at target cube. " +
                    "Inspector buttons work anytime after Play.");
            }
        }

        void Update()
        {
            if (!_banksReady || vfxOwner == null)
                return;

            if (Input.GetKeyDown(KeyCode.Alpha1))
                SpawnHandBurst();
            if (Input.GetKeyDown(KeyCode.Alpha2))
                SpawnFootBurst();
            if (Input.GetKeyDown(KeyCode.Alpha3))
                SpawnOrToggleAura();
            if (Input.GetKeyDown(KeyCode.Alpha4))
                SpawnWorldBurstAtTarget();
        }

        [TitleGroup("Bank spawn (Project Alice / EventSequencer VFX tag)")]
        [Button]
        public void SpawnHandBurst()
        {
            if (!EnsureOwner())
                return;

            vfxOwner.Request_SpawnVfx(HandBurstId);
        }

        [TitleGroup("Bank spawn (Project Alice / EventSequencer VFX tag)")]
        [Button]
        public void SpawnFootBurst()
        {
            if (!EnsureOwner())
                return;

            vfxOwner.Request_SpawnVfx(FootBurstId);
        }

        [TitleGroup("Tracked VFX")]
        [Button]
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
        [Button]
        public void DeactivateAura()
        {
            if (vfxOwner != null && vfxOwner.HasEntry(AuraLoopId))
                vfxOwner.DeactivateTrackedVfx(AuraLoopId);
        }

        [TitleGroup("Manual world spawn (HitFxManager-style)")]
        [Button]
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

            VfxManager.Instance.SpawnVfx(vfxSpawnReq);
        }

        bool EnsureOwner()
        {
            if (vfxOwner != null)
                return true;

            Debug.LogWarning("[SampleVfxDemo] VfxOwner is missing on the demo actor.");
            return false;
        }
    }
}
