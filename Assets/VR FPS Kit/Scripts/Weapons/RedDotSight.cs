using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedDotSight : MonoBehaviour
{
    [SerializeField]
    private float angle, distance;
    [SerializeField]
    private GameObject sightLeftEye, sightRightEye;
    private GameObject leftCamera, rightCamera;
    void Start()
    {
        leftCamera = GameObject.FindGameObjectWithTag("LeftSight");
        rightCamera = GameObject.FindGameObjectWithTag("RightSight");
    }
    void LateUpdate() {
        if(leftCamera == null || rightCamera == null)
            return;
        SetSightActive(sightLeftEye, leftCamera);
        SetSightActive(sightRightEye, rightCamera);
    }
    void SetSightActive(GameObject sight, GameObject eye)
    {
        float viewingAngle = Vector3.Angle(-transform.forward, eye.transform.position-transform.position);
        float viewingDistance = Vector3.Distance(transform.position, eye.transform.position);
        sight.SetActive(viewingAngle <= angle && viewingDistance <= distance);
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position - transform.forward*distance);
        float height = (Mathf.Tan(angle * (Mathf.PI/180f)) * distance);
        Gizmos.DrawLine(transform.position, transform.position - transform.forward*distance + transform.up*height);
        Gizmos.DrawLine(transform.position, transform.position - transform.forward*distance - transform.up*height);
        Gizmos.DrawLine(transform.position, transform.position - transform.forward*distance + transform.right*height);
        Gizmos.DrawLine(transform.position, transform.position - transform.forward*distance - transform.right*height);
    }
}
