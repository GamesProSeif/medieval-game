using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using System.Timers;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public Animator anim;
    public Animator anim2;
    public GameObject Fade;
    private bool startButtonClicked;
    private float elapsedTime = 0;
    // private AudioSource song;

    private void Awake()
    {
        // FindObjectOfType<AudioManager>().Play("MainMenuMusic");
    }

    public void StartGame()
    {
        startButtonClicked = true;
        anim.SetBool("gameStart", true);
        Fade.SetActive(true);
        // song = GameObject.Find("Main Camera").GetComponent<AudioSource>();
    }

    private void Update()
    {
        if(startButtonClicked)
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime > 6)
            {
                startButtonClicked=false;
                SceneManager.LoadScene("level 1");
            }
            else if (elapsedTime > 3 && elapsedTime < 5)
            {
                anim2.SetBool("fadeOut", true);
                // song.volume -= Time.deltaTime * 0.4f;
            }
        }  
       
    }
    public void ExitGame()
    {
        Application.Quit();
    }
}
