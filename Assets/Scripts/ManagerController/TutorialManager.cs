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
        tutorialText.text = textList[0];
        imageNumber = 0;
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
        ChangeScene("Game");
    }

    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
