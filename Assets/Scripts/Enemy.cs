using UnityEngine;
using UnityEngine.AI;
using static Constants.AnimationVarNames;

[RequireComponent(typeof(Animator), typeof(NavMeshAgent))]
public class Enemy : MonoBehaviour, IDamageable, IAttacker, IMovable
{
    [Header("Base Stats")]
    [SerializeField] private float maxHealth;
    [SerializeField] private float baseDamage = 1;
    [SerializeField] private float baseAttackSpeed = 1;
    [SerializeField] private float baseAttackRange = 2;

    private Animator _animatorController;
    private NavMeshAgent _navMeshAgent;
    private EnemySpawner _enemySpawner;
    private Player _player;

    public float Health { get; set; }
    public bool IsDead { get; set; }
    public float Damage { get; set; }
    public float AttackSpeed { get; set; }
    public float AttackRange { get; set; }
    public float LastAttackTime { get; set; }
    public float MovementSpeed { get; set; }

    private void Awake()
    {
        _animatorController = GetComponent<Animator>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
    }

    public void Init(EnemySpawner enemySpawner)
    {
        if (_enemySpawner == null)
        {
            _enemySpawner = enemySpawner;
            _player = Player.Instance;
        }
        _enemySpawner.AddEnemy(this);
        _navMeshAgent.SetDestination(_player.transform.position);
        InitStats();
        ResetForReuse();
    }

    private void InitStats()
    {
        Health = maxHealth;
        IsDead = false;
        Damage = baseDamage;
        AttackSpeed = baseAttackSpeed;
        AttackRange = baseAttackRange;
        LastAttackTime = 0;
        MovementSpeed = 0;
    }

    private void ResetForReuse()
    {
        _animatorController.Play(Universal.Idle);
        _navMeshAgent.isStopped = false;
    }

    private void Update()
    {
        if (IsDead) { return; }
        AI();
    }

    public void TakeDamage(float damage)
    {
        if (IsDead) { return; }
        Health -= damage;
        if (Health <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        _enemySpawner.RemoveEnemy(this);
        IsDead = true;
        _animatorController.SetTrigger(Universal.Die);
        _navMeshAgent.isStopped = true;
        GameEvents.OnEnemyDeath?.Invoke();
    }

    private void AI()
    {
        float distance = Vector3.Distance(transform.position, _player.transform.position);
        if (distance <= AttackRange)
        {
            AutoAttacking();
        }
        else
        {
            Move();
        }
        ApplyMovementAnimation();
    }

    private void AutoAttacking()
    {
        _navMeshAgent.isStopped = true;
        MovementSpeed = 0;
        Attack();
    }

    public void Attack()
    {
        if (Time.time - LastAttackTime > AttackSpeed)
        {
            LastAttackTime = Time.time;
            _player.TakeDamage(Damage);
            _animatorController.SetTrigger(Universal.Attack);
        }
    }

    public void Move()
    {
        _navMeshAgent.isStopped = false;
        MovementSpeed = _navMeshAgent.speed;
        _navMeshAgent.SetDestination(_player.transform.position);
    }

    private void ApplyMovementAnimation()
    {
        _animatorController.SetFloat(Universal.Speed, MovementSpeed);
    }
}
