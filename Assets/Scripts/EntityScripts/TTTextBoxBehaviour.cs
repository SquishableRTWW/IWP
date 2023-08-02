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
            if (StateNameController.isInTutorial && Manager.Instance.tutorialNumber == 1)
            {

            }
            else if (StateNameController.isInTutorial && Manager.Instance.tutorialNumber < 7)
            {
                SoundManager.Instance.PlaySound(SoundManager.Sound.TutorialClick);
                Manager.Instance.tutorialNumber++;
                Manager.Instance.SpawnNextTutorial(Manager.Instance.tutorialNumber);
                Destroy(gameObject);
            }
            else if (StateNameController.isInTutorial && Manager.Instance.tutorialNumber == 7)
            {
                if (Manager.Instance.mouseController.character != null)
                {
                    SoundManager.Instance.PlaySound(SoundManager.Sound.TutorialClick);
                    Manager.Instance.tutorialNumber++;
                    Manager.Instance.SpawnNextTutorial(Manager.Instance.tutorialNumber);
                    Destroy(gameObject);
                }
            }
            else if (StateNameController.isInTutorial && Manager.Instance.tutorialNumber == 8)
            {

            }
            else if (Manager.Instance.tutorialNumber > 8)
            {
                if (!Manager.Instance.prepCanvas.gameObject.activeInHierarchy)
                {
                    SoundManager.Instance.PlaySound(SoundManager.Sound.TutorialClick);
                    Manager.Instance.tutorialNumber++;
                    Manager.Instance.SpawnNextTutorial(Manager.Instance.tutorialNumber);
                    Destroy(gameObject);
                }
            }
            else if (StateNameController.isInTutorial && Manager.Instance.tutorialNumber == 11)
            {

            }
            else if (Manager.Instance.tutorialNumber > 11)
            {
                if (!Manager.Instance.prepCanvas.gameObject.activeInHierarchy)
                {
                    SoundManager.Instance.PlaySound(SoundManager.Sound.TutorialClick);
                    Manager.Instance.tutorialNumber++;
                    Manager.Instance.SpawnNextTutorial(Manager.Instance.tutorialNumber);
                    Destroy(gameObject);
                }
            }
        }
        //else if (Input.GetMouseButtonDown(1) && Manager.Instance.playerTurn)
        //{
        //    Manager.Instance.isInTutorial = false;

        //    Destroy(gameObject);
        //}
    }
}
