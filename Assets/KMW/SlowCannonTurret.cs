using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowCannonTurret : MonoBehaviour
{
    [Header("공격 설정")]
    public float range = 100f;            // 포탑 사거리
    public float fireInterval = 3f;      // 발사 간격
    public float aoeRadius = 10f;         // 폭발 범위 반경
    public GameObject shellPrefab;       // SlowCannonShell 프리팹

    [Header("발사 위치 (여러 개 가능)")]
    public Transform[] firePoints;       // 포탄 발사 위치들

    [Header("회전 파츠")]
    public TurretYawController yawPart;      // 좌우 회전 파츠
    public TurretPitchController pitchPart;  // 상하 회전 파츠

    float fireTimer = 0f;
    int currentFirePointIndex = 0;

    void Update()
    {
        // 사거리 내에서 "아직 둔화 안 걸린" 적들 중 가장 큰 무리의 중심 찾기
        Transform target = FindBestClusterTarget();

        // 회전 파츠
        if (yawPart != null) yawPart.SetTarget(target);
        if (pitchPart != null) pitchPart.SetTarget(target);

        fireTimer -= Time.deltaTime;
        if (target != null && fireTimer <= 0f)
        {
            Shoot(target.position);
            fireTimer = fireInterval;
        }
    }

    Transform FindBestClusterTarget()
    {
        GameObject[] allEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (allEnemies.Length == 0) return null;

        float rangeSqr = range * range;
        float aoeSqr = aoeRadius * aoeRadius;
        Vector3 myPos = transform.position;

        // 사거리 안의 모든 적
        List<Transform> inRangeAll = new List<Transform>();
        // 사거리 안 + 아직 슬로우 안 걸린 적
        List<Transform> inRangeNotSlowed = new List<Transform>();

        foreach (var e in allEnemies)
        {
            if (e == null) continue;

            EnemyMove em = e.GetComponent<EnemyMove>();
            if (em == null) continue;

            float sqr = (e.transform.position - myPos).sqrMagnitude;
            if (sqr > rangeSqr) continue;

            inRangeAll.Add(e.transform);

            if (!em.IsSlowed)
            {
                inRangeNotSlowed.Add(e.transform);
            }
        }

        // 1순위: 아직 슬로우 안 걸린 적들
        List<Transform> targetList = null;

        if (inRangeNotSlowed.Count > 0)
        {
            targetList = inRangeNotSlowed;
        }
        else if (inRangeAll.Count > 0)
        {
            // 전부 슬로우라면 → 슬로우 포함 전체를 대상으로 클러스터 계산
            targetList = inRangeAll;
        }
        else
        {
            return null;
        }

        // 선택된 리스트에서 가장 큰 무리의 중심이 될 적 찾기
        Transform bestTarget = null;
        int bestCount = 0;

        for (int i = 0; i < targetList.Count; i++)
        {
            Vector3 center = targetList[i].position;
            int count = 0;

            for (int j = 0; j < targetList.Count; j++)
            {
                float d2 = (targetList[j].position - center).sqrMagnitude;
                if (d2 <= aoeSqr)
                    count++;
            }

            if (count > bestCount)
            {
                bestCount = count;
                bestTarget = targetList[i];
            }
        }

        return bestTarget;
    }


    void Shoot(Vector3 targetPos)
    {
        if (shellPrefab == null)
        {
            Debug.LogWarning("슬로우 포탄 shellPrefab이 설정되지 않았습니다.");
            return;
        }

        if (firePoints == null || firePoints.Length == 0)
        {
            Debug.LogWarning("firePoints가 비어 있습니다. 슬로우 포탑에 발사 위치를 하나 이상 지정하세요.");
            return;
        }

        Transform firePoint = firePoints[currentFirePointIndex];
        if (firePoint == null)
        {
            Debug.LogWarning("firePoints 배열 안에 null이 있습니다. Inspector에서 확인하세요.");
            return;
        }

        currentFirePointIndex++;
        if (currentFirePointIndex >= firePoints.Length)
            currentFirePointIndex = 0;

        Vector3 dir = (targetPos - firePoint.position).normalized;
        Quaternion rot = Quaternion.LookRotation(dir, Vector3.up);

        GameObject shellObj = Instantiate(shellPrefab, firePoint.position, rot);
        SlowCannonShell shell = shellObj.GetComponent<SlowCannonShell>();
        if (shell != null)
        {
            shell.Launch(firePoint.position, targetPos, aoeRadius);
        }
    }
}
