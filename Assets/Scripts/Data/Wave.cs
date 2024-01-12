using UnityEngine;

[CreateAssetMenu(fileName = "Wave", menuName = "Data/Waves")]
public class Wave : ScriptableObject
{
    [SerializeField] private EnemyParams[] EnemyParams;

    public EnemyParams[] getEnemyParams { get { return EnemyParams; } }
}
