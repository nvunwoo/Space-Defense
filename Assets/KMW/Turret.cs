using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    [Header("공격 설정")]
    public float range = 100f;          // 사거리
    public float fireInterval = 0.5f;  // 발사 간격 (초당 2발이면 0.5)
    public GameObject bulletPrefab;    // 총알 프리팹
    public Transform firePoint;        // 총알이 나가는 위치

    void Start()
    {
        // 생성 후 바로 쏘지 않고 1초 쉬었다가 루틴 시작
        StartCoroutine(FireRoutine());
    }

    IEnumerator FireRoutine()
    {

        // 계속 반복
        while (true)
        {
            Transform target = FindNearestEnemyInRange();

            if (target != null)
            {
                Shoot(target);
            }

            // 발사 간격만큼 대기
            yield return new WaitForSeconds(fireInterval);
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

            // 사거리 안에 있으면서, 지금까지 찾은 것보다 더 가까우면 갱신
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
        if (bulletPrefab == null || firePoint == null)
        {
            Debug.LogWarning("bulletPrefab 또는 firePoint가 설정되지 않았습니다.");
            return;
        }

        // 목표 방향 계산
        Vector3 dir = (target.position - firePoint.position).normalized;

        // 총알 방향을 목표를 바라보게 회전값 생성
        Quaternion rot = Quaternion.LookRotation(dir);

        // 총알 생성
        Instantiate(bulletPrefab, firePoint.position, rot);
    }
}