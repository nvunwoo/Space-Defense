using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class EnemyMove2 : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 2f;
    public float gravity = 9.81f;

    [Header("Combat")]
    public float attackDistance = 6f;
    public GameObject projectilePrefab;
    public Transform projectileSpawnPoint;

    [Header("HP")]
    public int maxHP = 10;

    private CharacterController controller;
    private Animator animator;
    private Transform wall;
    private Vector3 velocity;

    private int hp;
    private bool isDead = false;
    private bool isAttacking = false;

    // ───── 슬로우 관련 ─────
    private bool isSlowed = false;
    private float slowTimer = 0f;
    private float slowMultiplier = 1f;
    // ───────────────────────


    void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
    }

    void OnEnable()
    {
        hp = maxHP;
        isDead = false;
        isAttacking = false;
        isSlowed = false;
        slowTimer = 0f;
        slowMultiplier = 1f;
        velocity = Vector3.zero;

        if (animator) animator.Play("walk");

        FindNearestWall();
    }


    void Update()
    {
        if (isDead || isAttacking) return;

        if (!wall)
        {
            MoveForward();
            return;
        }

        float dist = Vector3.Distance(transform.position, wall.position);

        if (dist <= attackDistance)
        {
            StartAttack2();
            return;
        }

        MoveForward();
    }


    void MoveForward()
    {
        // 슬로우 유지하면서 walk 계속
        float currentSpeed = moveSpeed * slowMultiplier;

        Vector3 moveDir = transform.forward * currentSpeed;

        if (!controller.isGrounded)
            velocity.y -= gravity * Time.deltaTime;
        else
            velocity.y = -1f;

        controller.Move((moveDir + velocity) * Time.deltaTime);

        if (animator && !animator.GetCurrentAnimatorStateInfo(0).IsName("walk"))
            animator.Play("walk");
    }


    void StartAttack2()
    {
        isAttacking = true;
        if (animator) animator.Play("attack2", 0, 0);
    }

    
    // Attack2 애니메이션 이벤트에서 호출
    public void FireProjectile()
    {
        if (!projectilePrefab || !projectileSpawnPoint) return;

        GameObject p = Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);
        // 발사체는 스폰 방향 그대로 날아감
    }
    

    void FindNearestWall()
    {
        GameObject[] walls = GameObject.FindGameObjectsWithTag("Wall");
        if (walls.Length == 0) return;

        float min = Mathf.Infinity;
        foreach (var w in walls)
        {
            float d = Vector3.Distance(transform.position, w.transform.position);
            if (d < min)
            {
                min = d;
                wall = w.transform;
            }
        }
    }

    //Bullet 충돌 처리
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            TakeDamage(2);
        }
        
    }


    // ---------------- HP ----------------
    public void TakeDamage(int dmg)
    {
        if (isDead) return;

        hp -= dmg;
        if (animator)
            animator.Play("hit", 0, 0);
            animator.Play("walk");

        if (hp <= 0)
        {
            Die();
        }
    }

    public void ApplySlow(float multiplier, float duration)
    {
        isSlowed = true;
        slowMultiplier = multiplier;
        slowTimer = duration;
    }
    // -------------------------------------


    void Die()
    {
        isDead = true;
        isAttacking = false;

        if (animator) animator.Play("die");

        StartCoroutine(ReturnToPool());
    }

    System.Collections.IEnumerator ReturnToPool()
    {
        float len = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(len);
        EnemyPool.Instance.ReturnEnemy(gameObject);
    }
}
