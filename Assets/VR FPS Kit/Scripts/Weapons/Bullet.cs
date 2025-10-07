using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField]
    private float speed = 20f, gravity = 9.81f, deathTime = 5f, deviance = 3f, damage = 10f;
    private float currentGravity = 0f;
    public GameObject hitGraphic, enemyHitGraphic;
    private bool destroying = false;
    private void Start()
    {
        Invoke("DeathTimeout", deathTime);
        transform.Rotate(Random.insideUnitSphere * deviance);
    }
    void FixedUpdate()
    {
        if(destroying)
        {
                return;
        }
        currentGravity -= gravity * Time.deltaTime;
        Vector3 moveVector = transform.forward * speed;
        moveVector += Vector3.up * currentGravity;
        moveVector *= Time.deltaTime;
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(transform.position, moveVector, out hit, moveVector.magnitude))
        {
            destroying = true;
            if(hitGraphic != null && !IsEnemy(hit.transform.gameObject))
            {
                GameObject exp = (GameObject)Instantiate(hitGraphic, hit.point - moveVector.normalized * .01f, Quaternion.LookRotation(hit.normal));
                exp.transform.SetParent(hit.transform);
            }
            else if(enemyHitGraphic != null && IsEnemy(hit.transform.gameObject))
            {
                GameObject exp = (GameObject)Instantiate(enemyHitGraphic, hit.point - moveVector.normalized * .01f, Quaternion.LookRotation(hit.normal));
                exp.transform.SetParent(hit.transform);
            }
                
            hit.transform.gameObject.SendMessageUpwards("Damage", this, SendMessageOptions.DontRequireReceiver);
        }
        else
        {
            transform.position += moveVector;
        }
        
    }
    void DeathTimeout()
    {
        Destroy(gameObject);
    }
    public float GetDamage()
    {
        return damage;
    }
    public float GetSpeed()
    {
        return speed;
    }
    bool IsEnemy(GameObject enemy)
    {
        return enemy.transform.root.GetComponent<Zombie>() != null;
    }
}
