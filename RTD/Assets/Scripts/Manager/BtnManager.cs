using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class BtnManager : MonoBehaviour
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
        SceneManager.LoadScene(1);
    }
}
