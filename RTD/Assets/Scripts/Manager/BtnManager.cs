using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class BtnManager : Singleton<BtnManager>
{
    public void Tab(GameObject obj)
    {
        if (!obj.activeSelf)
            obj.SetActive(true);
        else
            obj.SetActive(false);
    }

    public void GameStart(string _level)
    {
        GameManager.Instance.level = (Level)Enum.Parse(typeof(Level), _level);
        AdMobManager.Instance.ToggleBannerAd(false);
        SceneManager.LoadScene(1);
    }


    public void Title()
    {
        SceneManager.LoadScene(0);
    }
}
