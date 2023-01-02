using Microsoft.Win32;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

public class DoorScript : MonoBehaviour
{   
    private Animator anim;
    private float timeElapsed;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (anim.GetBool("isOpened"))
        {
            if (timeElapsed < 3)
                timeElapsed += Time.deltaTime;
            else
            {

                anim.SetBool("isOpened", false);
                timeElapsed = 0;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        anim.SetBool("isOpened", true);

      
    }

}
