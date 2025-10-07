using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [Header("Grip")]
    [SerializeField]
    private Transform gripPrimary;
    [SerializeField]
    private Transform gripSecondary;
    [SerializeField]
    private GameObject handPrimary, handSecondary;
    [SerializeField]
    private GameObject grabHighlightPrimary, grabHighlightSecondary;
    
    [Header("Clip")]
    [Tooltip("The bullet type is stored in the clip for the gun")]
    [SerializeField]
    private Holster clipHolster;
    
    [SerializeField]
    private GameObject addNewClipGraphic;
    
    [Header("Firing")]
    [SerializeField]
    private float fireRate;
    [SerializeField]
    private int burstAmount;
    [SerializeField]
    private Transform muzzle;
    [SerializeField]
    private bool pistol;
    [SerializeField]
    private GameObject muzzleFlash;

    [Header("Gun Slide")]
    [SerializeField]
    private Transform slide;
    [SerializeField]
    private float slideDistance = 1f;

    [Header("Recoil")]
    [SerializeField]
    private Transform recoilPivot;
    [SerializeField]
    private bool virtualStock;
    
    

    [SerializeField]
    private float recoilAmountUp = 3f, recoilAmountSide = 2f, recoilRecenterPower = 3f;
    
    //Private variables
    private Clip clip;
    private int burstFired;
    private float nextFire;
    private Vector3 recoilVector, originalSlidePosition;
    private bool stabilized, held;
    private float highlightTimerPrimary, highlightTimerSecondary;


    private void Start() {
        handPrimary.SetActive(false);
        handSecondary.SetActive(false);
        originalSlidePosition = slide.localPosition;
    }
    private void Update() {
        if(clip == null)
            clip = clipHolster.GetClip();
        recoilVector = Vector3.Lerp(recoilVector, Vector3.forward*10f, recoilRecenterPower*Time.deltaTime);
        recoilPivot.localRotation = Quaternion.Lerp(recoilPivot.localRotation, Quaternion.LookRotation(recoilVector), 20f*Time.deltaTime);
        if(clip != null)
        {
            slide.localPosition = Vector3.MoveTowards(slide.localPosition, originalSlidePosition, 5f*Time.deltaTime);
            if(clip.transform.parent == null)
                clip = null;
        }
        addNewClipGraphic.SetActive(clip == null && held);
        if(Input.GetKeyDown(KeyCode.K))
        {
            Fire();
        }
        highlightTimerPrimary -= Time.deltaTime;
        highlightTimerSecondary -= Time.deltaTime;
        grabHighlightPrimary.SetActive(highlightTimerPrimary > 0f);
        grabHighlightSecondary.SetActive(highlightTimerSecondary > 0f);
    }
    public void Fire()
    {
        //Burst firing
        //0 full auto
        //1 semi auto
        //anything else is the burst amount
        bool burstCanFire = burstAmount == 0 || (burstAmount != 0 && burstFired < burstAmount);
        if(Time.time > nextFire && clip != null && clip.HasBullet() && burstCanFire)
        {
            if(muzzleFlash != null)
                Instantiate(muzzleFlash, muzzle.transform.position, muzzle.transform.rotation);
            Instantiate(clip.TakeBullet(), muzzle.transform.position, muzzle.transform.rotation);
            nextFire = Time.time + fireRate;
            float stabilizer = stabilized ? .3f : 1f;
            recoilVector += new Vector3(recoilAmountSide*Random.Range(-1f, 1f), recoilAmountUp*Random.Range(.5f, 1f), 0) * stabilizer;
            slide.localPosition = originalSlidePosition;
            slide.Translate(0, slideDistance, 0);
            burstFired += 1;
        }
        
    }
    public void Release()
    {
        burstFired = 0;
    }
    public Transform GetGripPrimary()
    {
        return gripPrimary;
    }
    public Transform GetGripSecondary()
    {
        return gripSecondary;
    }
    public Vector3 GetClipPosition()
    {
        return clipHolster.transform.position;
    }
    public bool IsPistol()
    {
        return pistol;
    }
    public void EnablePrimaryHand(bool enabled, bool leftHand)
    {
        handPrimary.SetActive(enabled);
        handPrimary.transform.localScale = new Vector3(leftHand ? -1 : 1, 1, 1);
        held = enabled;
    }
    public void EnableSecondaryHand(bool enabled, bool leftHand)
    {
        handSecondary.SetActive(enabled);
        handSecondary.transform.localScale = new Vector3(leftHand ? -1 : 1, 1, 1);
        stabilized = enabled;
    }
    public void EnablePrimaryHighlight()
    {
        highlightTimerPrimary = .1f;
    }
    public void EnableSecondaryHighlight()
    {
        highlightTimerSecondary = .1f;
    }
    public bool UsesVirtualStock()
    {
        return virtualStock;
    }
}
