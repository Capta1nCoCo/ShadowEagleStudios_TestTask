using System.Collections.Generic;
using UnityEngine;

public abstract class ObjectPool : MonoBehaviour
{
    [Header("Object Pool Parameters")]
    [SerializeField] protected int poolSize = 5;

    protected abstract void PopulatePools();

    protected virtual void SpawnPoolObjects(GameObject prefab, Queue<GameObject> pool)
    {
        for (var i = 0; i < poolSize; i++)
        {
            var obj = Instantiate(prefab, transform);
            pool.Enqueue(obj);
            obj.SetActive(false);
        }
    }

    protected virtual GameObject GetObjectFromPool(Queue<GameObject> pool)
    {
        var obj = pool.Dequeue();
        if (obj.activeSelf)
        {
            obj.SetActive(false);
        }
        obj.SetActive(true);
        pool.Enqueue(obj);
        return obj;
    }
}
