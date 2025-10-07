using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PainEffect : MonoBehaviour
{
    private new Renderer renderer;
    private float a;
    private bool pulsing = false;

    void Start()
    {
        renderer = GetComponent<Renderer>();
        //This sets up the two "heartbeats" for the pulse
        InvokeRepeating("Pulse", 0f, 1f);
        InvokeRepeating("Pulse", .3f, 1f);
    }

    void Update()
    {
        renderer.material.color = new Color(
            renderer.material.color.r,
            renderer.material.color.g,
            renderer.material.color.b,
            a);
        a = Mathf.Lerp(a, 0, 10f*Time.deltaTime);
    }
    public void Flash(float intensity)
    {
        a = Mathf.Min(a+intensity, 1f);
    }
    public void PulseOn()
    {
        pulsing = true;
    }
    public void PulseOff()
    {
        pulsing = false;
    }
    void Pulse()
    {
        if(pulsing)
            Flash(.4f);
    }
}
