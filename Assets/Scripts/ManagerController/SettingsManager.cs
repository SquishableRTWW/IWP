using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance;
    public Canvas settingsCanvas;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            DontDestroyOnLoad(settingsCanvas);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            settingsCanvas.gameObject.SetActive(!settingsCanvas.gameObject.activeInHierarchy);
        }
    }

    public void ShowSettings()
    {
        settingsCanvas.gameObject.SetActive(true);
    }
    public void HideSettings()
    {
        settingsCanvas.gameObject.SetActive(false);
    }

    public void QuitThisGame()
    {
        SoundManager.Instance.PlaySound(SoundManager.Sound.ButtonClick);
        SoundManager.Instance.ChangeMusic(SoundManager.Sound.CalmBGM);
        SceneManager.LoadScene("MainMenu");
    }
}
