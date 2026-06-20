using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System.Linq;
using RAXY.Utility.Gameplay;
using RAXY.Core.Addressable;

#if UNITY_EDITOR
using Sirenix.Utilities.Editor;
#endif

namespace RAXY.VfxManager
{
    [CreateAssetMenu(fileName = "VfxBank", menuName = "RAXY/VFX/Vfx Bank")]
    public class VfxBankSO : VfxBankBaseSO
    {
        [HideLabel]
        public VfxBank vfxBank;
        
        [HideReferenceObjectPicker]
        public override List<VfxEntry> VfxEntries => vfxBank.VfxEntries;
        public override List<VfxSpawnSetting> VfxSpawnSettings => vfxBank.VfxSpawnSettings;
    }

    [Serializable]
    public class VfxBank : IVfxBank, IAddressableAssetRequester
    {
        [TitleGroup("Bank")]
        [SerializeField]
        [HideReferenceObjectPicker]
        List<VfxEntry> vfxEntries;

        [TitleGroup("Bank")]
        public List<VfxEntry> VfxEntries => vfxEntries;

        [TitleGroup("Spawn Settings")]
        [HideIf("HideOtherGroups")]
        [SerializeField]
        [ListDrawerSettings(ListElementLabelName = "Label", OnTitleBarGUI = "DrawRefreshBtn", DefaultExpandedState = true)]
        List<VfxSpawnSetting> vfxSpawnSettings;
        public List<VfxSpawnSetting> VfxSpawnSettings => vfxSpawnSettings;

        public List<AssetReference> AssetReferences
        {
            get
            {
                var refs = new List<AssetReference>();

                foreach (var vfxEntry in vfxEntries)
                {
                    if (vfxEntry.UseAddressable)
                        refs.Add(vfxEntry.AssetReference);
                }

                return refs;
            }
        }

#if UNITY_EDITOR
        [HideInInspector]
        protected virtual bool HideOtherGroups => false;

        [TitleGroup("Editor Data")]
        [HideIf("HideOtherGroups")]
        [SerializeField]
        [OnValueChanged("OnValidate")]
        public NamedTransformSet spawnReference;

        [TitleGroup("Editor Data")]
        [HideIf("HideOtherGroups")]
        [SerializeField, ReadOnly]
        private VfxSpawnSetting_EditorData editorData;

        // Draw refresh icon button on Spawn Settings group
        private void DrawRefreshBtn()
        {
            if (SirenixEditorGUI.ToolbarButton(EditorIcons.Refresh))
            {
                OnValidate();
            }
        }
        private void OnValidate()
        {
            editorData = new VfxSpawnSetting_EditorData();
            editorData.VfxIds = VfxEntries.Select(x => x.vfxId).ToList();

            if (spawnReference)
            {
                editorData.TransformScaleDict = new Dictionary<string, Vector3>();

                foreach (var entry in spawnReference.entries)
                {
                    editorData.TransformScaleDict.Add(entry.entryId, entry.tranform.lossyScale);
                }
            }

            VfxSpawnSetting.Set_EditorData(editorData);
        }
#endif
    }
}
