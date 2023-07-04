using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ToolTipManager : MonoBehaviour
{
    public static ToolTipManager _instance;
    public TextMeshProUGUI toolTiptext;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Input.mousePosition;
    }

    public void SetAndShowToolTip(string message)
    {
        gameObject.SetActive(true);
        toolTiptext.text = message;
    }

    public void HideToolTip()
    {
        gameObject.SetActive(false);
        toolTiptext.text = string.Empty;
    }
}
