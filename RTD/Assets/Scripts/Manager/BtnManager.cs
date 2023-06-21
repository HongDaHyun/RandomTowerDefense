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

    public void ClickSound()
    {
        SoundManager.Instance.PlaySFX("ClickBtn");
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

        LevelDifficult(gm.level);

        AdMobManager.Instance.ToggleBannerAd(false);
        SceneManager.LoadScene(1);
    } // 레벨 설정에 따라

    public void AgainStart()
    {
        LevelDifficult(GameManager.Instance.level);
    }

    private void LevelDifficult(Level level)
    {
        GameManager gm = GameManager.Instance;

        switch (level)
        {
            case Level.Easy: // 쉬움
                gm.gatchaKey = 3;
                gm.maxHealth = 30;
                gm.maxExp = 5;
                gm.increaseExp = 0.2f;
                gm.maxWave = gm.levelNum[0];
                gm.curHealth = gm.maxHealth;
                break;
            case Level.Normal: // 보통
                gm.gatchaKey = 2;
                gm.maxHealth = 25;
                gm.maxExp = 5;
                gm.increaseExp = 0.3f;
                gm.maxWave = gm.levelNum[1];
                gm.curHealth = gm.maxHealth;
                break;
            case Level.Hard: // 어려움
                gm.gatchaKey = 1;
                gm.maxHealth = 20;
                gm.maxExp = 10;
                gm.increaseExp = 0.4f;
                gm.maxWave = gm.levelNum[2];
                gm.curHealth = gm.maxHealth;
                break;
            case Level.Infinite: // 무한
                gm.gatchaKey = 2;
                gm.maxHealth = 25;
                gm.maxExp = 10;
                gm.increaseExp = 0.4f;
                gm.maxWave = gm.levelNum[3];
                gm.curHealth = gm.maxHealth;
                break;
        }
        gm.gameTime = GameTime.Slow; // 속도 초기화
    }

    public void Title()
    {
        //초기화
        GameManager gm = GameManager.Instance;
        gm.isStart = false;
        gm.gameTime = GameTime.Slow; // 속도
        //경험치
        gm.expLevel = 0;
        gm.maxExp = 1000;
        gm.curExp = 0;

        Play();
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
        UIManager ui = UIManager.Instance;
        GameManager gm = GameManager.Instance;

        ui.gatchaPannel.gatchaBtn.interactable = false;
        ui.gatchaPannel.gatchaTab.interactable = false;

        // 0: Bow, 1: Fighter, 2: Fire, 3: Germ, 4: Gun, 5: Ice, 6: Sword, 7: Thunder 
        int ranType = UnityEngine.Random.Range(0, 7);

        // 0: Common, 1: Rare, 2: Epic, 3: Legend
        int ranTier = UnityEngine.Random.Range(0, gm.probability[0] + gm.probability[1] + gm.probability[2] + gm.probability[3]);
        if (ranTier < gm.probability[0])
            ranTier = 1;
        else if (ranTier < gm.probability[0] + gm.probability[1])
            ranTier = 2;
        else if (ranTier < gm.probability[0] + gm.probability[1] + gm.probability[2])
            ranTier = 3;
        else
            ranTier = 4;

        for (int i = 0; i < (4 * 6 + (ranTier - 1)) * 2; i++) //슬롯 돌리기
        {
            ui.slotObj.transform.localPosition += new Vector3(0, 100f, 0);
            if (ui.slotObj.transform.localPosition.y > 800f)
                ui.slotObj.transform.localPosition -= new Vector3(0, 800f, 0);
            yield return new WaitForSeconds(0.01f * Time.deltaTime);
        }
        yield return new WaitForSeconds(1f);
        ui.slotObj.transform.localPosition = new Vector3(0, 0, 0);
        ui.gatchaPannel.gatchaPannel.SetActive(false);
        ui.gatchaPannel.gatchaBtn.interactable = true;
        ui.gatchaPannel.gatchaTab.interactable = true;
        gm.gatchaKey--;
        SpawnManager.Instance.SpawnUnit((UnitType)ranType, (Tier)ranTier);

        QuestManager.Instance.gatchaNum++; // 퀘스트 9
    }

    public void SlotMachineBtn()
    {
        if (GameManager.Instance.gatchaKey < 1)
            return;
        StartCoroutine(SlotMachine());
    }

    public void ExitInfo()
    {
        UIManager.Instance.info.CloseInfo();
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

        if (thisUnit.unitStat.tier == Tier.Hidden) // 티어가 레전드면 리턴
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

                UIManager.Instance.quests[i].gameObject.SetActive(true);
                UIManager.Instance.quests[i].SetQuest(ranID); //퀘스트 생성
            }
        }
    }
}
