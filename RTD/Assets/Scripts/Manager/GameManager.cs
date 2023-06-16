using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public Level level;
    public GameTime gameTime = GameTime.Slow;
    public bool isStart;
    public int gatchaKey, maxHealth, curHealth, maxWave, expLevel, maxExp, curExp;
    public int[] probability = { (int)ProTier.Common, (int)ProTier.Rare, (int)ProTier.Epic, (int)ProTier.Legend };

    protected override void Awake()
    {
        base.Awake();
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
        curHealth--;

        CSVReader.Instance.csvList.UpgradeUnit(CSVReader.Instance.csvList.FindUnit(UnitType.Sword), UpUnit.Dmg, CharacManager.Instance.dmgSword); // Charac 17

        // GameOver
        if (curHealth <= 0)
        {
        }
    }
}