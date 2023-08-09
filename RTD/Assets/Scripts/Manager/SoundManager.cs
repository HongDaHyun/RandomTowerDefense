using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        if (SceneManager.GetActiveScene().buildIndex == 0)
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
        AudioSource findSfx = Array.Find(sfxPlayer, sfx => !sfx.isPlaying);

        if (findSfx != null)
            findSfx.PlayOneShot(Array.Find(sfx, sfx => sfx.name == p_sfxName).clip);
        else
            return;

    }
}