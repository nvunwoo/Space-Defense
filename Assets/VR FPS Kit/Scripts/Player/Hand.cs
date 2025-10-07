using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class Hand : MonoBehaviour
{
    [SerializeField]
    private float grabRadius=4f;
    [SerializeField]
    private GameObject handGraphic;
    [SerializeField]
    private bool leftHand = false;

    [SerializeField]
    private Hand otherHand;
    private HandInput hand;
    private Transform held;
    private Gun gun;
    private bool secondaryGrip = false;
    private Vector3 lastFramePosition, frameVelocity;

    class HolstersByDistance
    {
        public Holster hol;
        public float dist;
    }
    class GrabByDistance
    {
        public GameObject grab;
        public float dist;
    }
    
    private void Start() {
        hand = GetComponentInParent<HandInput>();
    }
    void Update()
    {
        frameVelocity = transform.position - lastFramePosition;
        lastFramePosition = transform.position;

        if(held == null)
        {
            ScanAndGrabObjects();
            
        }
        if(held != null)
        {
            if(!secondaryGrip)
            {
                HoldObjectAsPrimaryHand();
            }
            else
            {
                HoldObjectAsSecondaryHand();
            }
            if(((leftHand && !hand.GetLeftGrip()) || (!leftHand && !hand.GetRightGrip())) ||
                (secondaryGrip && otherHand.GetHeld() == null))
            {
                Release(secondaryGrip);
            }
        }
        
    }
    void HoldObjectAsPrimaryHand()
    {
        if(!otherHand.IsStabalizing())
        {
            //Aim down by 20deg for comfort
            Quaternion desiredRot = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(-transform.up, transform.forward), 20f);
            held.transform.rotation = Quaternion.Lerp(held.transform.rotation, desiredRot, 20f*Time.deltaTime);
        }
        if(gun != null)
        {
            if((leftHand && hand.GetLeftTrigger()) || (!leftHand && hand.GetRightTrigger()))
            {
                gun.Fire();
            }
            else
            {
                gun.Release();
            }
        }
    }
    void HoldObjectAsSecondaryHand()
    {
        //If we're holding something with 2 hands, stabilize it
        if(!IsHoldingPistol())
        {
            Vector3 forwardOne = (transform.position - otherHand.transform.position).normalized;
            //virtual stock uses the camera's rotation too
            Vector3 forwardTwo = held.transform.position - Camera.main.transform.position;
            Quaternion rot = gun.UsesVirtualStock() ? Quaternion.LookRotation(forwardOne + forwardTwo, otherHand.transform.up) : Quaternion.LookRotation(forwardOne, otherHand.transform.up);
            held.transform.rotation = Quaternion.Lerp(held.transform.rotation, rot, 20f*Time.deltaTime);
        }
    }
    void ScanAndGrabObjects()
    {
        List<GrabByDistance> grabByDistances = new List<GrabByDistance>();
        GameObject[] weapons = GameObject.FindGameObjectsWithTag("Weapon");
        GameObject[] grabbables = GameObject.FindGameObjectsWithTag("Grabbable");
        foreach(GameObject weapon in weapons)
        {
            grabByDistances.Add(new GrabByDistance{
                grab = weapon,
                dist = Vector3.Distance(transform.position, weapon.transform.position)
            });
            grabByDistances.Add(new GrabByDistance{
                grab = weapon,
                dist = Vector3.Distance(transform.position, weapon.GetComponent<Gun>().GetGripSecondary().position)
            });
            grabByDistances.Add(new GrabByDistance{
                grab = weapon,
                dist = Vector3.Distance(transform.position, weapon.GetComponent<Gun>().GetGripPrimary().position)
            });
            if(weapon.GetComponent<Collider>())
                grabByDistances.Add(new GrabByDistance{
                    grab = weapon,
                    dist = Vector3.Distance(transform.position, weapon.GetComponent<Collider>().ClosestPoint(transform.position))
                });
        }
        foreach(GameObject grabbable in grabbables)
        {
            if(grabbable.GetComponent<Clip>() == null ||
                (grabbable.GetComponent<Clip>() != null && !grabbable.GetComponent<Clip>().InGun()))
                {
                    grabByDistances.Add(new GrabByDistance{
                    grab = grabbable,
                    dist = Vector3.Distance(transform.position, grabbable.transform.position)
                    });
                }
                
        }
        grabByDistances = grabByDistances.OrderBy(x => x.dist).ToList();
        bool highlit = false;
        foreach(GrabByDistance gbd in grabByDistances)
        {
            bool grab = (leftHand && hand.GetLeftGripDown()) || (!leftHand && hand.GetRightGripDown());
            if(gbd.dist <= grabRadius)
            {
                if(!highlit)
                {
                    if(otherHand.GetHeld() != null && otherHand.IsHolding(gbd.grab.transform))
                    {
                        gbd.grab.transform.SendMessage("EnableSecondaryHighlight", SendMessageOptions.DontRequireReceiver);
                        highlit = true;
                    }
                    else
                    {
                        gbd.grab.transform.SendMessage("EnablePrimaryHighlight", SendMessageOptions.DontRequireReceiver);
                        highlit = true;
                    }
                }
                

                if(grab)
                {
                    held = gbd.grab.transform;
                    held.GetComponent<Rigidbody>().isKinematic = true;
                    if(gbd.grab.transform.GetComponent<Gun>())
                    {
                        gun = gbd.grab.transform.GetComponent<Gun>();
                        if(otherHand.IsHolding(held))
                        {
                            secondaryGrip = true;
                            Grab(true);
                        }
                        else
                        {
                            Grab(false);
                        }
                    }
                    else
                    {
                        Grab(false);
                    }
                    return;
                }
                    
                
            }
            
        }
    }
    void Grab(bool secondary)
    {
        if(secondary == false)
            held.SetParent(null);

        handGraphic.SetActive(false);
        if(!secondary)
        {

            Vector3 pos = gun == null ? held.position : gun.GetGripPrimary().position;
            held.rotation = transform.rotation;
            held.position = transform.position - (pos - held.position);
            held.SetParent(transform);
            if(gun)
            {
                gun.EnablePrimaryHand(true, leftHand);
            }
            else
            {
                held.SendMessage("EnablePrimaryHand", leftHand);
            }
            
        }
        else
        {
            if(gun)
            {
                gun.EnableSecondaryHand(true, leftHand);
            }
            else
            {
                held.SendMessage("EnableSecondaryHand", leftHand);
            }
        }
        
    }
    public void Release(bool secondary)
    {
        
        secondaryGrip = false;
        handGraphic.SetActive(true);
        if(secondary)
        {
            if(gun)
            {
                gun.EnableSecondaryHand(false, leftHand);
            }
            else
            {
                held.SendMessage("DisableSecondaryHand", leftHand);
            }
        }
        else
        {
            held.transform.SetParent(null);
            held.GetComponent<Rigidbody>().isKinematic = false;
            held.GetComponent<Rigidbody>().velocity = frameVelocity/Time.deltaTime;
            //Try holstering starting with the closest holster and going outwards
            Holster[] holsters = GameObject.FindObjectsOfType<Holster>();
            List<HolstersByDistance> holstersByDistances = new List<HolstersByDistance>();
            foreach(Holster holster in holsters)
                if(holster.GetComponent<Collider>())
                   holstersByDistances.Add(new HolstersByDistance {
                        hol = holster,
                        dist = Vector3.Distance(held.position,  holster.GetComponent<Collider>().ClosestPoint(held.position))
                    });
                else
                   holstersByDistances.Add(new HolstersByDistance {
                        hol = holster,
                        dist = Vector3.Distance(held.position,  holster.transform.position)
                    });
            holstersByDistances = holstersByDistances.OrderBy(x => x.dist).ToList();
            foreach(HolstersByDistance hbd in holstersByDistances)
                if(hbd.dist < grabRadius)
                    if(hbd.hol.HolsterObject(held.gameObject)) break;
            if(gun)
            {
                gun.EnablePrimaryHand(false, leftHand);
            }
            else
            {
                held.SendMessage("DisablePrimaryHand", leftHand);
            }
        }
        
        held = null;
        gun = null;
    }
    public bool IsStabalizing()
    {
        return secondaryGrip;
    }
    public bool IsHolding(Transform heldNew)
    {
        return held == heldNew;
    }

    public bool IsHoldingAClip()
    {
        if(held == null) return false;
        return held.GetComponent<Clip>() != null;
    }
    public bool IsHoldingPistol()
    {
        return gun.IsPistol();
    }
    public Transform GetHeld()
    {
        return held;
    }
    void OnDrawGizmosSelected() {
        Gizmos.DrawWireSphere(transform.position, grabRadius);
    }
}

