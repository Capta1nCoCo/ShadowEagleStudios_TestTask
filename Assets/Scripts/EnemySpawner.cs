using System.Collections.Generic;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(EnemyPool))]
public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private LevelConfig Config;

    private int currWave = 0;

    private List<Enemy> Enemies = new List<Enemy>();

    private EnemyPool _enemyPool;

    public List<Enemy> getEnemies { get { return Enemies; } }

    public void AddEnemie(Enemy enemy)
    {
        Enemies.Add(enemy);
    }

    public void RemoveEnemie(Enemy enemy)
    {
        Enemies.Remove(enemy);
        if (Enemies.Count == 0)
        {
            SpawnWave();
        }
    }

    private void Awake()
    {
        _enemyPool = GetComponent<EnemyPool>();
    }

    private void Start()
    {
        SpawnWave();
    }

    private void SpawnWave()
    {
        if (currWave >= Config.getWaves.Length)
        {
            GameEvents.OnVictory?.Invoke();
            return;
        }

        Wave wave = Config.getWaves[currWave];
        foreach (EnemyParams enemyParams in wave.getEnemyParams)
        {
            int enemyQuantity = enemyParams.getQuantity;
            EnemyType enemyType = enemyParams.getEnemyType;
            if (enemyQuantity > 0 && enemyType != EnemyType.None)
            {
                for (int i = 0; i < enemyQuantity; i++)
                {
                    Vector3 pos = new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10));
                    var enemy = _enemyPool.GetEnemyByType(enemyType);
                    enemy.transform.position = pos;
                    enemy.transform.rotation = Quaternion.identity;
                    enemy.GetComponent<Enemy>()?.Init(this);
                }
            }
        }
        currWave++;
    }
}
