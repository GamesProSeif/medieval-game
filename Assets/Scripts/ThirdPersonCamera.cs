using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    // Referances
    public PlayerController controller;
    public Transform orientation;
    public Transform player;
    public Transform combatLookAt;
    public CinemachineFreeLook basicCam;
    //public CinemachineFreeLook combatCam;
    public Rigidbody rb;
    public Vector3 inputDir;
    
    public cameraStyle currentCamera;
    // list of variables
    public enum cameraStyle { Explore, Combat }

    // variables
    public float rotationSpeed;


    // Start is called before the first frame update
    void Start()
    {
        currentCamera = cameraStyle.Explore;
        controller = GameObject.Find("Player").gameObject.GetComponent<PlayerController>();
        rb = GameObject.Find("Player").gameObject.GetComponent<Rigidbody>();
        combatLookAt = GameObject.Find("LookAt").gameObject.transform;
        orientation = GameObject.Find("Orientation").gameObject.transform;
        player = GameObject.Find("Player").gameObject.transform;
    }

    private void FixedUpdate()
    {
        if (!controller.isGameOver)
        {
            //combatCam.gameObject.SetActive(true);
            currentCamera = cameraStyle.Explore;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            // Explore camera setup
            if (currentCamera == cameraStyle.Explore)
            {
                // Calculate Vector between player and Camera
                Vector3 viewDir = player.position - new Vector3(transform.position.x, player.position.y, transform.position.z);
                orientation.forward = viewDir.normalized;

                //rotate player
                float horizontalInput = Input.GetAxis("Horizontal");
                float verticalInput = Input.GetAxis("Vertical");
                Vector3 inputDir = orientation.forward * verticalInput + orientation.right * horizontalInput;
                // If there is input rotate smoothly
                if (inputDir != Vector3.zero)
                    player.forward = Vector3.Slerp(player.forward, inputDir.normalized, Time.deltaTime * rotationSpeed);
            }

            // Combat Camera setup
            else if (currentCamera == cameraStyle.Combat)
            {
                Vector3 dirtoLookAt = player.position - new Vector3(transform.position.x, player.position.y, transform.position.z);
                orientation.forward = dirtoLookAt.normalized;
                player.forward = dirtoLookAt.normalized;

            }
         //   switchCamera();
        }
        else if (controller.isGameOver)
        {
           // combatCam.gameObject.SetActive(false);
            basicCam.gameObject.SetActive(false);
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
    }
    // Switching between the two cameras
    //void switchCamera()
    //{
    //    if(Input.GetKey(KeyCode.V))
    //    {
    //        if(currentCamera == cameraStyle.Explore)
    //        {
    //            basicCam.gameObject.SetActive(false);
    //            //combatCam.gameObject.SetActive(true);
    //            currentCamera = cameraStyle.Combat;

    //        }
    //        else if (currentCamera == cameraStyle.Combat)
    //        {
    //            basicCam.gameObject.SetActive(true);
    //           // combatCam.gameObject.SetActive(false);
    //            currentCamera = cameraStyle.Explore;
    //        }
    //    }
    //}
}
