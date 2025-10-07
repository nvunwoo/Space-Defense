using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageArea : MonoBehaviour
{
    [SerializeField]
    private float damageOnTrigger = 0f;
    
    private void OnTriggerEnter(Collider other) {
        other.SendMessage("Damage", damageOnTrigger, SendMessageOptions.DontRequireReceiver);     
    }
}
