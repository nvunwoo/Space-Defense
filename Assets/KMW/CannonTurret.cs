using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonTurret : MonoBehaviour
{
    [Header("기본 수치")]
    public float baseDamage = 10f;
    public float damagePerLevel = 5f;
    public int damageLevel = 1;
    public int maxDamageLevel = 10;

    public float baseExplosionRadius = 30f;
    public float radiusPerLevel = 5.0f;
    public int radiusLevel = 1;
    public int maxRadiusLevel = 10;

    public float baseFireInterval = 2f;
    public float fireIntervalMultPerLevel = 0.9f;
    public int fireRateLevel = 1;
    public int maxFireRateLevel = 10;

    [Header("공격 설정")]
    public float range = 100f;              // 포탑 사거리
    public GameObject shellPrefab;         // CannonShell 프리팹
    public Transform firePoint;            // 포탄 발사 위치

    [Header("회전 파츠")]
    public TurretYawController yawPart;    // 좌우 회전
    public TurretPitchController pitchPart;// 상하 회전

    float fireTimer = 0f;

    // 현재 실사용 수치
    public float CurrentDamage
        => baseDamage + (damageLevel - 1) * damagePerLevel;

    public float CurrentExplosionRadius
        => baseExplosionRadius + (radiusLevel - 1) * radiusPerLevel;

    public float CurrentFireInterval
        => baseFireInterval * Mathf.Pow(fireIntervalMultPerLevel, fireRateLevel - 1);

    void Update()
    {
        fireTimer -= Time.deltaTime;

        // 1) 사거리 내에서 "가장 큰 무리"의 중심이 되는 적 찾기
        Transform target = FindBestClusterTarget();

        // 2) 회전 파츠에 타겟 전달
        if (yawPart != null) yawPart.SetTarget(target);
        if (pitchPart != null) pitchPart.SetTarget(target);

        // 3) 쿨타임 끝났으면 발사
        if (target != null && fireTimer <= 0f)
        {
            Shoot(target.position);
            fireTimer = CurrentFireInterval;
        }
    }

    Transform FindBestClusterTarget()
    {
        GameObject[] allEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (allEnemies.Length == 0) return null;

        float rangeSqr = range * range;
        float aoeSqr = CurrentExplosionRadius * CurrentExplosionRadius;
        Vector3 myPos = transform.position;

        // 사거리 안 적만 모으기
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
        if (shellPrefab == null || firePoint == null)
        {
            Debug.LogWarning("shellPrefab 또는 firePoint가 설정되지 않았습니다.");
            return;
        }

        Vector3 dir = (targetPos - firePoint.position).normalized;
        Quaternion rot = Quaternion.LookRotation(dir, Vector3.up);

        GameObject shellObj = Instantiate(shellPrefab, firePoint.position, rot);
        CannonShell shell = shellObj.GetComponent<CannonShell>();
        if (shell != null)
        {
            shell.damage = CurrentDamage;
            shell.explosionRadius = CurrentExplosionRadius;
            shell.Launch(firePoint.position, targetPos);
        }
    }

    // ===== 업그레이드 함수들 (이 포탑 인스턴스만 강화) =====

    public void UpgradeDamage()
    {
        if (damageLevel >= maxDamageLevel) return;
        damageLevel++;
    }

    public void UpgradeRadius()
    {
        if (radiusLevel >= maxRadiusLevel) return;
        radiusLevel++;
    }

    public void UpgradeFireRate()
    {
        if (fireRateLevel >= maxFireRateLevel) return;
        fireRateLevel++;
    }
}
