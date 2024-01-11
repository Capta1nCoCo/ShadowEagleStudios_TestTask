public interface IDamageable
{
    public float Health { get; set; }
    public bool IsDead { get; set; }

    public void TakeDamage(float damage);
    public void Die();
}
