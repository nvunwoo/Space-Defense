using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Zombie : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform player;
    
    private Animator anim;
    private bool dead, attacking;
    private float nextAttack;
    [SerializeField]
    private float attackInterval, attackDamage;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        if(GameObject.FindGameObjectWithTag("Player") != null)
            player = GameObject.FindGameObjectWithTag("Player").transform;

        AudioSource source = GetComponent<AudioSource>();
        source.time = Random.Range(0f, source.clip.length-.1f);
    }
    void Update() {
        if(player == null)
            return;
        if(!dead)
        {
            agent.SetDestination(player.position);
            attacking = Vector3.Distance(transform.position, agent.destination) <= agent.stoppingDistance+(agent.radius*2f);
            anim.SetBool("Attacking", attacking);
            if(attacking)
        {
            if(Time.time > nextAttack)
            {
                player.SendMessageUpwards("Damage", attackDamage, SendMessageOptions.DontRequireReceiver);
                nextAttack = Time.time + attackInterval;
            }
        }
        }
        
    }
    void Die()
    {
        if(dead)
            return;
        dead = true;
        agent.speed = 0f;
        anim.SetTrigger("Die");
        Invoke("Cleanup", 30f);
        Destroy(agent);
        if(GetComponentInChildren<Collider>())
            Destroy(GetComponentInChildren<Collider>());
    }
    void Damage(Bullet b)
    {
        anim.SetTrigger("Hit");
        if(agent != null)
            agent.velocity += b.transform.forward*4f;
    }
    void Cleanup()
    {
        Destroy(gameObject);
    }
}
