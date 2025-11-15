using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret_Bullet : MonoBehaviour
{
    public float speed = 20f;      // 총알 속도
    public float lifeTime = 3f;    // 몇 초 뒤 자동 파괴

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
            // 일단은 적도 같이 삭제 (나중에 체력 시스템 붙이면 데미지로 바꾸면 됨)
            Destroy(gameObject);
        }
    }
}
