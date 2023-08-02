using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicSlider : MonoBehaviour
{
    public Slider musicSlider;

    // Start is called before the first frame update
    void Start()
    {
        SoundManager.Instance.ChangeMasterVolume(musicSlider.value);
        musicSlider.onValueChanged.AddListener(val => SoundManager.Instance.ChangeMasterVolume(val));
    }
}
