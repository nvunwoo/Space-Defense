using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret_Bullet : MonoBehaviour
{
    public float speed = 100f;      // 총알 속도
    public float lifeTime = 5f;    // 몇 초 뒤 자동 파괴

    void Update()
    {
        // 항상 바라보는 방향으로 직진
        transform.position += transform.forward * speed * Time.deltaTime;

        // 일정 시간 지나면 자동 파괴
        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0f)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Destroy(gameObject);
        }
    }
}
