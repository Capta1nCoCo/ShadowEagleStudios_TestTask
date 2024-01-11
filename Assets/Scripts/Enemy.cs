using UnityEngine;
using UnityEngine.AI;
using static Constants.AnimationVarNames;

[RequireComponent(typeof(Animator), typeof(NavMeshAgent))]
public class Enemy : MonoBehaviour, IDamageable, IAttacker
{
    [SerializeField] private float maxHealth;
    [SerializeField] private float baseDamage = 1;
    [SerializeField] private float baseAttackSpeed = 1;
    [SerializeField] private float baseAttackRange = 2;

    private Animator _animatorController;
    private NavMeshAgent _navMeshAgent;

    public float Health { get; set; }
    public bool IsDead { get; set; }
    public float Damage { get; set; }
    public float AttackSpeed { get; set; }
    public float AttackRange { get; set; }
    public float LastAttackTime { get; set; }

    private void Awake()
    {
        _animatorController = GetComponent<Animator>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        SceneManager.Instance.AddEnemie(this);
        _navMeshAgent.SetDestination(SceneManager.Instance.Player.transform.position);
        Init();
    }

    private void Init()
    {
        Health = maxHealth;
        IsDead = false;
        Damage = baseDamage;
        AttackSpeed = baseAttackSpeed;
        AttackRange = baseAttackRange;
    }

    private void Update()
    {
        if (IsDead) { return; }

        var distance = Vector3.Distance(transform.position, SceneManager.Instance.Player.transform.position);
     
        if (distance <= AttackRange)
        {
            _navMeshAgent.isStopped = true;
            Attack();
        }
        else
        {
            _navMeshAgent.isStopped = false;
            _navMeshAgent.SetDestination(SceneManager.Instance.Player.transform.position);
        }
        _animatorController.SetFloat(Universal.Speed, _navMeshAgent.speed);
        //Debug.Log(Agent.speed);
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
        SceneManager.Instance.RemoveEnemie(this);
        IsDead = true;
        _animatorController.SetTrigger(Universal.Die);
        _navMeshAgent.isStopped = true;
    }

    public void Attack()
    {
        if (Time.time - LastAttackTime > AttackSpeed)
        {
            LastAttackTime = Time.time;
            SceneManager.Instance.Player.TakeDamage(Damage);
            _animatorController.SetTrigger(Universal.Attack);
        }
    }
}
