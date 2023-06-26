using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Quest : MonoBehaviour
{
    public QuestStat questStat;
    public int curCondition, readyCondition_1, readyCondition_2;
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
        CheckSuccess();
        UpdateQuest();
    }

    public void SetQuest(int id)
    {
        GameManager gm = GameManager.Instance;
        // Reset
        curCondition = 0;
        button.interactable = false;
        questStat = CSVReader.Instance.csvList.FindQuest(id);
        texts[0].text = questStat.questName; //퀘스트 이름

        int ranTier = Random.Range(0, gm.probability[0] + gm.probability[1] + gm.probability[2] + gm.probability[3]);

        if (ranTier < gm.probability[0])
            tier = Tier.Common;
        else if (ranTier < gm.probability[0] + gm.probability[1])
            tier = Tier.Rare;
        else if (ranTier < gm.probability[0] + gm.probability[1] + gm.probability[2])
            tier = Tier.Epic;
        else
            tier = Tier.Legend;

        int plus = CharacManager.Instance.plusReward; // Charac 12

        reward = 1 * (int)tier + plus; // 티어에 따른 보상 증가
        questStat.condition += questStat.conditionWidth * ((int)tier - 1); // 티어에 따른 조건 증가

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

        // rewardText 설정
        if (plus == 0)
            texts[2].text = string.Format("키 {0} 증가", reward);
        else
            texts[2].text = string.Format("키 {0} + {1} 증가", reward - plus, plus);

        QuestManager.Instance.ReadyQuest(questStat, ref readyCondition_1, ref readyCondition_2);
        UpdateQuest();
    }

    private void UpdateQuest()
    {
        if (button.interactable)
            return;

        QuestManager.Instance.UpdateQuestCondition(questStat, ref readyCondition_1, ref readyCondition_2, ref curCondition);

        if (curCondition >= questStat.condition)
        {
            curCondition = questStat.condition;
            button.interactable = true;
        }

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

    public void ClickQuest()
    {
        GameManager.Instance.gatchaKey += reward;

        for(int i = 0; i < 3; i++)
        {
            UIManager.Instance.questPannel.quests[i].UpdateQuest();
        }

        QuestManager.Instance.questNum++; // 퀘스트 7

        // 비활성화
        gameObject.SetActive(false);
    }
}
