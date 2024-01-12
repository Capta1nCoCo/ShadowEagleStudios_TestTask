using System;
using UnityEngine;

[Serializable]
public struct EnemyParams
{
    [SerializeField] private EnemyType enemyType;
    [SerializeField] private int quantity;

    public EnemyType getEnemyType { get { return enemyType; } }
    public int getQuantity { get { return quantity; } }
}