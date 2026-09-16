using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RAXY.VfxManager
{
    public static class VfxPrefabUtility
    {
        public static bool IsAlive(Object unityObject)
        {
            return unityObject;
        }

        public static bool TryGetPrefabRoot(VfxEntry entry, out GameObject prefabRoot, out string failureReason)
        {
            prefabRoot = null;
            failureReason = null;

            if (entry == null)
            {
                failureReason = "VFX entry is null.";
                return false;
            }

            GameObject candidate = entry.UseAddressable ? entry.Asset : entry.DirectAsset;
            return TryGetPrefabRoot(candidate, out prefabRoot, out failureReason);
        }

        public static bool TryGetPrefabRoot(GameObject candidate, out GameObject prefabRoot, out string failureReason)
        {
            prefabRoot = null;
            failureReason = null;

            if (!IsAlive(candidate))
            {
                failureReason =
                    "Prefab reference is missing or destroyed. Reassign the VFX entry to a project prefab (not a scene instance).";
                return false;
            }

#if UNITY_EDITOR
            try
            {
                if (EditorUtility.IsPersistent(candidate))
                {
                    prefabRoot = candidate;
                    return true;
                }
            }
            catch (MissingReferenceException)
            {
                failureReason = "Prefab reference was destroyed.";
                return false;
            }
#endif

            try
            {
                if (!candidate.scene.IsValid())
                {
                    prefabRoot = candidate;
                    return true;
                }

                failureReason = "VFX entry must reference a project prefab, not a scene object.";
                return false;
            }
            catch (MissingReferenceException)
            {
                failureReason = "Prefab reference was destroyed.";
                return false;
            }
        }
    }
}
