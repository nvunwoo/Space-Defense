using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : MonoBehaviour
{
    public static EnemyPool Instance;

    [SerializeField] private GameObject[] enemyPrefabs;   // ★ 여러 프리팹 받기
    [SerializeField] private int poolSizePerType = 5;     // 각 프리팹당 풀 크기

    private Dictionary<GameObject, Queue<GameObject>> pools
        = new Dictionary<GameObject, Queue<GameObject>>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // ★ 프리팹 종류별로 각각 풀 생성
        foreach (var prefab in enemyPrefabs)
        {
            var queue = new Queue<GameObject>();
            pools.Add(prefab, queue);

            for (int i = 0; i < poolSizePerType; i++)
            {
                var obj = Instantiate(prefab);
                obj.SetActive(false);
                queue.Enqueue(obj);
            }
        }
    }

    public GameObject GetEnemy(Vector3 position, Quaternion rotation)
    {
        // ★ 랜덤으로 어떤 프리팹을 꺼낼지 선택
        GameObject prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
        Queue<GameObject> queue = pools[prefab];

        // ★ 풀 안에 있으면 꺼내고, 없으면 새로 생성
        GameObject enemy = queue.Count > 0 ? queue.Dequeue() : Instantiate(prefab);
        enemy.transform.SetPositionAndRotation(position, rotation);
        enemy.SetActive(true);
        return enemy;
    }

    public void ReturnEnemy(GameObject enemy)
    {
        enemy.SetActive(false);

        // ★ 되돌아온 적이 어떤 프리팹에서 온 애인지 판별
        GameObject key = null;

        foreach (var prefab in enemyPrefabs)
        {
            if (enemy.name.Contains(prefab.name))
            {
                key = prefab;
                break;
            }
        }

        // 못 찾았으면 그냥 첫 번째 프리팹 큐에 넣음 (최후의 안전장치)
        if (key == null) key = enemyPrefabs[0];

        pools[key].Enqueue(enemy);
    }
}
