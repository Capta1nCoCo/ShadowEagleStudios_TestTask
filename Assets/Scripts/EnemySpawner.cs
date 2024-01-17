using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyPool))]
public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private LevelConfig Config;

    [Header("Spawn Position Params")]
    [SerializeField] private float defaultPosDelta = 10f;
    [SerializeField] private float relativePosDelta = 2f;

    private int currWave = 0;
    private Vector3 lastSpawnPos;

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
        SpawnNewWave();
    }

    private void SpawnNewWave()
    {
        if (Enemies.Count == 0)
        {
            lastSpawnPos = Vector3.zero;
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
            ProcessEnemyParams(enemyParams, lastSpawnPos);
        }
        currWave++;
        GameEvents.OnWaveSpawned?.Invoke(currWave, numWaves);
    }

    public void ProcessEnemyParams(EnemyParams enemyParams, Vector3 pos)
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
            Vector3 spawnPos = GetRandomRelativePos(pos);
            var enemy = _enemyPool.GetEnemyByType(enemyType);
            enemy.transform.rotation = Quaternion.identity;
            enemy.GetComponent<ISpawn>()?.InitSpawn(this, spawnPos);
        }
    }

    private Vector3 GetRandomRelativePos(Vector3 pos)
    {
        return pos == Vector3.zero ? new Vector3(Random.Range(-defaultPosDelta, defaultPosDelta), 0, Random.Range(-defaultPosDelta, defaultPosDelta)) :
                        new Vector3(pos.x + Random.Range(-relativePosDelta, relativePosDelta), 0, pos.z + Random.Range(-relativePosDelta, relativePosDelta));
    }
}