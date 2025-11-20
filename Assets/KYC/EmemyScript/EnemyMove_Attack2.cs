using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class EnemyMove_Attack2 : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float gravity = 9.81f;

    [Header("Wall 관련")]
    public string wallTag = "Wall";
    public float attackDistance = 6f;

    private CharacterController controller;
    private Animator animator;
    private Vector3 velocity;

    private Transform targetWall;   // Wall 대상
    private bool isAttacking = false;
    private bool isDead = false;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
    }

    void OnEnable()
    {
        isAttacking = false;
        isDead = false;
        velocity = Vector3.zero;

        if (animator)
            animator.Play("walk");

        // 스폰될 때 가장 가까운 Wall 찾기
        FindNearestWall();
    }

    void Update()
    {
        if (isDead) return;

        // Wall을 못 찾으면 이동만 함
        if (targetWall == null)
        {
            MoveForward();
            return;
        }

        float distance = Vector3.Distance(transform.position, targetWall.position);

        // 공격 거리 안에 들어오면 Attack1
        if (!isAttacking && distance <= attackDistance)
        {
            StartAttack2();
            return;
        }

        if (!isAttacking)
            MoveForward();
    }


    //이동
    void MoveForward()
    {
        Vector3 moveDir = transform.forward * moveSpeed;

        if (!controller.isGrounded)
            velocity.y -= gravity * Time.deltaTime;
        else
            velocity.y = -1f;

        controller.Move((moveDir + velocity) * Time.deltaTime);
    }

    //  공격 2 (투사체 던지기)
    void StartAttack2()
    {
        isAttacking = true;

        if (animator)
            animator.Play("attack1", 0, 0);

        // Attack2 애니메이션 끝나면 다시 Walk로 복귀
        StartCoroutine(Attack2Cooldown());
    }

    System.Collections.IEnumerator Attack2Cooldown()
    {
        float len = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(len);

        isAttacking = false;

        if (animator)
            animator.Play("walk");
    }

    //  가장 가까운 Wall 자동 탐색
    void FindNearestWall()
    {
        GameObject[] walls = GameObject.FindGameObjectsWithTag(wallTag);
        if (walls.Length == 0) return;

        float minDist = float.MaxValue;
        Transform nearest = null;

        foreach (GameObject w in walls)
        {
            float dist = Vector3.Distance(transform.position, w.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = w.transform;
            }
        }

        targetWall = nearest;
    }
}
