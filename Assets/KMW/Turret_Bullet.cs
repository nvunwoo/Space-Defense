using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret_Bullet : MonoBehaviour
{
    public float speed = 100f;
    public float lifeTime = 3f;

    [HideInInspector] public float damage = 1f;
    [HideInInspector] public float critChance = 0f;        // 0~1
    [HideInInspector] public float critMultiplier = 1.5f;  // 크리 배수

    [Header("크리/노말 메쉬 오브젝트")]
    public GameObject normalMesh;   // 일반 총알 모델
    public GameObject critMesh;     // 크리 총알 모델

    private bool isCrit = false;    // 이 탄이 크리탄인지 여부

    void Start()
    {
        // 1) 총알 생성 시 크리 여부 한 번 결정
        isCrit = Random.value < critChance;

        // 2) 크리 여부에 따라 메쉬 On/Off
        if (normalMesh != null)
            normalMesh.SetActive(!isCrit);

        if (critMesh != null)
            critMesh.SetActive(isCrit);
    }

    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;

        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0f)
            Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Enemy"))
            return;

        float finalDamage = damage;
        if (isCrit)
            finalDamage *= critMultiplier;

        // EnemyMove / EnemyMove2 둘 다 처리
        var e1 = other.GetComponent<EnemyMove>();
        if (e1 != null)
            e1.TakeDamage(Mathf.RoundToInt(finalDamage));

        var e2 = other.GetComponent<EnemyMove2>();
        if (e2 != null)
            e2.TakeDamage(Mathf.RoundToInt(finalDamage));

        Destroy(gameObject);
    }
}
