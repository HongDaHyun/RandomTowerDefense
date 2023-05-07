using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
                    if (WaveManager.Instance.curWave != goalWave)
                        yield return new WaitForEndOfFrame();
                }
        }
    }

    public void GetReward(int rewardAmount)
    {
        GameManager.Instance.gatchaKey += rewardAmount;
    }
}
