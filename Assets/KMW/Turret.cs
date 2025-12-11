using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    [Header("기본 공격 수치")]
    public float baseDamage = 10f;              // 기본 공격력
    public float damagePerLevel = 2f;           // 공격력 레벨당 증가량

    public float baseFireInterval = 0.5f;       // 기본 공격 간격(초)
    public float fireIntervalMultPerLevel = 0.9f; // 공속 레벨당 배율(0.9면 점점 빨라짐)

    public float critChancePerLevel = 0.1f;     // 크리 레벨당 +10%
    public float baseCritMultiplier = 1.5f;     // 기본 크리 배율(1.5배)
    public float critMultPerLevel = 0.25f;      // 크리 피해 레벨당 증가

    [Header("레벨 상태 (이 포탑 개별 적용)")]
    public int damageLevel = 1;
    public int fireRateLevel = 1;
    public int critLevel = 0;           // 크리 확률 레벨
    public int critDamageLevel = 0;     // 크리 피해 레벨

    public int maxDamageLevel = 10;
    public int maxFireRateLevel = 10;
    public int maxCritLevel = 10;       // 확률 100%까지
    public int maxCritDamageLevel = 10;

    [Header("공격/조준 설정")]
    public float range = 100f;          // 사거리
    public GameObject bulletPrefab;     // 총알 프리팹
    public Transform[] firePoints;      // 총구들 (좌우 2개 등)
    int fireIndex = 0;

    [Header("회전 파츠 연결")]
    public TurretYawController yawPart;     // 좌우 회전 파츠
    public TurretPitchController pitchPart; // 상하 회전 파츠

    float fireTimer = 0f;

    [Header("발사 사운드")]
    public AudioClip fireSound;      // 발사 사운드 파일

    // 현재 실사용 수치들
    public float CurrentDamage
        => baseDamage + (damageLevel - 1) * damagePerLevel;

    public float CurrentFireInterval
        => baseFireInterval * Mathf.Pow(fireIntervalMultPerLevel, fireRateLevel - 1);

    public float CurrentCritChance
        => Mathf.Min(1f, critLevel * critChancePerLevel); // 1 == 100%

    public float CurrentCritMultiplier
        => baseCritMultiplier + critDamageLevel * critMultPerLevel;

    void Update()
    {
        fireTimer -= Time.deltaTime;

        // 1) 사거리 내 가장 가까운 적 찾기
        Transform target = FindNearestEnemyInRange();

        // 2) 회전 파츠에 타겟 전달
        if (yawPart != null) yawPart.SetTarget(target);
        if (pitchPart != null) pitchPart.SetTarget(target);

        // 3) 타겟이 있고 쿨타임이 끝났으면 발사
        if (target != null && fireTimer <= 0f)
        {
            Shoot(target);
            fireTimer = CurrentFireInterval;
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

        // 이번에 쓸 총구 선택 (좌/우 번갈아)
        Transform fp = firePoints[fireIndex];
        fireIndex = (fireIndex + 1) % firePoints.Length;

        // 총구에 붙은 AudioSource에서 소리 재생
        if (fireSound != null)
        {
            AudioSource src = fp.GetComponent<AudioSource>();
            if (src != null)
            {
                src.PlayOneShot(fireSound);
            }
        }

        Vector3 dir = (target.position - fp.position).normalized;
        Quaternion rot = Quaternion.LookRotation(dir);

        GameObject go = Instantiate(bulletPrefab, fp.position, rot);
        Turret_Bullet bullet = go.GetComponent<Turret_Bullet>();
        if (bullet != null)
        {
            bullet.damage = CurrentDamage;
            bullet.critChance = CurrentCritChance;
            bullet.critMultiplier = CurrentCritMultiplier;
        }
    }


    // ===== 업그레이드 함수들 (이 포탑 인스턴스만 강화) =====

    // 공격력 업글
    public void UpgradeDamage()
    {
        if (damageLevel >= maxDamageLevel) return;
        damageLevel++;
        // 여기서 이 포탑만 이펙트/사운드 재생해도 됨
    }

    // 공격속도 업글
    public void UpgradeFireRate()
    {
        if (fireRateLevel >= maxFireRateLevel) return;
        fireRateLevel++;
    }

    // 크리티컬 업글: 확률 → 100%까지, 그 다음부터는 피해 증가
    public void UpgradeCrit()
    {
        if (CurrentCritChance < 1f && critLevel < maxCritLevel)
        {
            // 아직 100% 미만이면 크리 확률 레벨업
            critLevel++;
        }
        else
        {
            // 100%이면 크리 피해 레벨업
            if (critDamageLevel >= maxCritDamageLevel) return;
            critDamageLevel++;
        }
    }
}
