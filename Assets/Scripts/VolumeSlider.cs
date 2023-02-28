using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    void Start()
    {
        var slider = GetComponent<Slider>();
        var audioManager = FindObjectOfType<AudioManager>();

        slider.value = audioManager.masterVolume;

        slider.onValueChanged.AddListener((v) =>
        {
            audioManager.masterVolume = v;
        });
    }

    void Update()
    {
        
    }
}
