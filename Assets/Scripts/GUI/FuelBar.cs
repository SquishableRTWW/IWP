using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FuelBar : MonoBehaviour
{
    public Slider slider;
    public Image fill;

    public void SetMaxFuel(int fuel)
    {
        slider.maxValue = fuel;
        slider.value = fuel;
    }

    public void SetFuel(int fuel)
    {
        slider.value = fuel;
    }

    public void SetBarLimit(int amount)
    {
        slider.maxValue = amount;
    }
}
