using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CannonShell : MonoBehaviour
{
    public float speed = 50f;        // 포탄 속도
    public float explosionRadius = 20f; // 폭발 범위 반경
    public float lifeTime = 5f;      // 최대 생존 시간

    private Vector3 targetPos;
    private bool launched = false;

    // Turret에서 호출해서 발사 시작
    public void Launch(Vector3 startPos, Vector3 targetPosition, float radius)
    {
        transform.position = startPos;
        targetPos = targetPosition;
        explosionRadius = radius;
        launched = true;
    }

    void Update()
    {
        if (!launched) return;

        Vector3 dir = targetPos - transform.position;
        float step = speed * Time.deltaTime;

        // 목적지에 거의 도달
        if (dir.magnitude <= step)
        {
            Explode();
            return;
        }

        transform.position += dir.normalized * step;
        transform.forward = dir.normalized;

        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0f)
        {
            Explode();
        }
    }

    void Explode()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (var col in hits)
        {
            if (col.CompareTag("Enemy"))
            {
                EnemyMove enemy = col.GetComponentInParent<EnemyMove>();
                if (enemy != null)
                {
                    enemy.TakeDamage(1);  // EnemyMove 안의 함수 호출
                }
            }
        }

        Destroy(gameObject);
    }

    // 씬에서 폭발 범위 확인용 기즈모(에디터에서만 보임)
    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.4f);
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}