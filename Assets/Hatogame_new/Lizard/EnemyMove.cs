using System.Collections;
using System.Collections.Generic;
/*
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
*/
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
}