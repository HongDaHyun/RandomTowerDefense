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
        GameManager.Instance.SetGM();

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

        // UI 설정
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

        Tier tier = (Tier)ranTier;
        UnitType type = (UnitType)ranType;

        if (!QuestManager.Instance.unitTypesList.Contains(type))
            QuestManager.Instance.unitTypesList.Add(type); // 퀘스트 6

        SpawnManager.Instance.SpawnUnit(type, tier);

        QuestManager.Instance.gatchaNum++; // 퀘스트 9
    }

    public void SlotMachineBtn()
    {
        if (GameManager.Instance.gatchaKey < 1)
            return;
        SoundManager.Instance.PlaySFX("SlotMachine");
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
        SpawnManager sm = SpawnManager.Instance;
        TileGround tileGround = UIManager.Instance.clickTile.GetComponentInParent<TileGround>();
        Unit thisUnit = tileGround.GetComponentInChildren<Unit>();

        if (thisUnit.unitStat.tier == Tier.Legend) // 티어가 레전드면 리턴
            return;

        Unit otherUnit = sm.FindUnit(thisUnit);
        
        if(otherUnit != null)
        {
            SoundManager.Instance.PlaySFX("UpgradeUnit"); // 사운드

            sm.DeSpawnUnit(otherUnit); // 순서 중요
            sm.DeSpawnUnit(thisUnit); // 순서 바꾸지 마셈

            sm.SpawnVFX("UpgradeVFX", thisUnit.transform);

            int ranType = UnityEngine.Random.Range(0, 7);

            sm.SpawnUnit((UnitType)ranType, thisUnit.unitStat.tier + 1); // 업그레이드
            QuestManager.Instance.evolNum++; // 퀘스트 23
        }
    }

    public void RefreshQuestBtn()
    {
        UIManager um = UIManager.Instance;

        int ranID;
        List<int> IDs = new List<int>(); //중복 방지 리스트

        int length = um.questPannel.quests.Length;
        for (int i = 0; i < length; i++)
        {
            ranID = UnityEngine.Random.Range(0, CSVReader.Instance.csvList.questStat.Length);
            if (IDs.Contains(ranID)) // 중복이면 다시
                i--;
            else
            {
                IDs.Add(ranID);

                um.questPannel.quests[i].gameObject.SetActive(true);
                um.questPannel.quests[i].SetQuest(ranID); //퀘스트 생성
            }
        }

        um.questPannel.curCool = 0;
        QuestCool();
    }

    public void QuestCool()
    {
        UIManager um = UIManager.Instance;

        if (um.questPannel.curCool < um.questPannel.maxCool)
            um.questPannel.refreshBtn.interactable = false;
        else
            um.questPannel.refreshBtn.interactable = true;

        um.questPannel.coolTxt.text = string.Format("{0}/{1}", um.questPannel.curCool, um.questPannel.maxCool);
    }

    public void LevelUpBtn()
    {
        UIManager um = UIManager.Instance;

        GameManager.Instance.levelPoint--;

        um.characteristicPannel.SetActive(true); // 특성 패널 활성화
        um.InitCard(); // 카드 생성

        um.levelUpBtn.SetUI(); // UI 설정
    }
}
