using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightupOnHit : MonoBehaviour
{
    [SerializeField]
    private Material unhit, hit;

    private float hitTimer;
    private new Renderer renderer;
    private void Start() {
        renderer = GetComponent<Renderer>();
    }
    // Start is called before the first frame update
    void FixedUpdate()
    {
        hitTimer -= Time.fixedDeltaTime;
        renderer.material = hitTimer <= 0f ? unhit : hit;
    }

    // Update is called once per frame
    void Damage(Bullet b)
    {
        hitTimer = 3f;
    }
}
