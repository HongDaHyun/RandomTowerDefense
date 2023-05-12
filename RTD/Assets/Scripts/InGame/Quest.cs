using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Quest : MonoBehaviour
{
    public QuestStat questStat;
    public int curCondition;
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
        questStat = CSVReader.Instance.QuestExport(id);
        texts[0].text = questStat.questName; //퀘스트 이름
        texts[1].text = string.Format(questStat.explanation, curCondition, questStat.condition);

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
    }

    private void Update()
    {
        if (curCondition < questStat.condition)
        {
            button.interactable = false;
            return;
        }
        else
            button.interactable = true; //최적화 필요
    }
}
