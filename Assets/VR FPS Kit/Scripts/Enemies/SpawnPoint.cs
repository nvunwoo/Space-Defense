using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [SerializeField]
    private GameObject[] enemies;

    [SerializeField]
    private float spawnRadius, initialDelay = 10f, spawnFrequency = 20f;

    void Start()
    {
        InvokeRepeating("Spawn", initialDelay, Random.Range(spawnFrequency/2f, spawnFrequency));
    }
    void Spawn()
    {
        Vector3 randomRadial = Random.insideUnitSphere*spawnRadius;
        randomRadial = new Vector3(randomRadial.x, 0, randomRadial.z);
        Instantiate(enemies[Random.Range(0, enemies.Length)], transform.position + randomRadial, transform.rotation);
    }
    void OnDrawGizmosSelected() {
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
}
