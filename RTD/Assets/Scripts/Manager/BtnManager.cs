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
        switch(GameManager.Instance.level)
        {
            case Level.Easy:
                GameManager.Instance.gatchaKey = 3;
                GameManager.Instance.maxHealth = 30;
                GameManager.Instance.curHealth = GameManager.Instance.maxHealth;
                break;
            case Level.Normal:
                GameManager.Instance.gatchaKey = 2;
                GameManager.Instance.maxHealth = 25;
                GameManager.Instance.curHealth = GameManager.Instance.maxHealth;
                break;
            case Level.Hard:
                GameManager.Instance.gatchaKey = 1;
                GameManager.Instance.maxHealth = 20;
                GameManager.Instance.curHealth = GameManager.Instance.maxHealth;
                break;
            case Level.Infinite:
                GameManager.Instance.gatchaKey = 2;
                GameManager.Instance.maxHealth = 25;
                GameManager.Instance.curHealth = GameManager.Instance.maxHealth;
                break;
        }
        AdMobManager.Instance.ToggleBannerAd(false);
        SceneManager.LoadScene(1);
    }

    public void Title()
    {
        SceneManager.LoadScene(0);
    }

    public void TabSlotMachine()
    {
        //Only Open
        if (!UIManager.Instance.clickTile.activeSelf || UIManager.Instance.clickTile.GetComponentInParent<TileGround>().isBuild) // 클릭타일 활성화 x -> 가챠패널 열리지 않음
            return;
        UIManager.Instance.gatchaPannel.SetActive(true);
    }

    IEnumerator SlotMachine()
    {
        int ran = UnityEngine.Random.Range(0, 4); // 0: Common, 1: Rare, 2: Epic, 3: Legend

        for (int i = 0; i < (4 * 6 + ran) * 4; i++)
        {
            UIManager.Instance.slotObj.transform.localPosition += new Vector3(0, 50f, 0);
            if (UIManager.Instance.slotObj.transform.localPosition.y > 800f)
                UIManager.Instance.slotObj.transform.localPosition -= new Vector3(0, 800f, 0);
            yield return new WaitForSeconds(0.01f);
        }
        yield return new WaitForSeconds(1f);
        UIManager.Instance.slotObj.transform.localPosition = new Vector3(0, 0, 0);
        UIManager.Instance.gatchaPannel.SetActive(false);
        GameManager.Instance.gatchaKey--;
        SpawnManager.Instance.SpawnUnit();
    }

    public void SlotMachineBtn()
    {
        if (GameManager.Instance.gatchaKey < 1)
            return;
        StartCoroutine(SlotMachine());
    }
}
