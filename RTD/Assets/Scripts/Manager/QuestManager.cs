using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : Singleton<QuestManager>
{
    private void Start()
    {
        BtnManager.Instance.RefreshQuestBtn();
    }

    public void UpQuest(int id)
    {
        for (int i = 0; i < 3; i++)
            UIManager.Instance.questPannel.quests[i].DoQuest(id);
    }

    public void UpQuest(int id, int size)
    {
        for (int i = 0; i < 3; i++)
            UIManager.Instance.questPannel.quests[i].DoQuest(id, size);
    }

    public void UpQuest(int[] id)
    {
        for (int i = 0; i < 3; i++)
        {
            for(int j = 0; j < id.Length; j++)
            {
                UIManager.Instance.questPannel.quests[j].DoQuest(id[j]);
            }
        }
    }
}