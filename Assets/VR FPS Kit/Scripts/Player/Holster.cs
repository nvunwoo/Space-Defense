using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Holster : MonoBehaviour
{
    [SerializeField]
    private bool pistols, guns, clips, grenades, other;
    [SerializeField]
    private string specificName;
    [SerializeField]
    private GameObject held;
    [SerializeField]
    private AudioClip holsterSound;
    // Start is called before the first frame update
    public bool HolsterObject(GameObject obj)
    {
        if(held != null && !IsHeldStillValid())
        {
            held = null;
        }
        if(held  != null)
        {
            return false;
        }
        if(!string.IsNullOrEmpty(specificName) && obj.name.StartsWith(specificName))
        {
            ParentToHolster(obj.transform);
            return true;
        }
        if(pistols && obj.GetComponent<Gun>() && obj.GetComponent<Gun>().IsPistol())
        {
            ParentToHolster(obj.transform);
            return true;
        }   
        if(guns && obj.GetComponent<Gun>() && !obj.GetComponent<Gun>().IsPistol())
        {
            ParentToHolster(obj.transform);
            return true;
        }   
        if (clips && obj.GetComponent<Clip>())
        {
            ParentToHolster(obj.transform);
            return true;
        }
        if (other)
        {
            ParentToHolster(obj.transform);
            return true;
        }
        return false;
    }
    void ParentToHolster(Transform obj)
    {
        PlayClip(holsterSound);
        obj.SetParent(transform);
        obj.localPosition = Vector3.zero;
        obj.localEulerAngles = Vector3.zero;
        if(obj.GetComponent<Rigidbody>() != null)
            obj.GetComponent<Rigidbody>().isKinematic = true;
        held = obj.gameObject;
    }
    public Clip GetClip()
    {
        if(IsHeldStillValid() && held.GetComponent<Clip>())
        {
            return held.GetComponent<Clip>();
        }
        return null;
    }
    bool IsHeldStillValid()
    {
        if(held == null) return false;
        return held.transform.parent == transform;
    }
    void PlayClip(AudioClip clip)
    {
        if(clip != null && GetComponent<AudioSource>() != null)
        {
        AudioSource source = GetComponent<AudioSource>();
        source.clip = clip;
        source.loop = false;
        source.Stop();
        source.Play();
        }
    }
}
