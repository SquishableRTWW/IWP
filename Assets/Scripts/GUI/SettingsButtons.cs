using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsButtons : MonoBehaviour
{
    [SerializeField] bool toggleMusic, toggleEffects, on;
    [SerializeField] TextMeshProUGUI buttonText;


    // Update is called once per frame
    public void Toggle()
    {
        SoundManager.Instance.PlaySound(SoundManager.Sound.ButtonClick);
        if (toggleEffects)
        {
            SoundManager.Instance.ToggleEffects();
        }
        else if (toggleMusic)
        {
            SoundManager.Instance.ToggleMusic();
        }
    }

    public void ChangeText(string buttonName)
    {
        on = !on;
        if (on)
        {
            buttonText.text = buttonName + ":\n" + "ON";
        }
        else
        {
            buttonText.text = buttonName + ":\n" + "OFF";
        }
    }
}
