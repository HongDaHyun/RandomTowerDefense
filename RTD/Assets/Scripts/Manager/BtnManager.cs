using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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
        for(int i = 0; i < UIManager.Instance.mainPannel.Length; i++)
        {
            UIManager.Instance.mainPannel[i].SetActive(false);
        }
        for (int i = 0; i < UIManager.Instance.inGamePannel.Length; i++)
        {
            UIManager.Instance.inGamePannel[i].SetActive(true);
        }
        AdMobManager.Instance.ToggleBannerAd(false);
    }
}
