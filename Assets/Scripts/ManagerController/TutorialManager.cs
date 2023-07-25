using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    public List<Sprite> tutorialImageList;
    public List<string> textList;
    public int imageNumber;

    // GUI Components
    public TextMeshProUGUI tutorialText;
    public Image tutorialImage;
    public Button nextButton;
    public Button previousButton;
    public Button startButton;

    // Start is called before the first frame update
    void Start()
    {
        tutorialImage.sprite = tutorialImageList[0];
        imageNumber = 0;

        textList = new List<string>();
        textList.Add("Prepare your mechs with weapons, equipment and fuel before a level. Fuel is NEEDED to move, be sure to assign it and assign it wisely.");
        textList.Add("Left click to select your characters.\n Move your characters to engage the enemy. Moving your mech too far will OVERHEAT it, leaving it useless for the rest of the turn.\n" +
            " Right click and hold to scroll around.");
        textList.Add("Defeat all enemy units to move onto the next level by using your weapons' attacks.\n Commanding mechs to attack takes command points shown on the top of the screen.");
        textList.Add("Get new items and mechs between each level during events. Read carefully what each event could give you.");

        tutorialText.text = textList[0];
    }

    // Update is called once per frame
    void Update()
    {
        tutorialImage.sprite = tutorialImageList[imageNumber];
        tutorialText.text = textList[imageNumber];

        if (imageNumber >= tutorialImageList.Count - 1)
        {
            nextButton.gameObject.SetActive(false);
            startButton.gameObject.SetActive(true);
        }
        else
        {
            nextButton.gameObject.SetActive(true);
            startButton.gameObject.SetActive(false);
        }

        if (imageNumber <= 0)
        {
            previousButton.gameObject.SetActive(false);
        }
        else
        {
            previousButton.gameObject.SetActive(true);
        }
    }

    public void NextImage()
    {
        imageNumber++;
    }
    public void PreviousImage()
    {
        imageNumber--;
    }
    
    public void StartGame()
    {
        Manager.Instance.isInTutorial = true;
        ChangeScene("Game");
    }

    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
