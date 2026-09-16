using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RAXY.VfxManager
{
    [Serializable]
    public class VfxBankProvider
    {
        [SerializeField]
        bool useVfxBankSO;

        [ShowIf("useVfxBankSO")]
        [SerializeField]
        VfxBankSO vfxBankSO;

        [HideIf("useVfxBankSO")]
        [SerializeField]
        VfxBank vfxBank;

        public IVfxBank VfxBank => useVfxBankSO ? vfxBankSO : vfxBank;
    }
}
