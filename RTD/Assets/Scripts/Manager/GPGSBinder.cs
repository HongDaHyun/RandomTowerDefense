using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using GooglePlayGames.BasicApi.Events;


public class GPGSBinder
{
    static GPGSBinder inst = new GPGSBinder();
    public static GPGSBinder Inst => inst;



    ISavedGameClient SavedGame =>
        PlayGamesPlatform.Instance.SavedGame;

    IEventsClient Events =>
        PlayGamesPlatform.Instance.Events;



    void Init()
    {
        var config = new PlayGamesClientConfiguration.Builder().EnableSavedGames().Build();
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();
    }


    public void Login(Action<bool, UnityEngine.SocialPlatforms.ILocalUser> onLoginSuccess = null)
    {
        Init();
        PlayGamesPlatform.Instance.Authenticate(SignInInteractivity.CanPromptAlways, (success) =>
        {
            onLoginSuccess?.Invoke(success == SignInStatus.Success, Social.localUser);
        });
    }

    public void Logout()
    {
        PlayGamesPlatform.Instance.SignOut();
    }


    public void SaveCloud(string fileName, string saveData, Action<bool> onCloudSaved = null)
    {
        SavedGame.OpenWithAutomaticConflictResolution(fileName, DataSource.ReadCacheOrNetwork,
            ConflictResolutionStrategy.UseLastKnownGood, (status, game) =>
            {
                if (status == SavedGameRequestStatus.Success)
                {
                    var update = new SavedGameMetadataUpdate.Builder().Build();
                    byte[] bytes = System.Text.Encoding.UTF8.GetBytes(saveData);
                    SavedGame.CommitUpdate(game, update, bytes, (status2, game2) =>
                    {
                        onCloudSaved?.Invoke(status2 == SavedGameRequestStatus.Success);
                    });
                }
            });
    }

    public void LoadCloud(string fileName, Action<bool, string> onCloudLoaded = null)
    {
        SavedGame.OpenWithAutomaticConflictResolution(fileName, DataSource.ReadCacheOrNetwork,
            ConflictResolutionStrategy.UseLastKnownGood, (status, game) =>
            {
                if (status == SavedGameRequestStatus.Success)
                {
                    SavedGame.ReadBinaryData(game, (status2, loadedData) =>
                    {
                        if (status2 == SavedGameRequestStatus.Success)
                        {
                            string data = System.Text.Encoding.UTF8.GetString(loadedData);
                            onCloudLoaded?.Invoke(true, data);
                        }
                        else
                            onCloudLoaded?.Invoke(false, null);
                    });
                }
            });
    }

    public void DeleteCloud(string fileName, Action<bool> onCloudDeleted = null)
    {
        SavedGame.OpenWithAutomaticConflictResolution(fileName,
            DataSource.ReadCacheOrNetwork, ConflictResolutionStrategy.UseLongestPlaytime, (status, game) =>
            {
                if (status == SavedGameRequestStatus.Success)
                {
                    SavedGame.Delete(game);
                    onCloudDeleted?.Invoke(true);
                }
                else
                    onCloudDeleted?.Invoke(false);
            });
    }

    #region AchieveMent
    public void ShowAchievementUI() =>
        Social.ShowAchievementsUI();

    private void UnlockAchievement(string gpgsId, Action<bool> onUnlocked = null) =>
        Social.ReportProgress(gpgsId, 100, success => onUnlocked?.Invoke(success));

    private void IncrementAchievement(string gpgsId, int steps, Action<bool> onUnlocked = null) =>
        PlayGamesPlatform.Instance.IncrementAchievement(gpgsId, steps, success => onUnlocked?.Invoke(success));

    string[] clear = { RTD.achievement, RTD.achievement_2, RTD.achievement_3, RTD.achievement_52, RTD.achievement_53, RTD.achievement_54, RTD.achievement_55,
    RTD.achievement_56, RTD.achievement_57, RTD.achievement_58, RTD.achievement_59, RTD.achievement_60, RTD.achievement_61, RTD.achievement_62, RTD.achievement_63, RTD.achievement_64};
    // 클리어 업적 (1 ~ 3)
    public void ClearAchievement(Stage curStage, Level curLevel)
    {
        int index = 0 + 3 * (int)curStage;

        UnlockAchievement(clear[index + (int)curLevel]); // 레벨 클리어

        if(curLevel == Level.Hard)
            UnlockAchievement(clear[Enum.GetValues(typeof(Stage)).Length * 3 + (int)curStage]); // 스테이지 클리어
    }

    string[] charac = {RTD.achievement_12, RTD.achievement_13, RTD.achievement_14, RTD.achievement_15, RTD.achievement_16, RTD.achievement_17, RTD.achievement_18, RTD.achievement_19,
        RTD.achievement_20, RTD.achievement_21, RTD.achievement_22, RTD.achievement_23, RTD.achievement_24, RTD.achievement_25, RTD.achievement_26, RTD.achievement_27, RTD.achievement_28,
        RTD.achievement_29, RTD.achievement_30, RTD.achievement_31, RTD.achievement_32, RTD.achievement_33, RTD.achievement_34, RTD.achievement_35, RTD.achievement_36, RTD.achievement_37,
        RTD.achievement_38, RTD.achievement_39, RTD.achievement_40, RTD.achievement_41, RTD.achievement_42, RTD.achievement_43, RTD.achievement_44, RTD.achievement_45, RTD.achievement_46,
        RTD.achievement_47, RTD.achievement_48, RTD.achievement_49, RTD.achievement_50, RTD.achievement_51};
    public void CharacAchievement(int id) // 특성 업적들
    {
        UnlockAchievement(charac[id]);
    }

    // 기타 업적들(4 ~ 11)
    public void ETCAchievement(string gpgsId)
    {
        UnlockAchievement(gpgsId);
    }
    #endregion

    #region LeaderBoard
    public void ShowAllLeaderboardUI() =>
        Social.ShowLeaderboardUI();

    public void ShowTargetLeaderboardUI(string gpgsId) =>
        ((PlayGamesPlatform)Social.Active).ShowLeaderboardUI(gpgsId);

    public void ReportLeaderboard(string gpgsId, long score, Action<bool> onReported = null) =>
        Social.ReportScore(score, gpgsId, success => onReported?.Invoke(success));

    public void LoadAllLeaderboardArray(string gpgsId, Action<UnityEngine.SocialPlatforms.IScore[]> onloaded = null) =>
        Social.LoadScores(gpgsId, onloaded);

    public void LoadCustomLeaderboardArray(string gpgsId, int rowCount, LeaderboardStart leaderboardStart,
        LeaderboardTimeSpan leaderboardTimeSpan, Action<bool, LeaderboardScoreData> onloaded = null)
    {
        PlayGamesPlatform.Instance.LoadScores(gpgsId, leaderboardStart, rowCount, LeaderboardCollection.Public, leaderboardTimeSpan, data =>
        {
            onloaded?.Invoke(data.Status == ResponseStatus.Success, data);
        });
    }

    string[] stage = { RTD.leaderboard, RTD.leaderboard_2, RTD.leaderboard_3, RTD.leaderboard_4 };
    public void StageLeaderBoard(Stage curStage, Level curLevel)
    {
        if (curLevel != Level.Infinite)
            return;

        int stageIndex = (int)curStage;

        ReportLeaderboard(stage[stageIndex], WaveManager.Instance.curWave);
    }
    #endregion


    public void IncrementEvent(string gpgsId, uint steps)
    {
        Events.IncrementEvent(gpgsId, steps);
    }

    public void LoadEvent(string gpgsId, Action<bool, IEvent> onEventLoaded = null)
    {
        Events.FetchEvent(DataSource.ReadCacheOrNetwork, gpgsId, (status, iEvent) =>
        {
            onEventLoaded?.Invoke(status == ResponseStatus.Success, iEvent);
        });
    }

    public void LoadAllEvent(Action<bool, List<IEvent>> onEventsLoaded = null)
    {
        Events.FetchAllEvents(DataSource.ReadCacheOrNetwork, (status, events) =>
        {
            onEventsLoaded?.Invoke(status == ResponseStatus.Success, events);
        });
    }

}