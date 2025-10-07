using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField]
    private float toughness = 1f;
    private float health = 100f;

    [SerializeField]
    private PainEffect effect;
    [SerializeField]
    private GameObject enableOnDeath;
    [SerializeField]
    private bool destroyOnDeath;
    [SerializeField]
    private AudioClip[] hurtSounds;
    [SerializeField]
    private AudioClip deathSound;

    void Damage(Bullet b)
    {
        Damage(b.GetDamage());
    }
    void Damage(float d)
    {
        health -= d/toughness;
        if(health <= 0)
        {
            PlayClip(deathSound);
            SendMessage("Die", SendMessageOptions.DontRequireReceiver);
            
            if(destroyOnDeath)
                Destroy(gameObject);

            if(enableOnDeath != null)
                enableOnDeath.SetActive(true);
        }
        else if (hurtSounds.Length != 0)
        {
            PlayClip(hurtSounds[Random.Range(0, hurtSounds.Length)]);
        }
        if(effect)
        {
            effect.Flash(1f);
            if(health < 25f)
                effect.PulseOn();
            else
                effect.PulseOff();
        }
        
            
    }
    void PlayClip(AudioClip clip)
    {
        if(clip != null && GetComponent<AudioSource>() != null)
        {
        AudioSource source = GetComponent<AudioSource>();
        source.clip = clip;
        source.pitch = 1f;
        source.volume = 1f;
        source.loop = false;
        source.PlayScheduled(0.0);
        }
    }
}
