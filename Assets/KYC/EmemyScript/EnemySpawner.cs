using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private float spawnInterval = 2f;

    [Header("Spawn Range")]
    [SerializeField] private float rangeX = 5f;
    [SerializeField] private float rangeZ = 5f;

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
        // X, Z ¸ðµÎ ·£´ý
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
