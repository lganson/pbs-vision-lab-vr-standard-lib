using System.Collections.Generic;
using Standard_Library;
using UnityEngine;
using UnityEngine.Events;

namespace DefaultNamespace
{
    public class PersistentObjectPoolManager : Singleton<PersistentObjectPoolManager>
    {
        private readonly Dictionary<System.Type, object> _pools = new Dictionary<System.Type, object>();
        private static readonly UnityEvent ClearObjectPools = new UnityEvent();
        private UnityAction _clearPoolAction;
        public override void Awake()
        {
            base.Awake();
            SetDontDestroyOnLoad();
            _clearPoolAction = () => {ClearObjectPools.Invoke();};
            GlobalTaskSequencer.OnSequenceReset.AddListener(_clearPoolAction);
        }

        public static PersistentObjectPool<T> GetPool<T>() where T : class
        {
            var type = typeof(T);
            if (!GetInstance()._pools.ContainsKey(type))
            {
                var pool = new PersistentObjectPool<T>();
                Debug.Log("Creating new pool for " + type);
                GetInstance()._pools[type] = pool;
                ClearObjectPools.AddListener(pool.ClearPool);
            }
            return (PersistentObjectPool<T>)GetInstance()._pools[type];
        }

        public void OnDestroy()
        {
            ClearObjectPools.Invoke();
            ClearObjectPools.RemoveAllListeners();
            GlobalTaskSequencer.OnSequenceReset.RemoveListener(_clearPoolAction);
        }
    }
}