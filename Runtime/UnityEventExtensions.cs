using UnityEngine.Events;

namespace Standard_Library
{
    public static class UnityEventExtensions 
    {
        public static void AddOnce(this UnityEvent unityEvent, UnityAction call)
        {
            if (unityEvent == null || call == null) return;
            UnityAction wrapper = null;
            wrapper = () =>
            {
                unityEvent.RemoveListener(wrapper);
                call();
            };
            unityEvent.AddListener(wrapper);
        }
        public static void AddOnce<T>(this UnityEvent<T> unityEvent, UnityAction<T> call)
        {
            if (unityEvent == null || call == null) return;

            UnityAction<T> wrapper = null;
            wrapper = (value) =>
            {
                unityEvent.RemoveListener(wrapper);
                call(value);
            };
            unityEvent.AddListener(wrapper);
        }
    }
}

