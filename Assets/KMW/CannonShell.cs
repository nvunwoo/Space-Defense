using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonShell : MonoBehaviour
{
    public float speed = 50f;
    public float lifeTime = 5f;

    [HideInInspector] public float damage = 30f;        // Turret에서 넘겨줌
    [HideInInspector] public float explosionRadius = 3f;// Turret에서 넘겨줌

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

        // 거의 도착하면 폭발
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
        // 폭발 범위 안의 모든 Enemy에게 데미지
        Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (var col in hits)
        {
            if (!col.CompareTag("Enemy")) continue;

            // EnemyMove / EnemyMove2 둘 다 지원
            var e1 = col.GetComponent<EnemyMove>();
            if (e1 != null)
                e1.TakeDamage(Mathf.RoundToInt(damage));

            var e2 = col.GetComponent<EnemyMove2>();
            if (e2 != null)
                e2.TakeDamage(Mathf.RoundToInt(damage));
        }

        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.4f);
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
