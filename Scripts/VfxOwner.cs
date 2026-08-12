using System;
using System.Collections.Generic;
using System.Linq;
using RAXY.Utility.Gameplay;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RAXY.VfxManager
{
    public class VfxOwner : MonoBehaviour, IVfxBank
    {
        [TitleGroup("Reference")]
        public NamedTransformSet spawnPoint;
        [TitleGroup("Reference")]
        [SerializeField] 
        List<VfxBankBaseSO> bankSoList = new();
        
        [TitleGroup("Data")]
        [ShowInInspector]
        [HideReferenceObjectPicker]
        public List<IVfxBank> Banks { get; private set; } = new();

        [TitleGroup("Data")]
        [ShowInInspector]
        [HideReferenceObjectPicker]
        public List<VfxEntry> VfxEntries { get; private set; }

        [TitleGroup("Data")]
        [ShowInInspector]
        [HideReferenceObjectPicker]
        public Dictionary<string, VfxEntry> VfxEntryDict { get; set; }

        [TitleGroup("Data")]
        [ShowInInspector]
        [HideReferenceObjectPicker]
        public List<VfxSpawnSetting> VfxSpawnSettings { get; set; }

        [TitleGroup("Data")]
        [ShowInInspector]
        [HideReferenceObjectPicker]
        public Dictionary<string, VfxSpawnSetting> VfxSpawnSettingDict { get; set; }

        [TitleGroup("Debug")]
        [ShowInInspector]
        public Dictionary<string, VfxInstance> trackedVfxInstance = new Dictionary<string, VfxInstance>();

        [TitleGroup("Debug")]
        [ShowInInspector]
        [HideReferenceObjectPicker]
        VfxSpawnRequest _debugReq;

        void Start()
        {
            foreach (var bankSO in bankSoList)
            {
                AddBank(bankSO, false);
            }

            BuildDictionary();
        }

        [TitleGroup("Debug Function")]
        [Button]
        public void BuildDictionary()
        {
            VfxEntries = new();
            VfxEntryDict = new();
            VfxSpawnSettings = new();
            VfxSpawnSettingDict = new();

            foreach (var bank in Banks)
            {
                VfxEntries.AddRange(bank.VfxEntries);
                VfxSpawnSettings.AddRange(bank.VfxSpawnSettings);
            }

            foreach (var vfxEntry in VfxEntries)
            {
                VfxEntryDict.Add(vfxEntry.vfxId, vfxEntry);
            }

            foreach (var vfxSpawnPoint in VfxSpawnSettings)
            {
                VfxSpawnSettingDict.Add(vfxSpawnPoint.spawnSettingId, vfxSpawnPoint);
            }
        }

        [TitleGroup("Debug Function")]
        [Button]
        public void AddBank(IVfxBank incomingBank, bool buildDict = true)
        {
            if (Banks == null)
                Banks = new List<IVfxBank>();
            
            if (Banks.Contains(incomingBank))
                return;
            
            Banks.Add(incomingBank);

            if (buildDict)
                BuildDictionary();
        }

        public void ReplaceSpawnPoint(NamedTransformSet spawnPoint)
        {
            this.spawnPoint = spawnPoint;
        }

        // ===================================================
        // REGISTER / UNREGISTER
        // ===================================================

        public void Register(VfxInstance instance)
        {
            if (instance == null || instance.Request == null)
                return;

            string key = instance.Request.reqId;

            // If already exists, replace the reference safely
            if (trackedVfxInstance.ContainsKey(key))
                trackedVfxInstance[key] = instance;
            else
                trackedVfxInstance.Add(key, instance);

            instance.SetOwner(this);
        }

        public void Unregister(VfxInstance instance)
        {
            if (instance == null || instance.Request == null)
                return;

            if (trackedVfxInstance.ContainsKey(instance.Request.reqId))
                trackedVfxInstance.Remove(instance.Request.reqId);
        }

        public bool HasEntry(string entryId)
        {
            return trackedVfxInstance.ContainsKey(entryId);
        }

        public VfxInstance GetVfx(string entryId)
        {
            if (trackedVfxInstance.TryGetValue(entryId, out var inst))
                return inst;

            return null;
        }

        // ===================================================
        // TOGGLE
        // ===================================================

        [TitleGroup("Debug Function")]
        [Button]
        public void Request_SpawnVfx(string spawnSettingId)
        {
            var req = VfxManager.Instance.BuildVfxSpawnRequest(this, spawnSettingId, this);
            _debugReq = req;
            VfxManager.Instance.SpawnVfx(req);
        }

        [Button, HorizontalGroup("Debug Function/Op")]
        public void ActivateTrackedVfx(string vfxId)
        {
            ToggleVfx(vfxId, true);
        }

        [Button, HorizontalGroup("Debug Function/Op")]
        public void DeactivateTrackedVfx(string vfxId)
        {
            ToggleVfx(vfxId, false);
        }

        public void ToggleVfx(string vfxId, bool enable)
        {
            if (trackedVfxInstance.TryGetValue(vfxId, out var inst))
            {
                inst.gameObject.SetActive(enable);
                if (enable)
                    inst.Particle?.Play();
            }
        }

        public void ToggleVfx(VfxInstance instance, bool enable)
        {
            if (instance == null || instance.Request == null) 
                return;

            ToggleVfx(instance.Request.reqId, enable);
        }

        public void ForceStop(string entryId)
        {
            var inst = GetVfx(entryId);
            if (inst != null)
            {
                inst.gameObject.SetActive(false);
                inst.Particle?.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }
        }

        // ===================================================
        // MASS CONTROL (very useful for interrupted animations)
        // ===================================================

        public void StopAllVfx()
        {
            foreach (var kvp in trackedVfxInstance)
            {
                var inst = kvp.Value;
                if (inst != null && inst.gameObject.activeSelf)
                {
                    inst.gameObject.SetActive(false);
                    inst.Particle?.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                }
            }
        }

        public void DestroyAllVfx()
        {
            foreach (var kvp in trackedVfxInstance)
            {
                var inst = kvp.Value;
                if (inst != null)
                    Destroy(inst.gameObject);
            }

            trackedVfxInstance.Clear();
        }

        // Called by VfxInstance when returned to pool / destroyed
        public void OnInstanceDestroyed(VfxInstance instance)
        {
            Unregister(instance);
        }
    }
}
