using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Require a character controller to be attached to the same game object
[RequireComponent(typeof(PlayerMotor))]

public class PlayerMovementInput : MonoBehaviour
{
    private PlayerMotor motor;
    private Transform head;
    private new CharacterController collider;
    private HandInput hands;
    
    [SerializeField]
    private AudioClip[] footstepSounds;
    private float footstepNext;

    // Use this for initialization
    void Awake()
    {
        collider = GetComponent<CharacterController>();
        hands = GetComponent<HandInput>();
        motor = GetComponent<PlayerMotor>();
        head = GetComponentInChildren<Camera>().transform;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMove();
        UpdateJump();
        UpdateCollider();
        UpdateSounds();
    }
    void UpdateMove()
    {
        Vector3 directionVector = new Vector3(hands.GetLeftStick().x, 0, hands.GetLeftStick().y);
        motor.inputMoveDirection = head.transform.rotation * directionVector;
        if(hands.GetRightStickLeftSnap()) transform.RotateAround(head.transform.position, Vector3.up, -45);
        if(hands.GetRightStickRightSnap()) transform.RotateAround(head.transform.position, Vector3.up, 45);
        
    }
    void UpdateJump()
    {
        motor.inputJump = hands.GetAButton();
    }
    void UpdateCollider()
    {
        collider.height = head.localPosition.y + collider.radius;
        collider.center = new Vector3(head.localPosition.x, collider.height/2f, head.localPosition.z);
    }
    void UpdateSounds()
    {
        if(footstepSounds.Length == 0)
            return;

        if(motor.grounded)
            footstepNext -= motor.movement.velocity.magnitude*Time.deltaTime;

        if(footstepNext < 0f)
        {
            footstepNext = 1.1f;
            PlayClip(footstepSounds[Random.Range(0, footstepSounds.Length)]);
        }
            
    }
    void PlayClip(AudioClip clip)
    {
        if(clip != null && GetComponent<AudioSource>() != null)
        {
            AudioSource source = GetComponent<AudioSource>();
            source.clip = clip;
            source.loop = false;
            source.volume = .5f;
            source.pitch = Random.Range(.8f, 1.2f);
            source.Stop();
            source.Play();
        }
    }
}