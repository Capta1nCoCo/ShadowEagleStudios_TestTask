using UnityEngine;

public class DeathSpawner : MonoBehaviour
{
    [SerializeField] private EnemyParams enemiesToSpawn;

    public void SpawnEnemiesInDeathArea()
    {
        Vector3 deathPos = transform.position;
        GameEvents.OnDeathSpawn?.Invoke(enemiesToSpawn, deathPos);
    }
}
