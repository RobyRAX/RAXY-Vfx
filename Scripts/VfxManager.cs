using System;
using System.Collections.Generic;
using RAXY.Pooling;
using RAXY.Utility;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RAXY.VfxManager
{
    public class VfxManager : Singleton<VfxManager>
    {
        [SerializeField] VfxSpawnRequest testRequest;

        [Button]
        void SpawnTestRequest()
        {
            SpawnVfx(testRequest);
        }

        public VfxSpawnRequest BuildVfxSpawnRequest(IVfxBank bank, string spawnSettingId, VfxOwner owner, bool addComponentIfMissing = false)
        {
            var spawnSetting = bank.GetVfxSpawnSetting(spawnSettingId);
            if (spawnSetting == null)
                return null;
            var vfx = bank.GetVfxEntry(spawnSetting.vfxId).Asset;
            
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

        public void SpawnVfx(VfxSpawnRequest req)
        {
            if (req == null)
                return;
            if (req.vfxPrefab == null)
                return;

            bool alreadyRegistered = req.registerToOwner && 
                                        req.owner &&
                                        req.owner.HasEntry(req.reqId);

            // 1. Ambil object (pooling atau instantiate)
            VfxInstance vfxInstance;
            var originalPoolable = req.vfxPrefab.PoolableObject;

            if (alreadyRegistered)
            {
                vfxInstance = req.owner.GetVfx(req.reqId);
                req.owner.ActivateTrackedVfx(req.reqId);
            }
            else
            {
                if (originalPoolable != null) // if (req.vfxPrefab.TryGetComponent(out PoolableObject originalPoolable))
                {
                    var pooledObj = ObjectPoolService.Instance.GetPoolableObject(originalPoolable);
                    vfxInstance = pooledObj.GetComponent<VfxInstance>();
                }
                else
                {
                    vfxInstance = Instantiate(req.vfxPrefab);
                }
            }

            if (req.registerToOwner && req.owner)
            {
                vfxInstance.SetRequest(req);
                vfxInstance.DestroyPoolable();
                req.owner?.Register(vfxInstance);
                req.setAsChild = true;
            }

            // 2. Tentukan spawn position, rotation, scale
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
                Transform selectedSlot;

                selectedSlot = spawnPoint.GetEntry(req.spawnPointEntryId).tranform;

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

            // Apply position and rotation
            vfxInstance.transform.SetPositionAndRotation(spawnPos, spawnRot);
            req.setAsChild = false;
        }
    }

    public enum VfxSpawnRequestPointType
    {
        Vector3, ObjectTransform, VfxSpawnPoint 
    }
}
