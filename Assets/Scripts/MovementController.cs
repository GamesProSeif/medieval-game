using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    private Rigidbody rb;
    private Transform followTransform;
    private Animator animator;
    private bool isGrounded = true;
    private Vector3 playerMovementInput = Vector3.zero;
    private Vector2 look = Vector2.zero;

    [Header("Dynamics")]
    public float Speed;
    public float jumpForce;
    public float groundDrag;

    [Header("Player Step Climb")]
    public GameObject stepRayLower;
    public GameObject stepRayUpper;
    public float stepHeight = 0.3f;
    public float stepSmooth = 0.1f;

    [Header("Settings")]
    public float horizontalSensitivity;
    public float verticalSensitivity;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GameObject.Find("PlayerBody").gameObject.GetComponent<Animator>();
        followTransform = GameObject.Find("Neck").transform;
    }

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void FixedUpdate()
    {
        MovePlayer();
        StepClimb();
    }
    void Update()
    {
        playerMovementInput =
            new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        look.x = Input.GetAxis("Mouse X") * horizontalSensitivity;
        look.y = Input.GetAxis("Mouse Y") * verticalSensitivity * -1;

        if (isGrounded)
            rb.drag = groundDrag;
        else rb.drag = 0;

        SpeedControl();
        CheckMoveDirection();
        animator.SetFloat("Speedf", rb.velocity.magnitude);
    }

    void MovePlayer()
    {
        #region Player Movement

        if (playerMovementInput.sqrMagnitude > 0f)
        {
            Vector3 moveDir = transform.forward * playerMovementInput.z + transform.right * playerMovementInput.x;
            rb.AddForce(moveDir.normalized * Speed * 10f, ForceMode.Force);
            if (Input.GetKey(KeyCode.Space) && isGrounded)
            {
                rb.AddForce(jumpForce * Vector3.up, ForceMode.Impulse);
                isGrounded = false;
            }
        }

        #endregion

        #region Player Based Rotation
        if (look.x != 0f)
            transform.rotation *= Quaternion.AngleAxis(look.x, Vector3.up);
        #endregion

        #region Follow Transform Rotation

        if (look.y != 0f)
        {
            followTransform.rotation *= Quaternion.AngleAxis(look.y, Vector3.right);

            var angles = followTransform.localEulerAngles;
            angles.z = 0;

            var angle = followTransform.localEulerAngles.x;

            // Clamp the Up/Down rotation
            if (angle > 180 && angle < 340)
                angles.x = 340;
            else if (angle < 180 && angle > 40)
                angles.x = 40;

            followTransform.localEulerAngles = angles;
        }

        #endregion
    }

    void CheckMoveDirection()
    {
        //checking what direction the user is moving in for the animation to take place
        //pressing one of the keys disables the others (if better way found please implement ana dma8y wag3any)
        if(Input.GetKey(KeyCode.D))
        {
            animator.SetBool("isMovingRight", true);
            animator.SetBool("isMovingBackwards",false);
            animator.SetBool("isMovingLeft", false);
            animator.SetBool("isMovingForward", false);
        }
        else if (Input.GetKey(KeyCode.A))
        {
            animator.SetBool("isMovingLeft", true);
            animator.SetBool("isMovingRight", false);
            animator.SetBool("isMovingForward", false);
            animator.SetBool("isMovingBackwards", false);
        }
        else if (Input.GetKey(KeyCode.W))
        {
            animator.SetBool("isMovingForward", true);
            animator.SetBool("isMovingBackwards", false);
            animator.SetBool("isMovingLeft", false);
            animator.SetBool("isMovingRight", false);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            animator.SetBool("isMovingBackwards", true);
            animator.SetBool("isMovingForward", false);
            animator.SetBool("isMovingRight",false);
            animator.SetBool("isMovingLeft", false);
           
        }
       
    }
    void SpeedControl()
    {
        // Calculating the Velocity of the player 
        Vector3 flatVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        if (flatVelocity.magnitude > Speed)
        {
            // Checking if the velocity is higher than the speed Set , if true , limiting the speed to the move speed
            Vector3 limitedVelocity = flatVelocity.normalized * Speed;
            rb.velocity =
                new Vector3(limitedVelocity.x, rb.velocity.y, limitedVelocity.z);
            
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = true;
    }

    void StepClimb()
    {
         RaycastHit hitLower;
        if (Physics.Raycast(stepRayLower.transform.position, transform.TransformDirection(Vector3.forward), out hitLower, 0.1f))
        {
             RaycastHit hitUpper;
             if(!Physics.Raycast(stepRayUpper.transform.position, transform.TransformDirection(Vector3.forward), out hitUpper,0.2f))
             {
                      rb.position -= new Vector3(0f, -stepSmooth, 0f);
             }
        }

        RaycastHit hitLower45; 
        if (Physics.Raycast(stepRayLower.transform.position, transform.TransformDirection(1.5f, 0f, 1f), out hitLower45, 0.1f))
        {
            RaycastHit hitUpper45;
            if (!Physics.Raycast(stepRayLower.transform.position, transform.TransformDirection(1.5f, 0f, 1f), out hitUpper45, 0.2f))
            {
                rb.position -= new Vector3(0f, -stepSmooth, 0f);
            }
        }


        RaycastHit hitLowerMinus45;
        if(Physics.Raycast(stepRayLower.transform.position, transform.TransformDirection(-1.5f, 0f, 1f), out hitLowerMinus45,0.1f))
        {
            RaycastHit hitUpperMinus45;
            if(!Physics.Raycast(stepRayLower.transform.position, transform.TransformDirection(-1.5f, 0f, 1f), out hitUpperMinus45, 0.2f))
            {
                rb.position -= new Vector3(0f, -stepSmooth, 0f);
            }
        }
    }
}

