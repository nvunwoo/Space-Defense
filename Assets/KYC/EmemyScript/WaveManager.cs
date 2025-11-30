using UnityEngine;
using System.Collections;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance;

    public int currentWave = 1;
    public int maxWave = 5;

    public float waveDuration = 60f;   // 웨이브 1개 지속 시간
    public float prepDuration = 5f;    // 웨이브 사이 준비 시간

    public bool isPreparing = false;

    [Header("난이도 배수")]
    public float hpMultiplierPerWave = 1.35f;       // HP 증가율
    public float spawnSpeedMultiplierPerWave = 1.15f; // 스폰 속도 증가율

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        StartCoroutine(WaveRoutine());
    }

    IEnumerator WaveRoutine()
    {
        while (currentWave <= maxWave)
        {
            // === 웨이브 시작 ===
            Debug.Log($"Wave {currentWave} 시작");
            isPreparing = false;

            yield return new WaitForSeconds(waveDuration);

            // === 준비 시간 ===
            if (currentWave < maxWave)
            {
                isPreparing = true;
                Debug.Log($"Wave {currentWave} 종료 — {prepDuration}초 준비 시간");

                yield return new WaitForSeconds(prepDuration);
            }

            currentWave++;
        }

        Debug.Log("모든 웨이브 종료");
    }

    // 웨이브 기반 HP 배수
    public float GetHPBoost()
    {
        return Mathf.Pow(hpMultiplierPerWave, currentWave - 1);
    }

    // 웨이브 기반 스폰 속도 배수
    public float GetSpawnSpeedMultiplier()
    {
        return Mathf.Pow(spawnSpeedMultiplierPerWave, currentWave - 1);
    }
}
