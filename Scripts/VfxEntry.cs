using System;
using RAXY.Core.Addressable;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RAXY.VfxManager
{
    [Serializable]
    public class VfxEntry
    {
        [InlineButton("SetId", "Set Id")]
        public string vfxId;

        [HideReferenceObjectPicker]
        public AddressableAssetProviderGameObject vfxPrefabProvider = new();

        public bool UseAddressable =>
            vfxPrefabProvider != null && vfxPrefabProvider.UseAddressable;

        public GameObject DirectAsset => vfxPrefabProvider?.DirectAsset;
        public GameObject Asset => vfxPrefabProvider?.Asset;
        public AssetReferenceT<GameObject> AssetReference => vfxPrefabProvider?.AssetReference;

        public void SetDirectPrefabReference(GameObject prefab)
        {
            vfxPrefabProvider ??= new AddressableAssetProviderGameObject();
            vfxPrefabProvider.Asset = prefab;
        }

#if UNITY_EDITOR
        public virtual void SetId()
        {
            if (vfxPrefabProvider == null)
            {
                vfxId = "";
                return;
            }

            if (vfxPrefabProvider.UseAddressable)
                vfxId = vfxPrefabProvider.AssetReference?.editorAsset.name ?? "";
            else
                vfxId = vfxPrefabProvider.DirectAsset?.name ?? "";
        }
#endif
    }
}
