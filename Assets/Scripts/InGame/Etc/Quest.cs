using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Quest : MonoBehaviour
{
    public QuestStat questStat;
    public int curCondition;
    private Tier tier;
    private int reward;

    public TextMeshProUGUI[] texts;
    public Button button;

    private void Update()
    {
        texts[1].text = string.Format(questStat.explanation, curCondition, questStat.condition); // 설명
    }

    public void SetQuest(int id)
    {
        GameManager gm = GameManager.Instance;

        // 퀘스트 불러오기
        questStat = CSVReader.Instance.csvList.FindQuest(id);

        // Reset
        curCondition = 0;
        button.interactable = false;

        // 티어 생성
        int ranTier = Random.Range(0, gm.probability[0] + gm.probability[1] + gm.probability[2] + gm.probability[3]);
        if (ranTier < gm.probability[0])
            tier = Tier.Common;
        else if (ranTier < gm.probability[0] + gm.probability[1])
            tier = Tier.Rare;
        else if (ranTier < gm.probability[0] + gm.probability[1] + gm.probability[2])
            tier = Tier.Epic;
        else
            tier = Tier.Legend;

        // 텍스트 설정
        texts[0].text = questStat.questName; // 제목
        texts[0].color = UIManager.Instance.TierColor(tier); // 제목 색상

        int plus = CharacManager.Instance.plusReward; // Charac 12

        reward = 1 * (int)tier + plus; // 티어에 따른 보상 증가
        questStat.condition += questStat.conditionWidth * ((int)tier - 1); // 티어에 따른 조건 증가 (MaxCondition)

        // rewardText 설정
        if (plus == 0)
            texts[2].text = string.Format("키 {0} 증가", reward);
        else
            texts[2].text = string.Format("키 {0} + {1} 증가", reward - plus, plus);

        CheckSuccess();
    }

    public void DoQuest(int id)
    {
        if (button.interactable)
            return;

        if (questStat.ID == id)
            curCondition++;

        CheckSuccess();
    }

    public void DoQuest(int id, int size)
    {
        if (button.interactable)
            return;

        if (questStat.ID == id)
            curCondition += size;

        CheckSuccess();
    }

    private void CheckSuccess()
    {
        if (curCondition >= questStat.condition)
        {
            button.interactable = true;
            UIManager.Instance.questPannel.notice.SetActive(true); // 퀘스트 알림 활성화
            return;
        }
        button.interactable = false;
    }

    public void ClickQuest()
    {
        BtnManager.Instance.ClickSound();

        if (questStat.ID == 3)
            AdMobManager.Instance.ShowRewardAd(reward);
        
        else
            GameManager.Instance.GainGatcha(reward);

        QuestManager.Instance.UpQuest(6); // 퀘스트 6
        GPGSBinder.Inst.ETCAchievement(RTD.achievement_11); // 업적 11

        // 비활성화
        gameObject.SetActive(false);

        int activeQuestCount = 0;
        foreach (Quest quest in UIManager.Instance.questPannel.quests)
        {
            if (quest.gameObject.activeSelf)
            {
                activeQuestCount++;
            }
        }

        if (activeQuestCount <= 0)
            BtnManager.Instance.RefreshQuestBtn();
    }
}
