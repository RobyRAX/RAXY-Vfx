using System.Collections.Generic;
using RAXY.Core.Addressable;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RAXY.VfxManager
{
    public abstract class VfxBankBaseSO : ScriptableObject, IVfxBank, IAddressableAssetRequester
    {
        public abstract List<VfxEntry> VfxEntries { get; }
        public Dictionary<string, VfxEntry> VfxEntryDict { get; set; }

        public abstract List<VfxSpawnSetting> VfxSpawnSettings { get; }
        public Dictionary<string, VfxSpawnSetting> VfxSpawnSettingDict { get; set; }

        public List<AssetReference> AssetReferences
        {
            get
            {
                var refs = new List<AssetReference>();

                foreach (var entry in VfxEntries)
                {
                    if (entry.UseAddressable)
                        refs.Add(entry.AssetReference);
                }

                return refs;
            }
        }

        public void BuildDictionary()
        {
            VfxEntryDict = new();
            foreach (var vfxEntry in VfxEntries)
            {
                VfxEntryDict.Add(vfxEntry.vfxId, vfxEntry);
            }

            VfxSpawnSettingDict = new();
            foreach (var vfxSpawnPoint in VfxSpawnSettings)
            {
                VfxSpawnSettingDict.Add(vfxSpawnPoint.spawnSettingId, vfxSpawnPoint);
            }
        }
    }
}
