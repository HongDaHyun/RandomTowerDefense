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

    public void Pause() // 멈춤
    {
        Time.timeScale = 0;
    }

    public void Play() // 재생
    {
        Time.timeScale = (int)GameManager.Instance.gameTime / 10f;
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
                gm.maxExp = 3;
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
                gm.maxExp = 3;
                gm.maxWave = int.Parse(UIManager.Instance.levelWaveTxt[2].text);
                gm.curHealth = gm.maxHealth;
                break;
            case Level.Infinite:
                gm.gatchaKey = 2;
                gm.maxHealth = 25;
                gm.maxExp = 3;
                gm.maxWave = 40;
                gm.curHealth = gm.maxHealth;
                break;
        }
        GameManager.Instance.gameTime = GameTime.Slow; // 속도 초기화
        AdMobManager.Instance.ToggleBannerAd(false);
        SceneManager.LoadScene(1);
    } // 레벨 설정에 따라

    public void Title()
    {
        GameManager.Instance.isStart = false;
        GameManager.Instance.gameTime = GameTime.Slow; // 속도 초기화
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
        int ranType = UnityEngine.Random.Range(0, 7); // 0: Bow, 1: Fighter ...
        int ranTier = UnityEngine.Random.Range(0, 100); // 0: Common, 1: Rare, 2: Epic, 3: Legend
        if (ranTier >= 99)
            ranTier = 3;
        else if (ranTier >= 95)
            ranTier = 2;
        else if (ranTier >= 85)
            ranTier = 1;
        else
            ranTier = 0;

        for (int i = 0; i < (4 * 6 + ranTier) * 2; i++) //슬롯 돌리기
        {
            UIManager.Instance.slotObj.transform.localPosition += new Vector3(0, 100f, 0);
            if (UIManager.Instance.slotObj.transform.localPosition.y > 800f)
                UIManager.Instance.slotObj.transform.localPosition -= new Vector3(0, 800f, 0);
            yield return new WaitForSeconds(0.01f * Time.deltaTime);
        }
        yield return new WaitForSeconds(1f);
        UIManager.Instance.slotObj.transform.localPosition = new Vector3(0, 0, 0);
        UIManager.Instance.gatchaPannel.gatchaPannel.SetActive(false);
        UIManager.Instance.gatchaPannel.gatchaBtn.interactable = true;
        UIManager.Instance.gatchaPannel.gatchaTab.interactable = true;
        GameManager.Instance.gatchaKey--;
        SpawnManager.Instance.SpawnUnit((UnitType)ranType, (Tier)ranTier);

        QuestManager.Instance.gatchaNum++; // 퀘스트 9
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
            case GameTime.Slow:
                GameManager.Instance.gameTime = GameTime.Commonly;
                break;
            case GameTime.Commonly:
                GameManager.Instance.gameTime = GameTime.Fast;
                break;
            case GameTime.Fast:
                GameManager.Instance.gameTime = GameTime.Slow;
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
                QuestManager.Instance.evolNum++; // 퀘스트 23
                return;
            }
        }
    }

    public void InitQuest()
    {
        int ranID;
        List<int> IDs = new List<int>(); //중복 방지 리스트
        for (int i = 0; i < UIManager.Instance.quests.Length; i++)
        {
            ranID = UnityEngine.Random.Range(0, CSVReader.Instance.csvList.questStat.Length);
            if (IDs.Contains(ranID)) // 중복이면 다시
                i--;
            else
            {
                IDs.Add(ranID);
                UIManager.Instance.quests[i].SetQuest(ranID); //퀘스트 생성
            }
        }
        UIManager.Instance.quests[0].SetQuest(10); //퀘스트 생성
    }
}
