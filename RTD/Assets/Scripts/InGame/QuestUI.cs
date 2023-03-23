using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestUI : MonoBehaviour
{
    public Quest quest;
    public int curCondition;
    private RewardType rewardType;
    private Tier tier;
    private float reward;

    private Text[] texts;
    private Button button;

    private void Awake()
    {
        texts = GetComponentsInChildren<Text>();
        button = GetComponent<Button>();
    }

    public void SetQuest(int id)
    {
        curCondition = 0;
        quest = CSVReader.Instance.QuestExport(id);
        texts[0].text = quest.questName; //퀘스트 이름
        texts[1].text = string.Format(quest.explanation, curCondition, quest.condition);

        int ranNum = Random.Range(0, System.Enum.GetValues(typeof(RewardType)).Length);
        rewardType = (RewardType)ranNum; //리워드 타입
        ranNum = Random.Range(0, System.Enum.GetValues(typeof(Tier)).Length);
        tier = (Tier)ranNum; //리워드 티어

        switch(rewardType)
        {
            case RewardType.키:
                reward = 1 + 1 * (int)tier;
                break;
            case RewardType.체력회복:
                reward = 2 + 2 * (int)tier;
                break;
            case RewardType.최대체력:
                reward = 1 + 1 * (int)tier;
                break;
            case RewardType.데미지:
                reward = 1 + 1 * (int)tier;
                break;
            case RewardType.치명타확률:
                reward = 1 + 2 * (int)tier;
                break;
            case RewardType.치명타데미지:
                reward = 3 + 1 * (int)tier;
                break;
            case RewardType.공격속도:
                reward = 0.1f + 0.1f * (int)tier;
                break;
            case RewardType.사거리:
                reward = 0.1f + 0.1f * (int)tier;
                break;
            case RewardType.디버프지속:
                reward = 0.1f + 0.1f * (int)tier;
                break;
        }

        switch(tier)
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

        texts[2].text = string.Format("{0} {1} 증가", rewardType, reward);

        StopAllCoroutines();
        StartCoroutine(QuestManager.Instance.DoQuest(quest));
    }

    private void Update()
    {
        if (curCondition < quest.condition)
        {
            button.interactable = false;
            return;
        }
        else
            button.interactable = true; //최적화 필요
    }
}
