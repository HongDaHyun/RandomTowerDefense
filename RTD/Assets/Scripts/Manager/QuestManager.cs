using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : Singleton<QuestManager>
{
    [HideInInspector] public int killEnemyNum, questNum, gatchaNum; // 처치한 적 수, 퀘스트 수, 가챠 키 수
    [HideInInspector] public int evolNum; // 진화 수
    [HideInInspector] public List<UnitType> unitTypesList;
    [HideInInspector] public int[] tierNum = new int[] { 0, 0, 0, 0 };
    [HideInInspector] public int[] unitNum = new int[] { 0, 0, 0, 0, 0, 0, 0, 0 };

    public void ReadyQuest(QuestStat questStat, ref int readyCondition_1, ref int readyCondition_2)
    {
        switch (questStat.ID)
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
                killEnemyNum = 0;
                break;

            case 6:
                unitTypesList = new List<UnitType>();
                break;

            case 7:
                questNum = 0;
                break;

            case 8:
                readyCondition_1 = GameManager.Instance.curHealth;
                break;

            case 9:
                gatchaNum = 0;
                break;

            case 10:
            case 11:
            case 12:
            case 13:
                tierNum = new int[] { 0, 0, 0, 0 };
                break;

            case 15:
            case 16:
            case 17:
            case 18:
            case 19:
            case 20:
            case 21:
            case 22:
                unitNum = new int[] { 0, 0, 0, 0, 0, 0, 0, 0 };
                break;

            case 23:
                evolNum = 0;
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

    public void UpdateQuestCondition(QuestStat questStat, ref int readyCondition_1, ref int readyCondition_2, ref int curCondition)
    {
        switch (questStat.ID)
        {
            case 0:
                curCondition = WaveManager.Instance.curWave - readyCondition_1;
                break;

            case 1:
                if (readyCondition_2 > GameManager.Instance.curHealth) //데미지를 입을 시 초기화
                {
                    readyCondition_1 = WaveManager.Instance.curWave;
                    curCondition = 0;
                    break;
                }
                curCondition = WaveManager.Instance.curWave - readyCondition_1;
                break;

            case 2:
                curCondition = GameManager.Instance.gatchaKey;
                break;

            case 3:
                curCondition = GameManager.Instance.maxHealth - GameManager.Instance.curHealth;
                break;

            case 5:
                curCondition = killEnemyNum;
                break;

            case 6:
                curCondition = unitTypesList.Count;
                break;

            case 7:
                curCondition = questNum;
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
                curCondition = gatchaNum;
                break;

            case 10:
            case 11:
            case 12:
            case 13:
                curCondition = tierNum[questStat.ID - 10];
                break;

            case 15:
            case 16:
            case 17:
            case 18:
            case 19:
            case 20:
            case 21:
            case 22:
                curCondition = unitNum[questStat.ID - 15];
                break;

            case 23:
                curCondition = evolNum;
                break;

            case 24:
                curCondition = GameManager.Instance.maxHealth - readyCondition_1;
                break;

            case 25:
            //보스 제작중

            default: // 14(꽝)
                break;
        }
    }
}
