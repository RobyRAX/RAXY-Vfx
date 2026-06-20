using System;
using RAXY.Core.Addressable;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RAXY.VfxManager
{
    [Serializable]
    public class VfxOverrideEntry : AddressableAssetProviderGameObject
    {
        [PropertyOrder(-1)]
        [ShowInInspector]
        public string MasterVfxId => masterEntry?.vfxId ?? "";

        [SerializeReference]
        [HideInInspector]
        VfxEntry masterEntry;

        public void SetMasterEntry(VfxEntry masterEntry)
        {
            this.masterEntry = masterEntry;
        }

        public VfxOverrideEntry() { }
        public VfxOverrideEntry(VfxEntry masterEntry)
        {
            SetMasterEntry(masterEntry);
        }

#if UNITY_EDITOR
        bool ValidateVfxInstance
        {
            get
            {
                if (AssetReference == null)
                    return false;

                var asset = AssetReference.editorAsset;
                if (asset == null)
                    return false;

                return asset.GetComponent<VfxInstance>() == null;
            }
        }
#endif
    }
}
