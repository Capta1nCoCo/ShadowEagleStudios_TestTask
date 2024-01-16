using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : ObjectPool
{
    [SerializeField] private GameObject goblinPrefab;
    [SerializeField] private GameObject hobGoblinPrefab;

    private Queue<GameObject> goblinPool = new Queue<GameObject>();
    private Queue<GameObject> hobGoblinPool = new Queue<GameObject>();

    private void Awake()
    {
        PopulatePools();
    }

    protected override void PopulatePools()
    {
        SpawnPoolObjects(goblinPrefab, goblinPool);
        SpawnPoolObjects(hobGoblinPrefab, hobGoblinPool);
    }

    public GameObject GetEnemyByType(EnemyType type)
    {
        switch (type)
        {
            case EnemyType.Goblin: 
                return GetObjectFromPool(goblinPool);

            case EnemyType.HobGoblin:
                return GetObjectFromPool(hobGoblinPool);

            default: 
                return null;
        }
    }
}
