using System;
using RAXY.Core.Addressable;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RAXY.VfxManager
{
    [Serializable]
    public class VfxEntry : AddressableAssetProviderGameObject
    {
        [InlineButton("SetId", "Set Id")]
        public string vfxId;

#if UNITY_EDITOR
        public virtual void SetId()
        {
            if (useAddressable)
                vfxId = assetReference?.editorAsset.name ?? "";
            else
                vfxId = directAsset?.name ?? "";

        }
#endif
    }
}
