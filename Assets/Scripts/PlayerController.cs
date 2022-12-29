using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using UnityEditor;
using UnityEngine.EventSystems;
using System.Security.Cryptography;

public class PlayerController : MonoBehaviour
{

    Vector3 PlayerMovementInput;

     //public TextMeshProUGUI text;
   // public AudioSource audi;
    //public GameObject layout;
    //public Button button;
    // public GameObject zero;
    //  public GameObject one;

  

    // Wanted References
    private Rigidbody playerBody;
    private Collider playerCollider;
    public Transform Orientation;
    public Transform targetLookAt;
    public ThirdPersonCamera Camera;
    //public Animator anim;

    // Changable Variables
    public float Speed;
    public float jumpForce;
    public float groundDrag;

    [Header("Player Step Climb")]
    public GameObject stepRayLower;
    public GameObject stepRayUpper;
    public float stepHeight = 0.3f;
    public float stepSmooth = 0.1f;

    // Bools
    bool isGrounded;
    public bool isGameOver;


    // Stats
   public int Health;
   public int Strength;
   public int Exp;

    
    


    void Awake()
    {
        playerBody = GetComponent<Rigidbody>();
        playerCollider = GetComponent<Collider>();
        isGrounded = true;

      //  layout.SetActive(false);
       // zero.transform.position = transform.gameObject.transform.position;
      //  one.transform.position = GameObject.Find("Enemy").gameObject.transform.position;

        stepRayUpper.transform.position = new Vector3(stepRayUpper.transform.position.x, stepHeight, stepRayUpper.transform.position.z);
       // anim = GetComponent<Animator>();  

        // audi.Stop();
    }
    // FixedUpdate is called before the Update Method
    private void FixedUpdate()
    {
        if (!isGameOver)
        {
            MovePlayer();
            stepClimb();
            
        }
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
        //anim.SetFloat("speedf",playerBody.velocity.magnitude);
    }
 
    void MovePlayer()
    {
        
        
            // Getting the Direction of the player
            Vector3 moveDir = Orientation.forward * PlayerMovementInput.z + Orientation.right * PlayerMovementInput.x;
            // Moving the player in set direction
            playerBody.AddForce(moveDir.normalized * Speed * 10f, ForceMode.Force);
            // Checking if the player is on the ground before performing another Jump
        if (Input.GetKey(KeyCode.Space) && isGrounded)
        {
            // Adding force to the player in the y axis
            playerBody.AddForce(jumpForce * Vector3.up, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    void stepClimb()
    {
             RaycastHit hitLower;
        if (Physics.Raycast(stepRayLower.transform.position, transform.TransformDirection(Vector3.forward), out hitLower, 0.1f))
        {
                         RaycastHit hitUpper;
                         if(!Physics.Raycast(stepRayUpper.transform.position, transform.TransformDirection(Vector3.forward), out hitUpper,0.2f))
                         {
                                  playerBody.position -= new Vector3(0f, -stepSmooth, 0f);
                         }
        }

            RaycastHit hitLower45; 
        if (Physics.Raycast(stepRayLower.transform.position, transform.TransformDirection(1.5f, 0f, 1f), out hitLower45, 0.1f))
        {
                RaycastHit hitUpper45;
                if (!Physics.Raycast(stepRayLower.transform.position, transform.TransformDirection(1.5f, 0f, 1f), out hitUpper45, 0.2f))
                {
                    playerBody.position -= new Vector3(0f, -stepSmooth, 0f);
                }

        }


            RaycastHit hitLowerMinus45;
        if(Physics.Raycast(stepRayLower.transform.position, transform.TransformDirection(-1.5f, 0f, 1f), out hitLowerMinus45,0.1f))
        {
                RaycastHit hitUpperMinus45;
                if(!Physics.Raycast(stepRayLower.transform.position, transform.TransformDirection(-1.5f, 0f, 1f), out hitUpperMinus45, 0.2f))
                {
                    playerBody.position -= new Vector3(0f, -stepSmooth, 0f);
                }

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
           // audi.Play();
          // text.gameObject.SetActive(true);
        }
    }

   public void GameOver()
    {

        isGameOver = true;
        //layout.SetActive(true);

    }

    public void restartGame()
    {
        isGameOver = false;

      //  gameObject.transform.position = zero.transform.position;
       // GameObject.Find("Enemy").gameObject.transform.position = one.transform.position;
       // layout.SetActive(false);
    }


}



