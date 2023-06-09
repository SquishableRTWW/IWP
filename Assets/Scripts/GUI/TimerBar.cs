using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerBar : MonoBehaviour
{
    public Slider slider;
    public Image fill;

    public void SetMaxTime(float time)
    {
        slider.maxValue = time;
        slider.value = time;
    }

    public void SetTime(float time)
    {
        slider.value = time;
    }

    private void Update()
    {

        if (slider.value <= 20)
        {
            fill.GetComponent<Image>().color = new Color(1, 0.67f, 0, 1);
        }
        else if (slider.value <= 10)
        {
            fill.GetComponent<Image>().color = new Color(1, 0, 0, 1);
        }
        else
        {
            fill.GetComponent<Image>().color = new Color(0.26f, 0.95f, 1, 1);
        }
    }
}
