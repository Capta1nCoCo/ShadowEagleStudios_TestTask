using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 5f;

    public float Hp;
    public float Damage;
    public float AtackSpeed;
    public float AttackRange = 2;

    private float lastAttackTime = 0;
    private bool isDead = false;
    public Animator AnimatorController;

    private void Update()
    {
        if (isDead)
        {
            return;
        }

        if (Hp <= 0)
        {
            Die();
            return;
        }

        //Enemie closestEnemie = FindClosestEnemy();

        AttackClosestEnemy(FindClosestEnemy());

        Move();
    }

    private Enemie FindClosestEnemy()
    {
        var enemies = SceneManager.Instance.Enemies;
        Enemie closestEnemie = null;

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

    private void AttackClosestEnemy(Enemie closestEnemie)
    {
        if (closestEnemie != null)
        {
            var distance = Vector3.Distance(transform.position, closestEnemie.transform.position);
            bool isAttackOffCooldown = Time.time - lastAttackTime > AtackSpeed;

            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                if (distance <= AttackRange)
                {
                    if (isAttackOffCooldown)
                    {
                        transform.transform.rotation = Quaternion.LookRotation(closestEnemie.transform.position - transform.position);
                        ProcessAttackWithCooldown();
                        closestEnemie.Hp -= Damage;
                    }
                }
                else if (isAttackOffCooldown)
                {
                    ProcessAttackWithCooldown();
                }
            }
            
        }

        void ProcessAttackWithCooldown()
        {
            lastAttackTime = Time.time;
            AnimatorController.SetTrigger("Attack");
        }
    }

    private void Die()
    {
        isDead = true;
        AnimatorController.SetTrigger("Die");

        SceneManager.Instance.GameOver();
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
    }
}
