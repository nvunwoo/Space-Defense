using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableOnTrigger : MonoBehaviour
{
    [SerializeField]
    private GameObject[] enableOnTrigger;
    [SerializeField]
    private bool destroySelf = false;

    private void OnTriggerEnter(Collider other) {

        foreach(GameObject go in enableOnTrigger)
            go.SetActive(true);

        if(destroySelf)
            Destroy(gameObject);
            
    }
}
