using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class EnemyMove : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float gravity = 9.81f;
    public int maxHP = 100;

    private CharacterController controller;
    private Vector3 velocity;
    private Animator animator;

    private int hp;
    private bool isDead = false;
    private bool isAttacking = false;

    // ───────── 둔화 관련 추가 ─────────
    private bool isSlowed = false;
    private float slowTimer = 0f;
    private float slowMultiplier = 1f;   // 1이면 정상 속도, 0.5면 50% 속도

    public bool IsSlowed => isSlowed;    // 외부에서 읽기용
    // ────────────────────────────────


    void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
    }

    void OnEnable()
    {
        hp = Mathf.RoundToInt(maxHP * WaveManager.Instance.GetHPBoost());
        isDead = false;
        isAttacking = false;
        velocity = Vector3.zero;

        // 슬로우 상태 초기화 (이 부분이 있어야 재사용 시 깨끗해집니다)
        isSlowed = false;
        slowTimer = 0f;
        slowMultiplier = 1f;
        // ──────────────────────────────

        if (animator)
            animator.Play("walk");
    }

    void Update()
    {
        if (isDead)
            return;  // 공격 중엔 이동하지 않음

        // ───── 둔화 시간 감소 & 해제 ─────
        if (isSlowed)
        {
            slowTimer -= Time.deltaTime;
            if (slowTimer <= 0f)
            {
                isSlowed = false;
                slowMultiplier = 1f;
            }
        }

        if (isAttacking)
        {
            if (animator)
                animator.Play("attack1");
        }
        else
        {
            MoveForward();
        }

            
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


    //Bullet 충돌 처리

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            TakeDamage(0);
        }
        else if (other.CompareTag("Wall"))
        {
            StartAttack();
        }
    }

    public void TakeDamage(int dmg)
    {
        if (isDead)
            return;

        hp -= dmg;

        // Hit 애니메이션 있어도 되고 없어도 됨
        if (animator)
            animator.Play("hit", 0, 0); // 즉시 재생
            //animator.Play("walk");

        if (hp <= 0)
        {
            Die();
        }
    }

    // ───── 둔화 적용 함수 추가 ─────
    public void ApplySlow(float multiplier, float duration)
    {
        if (isDead)
            return;

        // 더 강한 슬로우만 갱신하고 싶으면 조건 걸어도 됨
        isSlowed = true;
        slowMultiplier = multiplier; // 예: 0.5f → 50% 속도
        slowTimer = duration;
    }
    // ─────────────────────────────

    void StartAttack()
    {
        if (isDead)
            return;

        isAttacking = true;
        
    }

    
    //사망 처리
    void Die()
    {
        isDead = true;
        isAttacking = false;

        if (animator)
            animator.Play("die");

        // die 애니메이션 길이만큼 기다렸다가 풀로 반환
        StartCoroutine(ReturnToPoolAfterAnim());
    }

    System.Collections.IEnumerator ReturnToPoolAfterAnim()
    {
        float dieLength = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(dieLength);

        EnemyPool.Instance.ReturnEnemy(this.gameObject);
    }
}

/*
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class EnemyMove : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float gravity = 9.81f;

    private CharacterController controller;
    private Vector3 velocity;

    private Animator animator;

    private bool isAttacking = false;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>(); // 자식에 애니메이터 있을 경우
    }

    void OnEnable()
    {
        // 오브젝트 풀에서 다시 꺼낼 때 초기화
        isAttacking = false;
        velocity = Vector3.zero;

        if (animator)
            animator.Play("Walk");
    }

    void Update()
    {
        if (isAttacking)
            return;  // 공격 중엔 이동하지 않음

        // 앞으로 움직임
        Vector3 forwardMove = transform.forward * moveSpeed;

        // 중력 적용
        if (!controller.isGrounded)
            velocity.y -= gravity * Time.deltaTime;
        else
            velocity.y = -1f;

        controller.Move((forwardMove + velocity) * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Wall"))
        {
            StartAttack();
        }
    }

    private void StartAttack()
    {
        isAttacking = true;

        // 애니메이션 전환
        if (animator)
            animator.Play("attack1");
    }
}

*/