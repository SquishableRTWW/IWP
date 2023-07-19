using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class prepTT : MonoBehaviour
{
    public string message;

    private void OnMouseEnter()
    {
        PrepTTManager._instance.SetAndShowToolTip(message);
    }

    private void OnMouseExit()
    {
        PrepTTManager._instance.HideToolTip();
    }

    public void ShowItemToolTip(string msg)
    {
        message = msg;
        PrepTTManager._instance.SetAndShowToolTip(message);
    }
    public void HideItemToolTip()
    {
        PrepTTManager._instance.HideToolTip();
    }
}
