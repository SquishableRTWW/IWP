using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolTip : MonoBehaviour
{
    public string message;

    private void OnMouseEnter()
    {
        

        ToolTipManager._instance.SetAndShowToolTip(message);
    }

    private void OnMouseExit()
    {
        ToolTipManager._instance.HideToolTip();
    }


    // Attack button tool tip functions
    // NOTE: For attack buttons, message logic is found in the mouseController
    public void ShowAttackButtonToolTip()
    {
        ToolTipManager._instance.SetAndShowToolTip(message);
    }
    public void HideAttackButtonToolTip()
    {
        ToolTipManager._instance.HideToolTip();
    }
}
