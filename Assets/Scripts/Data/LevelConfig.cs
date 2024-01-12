using UnityEngine;

[CreateAssetMenu(fileName = "LevelConfig", menuName = "Data/LevelConfig")]
public class LevelConfig : ScriptableObject
{
    [SerializeField] private Wave[] Waves;

    public Wave[] getWaves { get { return Waves; } }
}