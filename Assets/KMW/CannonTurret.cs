using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonTurret : MonoBehaviour
{
    [Header("공격 설정")]
    public float range = 100f;            // 포탑 사거리
    public float fireInterval = 2f;      // 발사 간격
    public float aoeRadius = 20f;         // 폭발 범위 반경
    public GameObject shellPrefab;       // CannonShell 프리팹

    [Header("발사 위치")]
    public Transform[] firePoints;

    [Header("회전 파츠")]
    public TurretYawController yawPart;      // 좌우 회전 파츠
    public TurretPitchController pitchPart;  // 상하 회전 파츠


    float fireTimer = 0f;

    // 여러 총구를 순차적으로 사용하기 위한 인덱스
    int currentFirePointIndex = 0;

    void Update()
    {
        // 사거리 내에서 가장 큰 무리의 중심이 되는 적 찾기
        Transform target = FindBestClusterTarget();

        // 회전 파츠에 타겟 전달 없으면 null로 전달 회전 멈춤
        if (yawPart != null) yawPart.SetTarget(target);
        if (pitchPart != null) pitchPart.SetTarget(target);

        // 쿨타임 관리 & 발사
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

        // 사거리 안 적들만 모으기
        List<Transform> inRange = new List<Transform>();
        foreach (var e in allEnemies)
        {
            if (e == null) continue;
            float sqr = (e.transform.position - myPos).sqrMagnitude;
            if (sqr <= rangeSqr)
                inRange.Add(e.transform);
        }
        if (inRange.Count == 0) return null;

        // 각 적을 중심으로 aoeRadius 안에 몇 마리 있는지 세기
        Transform bestTarget = null;
        int bestCount = 0;

        for (int i = 0; i < inRange.Count; i++)
        {
            Vector3 center = inRange[i].position;
            int count = 0;

            for (int j = 0; j < inRange.Count; j++)
            {
                float d2 = (inRange[j].position - center).sqrMagnitude;
                if (d2 <= aoeSqr)
                    count++;
            }

            if (count > bestCount)
            {
                bestCount = count;
                bestTarget = inRange[i];
            }
        }

        return bestTarget;
    }

    void Shoot(Vector3 targetPos)
    {
        if (shellPrefab == null)
        {
            Debug.LogWarning("shellPrefab이 설정되지 않았습니다.");
            return;
        }

        if (firePoints == null || firePoints.Length == 0)
        {
            Debug.LogWarning("firePoints가 비어 있습니다. 총구(발사 위치)를 하나 이상 지정하세요.");
            return;
        }

        // 이번에 사용할 발사 위치 선택 (순차적으로 사용)
        Transform firePoint = firePoints[currentFirePointIndex];
        if (firePoint == null)
        {
            Debug.LogWarning("firePoints에 null이 있습니다. Inspector에서 확인해주세요.");
            return;
        }

        // 다음 발사 때는 다음 총구 사용
        currentFirePointIndex++;
        if (currentFirePointIndex >= firePoints.Length)
            currentFirePointIndex = 0;

        // 발사 방향(회전용)
        Vector3 dir = (targetPos - firePoint.position).normalized;
        Quaternion rot = Quaternion.LookRotation(dir, Vector3.up);

        GameObject shellObj = Instantiate(shellPrefab, firePoint.position, rot);
        CannonShell shell = shellObj.GetComponent<CannonShell>();
        if (shell != null)
        {
            shell.Launch(firePoint.position, targetPos, aoeRadius);
        }
    }
}