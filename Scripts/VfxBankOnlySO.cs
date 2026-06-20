using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RAXY.VfxManager
{
    [CreateAssetMenu(fileName = "VfxBankOnly", menuName = "RAXY/VFX/Vfx Bank Only")]
    public class VfxBankOnlySO : VfxBankBaseSO
    {
        [HideLabel]
        [SerializeField] VfxBankOnly vfxBank;

        public override List<VfxEntry> VfxEntries => vfxBank.VfxEntries;
        public override List<VfxSpawnSetting> VfxSpawnSettings => vfxBank.VfxSpawnSettings;
    }

    [Serializable]
    public class VfxBankOnly : VfxBank
    {
#if UNITY_EDITOR
        protected override bool HideOtherGroups => true;
#endif
    }
}
