using UnityEngine;

namespace Standard_Library
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;
        [Header("Singleton Config")]
        [SerializeField] private bool defaultDontDestroyOnLoad;
        public virtual void Awake()
        {
            if(_instance == null)
            {
                _instance = this as T;
                if(defaultDontDestroyOnLoad)  DontDestroyOnLoad(_instance);
                return;
            }
            if (_instance != this as T)
            {
                Debug.Log("Duplicate Singleton Instance Detected" + gameObject.name + "  " + _instance.name);
                Destroy(gameObject);
            }
            T[] objects = Object.FindObjectsByType<T>(FindObjectsSortMode.InstanceID);
            if (objects.Length > 0)
            {
                foreach (var obj in objects)
                {
                    if (obj != null) _instance = obj;
                }
            }
            else _instance = GetComponent<T>();
            if(defaultDontDestroyOnLoad)  DontDestroyOnLoad(_instance);
        }
        private void SetDontDestroyOnLoad()
        {
            DontDestroyOnLoad(_instance);
        }

        public static T GetInstance()
        {
            if (_instance) return _instance;
            _instance = FindAnyObjectByType<T>();
            if(_instance) return _instance;
            Debug.Log("Instance Not Found, creating new object");
            GameObject go = new GameObject(typeof(T).Name);
            go.AddComponent<T>();
            _instance = go.GetComponent<T>();
            return _instance;
        }

        protected static T GetInstanceNoSpawn()
        {
            return _instance;
        }
    }
}
