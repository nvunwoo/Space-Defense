using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretPitchController : MonoBehaviour
{
    public Transform target;          // Turret에서 자동으로 넣어줄 타겟
    public float rotateSpeed = 180f;  // 초당 회전 속도 (도)
    public float minPitch = -10f;     // 아래로 최대 각도 (도)
    public float maxPitch = 60f;      // 위로 최대 각도 (도)

    void Update()
    {
        if (target == null) return;

        // 타겟까지의 방향 (월드 기준)
        Vector3 dir = target.position - transform.position;
        if (dir.sqrMagnitude < 0.0001f) return;

        // 이 방향을 바라봤을 때의 회전값
        Quaternion lookRot = Quaternion.LookRotation(dir, Vector3.up);
        float desiredPitch = lookRot.eulerAngles.x;

        // 0~360 → -180~180으로 변환
        if (desiredPitch > 180f) desiredPitch -= 360f;

        // 각도 제한
        desiredPitch = Mathf.Clamp(desiredPitch, minPitch, maxPitch);

        // 현재 피치 구하기 (역시 -180~180으로)
        Vector3 localEuler = transform.localEulerAngles;
        float currentPitch = localEuler.x;
        if (currentPitch > 180f) currentPitch -= 360f;

        // 부드럽게 회전
        float newPitch = Mathf.MoveTowardsAngle(currentPitch, desiredPitch, rotateSpeed * Time.deltaTime);

        // X만 갱신, Y/Z는 0으로 고정 (Yaw는 부모가 담당한다고 가정)
        localEuler.x = newPitch;
        localEuler.y = 0f;
        localEuler.z = 0f;
        transform.localEulerAngles = localEuler;
    }

    public void SetTarget(Transform t)
    {
        target = t;
    }
}

