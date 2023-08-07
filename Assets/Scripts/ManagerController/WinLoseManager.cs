using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class WinLoseManager : MonoBehaviour
{
    public static WinLoseManager Instance;
    public TextMeshProUGUI winLoseText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            winLoseText.text = StateNameController.gameResult;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ReturnToMain()
    {
        SoundManager.Instance.PlaySound(SoundManager.Sound.ButtonClick);
        SceneManager.LoadScene("MainMenu");
    }
    public void RetryGame()
    {
        SoundManager.Instance.PlaySound(SoundManager.Sound.ButtonClick);
        SceneManager.LoadScene("Game");
    }
}
