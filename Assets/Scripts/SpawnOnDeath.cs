using UnityEngine;

public class SpawnOnDeath : MonoBehaviour
{
    [SerializeField] private EnemyType enemyType;
    [SerializeField] private int quantity;

    private void Awake()
    {
        GameEvents.OnEnemyDeath += SpawnEnemiesInDeathArea;
    }

    private void OnDestroy()
    {
        GameEvents.OnEnemyDeath -= SpawnEnemiesInDeathArea;
    }

    private void SpawnEnemiesInDeathArea()
    {
        Vector3 deathPos = transform.position;
    }
}
