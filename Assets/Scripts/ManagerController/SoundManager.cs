using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    [SerializeField] AudioSource musicSource, effectSource;
    public List<AudioClip> soundList;
    public enum Sound
    {
        CalmBGM,
        CombatBGM,
        TutorialClick,
        ButtonClick,
        CharacterMove1,
        CharacterMove2,
        SmoothboreSFX,
        BolterSFX,
        GrenadeLauncherSFX,
        LaserSFX,
        DragDropSFX,
        DieSFX
    }


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlaySound(Sound sound)
    {
        effectSource.PlayOneShot(soundList[(int)sound]);
    }

    public void ChangeMasterVolume(float value)
    {
        AudioListener.volume = value;
    }

    public void ToggleEffects()
    {
        effectSource.mute = !effectSource.mute;
    }
    public void ToggleMusic()
    {
        musicSource.mute = !musicSource.mute;
    }
}
