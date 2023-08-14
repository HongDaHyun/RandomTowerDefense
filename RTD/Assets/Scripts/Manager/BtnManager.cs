using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class BtnManager : Singleton<BtnManager>
{
    public void Tab(GameObject obj)
    {
        if (!obj.activeSelf)
        {
            obj.SetActive(true);
            UIManager.Instance.rayCastPannel.SetActive(true);
            obj.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
            obj.transform.DOScale(new Vector3(1f, 1f, 1f), 0.5f).SetEase(Ease.InExpo).SetEase(Ease.OutBounce).SetUpdate(true);
        }
        else
        {
            UIManager.Instance.rayCastPannel.SetActive(false);
            obj.transform.DOScale(new Vector3(0.05f, 0.05f, 0.05f), 0.25f).SetEase(Ease.InOutExpo).SetUpdate(true).OnComplete(() => obj.SetActive(false));
        }
    }

    public void Tab_NoRayCast(GameObject obj)
    {
        if (!obj.activeSelf)
        {
            obj.SetActive(true);
            obj.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
            obj.transform.DOScale(new Vector3(1f, 1f, 1f), 0.5f).SetEase(Ease.InExpo).SetEase(Ease.OutBounce).SetUpdate(true);
        }
        else
        {
            obj.transform.DOScale(new Vector3(0.05f, 0.05f, 0.05f), 0.25f).SetEase(Ease.InOutExpo).SetUpdate(true).OnComplete(() => obj.SetActive(false));
        }
    }

    public void Pop(GameObject obj)
    {
        if (obj.activeSelf)
            return;
        SoundManager.Instance.PlaySFX("Pop");
        obj.SetActive(true);
        obj.transform.localScale = new Vector3(0f, 0f, 0f);
        Sequence seq = DOTween.Sequence();
        seq.Append(obj.transform.DOScale(new Vector3(1f, 1f, 1f), 0.5f).SetEase(Ease.OutBounce));
        seq.AppendInterval(2f);
        seq.Append(obj.transform.DOScale(new Vector3(0f, 0f, 0f), 0.25f).SetEase(Ease.InBack).OnComplete(() => obj.SetActive(false)));
        seq.Play();
    }

    public void SpawnBubbleBtn(string s) // Help
    {
        SpawnManager.Instance.SpawnTxtBubble(EventSystem.current.currentSelectedGameObject.transform, s);
    }

    public void TabSlotMachine()
    {
        //Only Open
        UIManager um = UIManager.Instance;
        if (!um.clickTile.activeSelf || um.clickTile.GetComponentInParent<TileGround>().isBuild) // 클릭타일 활성화 x -> 가챠패널 열리지 않음
        {
            HelpTxt(um, "먼저 타일을 선택하세요!");
            return;
        }
        Tab(um.gatchaPannel.gatchaPannel);
    }

    public void HelpTxt(UIManager um, string s)
    {
        if (um.helpTxt.gameObject.activeSelf)
            return;

        um.helpTxt.gameObject.SetActive(true);

        um.helpTxt.SetText(s);

        um.PopUp(um.helpTxt, 450f);
    }

    public void TabFrame()
    {
        UIManager um = UIManager.Instance;
        GameManager gm = GameManager.Instance;
        if (gm.is30) // 30프레임이었다면
        {
            um.pausePannel.frameBtn.SetActive(false);
            gm.is30 = false;
        }
        else
        {
            um.pausePannel.frameBtn.SetActive(true);
            gm.is30 = true;
        }
        gm.SetFrame();
        ES3.Save("Is30", gm.is30, gm.saveDirectory); // 세이브 디렉토리
    }

    public void ClickStagePannel()
    {
        UIManager um = UIManager.Instance;

        for (int i = 0; i < um.selectStageBtn.Length; i++)
        {
            um.selectStageBtn[i].interactable = GameManager.Instance.clearData[i].isClear;
        }
    }

    public void ClickLevelPannel()
    {
        GameManager gm = GameManager.Instance;

        for (int i = 0; i < 3; i++)
        {
            UIManager.Instance.selectLevelBtn[i].interactable = gm.clearData[(int)gm.stage].isLevelClear[i];
        }
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

    public void ChangeStage(string _stage)
    {
        GameManager.Instance.stage = (Stage)Enum.Parse(typeof(Stage), _stage);
    }

    public void GameStartFX(string _level)
    {
        GameManager gm = GameManager.Instance;

        if (gm.isSceneLoading)
            return;

        gm.isSceneLoading = true;

        UIManager.Instance.fx.OnTransitionExitEnded += () => GameStartBtn(_level);
        UIManager.Instance.fx.TransitionExit();
    }

    public void GameStartBtn(string _level)
    {
        GameManager.Instance.GameStart((Level)Enum.Parse(typeof(Level), _level));

        Play();
    } // 레벨 설정에 따라

    public void AgainStart()
    {
        GameManager gm = GameManager.Instance;

        AdMobManager.Instance.frontAd.OnAdClosed += (sender, e) => GameStartFX(gm.level.ToString());
        AdMobManager.Instance.ShowFrontAd();
    }

    public void TitleAd()
    {
        AdMobManager.Instance.frontAd.OnAdClosed += (sender, e) => TitleFX();
        AdMobManager.Instance.ShowFrontAd();
    }

    public void TitleFX()
    {
        GameManager gm = GameManager.Instance;

        if (gm.isSceneLoading)
            return;

        gm.isSceneLoading = true;

        UIManager.Instance.fx.OnTransitionExitEnded += Title;
        UIManager.Instance.fx.TransitionExit();
    }

    private void Title()
    {
        SceneManager.sceneLoaded += GameManager.Instance.TitleLoad;
        SceneManager.LoadSceneAsync(0);
    }

    public void SurrenBtn()
    {
        GameManager gm = GameManager.Instance;

        if (gm.isTutorial)
        {
            HelpTxt(UIManager.Instance, "튜토리얼에서는 항복할 수 없습니다.");
            return;
        }

        gm.LoseHealth(gm.maxHealth);
    }

    IEnumerator SlotMachine()
    {
        UIManager ui = UIManager.Instance;
        GameManager gm = GameManager.Instance;

        ui.gatchaPannel.gatchaBtn.interactable = false;
        ui.gatchaPannel.gatchaTab.interactable = false;

        // 0: Bow, 1: Fighter, 2: Fire, 3: Germ, 4: Gun, 5: Ice, 6: Sword, 7: Thunder 
        int ranType = UnityEngine.Random.Range(0, 8);

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

        if (gm.isTutorial)
        {
            ranType = 0;
            ranTier = 1;
        }

        int slotSpeed = gm.is30 ? 12 : 24;

        // UI 설정
        for (int i = 0; i < (slotSpeed + (ranTier - 1)) * 2; i++) //슬롯 돌리기
        {
            ui.slotObj.transform.localPosition += new Vector3(0, 100f, 0);
            if (ui.slotObj.transform.localPosition.y > 800f)
                ui.slotObj.transform.localPosition -= new Vector3(0, 800f, 0);
            yield return new WaitForSeconds(0.005f * Time.deltaTime);
        }
        yield return new WaitForSecondsRealtime(0.25f);
        ui.slotObj.transform.localPosition = new Vector3(0, 0, 0);
        Tab(ui.gatchaPannel.gatchaPannel);
        ui.gatchaPannel.gatchaBtn.interactable = true;
        ui.gatchaPannel.gatchaTab.interactable = true;
        gm.LoseGatcha(1);

        Tier tier = (Tier)ranTier;
        UnitType type = (UnitType)ranType;

        SpawnManager.Instance.SpawnUnit(type, tier);

        QuestManager.Instance.UpQuest(8); // 퀘스트 8
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
        UIManager.Instance.CloseInfoClickTile();
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
        
        if(otherUnit != null) // 업그레이드
        {
            sm.DeSpawnUnit(otherUnit); // 순서 중요
            sm.DeSpawnUnit(thisUnit); // 순서 바꾸지 마셈

            if ((int)thisUnit.unitStat.tier < 3) // Rare이하만
            {
                SoundManager.Instance.PlaySFX("UpgradeUnit"); // 사운드
                sm.SpawnVFX("UpgradeVFX", thisUnit.transform); // Visual
            }

            int ranType = UnityEngine.Random.Range(0, 7);

            sm.SpawnUnit((UnitType)ranType, thisUnit.unitStat.tier + 1); // 한 티어 높은 유닛 생성
            UIManager.Instance.CloseInfoClickTile();
            QuestManager.Instance.UpQuest(22); // 퀘스트 22
        }
    }

    public void RefreshQuestBtn()
    {
        UIManager um = UIManager.Instance;

        int ranID;
        List<int> IDs = new List<int>(); //중복 방지 리스트

        for (int i = 0; i < 3; i++)
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

        // 퀘스트 패널
        um.questPannel.curCool = 0;
        um.questPannel.refreshBtn.interactable = false;

        QuestCool();
    }

    public void QuestCool()
    {
        UIManager.Instance.questPannel.QuestCool();
    }

    public void LevelUpBtn()
    {
        UIManager um = UIManager.Instance;

        GameManager.Instance.levelPoint--;

        um.InitCard(); // 카드 생성

        um.levelUpBtn.SetUI(); // UI 설정
    }

    public void HideBtn()
    {
        if (CharacManager.Instance.isLevelUping)
            return;

        UIManager um = UIManager.Instance;

        um.characCardPannel.btnTxt.text = um.characCardPannel.miniPannel.activeSelf ? ">" : "<";

        Tab_NoRayCast(um.characCardPannel.miniPannel);
    }

    public void ShopSetSelectBtn(int id)
    {
        UIManager um = UIManager.Instance;
        um.mainShopPannel.curSelect = id;
        um.mainShopPannel.UpdateShopUI(CSVReader.Instance.csvList.FindShop(id));
    }

    public void ShopBtn()
    {
        UIManager um = UIManager.Instance;
        CSVList cl = CSVReader.Instance.csvList;

        cl.UpgradeShop(cl.FindShop(um.mainShopPannel.curSelect));
        UIManager.Instance.mainShopPannel.UpdateSelectUI();
    }

    public void UpdateShopPannel()
    {
        UIManager.Instance.mainShopPannel.UpdateCurMoney();
        UIManager.Instance.mainShopPannel.UpdateSelectUI();
    }

    public void ShowLeaderBoard()
    {
        GPGSBinder.Inst.ShowAllLeaderboardUI();
    }

    public void ShowAchieve()
    {
        GPGSBinder.Inst.ShowAchievementUI();
    }

    public void TutoTextId()
    {
        UIManager.Instance.tutorialPannel.textId++;
    }

    public void TutoStart()
    {
        GameManager.Instance.isTutorial = true;
        GameStartFX("Easy");
    }
}
