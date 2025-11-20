using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowCannonShell : MonoBehaviour
{
    public float speed = 50f;            // 포탄 속도
    public float explosionRadius = 10f;  // 폭발 범위 반경
    public float lifeTime = 5f;          // 최대 생존 시간

    public float slowMultiplier = 0.5f;  // 0.5f면 50% 속도
    public float slowDuration = 3f;      // 둔화 지속 시간

    private Vector3 targetPos;
    private bool launched = false;

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
                    // 슬로우 갱신
                    enemy.ApplySlow(slowMultiplier, slowDuration);
                    // 데미지 적용(제일낮게)
                    enemy.TakeDamage(1);

                }
            }
        }

        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0f, 0.5f, 1f, 0.4f);
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
