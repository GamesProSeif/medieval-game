using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    public bool moveLight = true;
    public float pulseSpeed = 0.1f;
    public float pulseMinimum = 0.8f;
    public float pulseMaximum = 1.2f;

    private Light lightSource;
    private Vector3 origin;
    private Vector3 target = Vector3.zero;
    private float time = 0.5f;
    private float timer = 0;
    // Start is called before the first frame update
    void Start()
    {
        origin = transform.position;
        lightSource = GetComponent<Light>();
    }

    // Update is called once per frame
    void Update()
    {
        float scale = pulseMaximum + Mathf.PingPong(Time.time * pulseSpeed, pulseMaximum - pulseMinimum);
        lightSource.intensity = scale;

        if(moveLight)
        {
            timer -= Time.deltaTime;
            if(timer < 0)
            {
                timer = time;
                target = origin + Random.insideUnitSphere * 0.03f;
            }
            transform.position = Vector3.Lerp(transform.position, target, 0.01f);
        }
    }
}
