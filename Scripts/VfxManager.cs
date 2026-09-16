using RAXY.Pooling;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RAXY.VfxManager
{
    public class VfxManager : MonoBehaviour
    {
        static VfxManager _instance;

        public static VfxManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindAnyObjectByType<VfxManager>();

                    if (_instance == null)
                    {
                        var go = new GameObject(nameof(VfxManager));
                        _instance = go.AddComponent<VfxManager>();
                        DontDestroyOnLoad(go);
                    }
                }

                return _instance;
            }
        }

        [SerializeField]
        VfxSpawnRequest testRequest;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetStatics()
        {
            _instance = null;
        }

        void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Debug.LogWarning("[VfxManager] Trying to create a second instance. Destroying the duplicate.");
                Destroy(gameObject);
                return;
            }

            _instance = this;
        }

        void OnDestroy()
        {
            if (_instance == this)
                _instance = null;
        }

        [Button]
        void SpawnTestRequest()
        {
            SpawnVfx(testRequest);
        }

        public static VfxSpawnRequest BuildVfxSpawnRequest(
            IVfxBank bank,
            string spawnSettingId,
            VfxOwner owner,
            bool addComponentIfMissing = false)
        {
            _ = Instance;

            var spawnSetting = bank.GetVfxSpawnSetting(spawnSettingId);
            if (spawnSetting == null)
            {
                Debug.LogWarning($"[VfxManager] Spawn setting '{spawnSettingId}' was not found on the VFX bank.");
                return null;
            }

            var entry = bank.GetVfxEntry(spawnSetting.vfxId);
            if (entry == null)
            {
                Debug.LogWarning(
                    $"[VfxManager] VFX entry '{spawnSetting.vfxId}' was not found (spawn setting '{spawnSettingId}').");
                return null;
            }

            GameObject vfx = null;
            if (bank is VfxOwner vfxOwnerBank)
                vfx = vfxOwnerBank.GetCachedPrefabRoot(spawnSetting.vfxId);

            if (!VfxPrefabUtility.TryGetPrefabRoot(vfx, out vfx, out var prefabFailure) &&
                !VfxPrefabUtility.TryGetPrefabRoot(entry, out vfx, out prefabFailure))
            {
                Debug.LogWarning(
                    $"[VfxManager] Cannot resolve VFX prefab for '{spawnSetting.vfxId}' (spawn setting '{spawnSettingId}'): {prefabFailure}");
                return null;
            }

            var vfxInstance = vfx.GetComponent<VfxInstance>();
            if (vfxInstance == null)
            {
                if (addComponentIfMissing)
                {
                    vfxInstance = vfx.AddComponent<VfxInstance>();
                }
                else
                {
                    Debug.LogWarning($"[VfxManager] VFX prefab '{vfx.name}' does not have a VfxInstance component.");
                    return null;
                }
            }

            VfxSpawnRequest newReq = new VfxSpawnRequest
            {
                owner = owner,
                reqId = spawnSetting.spawnSettingId,
                vfxPrefab = vfxInstance,
                spawnPointType = spawnSetting.spawnPointType,
                registerToOwner = spawnSetting.registerToOwner,
                setAsChild = spawnSetting.setAsChild,
                overrideScale = spawnSetting.overrideScale,
                spawnPos = spawnSetting.spawnPos,
                spawnRot = spawnSetting.spawnRot,
                isRelativeToOwner = spawnSetting.isRelativeToOwner,
                vfxScaleOverride_Vector3 = spawnSetting.vfxScaleOverride_Vector3,
                spawnPointEntryId = spawnSetting.spawnPointEntryId
            };

            return newReq;
        }

        public static void SpawnVfx(VfxSpawnRequest req)
        {
            _ = Instance;

            if (req == null)
                return;
            if (req.vfxPrefab == null)
                return;

            bool alreadyRegistered = req.registerToOwner &&
                                     req.owner &&
                                     req.owner.HasEntry(req.reqId);

            VfxInstance vfxInstance;
            var originalPoolable = req.vfxPrefab.PoolableObject;

            if (alreadyRegistered)
            {
                vfxInstance = req.owner.GetVfx(req.reqId);
                req.owner.ActivateTrackedVfx(req.reqId);
            }
            else
            {
                if (originalPoolable != null)
                {
                    var pooledObj = ObjectPoolService.Instance.GetPoolableObject(originalPoolable);
                    vfxInstance = pooledObj.GetComponent<VfxInstance>();
                }
                else
                {
                    vfxInstance = Object.Instantiate(req.vfxPrefab);
                }
            }

            if (req.registerToOwner && req.owner)
            {
                vfxInstance.SetRequest(req);
                vfxInstance.DestroyPoolable();
                req.owner?.Register(vfxInstance);
                req.setAsChild = true;
            }

            Vector3 spawnPos = default;
            Quaternion spawnRot = default;

            if (req.spawnPointType == VfxSpawnRequestPointType.Vector3)
            {
                if (req.isRelativeToOwner && req.owner)
                {
                    spawnPos = req.owner.transform.TransformPoint(req.spawnPos);
                    spawnRot = req.owner.transform.rotation * Quaternion.Euler(req.spawnRot);
                }
                else
                {
                    spawnPos = req.spawnPos;
                    spawnRot = Quaternion.Euler(req.spawnRot);
                }

                if (req.overrideScale)
                {
                    vfxInstance.transform.localScale = req.vfxScaleOverride_Vector3;
                }

                if (req.setAsChild && req.owner)
                {
                    vfxInstance.transform.SetParent(req.owner.transform);
                }
            }
            else if (req.spawnPointType == VfxSpawnRequestPointType.ObjectTransform)
            {
                if (req.spawnPoint_objectTransform != null)
                {
                    spawnPos = req.spawnPoint_objectTransform.position;
                    spawnRot = req.spawnPoint_objectTransform.rotation;
                }
                else
                {
                    spawnPos = req.owner?.transform.position ?? default;
                    spawnRot = req.owner?.transform.rotation ?? default;
                }

                if (req.overrideScale)
                {
                    vfxInstance.transform.localScale = req.spawnPoint_objectTransform.localScale;
                }

                if (req.setAsChild)
                {
                    if (req.spawnPoint_objectTransform)
                        vfxInstance.transform.SetParent(req.spawnPoint_objectTransform);
                    else if (req.owner)
                        vfxInstance.transform.SetParent(req.owner.transform);
                }
            }
            else if (req.spawnPointType == VfxSpawnRequestPointType.VfxSpawnPoint)
            {
                var spawnPoint = req.owner?.spawnPoint ?? null;
                if (spawnPoint == null)
                {
                    Debug.LogWarning($"[VfxManager] Owner {req.owner} has no spawnPoint.");
                    return;
                }

                Transform selectedSlot = spawnPoint.GetEntry(req.spawnPointEntryId).tranform;

                spawnPos = selectedSlot.position;
                spawnRot = selectedSlot.rotation;

                if (req.overrideScale)
                {
                    vfxInstance.transform.localScale = selectedSlot.lossyScale;
                }

                if (req.setAsChild)
                {
                    vfxInstance.transform.SetParent(selectedSlot);
                }
            }

            vfxInstance.transform.SetPositionAndRotation(spawnPos, spawnRot);
            req.setAsChild = false;
        }
    }

    public enum VfxSpawnRequestPointType
    {
        Vector3,
        ObjectTransform,
        VfxSpawnPoint
    }
}
