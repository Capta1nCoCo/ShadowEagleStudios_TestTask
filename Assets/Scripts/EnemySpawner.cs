using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyPool))]
public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private LevelConfig Config;

    private int currWave = 0;

    private List<Enemy> Enemies = new List<Enemy>();

    private EnemyPool _enemyPool;

    public List<Enemy> getEnemies { get { return Enemies; } }

    public void AddEnemy(Enemy enemy)
    {
        Enemies.Add(enemy);
    }

    public void RemoveEnemy(Enemy enemy)
    {
        Enemies.Remove(enemy);
        if (enemy.GetComponent<DeathSpawner>() != null)
        {
            return;
        }
        if (Enemies.Count == 0)
        {
            SpawnWave();
        }
    }

    private void Awake()
    {
        _enemyPool = GetComponent<EnemyPool>();
        GameEvents.OnDeathSpawn += ProcessEnemyParams;
    }

    private void OnDestroy()
    {
        GameEvents.OnDeathSpawn -= ProcessEnemyParams;
    }

    private void Start()
    {
        SpawnWave();
    }

    private void SpawnWave()
    {
        int numWaves = Config.getWaves.Length;
        if (currWave >= numWaves)
        {
            GameEvents.OnVictory?.Invoke();
            return;
        }

        Wave wave = Config.getWaves[currWave];
        foreach (EnemyParams enemyParams in wave.getEnemyParams)
        {
            ProcessEnemyParams(enemyParams, Vector3.zero);
        }
        currWave++;
        GameEvents.OnWaveSpawned?.Invoke(currWave, numWaves);
    }

    private void ProcessEnemyParams(EnemyParams enemyParams, Vector3 pos)
    {
        int enemyQuantity = enemyParams.getQuantity;
        EnemyType enemyType = enemyParams.getEnemyType;
        if (enemyQuantity > 0 && enemyType != EnemyType.None)
        {
            SpawnEnemies(enemyQuantity, enemyType, pos);
        }
    }

    private void SpawnEnemies(int enemyQuantity, EnemyType enemyType, Vector3 pos)
    {
        for (int i = 0; i < enemyQuantity; i++)
        {
            Vector3 spawnPos = pos == Vector3.zero ? new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10)) :
                new Vector3(pos.x + Random.Range(-3, 3), 0, pos.z + Random.Range(-3, 3));
            var enemy = _enemyPool.GetEnemyByType(enemyType);
            enemy.transform.position = spawnPos;
            enemy.transform.rotation = Quaternion.identity;
            enemy.GetComponent<Enemy>()?.Init(this);
        }
    }
}