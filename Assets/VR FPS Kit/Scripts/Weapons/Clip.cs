using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clip : MonoBehaviour
{
    [SerializeField]
    private int capacity;
    private int ammo;
    [SerializeField]
    private GameObject bullet;
    // Start is called before the first frame update
    void Start()
    {
        ammo = capacity;
    }
    public bool HasBullet()
    {
        return ammo > 0;
    }
    public GameObject TakeBullet()
    {
        ammo--;
        if(ammo == 0)
        {
            transform.SetParent(null);
            GetComponent<Rigidbody>().isKinematic = false;
            GetComponent<Rigidbody>().velocity = -transform.up * 2f;
            Destroy(this);
        }
        return bullet;
    }
    public bool InGun()
    {
        return GetComponentInParent<Gun>() != null;
    }
}
