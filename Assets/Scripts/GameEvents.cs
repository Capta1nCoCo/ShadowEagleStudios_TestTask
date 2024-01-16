using System;

public static class GameEvents
{
    public static Action OnVictory;
    public static Action OnDefeat;
    public static Action OnPlayerInit;
    public static Action OnCooldown;
    public static Action<float> OnHealthChanged;
    public static Action<int, int> OnWaveSpawned;
    public static Action OnEnemyDeath;
}
