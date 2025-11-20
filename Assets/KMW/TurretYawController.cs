using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretYawController : MonoBehaviour
{
    public Transform target;         // Turret에서 자동으로 넣어줌
    public float rotateSpeed = 180f; // 초당 회전 속도(도)

    void Update()
    {
        if (target == null) return;

        // 타겟 방향에서 높이는 제거  평면에서만 회전
        Vector3 dir = target.position - transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.0001f) return;

        Quaternion targetRot = Quaternion.LookRotation(dir.normalized, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRot,
            rotateSpeed * Time.deltaTime
        );
    }

    public void SetTarget(Transform t)
    {
        target = t;
    }
}
