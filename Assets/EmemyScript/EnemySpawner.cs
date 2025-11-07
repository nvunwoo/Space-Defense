using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private float spawnInterval = 2f;
    [SerializeField] private float spawnRangeX = 5f;
    [SerializeField] private float spawnZ = 0f;

    private float nextSpawn;

    void Update()
    {
        if (Time.time >= nextSpawn)
        {
            SpawnEnemy();
            nextSpawn = Time.time + spawnInterval;
        }
    }

    void SpawnEnemy()
    {
        // ·£´ý X ÁÂÇ¥¿¡¼­ ½ºÆù
        float randomX = Random.Range(-spawnRangeX, spawnRangeX);
        Vector3 spawnPos = new Vector3(randomX, 0f, spawnZ);

        EnemyPool.Instance.GetEnemy(spawnPos, Quaternion.identity);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector3(-spawnRangeX, 0, spawnZ), new Vector3(spawnRangeX, 0, spawnZ));
    }
}
