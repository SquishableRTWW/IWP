using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TTTextBoxBehaviour : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        //GetComponentInChildren<TextMeshProUGUI>().text = Manager.Instance.ttTextList[Manager.Instance.tutorialNumber];
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && Manager.Instance.playerTurn)
        {
            if (Manager.Instance.isInTutorial)
            {
                Manager.Instance.tutorialNumber++;
                Manager.Instance.SpawnNextTutorial(Manager.Instance.tutorialNumber);
            }
            Destroy(gameObject);
        }
    }
}
