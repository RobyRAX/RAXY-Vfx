using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RAXY.VfxManager
{
    public interface IVfxBank
    {
        [HideReferenceObjectPicker]
        public List<VfxEntry> VfxEntries { get; }
        
        [HideReferenceObjectPicker]
        public List<VfxSpawnSetting> VfxSpawnSettings { get; }

        public VfxEntry GetVfxEntry(string vfxId)
        {
            return VfxEntries.Find(x => x.vfxId == vfxId);
        }

        public VfxSpawnSetting GetVfxSpawnSetting(string spawnSettingId)
        {
            return VfxSpawnSettings.Find(x => x.spawnSettingId == spawnSettingId);
        }
    }

    public static class VfxBankExtension
    {
        public static void BuildDictionary(this IVfxBank vfxBank)
        {
        }
    }
}
