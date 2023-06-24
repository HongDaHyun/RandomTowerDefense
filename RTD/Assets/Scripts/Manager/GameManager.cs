using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public Level level;
    public GameTime gameTime = GameTime.Slow;
    public bool isStart;
    public int gatchaKey, maxHealth, curHealth, maxWave, expLevel, maxExp, levelPoint;
    public float curExp, increaseExp;
    public int[] probability = { (int)ProTier.Common, (int)ProTier.Rare, (int)ProTier.Epic, (int)ProTier.Legend };
    public int[] levelNum;
    public DEVICE device;

    CSVList cl;
    CharacManager cm;

    protected override void Awake()
    {
        base.Awake();
#if UNITY_EDITOR
        device = DEVICE.UNITY;
#elif UNITY_ANDROID
        device = DEVICE.ANDROID;
#endif

        Application.targetFrameRate = 60;

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

    private void Update()
    {
        if (curExp >= maxExp)
            LevelUp();
    }

    private void Start()
    {
        cl = CSVReader.Instance.csvList;
        cm = CharacManager.Instance;
    }

    public void SetGM()
    {
        //초기화
        isStart = false;
        gameTime = GameTime.Slow; // 속도
        //경험치
        expLevel = 0;
        maxExp = 1000;
        curExp = 0;
        levelPoint = 0;
    }

    public void GainHealth(int a)
    {
        curHealth += a;

        LimitHealth();
    }

    private void LimitHealth()
    {
        if (curHealth >= maxHealth)
        {
            curHealth = maxHealth;
            return;
        }
    }

    public void LoseHealth()
    {
        SoundManager.Instance.PlaySFX("Hurt");

        curHealth--;

        cl.UpgradeUnit(cl.FindUnit(UnitType.Sword), UpUnit.Dmg, cm.dmgSword); // Charac 17
        cl.UpgradeAllUnit(UpUnit.AtkSpeed, cm.madDmg); // Charac 37

        // GameOver
        if (curHealth <= 0)
        {
            GameOver();
        }
    }

    public void GameOver()
    {
        BtnManager.Instance.Pause();
        UIManager.Instance.gameOverPannel.SetActive(true);
    }

    public void LevelUp()
    {
        // 로비GM의 MaxExp를 1000으로 조정함으로 로비에서는 실행 X
        SoundManager.Instance.PlaySFX("LevelUp");

        curExp = 0;
        expLevel++;
        levelPoint++;
        maxExp += Mathf.RoundToInt(increaseExp * maxExp);

        UIManager.Instance.levelUpBtn.SetUI(); // UI 설정
    }
}