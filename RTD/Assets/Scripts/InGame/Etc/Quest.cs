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
        curCondition = 0;
        button.interactable = false; // Reset
        questStat = CSVReader.Instance.QuestExport(id);
        texts[0].text = questStat.questName; //퀘스트 이름

        int ranNum = Random.Range(0, System.Enum.GetValues(typeof(Tier)).Length);
        tier = (Tier)ranNum; //리워드 티어
        reward = 1 + 1 * (int)tier; // 티어에 따른 보상 증가
        questStat.condition += questStat.conditionWidth * (int)tier; // 티어에 따른 조건 증가

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
        UpdateQuest();
    }

    private void ReadyQuest()
    {
        switch(questStat.ID)
        {
            case 0:
                readyCondition_1 = WaveManager.Instance.curWave;
                break;

            case 1:
                readyCondition_1 = WaveManager.Instance.curWave;
                readyCondition_2 = GameManager.Instance.curHealth;
                break;

            case 4:
            //광고(후에 제작)

            case 5:
                QuestManager.Instance.killEnemyNum = 0;
                break;

            case 6:
                QuestManager.Instance.unitTypesList = new List<UnitType>();
                break;

            case 7:
                QuestManager.Instance.questNum = 0;
                break;

            case 8:
                readyCondition_1 = GameManager.Instance.curHealth;
                break;

            case 9:
                QuestManager.Instance.gatchaNum = 0;
                break;

            case 10:
            case 11:
            case 12:
            case 13:
                QuestManager.Instance.tierNum = new int[] { 0, 0, 0, 0 };
                break;

            case 15:
            case 16:
            case 17:
            case 18:
            case 19:
            case 20:
            case 21:
            case 22:
                QuestManager.Instance.unitNum = new int[] { 0, 0, 0, 0, 0, 0, 0, 0 };
                break;

            case 23:
                QuestManager.Instance.evolNum = 0;
                break;

            case 24:
                readyCondition_1 = GameManager.Instance.maxHealth;
                break;

            case 25:
                // 보스 제작중

            default:
                // 2, 3, 14(꽝)
                break;
        }
    }

    private void UpdateQuest()
    {
        switch (questStat.ID)
        {
            case 0:
                if (!WaveManager.Instance.isWave)
                    curCondition = WaveManager.Instance.curWave - readyCondition_1;
                break;

            case 1:
                if (readyCondition_2 > GameManager.Instance.curHealth) //데미지를 입을 시 초기화
                {
                    readyCondition_1 = WaveManager.Instance.curWave;
                    curCondition = 0;
                    break;
                }
                if (!WaveManager.Instance.isWave)
                    curCondition = WaveManager.Instance.curWave - readyCondition_1;
                break;

            case 2:
                curCondition = GameManager.Instance.gatchaKey;
                break;

            case 3:
                curCondition = GameManager.Instance.maxHealth - GameManager.Instance.curHealth;
                break;

            case 5:
                curCondition = QuestManager.Instance.killEnemyNum;
                break;

            case 6:
                curCondition = QuestManager.Instance.unitTypesList.Count;
                break;

            case 7:
                curCondition = QuestManager.Instance.questNum;
                break;

            case 8:
                curCondition = GameManager.Instance.curHealth - readyCondition_1;
                if (curCondition <= 0)
                {
                    curCondition = 0;
                    readyCondition_1 = GameManager.Instance.curHealth;
                }
                break;

            case 9:
                curCondition = QuestManager.Instance.gatchaNum;
                break;

            case 10:
            case 11:
            case 12:
            case 13:
                curCondition = QuestManager.Instance.tierNum[questStat.ID - 10];
                break;

            case 15:
            case 16:
            case 17:
            case 18:
            case 19:
            case 20:
            case 21:
            case 22:
                curCondition = QuestManager.Instance.unitNum[questStat.ID - 15];
                break;

            case 23:
                curCondition = QuestManager.Instance.evolNum;
                break;

            case 24:
                curCondition = GameManager.Instance.maxHealth - readyCondition_1;
                break;

            case 25:
                //보스 제작중

            default: // 14(꽝)
                break;
        }

        texts[1].text = string.Format(questStat.explanation, curCondition, questStat.condition);
    }

    private void CheckSuccess()
    {
        if (curCondition >= questStat.condition)
        {
            button.interactable = true;
            QuestManager.Instance.questNum++; // 퀘스트 7
            return;
        }
        button.interactable = false;
    }
}
