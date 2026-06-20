using System;
using System.Collections.Generic;
using System.Linq;
using RAXY.Core.Addressable;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RAXY.VfxManager
{
    [CreateAssetMenu(fileName = "VfxBankOverride", menuName = "RAXY/VFX/Vfx Bank Override")]
    public class VfxBankOverrideSO : VfxBankBaseSO, IAddressableAssetRequester
    {
        [HideLabel]
        public VfxBankOverride vfxBankOverride;

        public IVfxBank MasterBank => vfxBankOverride.MasterBank;
        public override List<VfxEntry> VfxEntries => vfxBankOverride.VfxEntries;
        public override List<VfxSpawnSetting> VfxSpawnSettings => vfxBankOverride.VfxSpawnSettings;

        public new List<AssetReference> AssetReferences => vfxBankOverride.AssetReferences;
    }

    [Serializable]
    public class VfxBankOverride : IVfxBank, IAddressableAssetRequester
    {
        [TitleGroup("Data")]
        [OnValueChanged("Reset")]
        [ShowInInspector]
        [HideReferenceObjectPicker]
        public IVfxBank MasterBank;

        [TitleGroup("Data")]
        //[ListDrawerSettings(IsReadOnly = true, DefaultExpandedState = true)]
        [SerializeField] 
        [TableList(AlwaysExpanded = true, IsReadOnly = true)]
        List<VfxOverrideEntry> overrides;

        public List<AssetReference> AssetReferences
        {
            get
            {
                var refs = new List<AssetReference>();

                foreach (var vfxEntry in overrides)
                {
                    if (vfxEntry.UseAddressable)
                        refs.Add(vfxEntry.AssetReference);
                }

                return refs;
            }
        }

#if UNITY_EDITOR
        [HorizontalGroup("Data/Op")]
        [GUIColor(1, 0, 0)]
        [Button]
        void Reset()
        {
            if (MasterBank == null)
                return;

            overrides = new List<VfxOverrideEntry>();

            foreach (var masterEntry in MasterBank.VfxEntries)
            {
                var newOverrideEntry = new VfxOverrideEntry(masterEntry);
                overrides.Add(newOverrideEntry);
            }
        }

        [HorizontalGroup("Data/Op")]
        [GUIColor(0, 1, 0)]
        [Button]
        void Refresh()
        {
            if (MasterBank == null)
                return;

            foreach (var overrideEntry in overrides)
            {
                var masterEntry = MasterBank.VfxEntries.Find(x => x.vfxId == overrideEntry.MasterVfxId);
                overrideEntry.SetMasterEntry(masterEntry);
            }

            // 1. Remove entries in overrides that don't exist in masterBank
            overrides.RemoveAll(o => MasterBank.VfxEntries.All(m => m.vfxId != o.MasterVfxId));

            // 2. Add missing masterBank entries
            foreach (var mEntry in MasterBank.VfxEntries)
            {
                bool exists = overrides.Any(o => o.MasterVfxId == mEntry.vfxId);
                if (!exists)
                    overrides.Add(new VfxOverrideEntry(mEntry));
            }
        }
#endif

        [TitleGroup("Result")]
        [ShowInInspector]
        public List<VfxEntry> VfxEntries
        {
            get
            {
                if (MasterBank == null || MasterBank.VfxEntries == null)
                    return null;

                List<VfxEntry> newEntries = new();
                foreach (var entry in MasterBank.VfxEntries)
                {
                    if (entry == null) 
                        continue;

                    var selectedOverrider = overrides?.Find(x => x.MasterVfxId == entry.vfxId);

                    var newVfxEntry = new VfxEntry();
                    newVfxEntry.vfxId = entry.vfxId;

                    if (selectedOverrider != null && selectedOverrider.Asset != null)
                    {
                        newVfxEntry.Asset = selectedOverrider.Asset;
                    }
                    else
                    {
                        newVfxEntry.Asset = entry.Asset;
                    }

                    newEntries.Add(newVfxEntry);
                }

                return newEntries;
            }
        }
        public List<VfxSpawnSetting> VfxSpawnSettings => MasterBank.VfxSpawnSettings;
    }
}
