using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Quest : MonoBehaviour
{
    public QuestStat questStat;
    public int curCondition, readyCondition;
    private Tier tier;
    private int reward;

    private Text[] texts;
    private Button button;

    private void Awake()
    {
        texts = GetComponentsInChildren<Text>();
        button = GetComponent<Button>();
    }

    private void OnEnable()
    {
        UpdateQuest();
        DoQuest();
        CheckSuccess();
    }

    public void SetQuest(int id)
    {
        curCondition = 0;
        button.interactable = false; // Reset
        questStat = CSVReader.Instance.QuestExport(id);
        texts[0].text = questStat.questName; //퀘스트 이름
        UpdateQuest();

        int ranNum = Random.Range(0, System.Enum.GetValues(typeof(Tier)).Length);
        tier = (Tier)ranNum; //리워드 티어
        reward = 1 + 1 * (int)tier; // 티어에 따른 보상 증가

        switch(tier) // 티어 색상
        {
            case Tier.Common:
                texts[0].color = Color.green;
                break;
            case Tier.Rare:
                texts[0].color = Color.blue;
                break;
            case Tier.Epic:
                texts[0].color = Color.magenta;
                break;
            case Tier.Legend:
                texts[0].color = Color.yellow;
                break;
        }

        texts[2].text = string.Format("키 {0} 증가", reward); //rewardText 설정

        ReadyQuest();
    }

    private void ReadyQuest()
    {
        switch(questStat.ID)
        {
            case 0:
                readyCondition = WaveManager.Instance.curWave;
                break;
        }
    }

    private void DoQuest()
    {
        switch(questStat.ID)
        {
            case 0:
                if(!WaveManager.Instance.isWave)
                    curCondition = WaveManager.Instance.curWave - readyCondition;
                break;
        }
    }

    private void UpdateQuest()
    {
        texts[1].text = string.Format(questStat.explanation, curCondition, questStat.condition);
    }

    private void CheckSuccess()
    {
        if (curCondition >= questStat.condition)
        {
            button.interactable = true;
            return;
        }
        button.interactable = false;
    }
}
