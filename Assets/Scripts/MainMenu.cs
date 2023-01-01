using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using System.Timers;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public Animator anim;
    private bool startButtonClicked;
    private float elapsedTime = 0;
    public void StartGame()
    {
        startButtonClicked = true;
        anim.SetBool("gameStart", true);
    }

    private void Update()
    {
        if(startButtonClicked)
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime > 3)
            {
                startButtonClicked=false;
                SceneManager.LoadScene("level 1");
            }
        }
       
       
    }
    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("QUITT!");
    }
}
