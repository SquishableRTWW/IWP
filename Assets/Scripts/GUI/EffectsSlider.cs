using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EffectsSlider : MonoBehaviour
{
    public Slider effectSlider;

    // Start is called before the first frame update
    void Start()
    {
        SoundManager.Instance.ChangeEffectsVolume(effectSlider.value);
        effectSlider.onValueChanged.AddListener(val => SoundManager.Instance.ChangeEffectsVolume(val));
    }
}
