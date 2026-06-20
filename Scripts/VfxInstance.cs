using System;
using RAXY.Pooling;
using RAXY.Utility;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RAXY.VfxManager
{
    public class VfxInstance : MonoBehaviour
    {
        public VfxOwner Owner { get; private set; }
        public string OriginalName { get; private set; }

        [InfoBox("If this instance doesn't use Pooling System, just leave it empty")]
        public PoolableObject PoolableObject;
        public ParticleSystem Particle;

        public VfxSpawnRequest Request { get; set; }

        [Button]
        void GetComponentFromSelf()
        {
            PoolableObject = GetComponent<PoolableObject>();
            Particle = GetComponent<ParticleSystem>();
        }

        void Awake()
        {
            GetComponentFromSelf();
            OriginalName = CustomUtility.GetObjectNameWithout_Clone(gameObject.name);
        }

        public void SetRequest(VfxSpawnRequest request)
        {
            Request = request;
        }

        public void DestroyPoolable()
        {
            Destroy(PoolableObject);
        }

        public void SetOwner(VfxOwner vfxOwner)
        {
            Owner = vfxOwner;
        }
#if UNITY_EDITOR
        void Oalidate()
        {
            GetComponentFromSelf();
        }
#endif
    }
}