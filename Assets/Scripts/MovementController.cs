using System.Collections;
using System.Collections.Generic;
using System.Drawing.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEngine.ProBuilder.Shapes;
using UnityEngine.Rendering;

public class MovementController : MonoBehaviour
{
    private Rigidbody rb;
    private Transform followTransform;
    private Animator animator;
    private GameManager gameManager;
    private bool isGrounded = true;
    private Vector3 playerMovementInput = Vector3.zero;
    private Vector2 look = Vector2.zero;
    private GameObject feet;
    private GameObject stepRayLower;
    private GameObject stepRayUpper;
    private StatsController stats;
    private LayerMask ignoreMe;
   

    [Header("Dynamics")]
    public float Speed;
    private float sprintSpeed;
    private float originalSpeed;
    public float jumpForce;
    public float groundDrag;
    List<ContactPoint> allCPs = new List<ContactPoint>();
    private bool movable;

    [Header("Player Step Climb")]
    public float maxStepHeight = 0.1f;
    public float stepSmooth = 0.01f;
    public float offset;


    [Header("Settings")]
    public float horizontalSensitivity;
    public float verticalSensitivity;
    private bool isSprinting;
 

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GameObject.Find("PlayerBody").gameObject.GetComponent<Animator>();
        gameManager = GameObject.Find("GameManager").gameObject.GetComponent<GameManager>();
        followTransform = GameObject.Find("Neck").transform;
        feet = GameObject.Find("Feet");
        stepRayLower = GameObject.Find("lowerRayCast");
        stepRayUpper = GameObject.Find("upperRayCast");
        stats = gameObject.GetComponent<StatsController>();
        ignoreMe = gameObject.layer;
    }

    private void Start()
    {
        gameManager.DisableCurser();
        sprintSpeed = 2 * Speed;
        originalSpeed = Speed;
        isGrounded = true;
    }

    private void FixedUpdate()
    {
       
        Vector3 lastVelocity = rb.velocity;
        MovePlayer();
        JumpPlayer();
        SprintPlayer();
        stepClimb();
        animator.SetFloat("Speedf", rb.velocity.magnitude);
        allCPs.Clear();
        //ContactPoint groundCP = default(ContactPoint);
        //Vector3 stepUpOffset = default(Vector3);
        //bool grounded = FindGround(out groundCP, allCPs);
        //bool stepUp = false;
        //if (grounded)
        //    stepUp = FindStep(out stepUpOffset, allCPs, groundCP);
        
        //if(stepUp)
        //{
        //    rb.position += stepUpOffset;
        //    rb.velocity = lastVelocity;
        //}
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

        if (rb.velocity.y < 0f || !isGrounded)
            rb.angularDrag = 5;
       

        SpeedControl();
        CheckMoveDirection();
        CheckIfGrounded();
        Vector3 moveDir = transform.forward * playerMovementInput.z + transform.right * playerMovementInput.x;
        Debug.DrawLine(feet.transform.position, moveDir);
    }

    void MovePlayer()
    {
        if (!stats.killed)
        {
            #region Player Movement

            if (playerMovementInput.sqrMagnitude > 0f)
            {

                Vector3 moveDir = transform.forward * playerMovementInput.z + transform.right * playerMovementInput.x;
                checkMovableTerrain(gameObject.transform.position, new Vector3(moveDir.x, 0, moveDir.z), 10f);
                if (movable)
                    rb.AddForce(moveDir.normalized * Speed * 5f, ForceMode.Force);
                rb.velocity.Set(moveDir.x, rb.velocity.y, moveDir.z);

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
        }
    }

            #endregion

            void JumpPlayer()
    {
        if (stats.killed) return;
        if(Input.GetKey(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(jumpForce * Vector3.up, ForceMode.Impulse);
        }
    }

    void SprintPlayer()
    {
        if (stats.killed) return;
        if (Input.GetKey(KeyCode.LeftShift) && !isSprinting)
        {
            Speed = sprintSpeed;
            isSprinting = true;
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            Speed = originalSpeed;
            isSprinting = false;
        }
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
        else
        {
            animator.SetBool("isMovingBackwards", false);
            animator.SetBool("isMovingForward", false);
            animator.SetBool("isMovingRight", false);
            animator.SetBool("isMovingLeft", false);
        }
       
    }
    void SpeedControl()
    {
        // Calculating the Velocity of the player 
        Vector3 flatVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        if (flatVelocity.magnitude > Speed)
        {
            //Checking if the velocity is higher than the speed Set , if true , limiting the speed to the move speed
            Vector3 limitedVelocity = flatVelocity.normalized * Speed;
            float limitedYVelocity = rb.velocity.y;
            
            rb.velocity =
                new Vector3(limitedVelocity.x, limitedYVelocity, limitedVelocity.z);
            
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        allCPs.AddRange(collision.contacts);

    }
    private void OnCollisionStay(Collision collision)
    {
        allCPs.AddRange(collision.contacts);
    }

    void CheckIfGrounded()
    {
        
        LayerMask layerMask = gameObject.layer;
        float maxDist = 0.5f;

        if (Physics.Raycast(feet.transform.position, transform.TransformDirection(Vector3.down), maxDist))
        {
            isGrounded = true;
        }
        else isGrounded = false;
        

    }

    private void checkMovableTerrain(Vector3 position, Vector3 desiredDirection, float distance)
    {
        Ray myRay = new Ray(position, desiredDirection);

        RaycastHit hit;
        
        if(Physics.Raycast(myRay, out hit, distance))
        {
            if (hit.collider.gameObject.tag == "Ground" || hit.collider.gameObject.tag == "Wall")
            {
                float slopeAngle = Mathf.Deg2Rad * Vector3.Angle(Vector3.up, hit.normal); //getting the angle between the up vector and the normal of the wall / ground infront : 90 for straight up walls , 0 for flat ground  

                float radius = Mathf.Abs(feet.transform.position.y / Mathf.Sin(slopeAngle));

                if(slopeAngle >= 70 * Mathf.Deg2Rad)
                {
                    if (hit.distance - gameObject.GetComponent<CapsuleCollider>().radius > (Mathf.Cos(slopeAngle) * radius) + 0.01)
                    {
                        movable =  true;
                        
                    }     
                    movable = false;
                }
                movable = true;

            }
        }
    }

    // climbing steps
    // these functions are not in use as of now
    bool FindGround(out ContactPoint groundCP, List<ContactPoint> allCPs)
    {
        groundCP = default(ContactPoint);
        bool found = false;
        foreach(ContactPoint cp in allCPs)
        {
            if (cp.normal.y > 0.0001f && (found == false || cp.normal.y > groundCP.normal.y))
            {
                groundCP = cp;
                found = true;
            }
        }
        return found;
    }

    bool FindStep(out Vector3 stepUpOffset, List<ContactPoint> allCPs, ContactPoint groundCP)
    {
        stepUpOffset = default(Vector3);
        Vector2 velocityXZ = new Vector2(rb.velocity.x, rb.velocity.z);
        if (velocityXZ.sqrMagnitude > 0.0001f)
            return false;
        foreach(ContactPoint cp in allCPs)
        {
            bool test = ResolveStepUp(out stepUpOffset, cp, groundCP);
            if (test)
                return test;

        }
        return false;
    }
    bool ResolveStepUp(out Vector3 stepUpOffset, ContactPoint stepTestCP, ContactPoint groundCP)
    {
        stepUpOffset = default(Vector3);
        Collider stepCollider = stepTestCP.otherCollider;
        
        if (Mathf.Abs(stepTestCP.normal.y) >= 0.01f)
            return false;

        if (!(stepTestCP.point.y - groundCP.point.y < maxStepHeight))
            return false;

        RaycastHit hitInfo;
        float stepHeight = groundCP.point.y + maxStepHeight + 0.0001f;
        Vector3 stepTestInvDir = new Vector3(-stepTestCP.normal.x, 0, -stepTestCP.normal.z);
        Vector3 origin = new Vector3(stepTestCP.point.x, stepHeight, stepTestCP.point.y);
        Vector3 direction = Vector3.down;
        if (!(stepCollider.Raycast(new Ray(origin, direction), out hitInfo, maxStepHeight)))
            return false;
        Vector3 stepUpPoint = new Vector3(stepTestCP.point.x, hitInfo.point.y + 0.0001f, stepTestCP.point.z);
        Vector3 stepUpPointOffset = stepUpPoint - new Vector3(stepTestCP.point.x, groundCP.point.y, stepTestCP.point.z);
        return true; 
    }

    // actual step climber that works
    void stepClimb()
    {
        RaycastHit hitLower;
        if (Physics.Raycast(stepRayLower.transform.position, transform.TransformDirection(-Vector3.up), out hitLower, 3f, ~ignoreMe))
        {
            
            if (isGrounded && (stepRayLower.transform.position.y - hitLower.point.y - offset <= maxStepHeight))
            {
                Vector3 targetVector = new Vector3(rb.position.x, hitLower.point.y, rb.position.z);
                rb.position = Vector3.Lerp(rb.position, targetVector, Time.deltaTime / 0.02f);
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            }
            if(!isGrounded && (stepRayLower.transform.position.y - hitLower.point.y - offset <= maxStepHeight))
            {
                rb.position = new Vector3(rb.position.x, hitLower.point.y, rb.position.z);
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            }
        }
    }
}


