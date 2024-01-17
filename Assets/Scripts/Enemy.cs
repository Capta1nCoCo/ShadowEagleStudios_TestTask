using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using static Constants.AnimationVarNames;
using static UnityEditor.PlayerSettings;

[RequireComponent(typeof(Animator), typeof(NavMeshAgent))]
public class Enemy : MonoBehaviour, IDamageable, IAttacker, IMovable, ISpawn
{
    [Header("Base Stats")]
    [SerializeField] private float maxHealth;
    [SerializeField] private float baseDamage = 1;
    [SerializeField] private float baseAttackSpeed = 1;
    [SerializeField] private float baseAttackRange = 2;

    [Header("Heal On Death")]
    [Tooltip("0 = no heal")]
    [SerializeField] private float restorePlayerHealth = 1;

    private Animator _animatorController;
    private NavMeshAgent _navMeshAgent;
    private EnemySpawner _enemySpawner;
    private Player _player;
    private DeathSpawner _deathSpawner;

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
        _deathSpawner = GetComponent<DeathSpawner>();
    }

    private void OnDisable()
    {
        _navMeshAgent.ResetPath();
    }

    public void InitSpawn(EnemySpawner spawner, Vector3 pos)
    {
        if (_enemySpawner == null)
        {
            _enemySpawner = spawner;
            _player = Player.Instance;
        }
        _enemySpawner.AddEnemy(this);
        InitStats();
        StartCoroutine(WarpWithDelay(pos));
        StartCoroutine(SetDestinationWithDelay());
    }

    private void InitStats()
    {
        Health = maxHealth;
        Damage = baseDamage;
        AttackSpeed = baseAttackSpeed;
        AttackRange = baseAttackRange;
        LastAttackTime = 0;
        MovementSpeed = 0;
    }

    private IEnumerator WarpWithDelay(Vector3 pos)
    {
        yield return new WaitForEndOfFrame();
        _navMeshAgent.Warp(pos);
    }

    private IEnumerator SetDestinationWithDelay()
    {
        yield return new WaitForEndOfFrame();
        IsDead = false;
        _animatorController.Play(Universal.Idle);
        _navMeshAgent.isStopped = false;
        _navMeshAgent.SetDestination(_player.transform.position);
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
        else
        {
            _animatorController.SetTrigger(EnemyCharacter.GetHit);
        }
    }

    public void Die()
    {
        SpawnOnDeath();
        _enemySpawner.RemoveEnemy(this);
        IsDead = true;
        _animatorController.SetTrigger(Universal.Die);
        _navMeshAgent.isStopped = true;
        _player.RestoreHealth(restorePlayerHealth);
    }

    private void SpawnOnDeath()
    {
        if (_deathSpawner != null)
        {
            _deathSpawner.SpawnEnemiesInDeathArea();
        }
    }

    private void AI()
    {
        if (IsInAttackRange())
        {
            AutoAttacking();
        }
        else
        {
            Move();
        }
        ApplyMovementAnimation();
    }

    private bool IsInAttackRange()
    {
        float distance = Vector3.Distance(transform.position, _player.transform.position);
        return distance <= AttackRange;
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
            _animatorController.SetTrigger(Universal.Attack);
        }
    }

    // Used by Animation Events
    public void DealDamage()
    {
        if (IsInAttackRange())
        {
            _player.TakeDamage(Damage);
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
