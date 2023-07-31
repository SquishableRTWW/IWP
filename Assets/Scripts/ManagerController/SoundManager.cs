using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SoundManager
{

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
    }


    public static void PlaySound(Sound sound)
    {
        GameObject soundGameObject = new GameObject("Sound");
        AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
        audioSource.PlayOneShot(Manager.Instance.soundList[(int)sound]);
    }
}
