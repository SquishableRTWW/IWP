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

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
    public void StartGame()
    {
        SoundManager.Instance.PlaySound(SoundManager.Sound.ButtonClick);
        StateNameController.isInTutorial = true;
        ChangeScene("Game");
    }
    public void StartGameNoTutorial()
    {
        SoundManager.Instance.PlaySound(SoundManager.Sound.ButtonClick);
        StateNameController.isInTutorial = false;
        ChangeScene("Game");
    }

    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
