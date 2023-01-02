using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class starFlicker : MonoBehaviour
{
    
    public float normalIntensity;
    public float flickerIntensity;
    public float intensityPercentage;

    private PostProcessVolume volume;
    private Bloom bloom;
  //  private float elapsedTime;
    
    // Start is called before the first frame update
    void Start()
    {
        volume = GetComponent<PostProcessVolume>();
        volume.profile.TryGetSettings(out bloom);
        bloom.intensity.Override(normalIntensity);
    }

    // Update is called once per frame
    void Update()
    {
       // elapsedTime += Time.deltaTime;
       // if (elapsedTime >= 1)
       // {
            float x = Random.Range(0f, 1f);
           // Debug.Log(x);
            if (x > intensityPercentage)
                bloom.intensity.Override(flickerIntensity);
            else bloom.intensity.Override(normalIntensity);
            //elapsedTime = 0f;
       // }
        
        
    }
    
}
