using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowCannonShell : MonoBehaviour
{
    public float speed = 20f;
    public float lifeTime = 5f;

    [HideInInspector] public float damage = 10f;          // Turret에서 설정
    [HideInInspector] public float explosionRadius = 5f;  // Turret에서 설정
    [HideInInspector] public float slowMultiplier = 0.7f; // 예: 0.7 → 30% 감속
    [HideInInspector] public float slowDuration = 2.0f;   // 슬로우 지속시간(초)

    private Vector3 targetPos;
    private bool launched = false;

    public void Launch(Vector3 startPos, Vector3 targetPosition)
    {
        transform.position = startPos;
        targetPos = targetPosition;
        launched = true;
    }

    void Update()
    {
        if (!launched) return;

        Vector3 dir = targetPos - transform.position;
        float step = speed * Time.deltaTime;

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
            if (!col.CompareTag("Enemy")) continue;

            // EnemyMove / EnemyMove2 둘 다 지원
            var e1 = col.GetComponent<EnemyMove>();
            if (e1 != null)
            {
                e1.TakeDamage(Mathf.RoundToInt(damage));
                e1.ApplySlow(slowMultiplier, slowDuration);
            }

            var e2 = col.GetComponent<EnemyMove2>();
            if (e2 != null)
            {
                e2.TakeDamage(Mathf.RoundToInt(damage));
                e2.ApplySlow(slowMultiplier, slowDuration);
            }
        }

        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.2f, 0.6f, 1f, 0.4f);
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
