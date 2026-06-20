using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RAXY.VfxManager
{
    [Serializable]
    public class VfxSpawnRequest
    {
        [ShowIf("isObjectTransform")]
        public Transform spawnPoint_objectTransform;
        public VfxOwner owner;

        public string reqId;
        public VfxInstance vfxPrefab;
        public VfxSpawnRequestPointType spawnPointType;

        public bool registerToOwner;
        [HideIf("@isVector3 || registerToOwner")]
        public bool setAsChild;
        public bool overrideScale;

        [ShowIf("@isVector3")]
        public bool isRelativeToOwner;

        [ShowIf("@isVector3")]
        public Vector3 spawnPos;

        [ShowIf("@isVector3")]
        public Vector3 spawnRot;

        [ShowIf("@overrideScale && isVector3")]
        public Vector3 vfxScaleOverride_Vector3 = Vector3.one;

        [ShowIf("@isVfxSpawnPoint")]
        public string spawnPointEntryId;

#if UNITY_EDITOR
        bool isVector3 => spawnPointType == VfxSpawnRequestPointType.Vector3;
        bool isObjectTransform => spawnPointType == VfxSpawnRequestPointType.ObjectTransform;
        bool isVfxSpawnPoint => spawnPointType == VfxSpawnRequestPointType.VfxSpawnPoint;
#endif
    }
}
