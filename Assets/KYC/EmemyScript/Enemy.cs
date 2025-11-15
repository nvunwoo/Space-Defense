using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Enemy : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float gravity = 9.81f;

    private CharacterController controller;
    private Animator animator;
    private Vector3 velocity;
    private bool isAttacking = false;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    void OnEnable()
    {
        // 적이 풀에서 다시 나올 때 초기화
        isAttacking = false;
        animator.Play("Walk");
    }

    void Update()
    {
        if (isAttacking) return;   // 공격 중에는 이동 중지

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

        // Walk 정지
        animator.Play("Attack1");
    }
}
