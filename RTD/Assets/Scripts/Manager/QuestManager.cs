using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RewardType { 키, 체력회복, 최대체력, 데미지, 치명타확률, 치명타데미지, 공격속도, 사거리, 디버프지속}

public class QuestManager : Singleton<QuestManager>
{
    public void InitQuest()
    {
        int ranID;
        List<int> IDs = new List<int>(); //중복 방지 리스트
        for(int i = 0; i < UIManager.Instance.quests.Length;i++)
        {
            ranID = Random.Range(0, CSVReader.Instance.csvList.quest.Length);
            if (IDs.Contains(ranID)) // 중복이면 다시
                i--;
            else
            {
                IDs.Add(ranID);
                UIManager.Instance.quests[i].SetQuest(ranID); //퀘스트 생성
            }
        }
    }

    public IEnumerator DoQuest(Quest quest)
    {
        switch (quest.ID)
        {
            case 0:
                int goalWave = WaveManager.Instance.curWave + quest.condition;
                while(true)
                {
                    Debug.Log("Loop");
                    if (WaveManager.Instance.curWave != goalWave)
                        yield return new WaitForEndOfFrame();
                }
        }
    }

    public void GetReward(RewardType type, float rewardAmount)
    {
        switch (type)
        {
            case RewardType.키:
                GameManager.Instance.gatchaKey += (int)rewardAmount;
                break;
            case RewardType.체력회복:
                GameManager.Instance.curHealth += (int)rewardAmount;
                break;
            case RewardType.최대체력:
                GameManager.Instance.maxHealth += (int)rewardAmount;
                break;
            case RewardType.데미지:
                CSVReader.Instance.ProjectileDmgUp(rewardAmount);
                break;
            case RewardType.치명타확률:
                CSVReader.Instance.ProjectileCritPercentUp((int)rewardAmount);
                break;
            case RewardType.치명타데미지:
                CSVReader.Instance.ProjectileCritDmgUp((int)rewardAmount);
                break;
            case RewardType.공격속도:
                CSVReader.Instance.ProjectileAsUp(rewardAmount);
                break;
            case RewardType.사거리:
                CSVReader.Instance.ProjectileRangeUp(rewardAmount);
                break;
            case RewardType.디버프지속:
                CSVReader.Instance.DebuffUp((int)rewardAmount);
                break;
        }
    }
}
