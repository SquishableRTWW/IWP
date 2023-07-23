using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CPPipBehaviour : MonoBehaviour
{
    public bool fadeIn;
    public bool fadeOut;

    public bool onFade;

    // Start is called before the first frame update
    void Start()
    {
        fadeOut = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (onFade)
        {
            if (fadeOut)
            {
                Color CPColor = GetComponent<Image>().color;
                float fadeAmount = CPColor.a - (4f * Time.deltaTime);

                CPColor = new Color(CPColor.r, CPColor.g, CPColor.b, fadeAmount);
                GetComponent<Image>().color = CPColor;

                if (CPColor.a <= 0)
                {
                    fadeOut = false;
                    fadeIn = true;
                }
            }
            if (fadeIn)
            {
                Color CPColor = GetComponent<Image>().color;
                float fadeAmount = CPColor.a + (4f * Time.deltaTime);

                CPColor = new Color(CPColor.r, CPColor.g, CPColor.b, fadeAmount);
                GetComponent<Image>().color = CPColor;

                if (CPColor.a >= 1)
                {
                    fadeIn = false;
                    fadeOut = true;
                }
            }
        }
    }
}
