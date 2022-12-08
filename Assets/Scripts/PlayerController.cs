using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using UnityEditor;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{

    Vector3 PlayerMovementInput;


    // Wanted References
    private Rigidbody playerBody;
    private Collider playerCollider;
    public Transform Orientation;
    public Transform targetLookAt;
    public ThirdPersonCamera Camera;

    // Changable Variables
    public float Speed;
    public float jumpForce;
    public float groundDrag;

    // Proof Player is on ground
    bool isGrounded;


    // Stats
   public int Health;
   public int Strength;
   public int Exp;

    
    


    // Start is called before the first frame update
    void Start()
    {
   
        playerBody = GetComponent<Rigidbody>();
        playerCollider = GetComponent<Collider>();
        isGrounded = true;

    }
    // FixedUpdate is called before the Update Method
    private void FixedUpdate()
    {
       
        MovePlayer();
    }
    // Update is called once per frame
    void Update()
    {
        // Get the Player Inputs
        PlayerMovementInput = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));

        // Control drag
        if (isGrounded)
            playerBody.drag = groundDrag;
        else playerBody.drag = 0;

        // Speed control
        speedControl();
    
    }
 
    void MovePlayer()
    {
        if (Camera.currentCamera == ThirdPersonCamera.cameraStyle.Explore)
        {
            // Getting the Direction of the player
            Vector3 moveDir = Orientation.forward * PlayerMovementInput.z + Orientation.right * PlayerMovementInput.x;
            // Moving the player in set direction
            playerBody.AddForce(moveDir.normalized * Speed * 10f, ForceMode.Force);
            // Checking if the player is on the ground before performing another Jump
            
        }
        else if (Camera.currentCamera == ThirdPersonCamera.cameraStyle.Combat)
        {
            Vector3 moveDir = transform.TransformDirection(PlayerMovementInput) * Speed;
            playerBody.velocity = new Vector3(moveDir.x, playerBody.velocity.y, moveDir.z);
        }


        if (Input.GetKey(KeyCode.Space) && isGrounded)
        {
            // Adding force to the player in the y axis
            playerBody.AddForce(jumpForce * Vector3.up, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    void speedControl()
    {
        // Calculating the Velocity of the player 
        Vector3 flatVelocity = new Vector3(playerBody.velocity.x, 0f, playerBody.velocity.z);
        if(flatVelocity.magnitude > Speed)
        {
            // Checking if the velocity is higher than the speed Set , if true , limiting the speed to the move speed
            Vector3 limitedVelocity = flatVelocity.normalized * Speed;
            playerBody.velocity = new Vector3(limitedVelocity.x, playerBody.velocity.y, limitedVelocity.z);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Checking if the player on ground
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = true;

        // Noted for Later Use ( Checking if player has collided with an object of type EXP, if true destroy the other object >> the EXP therefore adding the EXP to the variable
        else if(collision.gameObject.CompareTag("Exp"))
        {
            Destroy(collision.gameObject);
            Exp++;
        }
                }


}



