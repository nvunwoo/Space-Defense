using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private float baseSpawnInterval = 2f;

    [Header("Spawn Range")]
    [SerializeField] private float rangeX = 5f;
    [SerializeField] private float rangeZ = 5f;

    private float nextSpawn;

    void Update()
    {
        // 준비시간이면 스폰 정지
        if (WaveManager.Instance.isPreparing)
            return;

        float interval = baseSpawnInterval / WaveManager.Instance.GetSpawnSpeedMultiplier();

        if (Time.time >= nextSpawn)
        {
            SpawnEnemy();
            nextSpawn = Time.time + interval;
        }
    }

    void SpawnEnemy()
    {
        float randomX = Random.Range(-rangeX, rangeX);
        float randomZ = Random.Range(-rangeZ, rangeZ);

        Vector3 spawnPos = transform.position + new Vector3(randomX, 0f, randomZ);
        Quaternion spawnRot = Quaternion.Euler(0, -90f, 0);

        EnemyPool.Instance.GetEnemy(spawnPos, spawnRot);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
        Gizmos.DrawCube(transform.position, new Vector3(rangeX * 2f, 0.1f, rangeZ * 2f));

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(rangeX * 2f, 0.1f, rangeZ * 2f));
    }
}
