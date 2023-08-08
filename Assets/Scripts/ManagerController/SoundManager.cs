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

    private static Dictionary<Sound, float> soundTimerDictionary;

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

        soundTimerDictionary = new Dictionary<Sound, float>();
        soundTimerDictionary[Sound.CharacterMove1] = 0f;
        soundTimerDictionary[Sound.CharacterMove2] = 0f;
        soundTimerDictionary[Sound.DieSFX] = 0f;
    }

    public void PlaySound(Sound sound)
    {
        if (CanPlaySound(sound))
        {
            effectSource.PlayOneShot(soundList[(int)sound]);
        }
    }
    public void StopSound()
    {
        effectSource.Stop();
    }

    private static bool CanPlaySound(Sound sound)
    {
        switch (sound)
        {
            default:
                return true;
            case Sound.CharacterMove1:
                if (soundTimerDictionary.ContainsKey(sound))
                {
                    float lastTimePlayed = soundTimerDictionary[sound];
                    float playerMoveTimerMax = 8f;
                    if (lastTimePlayed + playerMoveTimerMax < Time.time)
                    {
                        soundTimerDictionary[sound] = Time.time;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return true;
                }
            case Sound.CharacterMove2:
                if (soundTimerDictionary.ContainsKey(sound))
                {
                    float lastTimePlayed = soundTimerDictionary[sound];
                    float playerMoveTimerMax = 0.5f;
                    if (lastTimePlayed + playerMoveTimerMax < Time.time)
                    {
                        soundTimerDictionary[sound] = Time.time;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return true;
                }
            case Sound.DieSFX:
                if (soundTimerDictionary.ContainsKey(sound))
                {
                    float lastTimePlayed = soundTimerDictionary[sound];
                    float playerMoveTimerMax = 2f;
                    if (lastTimePlayed + playerMoveTimerMax < Time.time)
                    {
                        soundTimerDictionary[sound] = Time.time;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return true;
                }
                //break;
        }
    }

    public void ChangeMusic(Sound sound)
    {
        musicSource.Stop();
        musicSource.PlayOneShot(soundList[(int)sound]);
        musicSource.loop = true;
    }

    public void ChangeMusicVolume(float value)
    {
        musicSource.volume = value;
    }
    public void ChangeEffectsVolume(float value)
    {
        effectSource.volume = value;
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
