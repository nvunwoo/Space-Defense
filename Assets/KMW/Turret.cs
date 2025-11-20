using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    [Header("공격 설정")]
    public float range = 100f;             // 사거리
    public float fireInterval = 0.5f;      // 발사 간격
    public GameObject bulletPrefab;        // 총알 프리팹

    [Header("총구 설정")]
    public Transform[] firePoints;         // 총구 두 개 이상
    int fireIndex = 0;

    [Header("회전 파츠")]
    public TurretYawController yawPart;    // 좌우 회전 파츠
    public TurretPitchController pitchPart;// 상하 회전 파츠

    float fireTimer = 0f;

    void Update()
    {
        // 1) 매 프레임 가장 가까운 적 자동 탐색
        Transform target = FindNearestEnemyInRange();

        // 2) 회전 파츠에 타겟 전달 (없으면 null 전달 → 회전 멈춤)
        if (yawPart != null) yawPart.SetTarget(target);
        if (pitchPart != null) pitchPart.SetTarget(target);

        // 3) 쿨타임 관리 + 발사
        fireTimer -= Time.deltaTime;
        if (target != null && fireTimer <= 0f)
        {
            Shoot(target);
            fireTimer = fireInterval;
        }
    }

    Transform FindNearestEnemyInRange()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        Transform nearest = null;
        float nearestSqrDist = range * range;
        Vector3 myPos = transform.position;

        foreach (GameObject enemy in enemies)
        {
            if (enemy == null) continue;

            float sqrDist = (enemy.transform.position - myPos).sqrMagnitude;

            // 사거리 안에 있으면서 지금까지 중 가장 가까우면 갱신
            if (sqrDist <= nearestSqrDist)
            {
                nearestSqrDist = sqrDist;
                nearest = enemy.transform;
            }
        }

        return nearest;
    }

    void Shoot(Transform target)
    {
        if (bulletPrefab == null || firePoints == null || firePoints.Length == 0)
        {
            Debug.LogWarning("bulletPrefab 또는 firePoints가 설정되지 않았습니다.");
            return;
        }

        // 이번에 쓸 총구 선택 (좌우 번갈아 사용)
        Transform fp = firePoints[fireIndex];
        fireIndex = (fireIndex + 1) % firePoints.Length;

        // 목표 방향 계산
        Vector3 dir = (target.position - fp.position).normalized;
        Quaternion rot = Quaternion.LookRotation(dir);

        // 총알 생성
        Instantiate(bulletPrefab, fp.position, rot);
    }
}
