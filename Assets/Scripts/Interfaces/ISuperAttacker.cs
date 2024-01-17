public interface ISuperAttacker : IAttacker
{
    public float SuperAttackSpeed { get; set; }
    public float SuperAttackDamage { get; set; }
    public float LastSuperTime { get; set; }

    public void SuperAttack();
    public void DealSuperDamage();
}
