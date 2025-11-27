using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarricadeHealth : MonoBehaviour
{
    [Header("체력 설정")]
    public int level = 1;              // 바리게이트 레벨
    public float baseMaxHP = 1000f;    // 기본 최대 체력
    public float maxHP;                // 현재 최대 체력 (레벨업 등으로 증가 가능)
    public float currentHP;            // 현재 체력

    [Header("회복 설정")]
    [Tooltip("1초마다 최대체력의 몇 %를 회복할지 (0.01 = 1%)")]
    public float regenPercentPerSecond = 0.01f; // 1% 회복

    public bool isDestroyed = false;   // 완전히 부서졌는지 여부

    void Awake()
    {
        // 레벨에 따라 최대체력 계산 (레벨당 +1000)
        if (level < 1) level = 1;

        maxHP = GetMaxHPForLevel(level);
        currentHP = maxHP;

        StartCoroutine(RegenerateRoutine());
    }
    //레벨 따라 최대체력 1000증가
    float GetMaxHPForLevel(int lvl)
    {
        return baseMaxHP + (lvl - 1) * 1000f;
    }

    // Enemy가 공격할 때 함수 호출
    public void TakeDamage(float damage)
    {
        if (isDestroyed)
            return;

        currentHP -= damage;
        if (currentHP <= 0f)
        {
            currentHP = 0f;
            OnDestroyed();
        }

        // 디버그 확인용
        // Debug.Log($"Barricade HP: {currentHP}/{maxHP}");
    }

    // 바리게이트가 완전히 부서졌을 때 처리
    void OnDestroyed()
    {
        isDestroyed = true;

        // 게임 오버 처리
        // gameObject.SetActive(false);
    }

    // 1초마다 최대체력의 1% 회복
    IEnumerator RegenerateRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);

            if (isDestroyed)
                continue; // 부서진 상태면 회복 안 함

            if (currentHP > 0f && currentHP < maxHP)
            {
                float healAmount = maxHP * regenPercentPerSecond; // 예: 1000 * 0.005 = 5
                currentHP += healAmount;

                if (currentHP > maxHP)
                    currentHP = maxHP;

                // Debug.Log($"Barricade Regen: +{healAmount}, HP: {currentHP}/{maxHP}");
            }
        }
    }

    // 레벨당 최대체력 +1000 체력 비율 유지
    public void LevelUp()
    {
        if (isDestroyed)
            return;

        // 현재 체력 비율 계산 (0~1)
        float ratio = (maxHP > 0f) ? (currentHP / maxHP) : 0f;

        // 레벨 증가
        level++;

        // 새 레벨 기준 최대 체력 재계산
        maxHP = GetMaxHPForLevel(level);

        // 체력 비율 유지 (풀피로 안 채우고 비율만 유지)
        currentHP = maxHP * ratio;

        // Debug.Log($"Barricade LevelUp → Lv.{level}, HP: {currentHP}/{maxHP}");
    }

    // 현재 체력 비율(HP 바 UI 등에 사용하기 좋음)
    public float GetHPRatio()
    {
        if (maxHP <= 0f) return 0f;
        return currentHP / maxHP;
    }
}