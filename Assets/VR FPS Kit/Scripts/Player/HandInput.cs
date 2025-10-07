using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class HandInput : MonoBehaviour
{
    private InputDevice leftHand, rightHand;
    private bool devicesInitialized = false; // 기기가 초기화되었는지 확인하는 변수 추가

    private bool a, aDown, aTracker,
                leftGrip, leftGripDown, leftGripTracker,
                leftTrigger, leftTriggerDown, leftTriggerTracker,
                rightGrip, rightGripDown, rightGripTracker,
                rightTrigger, rightTriggerDown, rightTriggerTracker,
                rightStickRight, rightStickRightSnap, rightStickRightSnapTracker,
                rightStickLeft, rightStickLeftSnap, rightStickLeftSnapTracker;
    private Vector2 leftStick, rightStick;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Update()
    {
        // 아직 기기가 초기화되지 않았다면 매 프레임 초기화를 시도합니다.
        if (!devicesInitialized)
        {
            InitializeDevices();
        }
    }

    // 기기를 찾는 로직을 별도의 함수로 분리합니다.
    private void InitializeDevices()
    {
        var leftHandDevices = new List<InputDevice>();
        InputDevices.GetDevicesAtXRNode(XRNode.LeftHand, leftHandDevices);
        // 왼쪽 손을 찾았는지 먼저 확인합니다.
        if (leftHandDevices.Count > 0)
        {
            leftHand = leftHandDevices[0];
            Debug.Log("Left Hand device found!");
        }

        var rightHandDevices = new List<InputDevice>();
        InputDevices.GetDevicesAtXRNode(XRNode.RightHand, rightHandDevices);
        // 오른쪽 손을 찾았는지 먼저 확인합니다.
        if (rightHandDevices.Count > 0)
        {
            rightHand = rightHandDevices[0];
            Debug.Log("Right Hand device found!");
        }

        // 양손이 모두 찾아졌을 때만 초기화 완료 처리합니다.
        if (leftHand.isValid && rightHand.isValid)
        {
            devicesInitialized = true;
        }
    }

    //ONLY CALL INPUT from fixedUpdate functions
    private void FixedUpdate() {

        // 기기가 초기화되지 않았다면 입력을 받지 않습니다.
        if (!devicesInitialized)
        {
            return;
        }

        rightHand.TryGetFeatureValue(CommonUsages.primaryButton, out a);
         rightHand.TryGetFeatureValue(CommonUsages.primary2DAxis, out rightStick);
         rightHand.TryGetFeatureValue(CommonUsages.gripButton, out rightGrip);
         rightHand.TryGetFeatureValue(CommonUsages.triggerButton, out rightTrigger);
         leftHand.TryGetFeatureValue(CommonUsages.primary2DAxis, out leftStick);
         leftHand.TryGetFeatureValue(CommonUsages.gripButton, out leftGrip);
         leftHand.TryGetFeatureValue(CommonUsages.triggerButton, out leftTrigger);
         aDown = GetIsDown(ref aTracker, a);
         leftGripDown = GetIsDown(ref leftGripTracker, leftGrip);
         rightGripDown = GetIsDown(ref rightGripTracker, rightGrip);
         leftTriggerDown = GetIsDown(ref leftTriggerTracker, leftTrigger);
         rightTriggerDown = GetIsDown(ref rightTriggerTracker, rightTrigger);
         rightStickLeftSnap = GetIsDown(ref rightStickLeftSnapTracker, rightStick.x < -.9);
         rightStickRightSnap = GetIsDown(ref rightStickRightSnapTracker, rightStick.x > .9);
    }

    public bool GetAButton()
    {
        return a;
    }
    public bool GetAButtonDown()
    {
        return aDown;
    }

    //Left Inputs
    
    public Vector2 GetLeftStick()
    {
        return leftStick;
    }
    public bool GetLeftGrip()
    {
        return leftGrip;
    }
    public bool GetLeftGripDown()
    {
        return leftGripDown;   
    }
    public bool GetLeftTrigger()
    {
        return leftTrigger;
    }
    public bool GetLeftTriggerDown()
    {
        return leftTriggerDown;
    }
    
    //Right Inputs
    
    public Vector2 GetRightStick()
    {
        return rightStick;
    }
    public bool GetRightStickLeftSnap()
    {
        return rightStickLeftSnap;
    }
    public bool GetRightStickRightSnap()
    {
        return rightStickRightSnap;
    }
    public bool GetRightGrip()
    {
        return rightGrip;
    }
    public bool GetRightGripDown()
    {
        return rightGripDown;
    }
    public bool GetRightTrigger()
    {
        return rightTrigger;
    }
    public bool GetRightTriggerDown()
    {
        return rightTriggerDown;
    }
    
    bool GetIsDown(ref bool downVariable, bool isDown)
    {
        if(downVariable != isDown)
        {
            downVariable = isDown;  
            if(isDown == true)
            {
                return true;
            }   
        }
        return false;
        
    }
}
