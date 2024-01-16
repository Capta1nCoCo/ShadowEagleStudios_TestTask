using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;
using static Constants.AnimationVarNames;

[RequireComponent(typeof(Animator))]
public class Player : MonoBehaviour, IDamageable, ISuperAttacker, IMovable
{
    public static Player Instance { get; private set; }

    [SerializeField] private float baseMovementSpeed = 5f;

    [Header("Health Stats")]
    [SerializeField] private float maxHealth;
    [SerializeField] private float healthOnKill = 1;

    [Header("Basic Attack")]
    [SerializeField] private float baseDamage = 1;
    [SerializeField] private float baseAttackSpeed = 1;
    [SerializeField] private float baseAttackRange = 2;

    [Header("Super Attack")]
    [SerializeField] private float superDamage = 2;
    [SerializeField] private float superAttackSpeed = 2;

    private Enemy closestEnemie;
    private float distance;

    private Animator _animatorController;
    private EnemySpawner _enemySpawner;

    public float Health { get; set; }
    public bool IsDead { get; set; }
    public float Damage { get; set; }
    public float AttackSpeed { get; set; }
    public float AttackRange { get; set; }
    public float LastAttackTime { get; set; }
    public float MovementSpeed { get; set; }
    public float SuperAttackSpeed { get; set; }
    public float SuperAttackDamage { get; set; }
    public float LastSuperTime { get; set; }

    [Inject]
    public void InjectDependencies(EnemySpawner enemySpawner)
    {
        _enemySpawner = enemySpawner;
    }

    private void Awake()
    {
        _animatorController = GetComponent<Animator>();
        ApplySingleton();
        GameEvents.OnEnemyDeath += RestoreHealth;
    }

    private void OnDestroy()
    {
        GameEvents.OnEnemyDeath -= RestoreHealth;
    }

    private void Start()
    {
        Init();
    }

    private void Update()
    {
        if (IsDead) { return; }
        GameEvents.OnCooldown?.Invoke();
        FindClosestEnemy();
        Move();
    }

    private void ApplySingleton()
    {
        if (FindObjectsOfType(GetType()).Length > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Init()
    {
        AssignStats();
        InvokeInitEvents();
    }

    private void AssignStats()
    {
        Health = maxHealth;
        IsDead = false;
        Damage = baseDamage;
        AttackSpeed = baseAttackSpeed;
        AttackRange = baseAttackRange;
        LastAttackTime = 0;
        MovementSpeed = baseMovementSpeed;
        SuperAttackSpeed = superAttackSpeed;
        SuperAttackDamage = superDamage;
        LastSuperTime = 0;
    }

    private void InvokeInitEvents()
    {
        GameEvents.OnHealthChanged?.Invoke(Health);
        GameEvents.OnPlayerInit?.Invoke();
    }

    public void TakeDamage(float damage)
    {
        if (IsDead) { return; }
        Health -= damage;
        GameEvents.OnHealthChanged?.Invoke(Health);
        if (Health <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        IsDead = true;
        _animatorController.SetTrigger(Universal.Die);
        GameEvents.OnDefeat?.Invoke();
    }

    private void RestoreHealth()
    {
        Health += healthOnKill;
        if (Health > maxHealth)
        {
            Health = maxHealth;
        }
        GameEvents.OnHealthChanged?.Invoke(Health);
    }

    private void FindClosestEnemy()
    {
        List<Enemy> enemies = _enemySpawner.getEnemies;
        closestEnemie = null;

        for (int i = 0; i < enemies.Count; i++)
        {
            var enemy = enemies[i];
            if (enemy == null)
            {
                continue;
            }

            if (closestEnemie == null)
            {
                closestEnemie = enemy;
                continue;
            }

            AssignClosestEnemy(enemy);
        }

        LockOnClosestEnemy();
    }

    private void AssignClosestEnemy(Enemy enemie)
    {
        float distance = Vector3.Distance(transform.position, enemie.transform.position);
        float closestDistance = Vector3.Distance(transform.position, closestEnemie.transform.position);
        if (distance < closestDistance)
        {
            closestEnemie = enemie;
        }
    }

    private void LockOnClosestEnemy()
    {
        if (closestEnemie != null)
        {
            distance = Vector3.Distance(transform.position, closestEnemie.transform.position);
            transform.transform.rotation = Quaternion.LookRotation(closestEnemie.transform.position - transform.position);
            ProcessAttack();
        }
    }

    private void ProcessAttack()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Attack();
        }
        else if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            SuperAttack();
        }
    }

    public void Attack()
    {
        if (IsDead) { return; }
        if (IsInAttackRange())
        {
            if (IsOffCooldown(AttackSpeed))
            {
                AttackWithCooldown(Universal.Attack);
                closestEnemie.TakeDamage(baseDamage);
            }
        }
        else if (IsOffCooldown(AttackSpeed))
        {
            AttackWithCooldown(Universal.Attack);
        }
    }

    public void SuperAttack()
    {
        if (IsDead) { return; }
        if (IsInAttackRange())
        {
            if (Time.time - LastSuperTime > SuperAttackSpeed)
            {
                AttackWithCooldown(PlayerCharacter.SuperAttack);
                closestEnemie.TakeDamage(SuperAttackDamage);
            }
        }
    }

    private bool IsInAttackRange()
    {
        return distance <= baseAttackRange;
    }

    private bool IsOffCooldown(float cooldown)
    {
        return Time.time - LastAttackTime > cooldown;
    }

    private void AttackWithCooldown(string attackName)
    {
        if (attackName == PlayerCharacter.SuperAttack)
        {
            LastSuperTime = Time.time;
        }
        else
        {
            LastAttackTime = Time.time;
        }
        _animatorController.SetTrigger(attackName);
    }

    public void Move()
    {
        float xInput = Input.GetAxis("Horizontal");
        float zInput = Input.GetAxis("Vertical");

        Vector2 inputNormalized = new Vector2(xInput, zInput).normalized;

        float xOffset = inputNormalized.x * MovementSpeed * Time.deltaTime;
        float zOffset = inputNormalized.y * MovementSpeed * Time.deltaTime;

        float rawXPos = transform.localPosition.x + xOffset;
        float rawZPos = transform.localPosition.z + zOffset;

        transform.localPosition = new Vector3(rawXPos, transform.localPosition.y, rawZPos);

        ApplyMovementAnimation(xInput, zInput);
    }

    private void ApplyMovementAnimation(float xInput, float zInput)
    {
        float currentSpeed = Mathf.Abs(xInput) > 0 || Mathf.Abs(zInput) > 0 ? MovementSpeed : 0;
        _animatorController.SetFloat(Universal.Speed, currentSpeed);
    }
}
