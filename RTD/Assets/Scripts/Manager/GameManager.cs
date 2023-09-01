using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    public Level level; public Stage stage;
    public GameTime gameTime = GameTime.Slow;
    public bool isStart, is30, isTutorial, isSceneLoading;
    public ClearData[] clearData;
    public int gatchaKey, maxHealth, curHealth, healthRegen, healthReduce, playerDefense, expLevel, maxExp, limitExp = 70, levelPoint, curMoney, killEnemyNum;
    public float curExp, increaseExp, plusExp, increaseHP_enemy;
    public int[] probability = { (int)ProTier.Common, (int)ProTier.Rare, (int)ProTier.Epic, (int)ProTier.Legend };
    public string saveDirectory = "Saves/Beta1";

    protected override void Awake()
    {
        base.Awake();

        GPGSBinder.Inst.Login();
        #region GPGS
        //GPGSBinder.Inst.ShowAchievementUI(); 업적보기
        //GPGSBinder.Inst.UnlockAchievement(GPGSIds.achievement_test); 업적성공
        //GPGSBinder.Inst.IncrementAchievement(GPGSIds.achievement_testlevel, 1); 업적(레벨) 1 증가
        //GPGSBinder.Inst.ShowAllLeaderboardUI(); 리더보드 보기
        //GPGSBinder.Inst.ShowTargetLeaderboardUI(GPGSIds.leaderboard_hdh4300); 특정 리더보드 보기
        //GPGSBinder.Inst.ReportLeaderboard(GPGSIds.leaderboard_hdh4300, 1000); 리더보드에 1000기입(시간이면 1000ms)
        //GPGSBinder.Inst.LoadAllLeaderboardArray(GPGSIds.leaderboard_hdh4300); 타겟 리더보드 모든 정보 불러오기
        //GPGSBinder.Inst.IncrementEvent(GPGSIds.event_testevent, 1); 이벤트 1 증가
        //GPGSBinder.Inst.LoadEvent(GPGSIds.event_testevent); 이벤트 불러오기
        //GPGSBinder.Inst.LoadAllEvent(); 이벤트 모두 불러오기
        #endregion

#if UNITY_EDITOR
        return;
#elif UNITY_ANDROID
        StartCoroutine(CheckAppUpdate.CheckForUpdate());
#endif
    }

    private void Start()
    {
        LoadData();
        SetFrame();
    }

    public void LoadData()  // 세이브 디렉토리
    {
        // Shop
        CSVReader.Instance.csvList.LoadShop();

        // 돈
        if (ES3.KeyExists("Money", saveDirectory)) // 세이브 디렉토리
            curMoney = ES3.Load<int>("Money", saveDirectory);

        // 클리어
        LoadClearData();

        // 프레임
        if (ES3.KeyExists("Is30", saveDirectory))
            is30 = ES3.Load<bool>("Is30", saveDirectory);
    }

    public void ReSetGM()
    {
        gatchaKey = 3; // 가챠 키
        //초기화
        gameTime = GameTime.Slow; // 속도
        //경험치
        expLevel = 1;
        maxExp = 1000;
        limitExp = 70;
        healthReduce = 0;
        playerDefense = 0;
        killEnemyNum = 0;
        curExp = 0;
        levelPoint = 0;
    }

    public void DifficultLevel()
    {
        ReSetGM();

        switch (level)
        {
            case Level.Easy: // 쉬움
                maxHealth = 30;
                maxExp = 10;
                increaseExp = 0.3f;
                increaseHP_enemy = 0f;
                break;
            case Level.Normal: // 보통
                maxHealth = 25;
                maxExp = 20;
                increaseExp = 0.5f;
                increaseHP_enemy = 50f;
                break;
            case Level.Hard: // 어려움
                maxHealth = 20;
                maxExp = 25;
                increaseExp = 0.7f;
                increaseHP_enemy = 100f;
                break;
            case Level.Infinite: // 무한
                maxHealth = 20;
                maxExp = 25;
                increaseExp = 0.7f;
                increaseHP_enemy = 100f;
                break;
        }

        if (isTutorial)
            gatchaKey = 2;
    }

    private void LoadPreviousData()
    {
        if (!ES3.FileExists(saveDirectory))
            return;

        if (ES3.KeyExists("ClearCommon", saveDirectory))
        {
            clearData[0].isLevelClear[0] = ES3.Load<bool>("ClearCommon",saveDirectory);
            ES3.DeleteKey("ClearCommon", saveDirectory);
        }
        if (ES3.KeyExists("ClearNormal", saveDirectory))
        {
            clearData[0].isLevelClear[1] = ES3.Load<bool>("ClearNormal", saveDirectory);
            ES3.DeleteKey("ClearNormal", saveDirectory);
        }
        if (ES3.KeyExists("ClearHard", saveDirectory))
        {
            clearData[0].isLevelClear[2] = ES3.Load<bool>("ClearHard", saveDirectory);
            clearData[0].isClear = ES3.Load<bool>("ClearHard", saveDirectory);
            ES3.DeleteKey("ClearHard", saveDirectory);
        }

        SaveClearData();
    }

    private void LoadClearData() // 세이브 디렉토리
    {
        if (ES3.KeyExists("ClearGrass", saveDirectory))
            clearData[0] = ES3.Load<ClearData>("ClearGrass", saveDirectory);
        if (ES3.KeyExists("ClearDesert", saveDirectory))
            clearData[1] = ES3.Load<ClearData>("ClearDesert", saveDirectory);
        if (ES3.KeyExists("ClearForest", saveDirectory))
            clearData[2] = ES3.Load<ClearData>("ClearForest", saveDirectory);
        if (ES3.KeyExists("ClearCity", saveDirectory))
            clearData[3] = ES3.Load<ClearData>("ClearCity", saveDirectory);

        LoadPreviousData();
    }

    private void SaveClearData() // 세이브 디렉토리
    {
        // 정식 출시 시 세이브 파일 따로 저장 (ex. Saves/ClearSave)
        ES3.Save("ClearGrass", clearData[0], saveDirectory);
        ES3.Save("ClearDesert", clearData[1], saveDirectory);
        ES3.Save("ClearForest", clearData[2], saveDirectory);
        ES3.Save("ClearCity", clearData[3], saveDirectory);
    }

    public void GainMaxHealth(int a)
    {
        maxHealth += a;

        // 업적 9
        if (maxHealth >= 120)
            GPGSBinder.Inst.ETCAchievement(RTD.achievement_9);

        QuestManager.Instance.UpQuest(23, a); // 퀘스트 23

        GainHealth(a);
    }

    public void GainHealth(int a)
    {
        curHealth = Mathf.Min(curHealth + a, maxHealth);

        QuestManager.Instance.UpQuest(7, a); // 퀘스트 7

        UIManager.Instance.upPannel.healthTxt.text = $"{curHealth}/{maxHealth}"; // UI
    }

    public void LoseHealth(int i)
    {
        if (i <= 0)
            return;

        CSVList cl = CSVReader.Instance.csvList;
        CharacManager cm = CharacManager.Instance;

        SoundManager.Instance.PlaySFX("Hurt");

        curHealth -= i;

        QuestManager.Instance.UpQuest(2, i); // 퀘스트 2

        cl.UpgradeUnit(cl.FindUnit(UnitType.Sword), UpUnit.Dmg, cm.dmgSword); // Charac 17
        cl.UpgradeAllUnit(UpUnit.AtkSpeed, cm.madDmg); // Charac 37

        // GameOver
        if (curHealth <= 0 && isStart)
        {
            curHealth = 0;
            isStart = false;
            GameOver();
        }

        UIManager.Instance.upPannel.healthTxt.text = $"{curHealth}/{maxHealth}"; // UI
    }

    public void GainGatcha(int a)
    {
        gatchaKey += a;
        QuestManager.Instance.UpQuest(1); // 퀘스트 1

        // 업적 10
        if (gatchaKey >= 10)
            GPGSBinder.Inst.ETCAchievement(RTD.achievement_10);

        UIManager.Instance.upPannel.gatchaKeyTxt.text = gatchaKey.ToString(); // UI
    }

    public void LoseGatcha(int a)
    {
        gatchaKey = Mathf.Max(gatchaKey - a, 0);

        UIManager.Instance.upPannel.gatchaKeyTxt.text = gatchaKey.ToString(); // UI
    }

    public void EarnMoney(int a)
    {
        curMoney += a;

        // 업적 (7 ~ 8)
        if (a >= 10000)
            GPGSBinder.Inst.ETCAchievement(RTD.achievement_7);
        if (curMoney >= 100000)
            GPGSBinder.Inst.ETCAchievement(RTD.achievement_8);

        ES3.Save("Money", curMoney, saveDirectory); // 세이브 디렉토리
    }

    public void GainExp(int a)
    {
        UIManager um = UIManager.Instance;

        curExp += a + (a * plusExp / 100);

        if (curExp >= maxExp)
            LevelUp(um);

        StartCoroutine(um.upPannel.EXPHandlerCoroutine(this)); 
    }

    public void LevelUp(UIManager um)
    {
        // 로비GM의 MaxExp를 1000으로 조정함으로 로비에서는 실행 X
        SoundManager.Instance.PlaySFX("LevelUp");

        curExp = 0;
        expLevel++;
        levelPoint++;
        maxExp = Mathf.Min(maxExp + Mathf.RoundToInt(increaseExp * maxExp), 40); // 최대 경험치 40

        // 업적 (5 ~ 6)
        if (expLevel >= 50)
            GPGSBinder.Inst.ETCAchievement(RTD.achievement_6);
        else if(expLevel >= 25)
            GPGSBinder.Inst.ETCAchievement(RTD.achievement_5);

        um.levelUpBtn.SetUI(); // UI 설정
        um.upPannel.levelTxt.text = $"{expLevel}Lv:";
    }

    public void GameOver()
    {
        GPGSBinder.Inst.StageLeaderBoard(stage, level);

        UIManager.Instance.gameOverPannel.SetUI(false);
    }

    public void GameClear()
    {
        UIManager.Instance.gameOverPannel.SetUI(true);
        clearData[(int)stage].isLevelClear[(int)level] = true;
        if (level == Level.Hard)
            clearData[(int)stage].isClear = true;

        GPGSBinder.Inst.ClearAchievement(stage, level); // 업적

        SaveClearData();
    }

    public void SetFrame()
    {
        if (is30)
            Application.targetFrameRate = 30;
        else
            Application.targetFrameRate = 60;
    }

    public void GameStart(Level _level)
    {
        isStart = true;
        level = _level;

        DifficultLevel();

        SceneManager.sceneLoaded += InGameLoad;
        SceneManager.LoadSceneAsync(1);
    }

    public void InGameLoad(Scene scene, LoadSceneMode mode)
    {
        CSVReader cr = CSVReader.Instance;

        SoundManager.Instance.PlayBGM(level.ToString());

        cr.CSVReading();
        cr.StartUpdateStat();

        SpawnManager.Instance.SpawnTilemap((int)stage);

        BtnManager.Instance.RefreshQuestBtn();

        if(isTutorial)
            StartCoroutine(UIManager.Instance.tutorialPannel.DoTutorial());

        isSceneLoading = false;
        SceneManager.sceneLoaded -= InGameLoad; // 초기화
    }

    public void TitleLoad(Scene scene, LoadSceneMode mode)
    {
        gameTime = GameTime.Slow;
        BtnManager.Instance.Play();
        SoundManager.Instance.PlayBGM("Lobby");

        isSceneLoading = false;
        SceneManager.sceneLoaded -= TitleLoad;
    }
}

[System.Serializable]
public struct ClearData
{
    public Stage stage;
    public bool[] isLevelClear;
    public bool isClear;
}