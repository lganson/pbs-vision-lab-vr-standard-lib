using System.Collections.Generic;
using Standard_Library;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class PersistentObjectPool<T>
{
    private List<T> _pooledObjects = new List<T>();

    public void InitPool(List<T> list)
    {
        _pooledObjects = list;
        Debug.Log("Created pool of type: " + typeof(T).Name +  " Length: " + _pooledObjects.Count);
    }

    public bool HasNext()
    {
        return _pooledObjects is { Count: > 0 };
    }
    public T GetNext()
    {
        if (HasNext())
        {
            T next = _pooledObjects[0];
            _pooledObjects.RemoveAt(0);
            Debug.Log(next);
            return next;
        }
        Debug.LogError("No more objects available in pool");
        throw new System.Exception("Pool exhausted");
    }

    public void ClearPool() => _pooledObjects?.Clear();

    public override string ToString()
    {
        return _pooledObjects.Count + " objects";
    }
}