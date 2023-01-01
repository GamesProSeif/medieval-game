using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequenceBegin : MonoBehaviour
{
    public Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    IEnumerator beginning()
    {
        yield return new WaitForSeconds(2);
       
        anim.Play("turningAround");
        yield return new WaitForSeconds(6);
    }
  
}
