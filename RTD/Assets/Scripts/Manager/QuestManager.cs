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

    public void DoQuest(Quest quest)
    {
        //switch(quest.ID)
        //{
        //    case 0:
                //퀘스트 작업 해야함
        //}
    }
}
