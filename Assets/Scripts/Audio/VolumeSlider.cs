using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    [SerializeField] private Slider _slider;
    void Start()
    {  
        AudioManager.Instance.ChangeMasterVolume(_slider.value);
        _slider.onValueChanged.AddListener(val => AudioManager.Instance.ChangeMasterVolume(val));     
    }
}
