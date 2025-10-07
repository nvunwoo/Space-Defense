using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vest : MonoBehaviour
{
    private Transform head;
    [SerializeField]
    private float neckLength;
    // Start is called before the first frame update
    void Awake() {
        head = transform.parent.GetComponentInChildren<Camera>().transform;
        
    }

    // Update is called once per frame
    void Update()
    {
        if(head == null)
        {
            Destroy(gameObject);
            return;
        }
        Quaternion headAngle = Quaternion.RotateTowards(head.rotation, Quaternion.LookRotation(head.up), 30f);
        Quaternion desiredRotation = Quaternion.Euler(0, headAngle.eulerAngles.y, 0);
        Vector3 desiredPosition = head.transform.position - (head.up * neckLength);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, desiredRotation, 360f*Time.deltaTime);
        transform.position = desiredPosition;
    }
    private void OnDrawGizmosSelected() {
        var head = transform.parent.GetComponentInChildren<Camera>().transform;
        //Show the neck
        Gizmos.DrawLine(head.transform.position, head.transform.position - (head.up * neckLength));
    }
}
