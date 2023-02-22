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
        GameManager gm = GameManager.Instance;
        gm.isStart = true;
        gm.level = (Level)Enum.Parse(typeof(Level), _level);
        switch(gm.level)
        {
            case Level.Easy:
                gm.gatchaKey = 3;
                gm.maxHealth = 30;
                gm.maxWave = int.Parse(UIManager.Instance.levelWaveTxt[0].text);
                gm.curHealth = gm.maxHealth;
                break;
            case Level.Normal:
                gm.gatchaKey = 2;
                gm.maxHealth = 25;
                gm.maxWave = int.Parse(UIManager.Instance.levelWaveTxt[1].text);
                gm.curHealth = gm.maxHealth;
                break;
            case Level.Hard:
                gm.gatchaKey = 1;
                gm.maxHealth = 20;
                gm.maxWave = int.Parse(UIManager.Instance.levelWaveTxt[2].text);
                gm.curHealth = gm.maxHealth;
                break;
            case Level.Infinite:
                gm.gatchaKey = 2;
                gm.maxHealth = 25;
                gm.maxWave = 40;
                gm.curHealth = gm.maxHealth;
                break;
        }
        AdMobManager.Instance.ToggleBannerAd(false);
        SceneManager.LoadScene(1);
    }

    public void Title()
    {
        GameManager.Instance.isStart = false;
        GameManager.Instance.gameTime = TimeScale.Slow;
        SceneManager.LoadScene(0);
    }

    public void TabSlotMachine()
    {
        //Only Open
        if (!UIManager.Instance.clickTile.activeSelf || UIManager.Instance.clickTile.GetComponentInParent<TileGround>().isBuild) // 클릭타일 활성화 x -> 가챠패널 열리지 않음
            return;
        UIManager.Instance.gatchaPannel.gatchaPannel.SetActive(true);
    }

    IEnumerator SlotMachine()
    {
        UIManager.Instance.gatchaPannel.gatchaBtn.interactable = false;
        UIManager.Instance.gatchaPannel.gatchaTab.interactable = false;
        //int ranType = UnityEngine.Random.Range(0, 8); // 0: Bow, 1: Fighter...
        int ranType = 0; // 활밖에 없기에 임시방편
        int ranTier = UnityEngine.Random.Range(0, 4); // 0: Common, 1: Rare, 2: Epic, 3: Legend

        for (int i = 0; i < (4 * 6 + ranTier) * 4; i++) //슬롯 돌리기
        {
            UIManager.Instance.slotObj.transform.localPosition += new Vector3(0, 50f, 0);
            if (UIManager.Instance.slotObj.transform.localPosition.y > 800f)
                UIManager.Instance.slotObj.transform.localPosition -= new Vector3(0, 800f, 0);
            yield return new WaitForSeconds(0.01f);
        }
        yield return new WaitForSeconds(1f);
        UIManager.Instance.slotObj.transform.localPosition = new Vector3(0, 0, 0);
        UIManager.Instance.gatchaPannel.gatchaPannel.SetActive(false);
        UIManager.Instance.gatchaPannel.gatchaBtn.interactable = true;
        UIManager.Instance.gatchaPannel.gatchaTab.interactable = true;
        GameManager.Instance.gatchaKey--;
        SpawnManager.Instance.SpawnUnit((UnitType)ranType, (Tier)ranTier);
    }

    public void SlotMachineBtn()
    {
        if (GameManager.Instance.gatchaKey < 1)
            return;
        StartCoroutine(SlotMachine());
    }

    public void SpeedBtn()
    {
        switch(GameManager.Instance.gameTime)
        {
            case TimeScale.Slow:
                GameManager.Instance.gameTime = TimeScale.Commonly;
                break;
            case TimeScale.Commonly:
                GameManager.Instance.gameTime = TimeScale.Fast;
                break;
            case TimeScale.Fast:
                GameManager.Instance.gameTime = TimeScale.Slow;
                break;
        }
        float curTime = (int)GameManager.Instance.gameTime / 10f;
        Time.timeScale = curTime;
        UIManager.Instance.gameSpeedTxt.text = "x " + curTime.ToString("F1");

    }

    public void UpgradeUnit()
    {
        List<Unit> unitList = SpawnManager.Instance.unitList;
        TileGround tileGround = UIManager.Instance.clickTile.GetComponentInParent<TileGround>();
        Unit thisUnit = tileGround.GetComponentInChildren<Unit>();

        if (thisUnit.unitStat.tier == Tier.Legend) // 티어가 레전드면 리턴
            return;

        for (int i = 0; i < unitList.Count; i++)
        {
            if(unitList[i].unitStat.unitName == thisUnit.unitStat.unitName && unitList[i] != thisUnit)
            {
                SpawnManager.Instance.DeSpawnUnit(unitList[i]); //순서 중요 (리스트 Remove때문)
                SpawnManager.Instance.DeSpawnUnit(thisUnit); //순서 바꾸지 말 것
                SpawnManager.Instance.SpawnUnit(thisUnit.unitStat.unitType, thisUnit.unitStat.tier + 1);
                return;
            }
        }
    }
}
