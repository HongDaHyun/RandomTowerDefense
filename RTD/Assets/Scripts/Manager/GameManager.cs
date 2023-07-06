using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public Level level;
    public GameTime gameTime = GameTime.Slow;
    public bool isStart, is30;
    public bool[] isClear;
    public int gatchaKey, maxHealth, curHealth, healthWave, maxWave, expLevel, maxExp, levelPoint, curMoney, killEnemyNum;
    public float curExp, increaseExp, plusExp;
    public int[] probability = { (int)ProTier.Common, (int)ProTier.Rare, (int)ProTier.Epic, (int)ProTier.Legend };
    public int[] levelNum;

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
    }

    private void Start()
    {
        LoadData();
        SetFrame();
    }

    public void SetGM()
    {
        //초기화
        isStart = false;
        gameTime = GameTime.Slow; // 속도
        //경험치
        expLevel = 1;
        maxExp = 1000;
        killEnemyNum = 0;
        curExp = 0;
        levelPoint = 0;
    }

    public void DifficultLevel(Level level)
    {
        switch (level)
        {
            case Level.Easy: // 쉬움
                gatchaKey = 3;
                maxHealth = 30;
                maxExp = 10;
                increaseExp = 0.3f;
                maxWave = levelNum[0];
                curHealth = maxHealth;
                break;
            case Level.Normal: // 보통
                gatchaKey = 3;
                maxHealth = 25;
                maxExp = 20;
                increaseExp = 0.5f;
                maxWave = levelNum[1];
                curHealth = maxHealth;
                break;
            case Level.Hard: // 어려움
                gatchaKey = 3;
                maxHealth = 20;
                maxExp = 30;
                increaseExp = 0.7f;
                maxWave = levelNum[2];
                curHealth = maxHealth;
                break;
            case Level.Infinite: // 무한
                gatchaKey = 3;
                maxHealth = 20;
                maxExp = 30;
                increaseExp = 0.7f;
                maxWave = levelNum[3];
                curHealth = maxHealth;
                break;
        }
        gameTime = GameTime.Slow; // 속도 초기화
    }

    public void LoadData()  // 세이브 디렉토리
    {
        // Shop
        CSVReader.Instance.csvList.LoadShop();

        // 돈
        if(ES3.KeyExists("Money", "Saves/Alpha2")) // 세이브 디렉토리
            curMoney = ES3.Load<int>("Money", "Saves/Alpha2");

        // 클리어
        LoadClearData();

        // 프레임
        if (ES3.KeyExists("Is30", "Saves/Alpha2"))
            is30 = ES3.Load<bool>("Is30", "Saves/Alpha2");
    }

    private void LoadClearData() // 세이브 디렉토리
    {
        // 정식 출시 시 세이브 파일 따로 저장 (ex. Saves/ClearSave)
        if (ES3.KeyExists("ClearCommon", "Saves/Alpha2"))
            isClear[0] = ES3.Load<bool>("ClearCommon", "Saves/Alpha2");
        if (ES3.KeyExists("ClearNormal", "Saves/Alpha2"))
            isClear[1] = ES3.Load<bool>("ClearNormal", "Saves/Alpha2");
        if (ES3.KeyExists("ClearHard", "Saves/Alpha2"))
            isClear[2] = ES3.Load<bool>("ClearHard", "Saves/Alpha2");
    }

    private void SaveClearData() // 세이브 디렉토리
    {
        // 정식 출시 시 세이브 파일 따로 저장 (ex. Saves/ClearSave)
        ES3.Save("ClearCommon", isClear[0], "Saves/Alpha2");
        ES3.Save("ClearNormal", isClear[1], "Saves/Alpha2");
        ES3.Save("ClearHard", isClear[2], "Saves/Alpha2");
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
        CSVList cl = CSVReader.Instance.csvList;
        CharacManager cm = CharacManager.Instance;

        SoundManager.Instance.PlaySFX("Hurt");

        curHealth -= i;

        QuestManager.Instance.UpQuest(2, i); // 퀘스트 2

        cl.UpgradeUnit(cl.FindUnit(UnitType.Sword), UpUnit.Dmg, cm.dmgSword); // Charac 17
        cl.UpgradeAllUnit(UpUnit.AtkSpeed, cm.madDmg); // Charac 37

        // GameOver
        if (curHealth <= 0)
        {
            curHealth = 0;
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

        ES3.Save<int>("Money", curMoney, "Saves/Alpha2"); // 세이브 디렉토리
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
        maxExp += Mathf.RoundToInt(increaseExp * maxExp);

        um.levelUpBtn.SetUI(); // UI 설정
        um.upPannel.levelTxt.text = $"{expLevel}Lv:";
    }

    public void GameOver()
    {
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
}