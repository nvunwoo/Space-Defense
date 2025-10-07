using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grabbable : MonoBehaviour
{
    [SerializeField]
    private GameObject hand, highlight;

    private float highlightTimer;

    private void Update() {
        highlightTimer -= Time.deltaTime; 
        highlight.SetActive(highlightTimer > 0f);   
    }
    void EnablePrimaryHand(bool leftHand)
    {
        hand.SetActive(true);
        hand.transform.localScale = new Vector3(leftHand ? -1 : 1, 1, 1);
    }

    void DisablePrimaryHand(bool leftHand)
    {
        hand.SetActive(false);
    }
    void EnablePrimaryHighlight()
    {
        highlightTimer = .1f;
    }
}
