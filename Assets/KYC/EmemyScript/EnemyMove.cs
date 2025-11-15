using System.Collections;
using System.Collections.Generic;
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

/*
 using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class EnemyMove : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float gravity = 9.81f;

    private CharacterController controller;
    private Vector3 velocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // 앞으로 움직임 (로컬 Z축)
        Vector3 forwardMove = transform.forward * moveSpeed;

        // 중력 적용
        if (!controller.isGrounded)
            velocity.y -= gravity * Time.deltaTime;
        else
            velocity.y = -1f; // 바닥에 딱 붙게

        // 이동 적용
        controller.Move((forwardMove + velocity) * Time.deltaTime);
    }
}*/