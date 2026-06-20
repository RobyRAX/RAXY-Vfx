using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RAXY.VfxManager
{
    [HideReferenceObjectPicker]
    [Serializable]
    public class VfxSpawnSetting
    {
        public string spawnSettingId;

        [ValueDropdown("vfxIds")]
        public string vfxId;

        [InfoBox("Keep in mind... ObjectTransform isn't available for entry. You have to build the request on runtime", "isObjectTransform")]
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
        [LabelText("Scale Override")]
        public Vector3 vfxScaleOverride_Vector3 = Vector3.one;

        [ShowIf("@isVfxSpawnPoint")]
        [ValueDropdown("spawnPointIds")]
        public string spawnPointEntryId;

#if UNITY_EDITOR
        string Label => $"{spawnSettingId} - {vfxId}";

        [ShowIf("@isVfxSpawnPoint && overrideScale")]
        [LabelText("Scale Override")]
        [Tooltip("Taken from spawn point's lossy scale")]
        [ShowInInspector]
        Vector3 VfxSpawnPointScale
        {
            get
            {
                if (spawnPointScaleDict == null)
                {
                    return Vector3.one;
                }

                if (string.IsNullOrEmpty(spawnPointEntryId))
                {
                    return Vector3.one;
                }

                if (!spawnPointScaleDict.TryGetValue(spawnPointEntryId, out var scale))
                {
                    return Vector3.one;
                }

                return scale;
            }
        }

        bool isVector3 => spawnPointType == VfxSpawnRequestPointType.Vector3;
        bool isObjectTransform => spawnPointType == VfxSpawnRequestPointType.ObjectTransform;
        bool isVfxSpawnPoint => spawnPointType == VfxSpawnRequestPointType.VfxSpawnPoint;

        static List<string> vfxIds;
        static List<string> spawnPointIds;
        static Dictionary<string, Vector3> spawnPointScaleDict;

        public static void Set_EditorData(VfxSpawnSetting_EditorData editorData)
        {
            if (editorData == null)
            {
                return;
            }

            if (editorData.VfxIds != null)
            {
                vfxIds = editorData.VfxIds;
            }

            if (editorData.TransformScaleDict != null)
            {
                spawnPointScaleDict = editorData.TransformScaleDict;
                spawnPointIds = spawnPointScaleDict.Keys.ToList();
            }
        }
#endif
    }

#if UNITY_EDITOR
    [Serializable]
    public class VfxSpawnSetting_EditorData
    {
        public List<string> VfxIds;
        [ShowInInspector]
        public Dictionary<string, Vector3> TransformScaleDict;
    }
#endif
}

