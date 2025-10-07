using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Restart : MonoBehaviour
{
    private HandInput hands;

    void Start()
    {
        hands = GetComponent<HandInput>();
    }

    void Update()
    {
        if(hands.GetLeftTriggerDown() || hands.GetRightTriggerDown())
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
