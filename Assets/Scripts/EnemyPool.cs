using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : ObjectPool
{
    [SerializeField] private GameObject goblinPrefab;

    private Queue<GameObject> goblinPool = new Queue<GameObject>();

    private void Awake()
    {
        PopulatePools();
    }

    protected override void PopulatePools()
    {
        SpawnPoolObjects(goblinPrefab, goblinPool);
    }

    public GameObject GetEnemyByType(EnemyType type)
    {
        switch (type)
        {
            case EnemyType.Goblin: 
                return GetObjectFromPool(goblinPool);

            default: 
                return null;
        }
    }
}
