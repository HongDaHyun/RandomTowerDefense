using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
}

public class SoundManager : Singleton<SoundManager>
{
    [SerializeField] Sound[] sfx = null;
    [SerializeField] Sound[] bgm = null;

    [SerializeField] AudioSource bgmPlayer = null;
    [SerializeField] AudioSource[] sfxPlayer = null;

    private void Start()
    {
        PlayBGM("Lobby");
    }

    public void PlayBGM(string p_bgmName)
    {
        bgmPlayer.clip = Array.Find(bgm, bgm => bgm.name == p_bgmName).clip;
        bgmPlayer.Play();
    }

    public void StopBGM()
    {
        bgmPlayer.Stop();
    }

    public void PlaySFX(string p_sfxName)
    {
        int length = sfxPlayer.Length;
        for (int i = 0; i < length; i++)
        {
            if(!sfxPlayer[i].isPlaying)
            {
                sfxPlayer[i].clip = Array.Find(sfx, sfx => sfx.name == p_sfxName).clip;
                sfxPlayer[i].Play();
            }    
        }
    }
}