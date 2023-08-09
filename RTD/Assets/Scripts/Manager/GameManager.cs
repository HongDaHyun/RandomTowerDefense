using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    public Level level;
    public GameTime gameTime = GameTime.Slow;
    public bool isStart, is30, isTutorial, isSceneLoading;
    public bool[] isClear;
    public int gatchaKey, maxHealth, curHealth, healthRegen, healthReduce, playerDefense, maxWave, expLevel, maxExp, limitExp = 70, levelPoint, curMoney, killEnemyNum;
    public float curExp, increaseExp, plusExp;
    public int[] probability = { (int)ProTier.Common, (int)ProTier.Rare, (int)ProTier.Epic, (int)ProTier.Legend };
    public int[] levelNum;
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
                break;
            case Level.Normal: // 보통
                maxHealth = 25;
                maxExp = 20;
                increaseExp = 0.5f;
                break;
            case Level.Hard: // 어려움
                maxHealth = 20;
                maxExp = 25;
                increaseExp = 0.7f;
                break;
            case Level.Infinite: // 무한
                maxHealth = 20;
                maxExp = 25;
                increaseExp = 0.7f;
                break;
        }
        if (isTutorial)
            gatchaKey = 2;

        maxWave = levelNum[(int)level];
    }

    private void LoadClearData() // 세이브 디렉토리
    {
        // 정식 출시 시 세이브 파일 따로 저장 (ex. Saves/ClearSave)
        if (ES3.KeyExists("ClearCommon", saveDirectory))
            isClear[0] = ES3.Load<bool>("ClearCommon", saveDirectory);
        if (ES3.KeyExists("ClearNormal", saveDirectory))
            isClear[1] = ES3.Load<bool>("ClearNormal", saveDirectory);
        if (ES3.KeyExists("ClearHard", saveDirectory))
            isClear[2] = ES3.Load<bool>("ClearHard", saveDirectory);
    }

    private void SaveClearData() // 세이브 디렉토리
    {
        // 정식 출시 시 세이브 파일 따로 저장 (ex. Saves/ClearSave)
        ES3.Save("ClearCommon", isClear[0], saveDirectory);
        ES3.Save("ClearNormal", isClear[1], saveDirectory);
        ES3.Save("ClearHard", isClear[2], saveDirectory);
    }

    public void GainMaxHealth(int a)
    {
        maxHealth += a;

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
        maxExp = Mathf.Min(maxExp + Mathf.RoundToInt(increaseExp * maxExp), 70); // 최대 경험치 70

        um.levelUpBtn.SetUI(); // UI 설정
        um.upPannel.levelTxt.text = $"{expLevel}Lv:";
    }

    public void GameOver()
    {
        if (level == Level.Infinite)
            GPGSBinder.Inst.ReportLeaderboard(RTD.leaderboard_infinite_ranking, WaveManager.Instance.curWave);

        UIManager.Instance.gameOverPannel.SetUI(false);
    }

    public void GameClear()
    {
        UIManager.Instance.gameOverPannel.SetUI(true);
        isClear[(int)level] = true;

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