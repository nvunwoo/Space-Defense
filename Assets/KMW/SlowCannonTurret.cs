using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowCannonTurret : MonoBehaviour
{
    [Header("기본 수치")]
    public float baseDamage = 5f;
    public float damagePerLevel = 2f;
    public int damageLevel = 1;
    public int maxDamageLevel = 10;

    // 범위(폭발 반경) 업그레이드
    public float baseEffectRadius = 30f;
    public float effectRadiusPerLevel = 5.0f;
    public int effectRadiusLevel = 1;
    public int maxEffectRadiusLevel = 10;

    // 둔화율 / 지속시간 업그레이드
    public float baseSlowPercent = 0.3f;     // 30% 감속 시작
    public float slowPercentPerLevel = 0.05f;// 레벨당 +5% → 30% → 70%
    public float maxSlowPercent = 0.7f;      // 70% 이상은 안 올라감
    public int slowLevel = 0;                // 둔화율 레벨

    public float baseSlowDuration = 2.0f;    // 둔화 지속시간 기본값
    public float slowDurationPerLevel = 0.3f;// 둔화 지속시간 레벨당 증가
    public int slowDurationLevel = 0;        // 둔화 지속시간 레벨

    [Header("공격 설정")]
    public float range = 100f;                // 타겟팅 사거리
    public float fireInterval = 1.5f;        // 고정 쿨타임(이 포탑은 공속 업그레이드 없음)
    public GameObject shellPrefab;           // SlowCannonShell 프리팹
    public Transform[] firePoints;
    private int fireIndex = 0;
    // 발사 위치

    [Header("회전 파츠")]
    public TurretYawController yawPart;
    public TurretPitchController pitchPart;

    float fireTimer = 0f;

    // ===== 현재 실사용 수치 =====
    public float CurrentDamage
        => baseDamage + (damageLevel - 1) * damagePerLevel;

    public float CurrentEffectRadius
        => baseEffectRadius + (effectRadiusLevel - 1) * effectRadiusPerLevel;

    // 0~1 범위 슬로우 비율 (1 - 이 값이 속도배수)
    public float CurrentSlowPercent
        => Mathf.Min(maxSlowPercent, baseSlowPercent + slowLevel * slowPercentPerLevel);

    // EnemyMove 쪽에 넘길 multiplier (예: 0.7이면 30% 감소)
    public float CurrentSlowMultiplier
        => 1f - CurrentSlowPercent;

    public float CurrentSlowDuration
        => baseSlowDuration + slowDurationLevel * slowDurationPerLevel;
    // ==============================

    void Update()
    {
        fireTimer -= Time.deltaTime;

        Transform target = FindBestClusterTarget();

        // 회전 파츠에 타겟 전달
        if (yawPart != null) yawPart.SetTarget(target);
        if (pitchPart != null) pitchPart.SetTarget(target);

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
        float aoeSqr = CurrentEffectRadius * CurrentEffectRadius;
        Vector3 myPos = transform.position;

        List<Transform> inRange = new List<Transform>();
        foreach (var e in allEnemies)
        {
            if (e == null) continue;
            float sqr = (e.transform.position - myPos).sqrMagnitude;
            if (sqr <= rangeSqr)
                inRange.Add(e.transform);
        }

        if (inRange.Count == 0) return null;

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
        if (firePoints.Length == 0 || shellPrefab == null)
            return;

        // 사용할 총구 선택
        Transform fp = firePoints[fireIndex];

        // 다음에 사용할 총구
        fireIndex = (fireIndex + 1) % firePoints.Length;

        Vector3 dir = (targetPos - fp.position).normalized;
        Quaternion rot = Quaternion.LookRotation(dir);

        GameObject shellObj = Instantiate(shellPrefab, fp.position, rot);
        SlowCannonShell shell = shellObj.GetComponent<SlowCannonShell>();
        if (shell != null)
        {
            shell.damage = CurrentDamage;
            shell.explosionRadius = CurrentEffectRadius;
            shell.slowMultiplier = CurrentSlowMultiplier;
            shell.slowDuration = CurrentSlowDuration;

            shell.Launch(fp.position, targetPos);
        }
    }


    // ===== 업그레이드 함수들 (이 포탑 인스턴스만 강화) =====

    // 공격력 업그레이드
    public void UpgradeDamage()
    {
        if (damageLevel >= maxDamageLevel) return;
        damageLevel++;
    }

    // 범위(폭발 반경) 업그레이드
    public void UpgradeEffectRadius()
    {
        if (effectRadiusLevel >= maxEffectRadiusLevel) return;
        effectRadiusLevel++;
    }

    // 둔화 업그레이드:
    //  - 아직 70% 미만이면 둔화율 증가
    //  - 70%에 도달하면 이후부터 둔화 지속시간 증가
    public void UpgradeSlow()
    {
        if (CurrentSlowPercent < maxSlowPercent)
        {
            slowLevel++;
        }
        else
        {
            slowDurationLevel++;
        }
    }
}
