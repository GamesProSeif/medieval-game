using Microsoft.Win32;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DoorScript : MonoBehaviour
{   
    private Animator anim;
    private float timeElapsed;
    private SelectionManger selection;
    private TextMeshProUGUI openDoorText;
    private GameObject canvas;

    private void Awake()
    {
        canvas = GameObject.Find("Canvas");
        openDoorText = canvas.transform.Find("pickupOpenDoor").gameObject.GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        anim = GetComponent<Animator>();
        selection = GameObject.Find("SelectionManager").GetComponent<SelectionManger>();
        openDoorText.gameObject.SetActive(false);
    }

    void Update()
    {
        openDoor();
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
        if(selection.mainSelection != null && selection.mainSelection.gameObject.tag == "Door")
        {
            openDoorText.gameObject.GetComponent<TextMeshProUGUI>().text = "Press E to Open Door";
            openDoorText.gameObject.SetActive(true);
        }
    }



    private void openDoor()
    {
        
        if (selection.mainSelection != null && selection.mainSelection.gameObject.tag == "Door")
        {

            if (Input.GetKey(KeyCode.E))
                anim.SetBool("isOpened", true);
        }
        else
            openDoorText.gameObject.SetActive(false);
    }
}
