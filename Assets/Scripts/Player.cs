using UnityEngine;
using static Constants.AnimationVarNames;

[RequireComponent(typeof(Animator))]
public class Player : MonoBehaviour, IDamageable, IAttacker
{
    [SerializeField] private float maxHealth;
    [SerializeField] private float baseDamage = 1;
    [SerializeField] private float baseAttackSpeed = 1;
    [SerializeField] private float baseAttackRange = 2;
    [SerializeField] private float movementSpeed = 5f;
    
    private Animator _animatorController;

    public float Health { get; set; }
    public bool IsDead { get; set; }
    public float Damage { get; set; }
    public float AttackSpeed { get; set; }
    public float AttackRange { get; set; }
    public float LastAttackTime { get; set; }

    private void Awake()
    {
        _animatorController = GetComponent<Animator>();
    }

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        Health = maxHealth;
        IsDead = false;
        Damage = baseDamage;
        AttackSpeed = baseAttackSpeed;
        AttackRange = baseAttackRange;
        LastAttackTime = 0;
    }

    private void Update()
    {
        if (IsDead) { return; }
        Attack();
        Move();
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
        IsDead = true;
        _animatorController.SetTrigger(Universal.Die);
        SceneManager.Instance.GameOver();
    }

    public void Attack()
    {
        LockOnClosestEnemy(FindClosestEnemy());
    }

    private Enemy FindClosestEnemy()
    {
        var enemies = SceneManager.Instance.Enemies;
        Enemy closestEnemie = null;

        for (int i = 0; i < enemies.Count; i++)
        {
            var enemie = enemies[i];
            if (enemie == null)
            {
                continue;
            }

            if (closestEnemie == null)
            {
                closestEnemie = enemie;
                continue;
            }

            var distance = Vector3.Distance(transform.position, enemie.transform.position);
            var closestDistance = Vector3.Distance(transform.position, closestEnemie.transform.position);

            if (distance < closestDistance)
            {
                closestEnemie = enemie;
            }

        }

        return closestEnemie;
    }

    private void LockOnClosestEnemy(Enemy closestEnemie)
    {
        if (closestEnemie != null)
        {
            var distance = Vector3.Distance(transform.position, closestEnemie.transform.position);
            transform.transform.rotation = Quaternion.LookRotation(closestEnemie.transform.position - transform.position);
            ProcessAttack(closestEnemie, distance);
        }
    }

    private void ProcessAttack(Enemy closestEnemie, float distance)
    {
        bool isAttackOffCooldown = Time.time - LastAttackTime > baseAttackSpeed;
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (distance <= baseAttackRange)
            {
                if (isAttackOffCooldown)
                {
                    AttackWithCooldown();
                    closestEnemie.TakeDamage(baseDamage);
                }
            }
            else if (isAttackOffCooldown)
            {
                AttackWithCooldown();
            }
        }
    }

    private void AttackWithCooldown()
    {
        LastAttackTime = Time.time;
        _animatorController.SetTrigger("Attack");
    }

    private void Move()
    {
        float xInput = Input.GetAxis("Horizontal");
        float zInput = Input.GetAxis("Vertical");

        Vector2 inputNormalized = new Vector2(xInput, zInput).normalized;

        float xOffset = inputNormalized.x * movementSpeed * Time.deltaTime;
        float zOffset = inputNormalized.y * movementSpeed * Time.deltaTime;

        float rawXPos = transform.localPosition.x + xOffset;
        float rawZPos = transform.localPosition.z + zOffset;

        transform.localPosition = new Vector3(rawXPos, transform.localPosition.y, rawZPos);

        ApplyMovementAnimation(xInput, zInput);
    }

    private void ApplyMovementAnimation(float xInput, float zInput)
    {
        float speed = xInput > 0 || zInput > 0 ? movementSpeed : 0;
        _animatorController.SetFloat(Universal.Speed, speed);
    }
}
