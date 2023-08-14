using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using DG.Tweening;
using Com.LuisPedroFonseca.ProCamera2D;
using UnityEngine.EventSystems;
using TMPro;
using Sirenix.OdinInspector;

public class UIManager : Singleton<UIManager>
{
    [HideInInspector] public ProCamera2DTransitionsFX fx;
    public GameObject rayCastPannel;
    [Title("메인메뉴", titleAlignment: TitleAlignments.Centered)]
    public MainShop mainShopPannel;
    public Button[] selectStageBtn;
    public Button[] selectLevelBtn;

    [Title("인 게임", titleAlignment: TitleAlignments.Centered)]
    public GameObject clickTile;
    public GatchaPannel gatchaPannel;
    public CharacCardPannel characCardPannel;
    public Info info;
    public CharacInfo characInfo;
    public Tutiorial tutorialPannel;
    public UpPannel upPannel;
    public QuestPannel questPannel;
    public LevelUpBtn levelUpBtn;
    public PausePannel pausePannel;
    public CharacteristicCard[] cards;
    public GameObject slotObj;
    public Text gameSpeedTxt;
    public GameOverPannel gameOverPannel;
    public TextMeshProUGUI helpTxt;

    protected override void Awake()
    {
        base.Awake();

        fx = Camera.main.GetComponent<ProCamera2DTransitionsFX>();
    }

    private void Start()
    {
        fx.TransitionEnter();
        
        if(SceneManager.GetActiveScene().buildIndex != 0)
        {
            if (GameManager.Instance.is30)
                pausePannel.frameBtn.SetActive(true);
            else
                pausePannel.frameBtn.SetActive(false);

            upPannel.UpdateUpPannel();
        }
    }

    public void PopUp(TextMeshPro meshPro)
    {
        meshPro.transform.DOMove(meshPro.transform.position + new Vector3(0f, 1f, 0f), 0.5f).SetEase(Ease.OutQuad).SetDelay(0.1f).OnComplete(() => //0.5f -> moveDuration
        {
            meshPro.DOFade(0f, 0.5f) //0.5f -> moveDuration
            .OnComplete(() =>
            {
                SpawnManager.Instance.DeSpawnDmgText(meshPro);
            });
        });
    }

    public void PopUp(TextMeshProUGUI meshPro, float endValue)
    {
        meshPro.alpha = 1f;
        meshPro.rectTransform.DOAnchorPosY(endValue, 0.5f).SetEase(Ease.OutQuad).SetUpdate(true).SetDelay(0.1f).OnComplete(() => //0.5f -> moveDuration
        {
            meshPro.DOFade(0f, 0.5f).SetUpdate(true) //0.5f -> moveDuration
            .OnComplete(() =>
            {
                meshPro.gameObject.SetActive(false);
                meshPro.rectTransform.anchoredPosition -= new Vector2(0, 200f);
            });
        });
    }

    public void InitCard()
    {
        int ranID;
        List<int> IDs = new List<int>(); //중복 방지 리스트
        int length = cards.Length;
        CharacteristicStat[] stats = CSVReader.Instance.csvList.characteristicStat;
        for (int i = 0; i < length; i++)
        {
            ranID = UnityEngine.Random.Range(0, stats.Length);
            if (IDs.Contains(ranID) || stats[ranID].level >= 8) // 중복이거나 만렙이면 다시
                i--;
            else
            {
                IDs.Add(ranID);
                cards[i].SetCard(ranID); //퀘스트 생성
            }
        }
    }

    public void CloseInfoClickTile()
    {
        clickTile.SetActive(false); // 클릭 타일 제거
        info.TabInfo(false); // 정보 창 닫기
    }

    public Color TierColor(Tier tier)
    {
        Color exportColor = Color.white;
        switch (tier) // 티어 색상
        {
            case Tier.Common:
                exportColor = Color.green;
                break;
            case Tier.Rare:
                exportColor = Color.blue;
                break;
            case Tier.Epic:
                exportColor = Color.magenta;
                break;
            case Tier.Legend:
                exportColor = Color.yellow;
                break;
            case Tier.Hidden:
                exportColor = Color.gray;
                break;
        }
        return exportColor;
    }
}
[Serializable]
public class MainShop
{
    public Text title, explain, cost, curMoney;
    public Text[] shopLevel;
    public Image icon;
    public int curSelect;

    public void UpdateSelectUI()
    {
        CSVList cl = CSVReader.Instance.csvList;
        ShopStat[] shops = cl.shopStat;

        // SelectPannel

        foreach (ShopStat shop in shops)
        {
            int shopIndex = shop.index;

            if (shop.curLevel < shop.maxLevel)
                shopLevel[shopIndex].text = $"Lv {shop.curLevel}";
            else
            {
                shopLevel[shopIndex].text = "Max";
                GPGSBinder.Inst.ETCAchievement(RTD.achievement_flex); // 업적 FLEX
            }
        }

        cost.text = cl.CalCost(cl.FindShop(curSelect)).ToString();
        UpdateCurMoney();
    }

    public void UpdateShopUI(ShopStat stat)
    {
        title.text = stat.shopName;
        icon.sprite = EventSystem.current.currentSelectedGameObject.transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<Image>().sprite;
        explain.text = string.Format(stat.explanation, stat.condition);
        cost.text = CSVReader.Instance.csvList.CalCost(stat).ToString();
        UpdateCurMoney();
    }

    public void UpdateCurMoney()
    {
        curMoney.text = GameManager.Instance.curMoney.ToString();
    }
}

[Serializable]
public class Info
{
    public RectTransform pannelTrans;
    public UnitInfo unitInfo;
    public EnemyInfo enemyInfo;

    public void SetUnitInfo(TileGround tile)
    {
        // 패널 활성화
        TabInfo(true);
        enemyInfo.pannel.SetActive(false);
        unitInfo.pannel.SetActive(true);
        enemyInfo.select = null;

        // 텍스트 동기화
        unitInfo.select = tile.GetComponentInChildren<Unit>();

        unitInfo.img.sprite = unitInfo.select.defSprite; // 이미지
        unitInfo.icon.sprite = SpriteData.Instance.typeIcon[(int)unitInfo.select.unitStat.unitType]; // 타입
        unitInfo.nameTxt.text = unitInfo.select.unitStat.unitName; // 이름
        unitInfo.nameTxt.color = UIManager.Instance.TierColor(unitInfo.select.unitStat.tier); // 이름 색 (티어)

        UpdateUnitUI();
    }

    public void UpdateUnitUI()
    {
        if (!pannelTrans.gameObject.activeSelf || unitInfo.select == null)
            return;
        unitInfo.dmgTxt.text = unitInfo.select.unitStat.dmg.ToString(); // 데미지
        unitInfo.atkSpeedTxt.text = $"{unitInfo.select.unitStat.atkSpeed}"; // 공속
        unitInfo.rangeTxt.text = unitInfo.select.unitStat.range.ToString(); // 사거리
        unitInfo.critPercentTxt.text = $"{unitInfo.select.unitStat.critPercent}%"; // 치명타 확률
        unitInfo.critDmgTxt.text = $"{unitInfo.select.unitStat.critDmg}%"; // 치명타 데미지
        unitInfo.totalDmgTxt.text = unitInfo.select.totalDmg.ToString("N2"); // 총 데미지

        unitInfo.unitRange.OnAtkRange(unitInfo.select.transform.position, unitInfo.select.unitStat.range); // 사거리 표시 UI
    }

    public void SetEnemyInfo(Enemy enemy)
    {
        // 패널 활성화
        TabInfo(true);
        unitInfo.unitRange.OffAtkRange();
        unitInfo.pannel.SetActive(false);
        enemyInfo.pannel.SetActive(true);
        unitInfo.select = null;

        enemyInfo.select = enemy;

        // 텍스트 동기화
        enemyInfo.img.sprite = enemy.defSprite; // 이미지
        enemyInfo.nameTxt.text = enemy.enemyStat.enemyName; // 이름
        enemyInfo.nameTxt.color = UIManager.Instance.TierColor(enemy.enemyStat.tier); // 이름 색 (티어)

        UpdateEnemyUI();
    }

    public void UpdateEnemyUI()
    {
        if (!pannelTrans.gameObject.activeSelf || enemyInfo.select == null)
            return;

        enemyInfo.hpTxt.text = $"{(int)enemyInfo.select.curHp}/{(int)enemyInfo.select.enemyStat.hp}"; // 체력
        enemyInfo.moveSpeedTxt.text = enemyInfo.select.enemyStat.moveSpeed.ToString(); // 이동속도
        enemyInfo.defenseTxt.text = enemyInfo.select.enemyStat.defense.ToString(); // 방어력
        enemyInfo.strengthTxt.text = enemyInfo.select.enemyStat.strength.ToString(); // 강인함
    }

    public void TabInfo(bool b)
    {
        if (b)
        {
            pannelTrans.gameObject.SetActive(true);
            pannelTrans.DOAnchorPosY(-140, 0.5f).SetEase(Ease.OutBounce).SetUpdate(true);
        }
        else
        {
            pannelTrans.DOAnchorPosY(-600, 0.25f).SetEase(Ease.InBack).SetUpdate(true).OnComplete(() => pannelTrans.gameObject.SetActive(false));
            enemyInfo.select = null;
            unitInfo.select = null;
            unitInfo.unitRange.OffAtkRange();
        }

    }
}

[Serializable]
public struct UnitInfo
{
    public GameObject pannel;
    public Image img, icon;
    public UnitRange unitRange;
    public Text nameTxt, dmgTxt, atkSpeedTxt, rangeTxt, critPercentTxt, critDmgTxt, totalDmgTxt;

    [HideInInspector]public Unit select;
}

[Serializable]
public struct EnemyInfo
{
    public GameObject pannel;
    public Image img;
    public Text nameTxt, hpTxt, moveSpeedTxt, defenseTxt, strengthTxt;

    [HideInInspector]public Enemy select;
}

[Serializable]
public class Tutiorial
{
    public GameObject pannel;
    public Text text;
    public int textId = 0;
    public Button btn;

    public RectTransform[] trans;

    public IEnumerator DoTutorial()
    {
        UIManager um = UIManager.Instance;
        SpawnManager sm = SpawnManager.Instance;
        GameManager gm = GameManager.Instance;

        pannel.SetActive(true);
        btn.interactable = false;

        Time.timeScale = 0;
        DotText("유닛을 소환하고 싶은\n<color=green>타일</color>을 클릭해봐!");

        yield return new WaitUntil(() => um.clickTile.activeSelf);

        DotText("<color=brown>유닛 뽑기 버튼</color>을\n클릭해볼래?");
        DotRect(0, true);

        yield return new WaitUntil(() => um.gatchaPannel.gatchaPannel.activeSelf);

        DotRect(0, false);
        Time.timeScale = (int)gm.gameTime / 10f;
        DotText("<color=brown>Push버튼</color>을 <color=red>눌러!</color>");
        yield return new WaitForSecondsRealtime(1.5f);
        DotText("<color=red>두</color><color=yellow>구</color><color=green>두</color><color=blue>구</color>");

        yield return new WaitUntil(() => sm.unitList.Count >= 1);

        btn.interactable = true;
        Time.timeScale = 0;
        DotText("나를 클릭해서\n<color=red>대화</color>를 이어가.");
        yield return new WaitWhile(() => textId == 0);
        DotText("유닛 소환 시 <color=brown>가챠 키\n1개</color>가 소모 돼.");
        yield return new WaitWhile(() => textId == 1);
        DotText("가챠 키 보유량은 <color=brown>왼쪽\n위</color>에서 확인 할 수 있어.");
        yield return new WaitWhile(() => textId == 2);
        DotText("같은 방법으로 유닛을\n하나 더 소환해볼래?");
        Time.timeScale = (int)gm.gameTime / 10f;
        btn.interactable = false;

        yield return new WaitUntil(() => sm.unitList.Count >= 2);

        btn.interactable = true;
        Time.timeScale = 0;
        DotText("오! <color=brown>같은 유닛</color>이 나왔네!");
        yield return new WaitWhile(() => textId == 3);
        DotText("<color=brown>같은 유닛</color>을 합쳐볼까?");
        yield return new WaitWhile(() => textId == 4);
        DotText("두 유닛 중 하나를 클릭\n하고, 유닛 정보창에 있는\n<color=brown>+버튼</color>을 눌러볼래?");
        Time.timeScale = (int)gm.gameTime / 10f;
        btn.interactable = false;

        yield return new WaitUntil(() => um.info.unitInfo.pannel.activeSelf);
        DotRect(1, true);

        yield return new WaitUntil(() => sm.unitList.Count < 2);

        btn.interactable = true;
        DotRect(1, false);
        DotText("적을 처치할 때마다\n<color=green>경험치</color>가 쌓이고 <color=blue>레벨</color>이\n올라.");
        yield return new WaitWhile(() => textId == 5);
        DotText("<color=blue>레벨</color>이 오르면 <color=green>특성</color>을\n선택할 수 있어!");
        yield return new WaitWhile(() => textId == 6);
        DotText("참고로 <color=green>경험치</color>와 <color=blue>레벨</color>은\n왼쪽 위에서 볼 수 있어.");
        yield return new WaitWhile(() => textId == 7);
        DotText("일단 레벨을 1 올려줄 테니\n<color=green>특성Up! 버튼</color>을 눌러봐");
        DotRect(2, true);
        gm.LevelUp(um);
        btn.interactable = false;

        yield return new WaitUntil(() => um.characCardPannel.pannel.activeSelf);

        DotRect(2, false);
        DotText("원하는 <color=green>특성</color>을 선택해~");

        yield return new WaitUntil(() => !um.rayCastPannel.activeSelf);

        btn.interactable = true;
        DotText("적이 최종 지점에 다다르면\n오른쪽 위 <color=red>체력</color>이 깎이고");
        yield return new WaitWhile(() => textId == 8);
        DotText("0미만이 되면 <color=red>게임오버</color>야.");
        yield return new WaitWhile(() => textId == 9);
        DotText("<color=green>퀘스트 버튼</color>을 눌러볼래?");
        DotRect(3, true);
        btn.interactable = false;

        yield return new WaitUntil(() => um.questPannel.pannel.activeSelf);

        btn.interactable = true;
        DotRect(3, false);
        DotText("<color=green>퀘스트</color>는 클리어 시\n<color=white>하얀색</color>으로 변하고,");
        yield return new WaitWhile(() => textId == 10);
        DotText("클릭 시 <color=brown>보상</color>을\n받을 수 있어.");
        yield return new WaitWhile(() => textId == 11);
        DotText("새로운 <color=green>퀘스트</color>를 받고\n싶다면, 왼쪽 <color=gray>리롤\n버튼</color>을 누르면 돼.");
        yield return new WaitWhile(() => textId == 12);
        DotText("<color=gray>리롤 버튼</color>의 쿨은 <color=brown>3웨이브</color>\n마다 돌아.");
        yield return new WaitWhile(() => textId == 13);
        DotText("뭐 이 정도면 된거같네!");
        yield return new WaitWhile(() => textId == 14);
        DotText("그럼 <color=brown>실전</color>에서 봐~~ 안녕!");
        btn.interactable = false;
        yield return new WaitForSecondsRealtime(2f);

        GPGSBinder.Inst.ETCAchievement(RTD.achievement_4); // 업적 4
        gm.isTutorial = false;
        BtnManager.Instance.TitleFX();
    }

    private void DotText(string s)
    {
        text.text = "";
        text.DOText(s, 1f, true).SetUpdate(true);
    }

    private void DotRect(int i, bool isScale)
    {
        if (isScale)
            trans[i].DOScale(1.5f, 0.5f).SetEase(Ease.InExpo).SetEase(Ease.OutBounce).SetUpdate(true);
        else
            trans[i].DOScale(1f, 0.5f).SetEase(Ease.InOutExpo).SetUpdate(true);
    }
}

[Serializable]
public struct PausePannel
{
    public GameObject frameBtn;
}

[Serializable]
public class UpPannel
{
    public CurWavePannel curWavePannel;
    public Text gatchaKeyTxt, healthTxt, waveTxt, timerTxt, levelTxt;
    public Slider expSlider;

    public void UpdateUpPannel()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0) // 씬이 0이면 함수 실행 x
            return;
        GameManager gm = GameManager.Instance;

        gatchaKeyTxt.text = gm.gatchaKey.ToString(); // UI
        healthTxt.text = $"{gm.curHealth}/{gm.maxHealth}"; // 체력
        levelTxt.text = $"{gm.expLevel}Lv:"; // 레벨

        SetWaveTxt(gm); // 웨이브

        expSlider.value = 0;
    }

    public void SetWaveTxt(GameManager gm)
    {
        if (gm.level == Level.Infinite)
            waveTxt.text = WaveManager.Instance.curWave.ToString();
        else
            waveTxt.text = $"{WaveManager.Instance.curWave}/{gm.maxWave}";
    }

    public IEnumerator EXPHandlerCoroutine(GameManager gm)
    {
        float imsi = gm.curExp / gm.maxExp;
        float elapsedTime = 0f;
        float duration = 0.5f; // 변경되는 시간 간격

        float startValue = expSlider.value; // 시작 값 저장

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            expSlider.value = Mathf.Lerp(startValue, imsi, elapsedTime / duration);
            yield return null;
        }

        expSlider.value = imsi; // 최종 값을 설정하여 정확히 일치하도록 합니다.
    }

    public void ToggleCurWavePannel(bool b)
    {
        if (b)
        {
            curWavePannel.text.text = $"×{WaveManager.Instance.maxWaveEnemy}";
            curWavePannel.trans.DOAnchorPosY(-200, 0.5f).SetEase(Ease.OutBounce).SetUpdate(true);
        }
        else
            curWavePannel.trans.DOAnchorPosY(-50, 0.5f).SetEase(Ease.InBack).SetUpdate(true);
    }

    public void UpdateCurWave(int a)
    {
        WaveManager wm = WaveManager.Instance;

        wm.maxWaveEnemy += a;
        curWavePannel.text.text = $"×{wm.maxWaveEnemy}";
    }
}

[Serializable]
public struct CurWavePannel
{
    public RectTransform trans;
    public Text text;
}

[Serializable]
public class QuestPannel
{
    public GameObject pannel, notice;
    public Quest[] quests;
    public Button refreshBtn;
    public Text coolTxt;
    public int maxCool, curCool;

    public void QuestCool()
    {
        coolTxt.text = string.Format("{0}/{1}", curCool, maxCool); // 퀘스트 쿨

        if (curCool >= maxCool)
            refreshBtn.interactable = true;
    }
}

[Serializable]
public struct GatchaPannel
{
    public GameObject gatchaPannel;
    public Button gatchaBtn;
    public Button gatchaTab;
}

[Serializable]
public struct CharacCardPannel
{
    public GameObject pannel, miniPannel;
    public Text btnTxt;
}

[Serializable]
public struct CardPannel
{
    public Image cardImg;
    public Text cardNameTxt, explanationTxt;
    public Image cardIconImg;
    public Image[] starImg;
}

[Serializable]
public class CharacInfo
{
    public RectTransform pannel;
    public List<CharacSlot> characSlots;

    public void CharacInfoPannel(CharacteristicCard card)
    {
        CharacSlot slot = characSlots.Find(slot => slot.name == card.characStat.charateristicName);

        // 새로운 특성 얻을 시
        if(slot == null)
        {
            SpawnManager sm = SpawnManager.Instance;

            Transform characSlot = sm.SpawnUI("CharacSlot");

            slot = characSlot.GetComponent<CharacSlot>();
            slot.name = card.characStat.charateristicName;

            characSlots.Add(slot);
        }
        // 기존에 존재하는 특성 얻을 시
        slot.SetUI(card);
    }
}

[Serializable]
public class LevelUpBtn
{
    public Button btn;
    public GameObject img;
    public Text text;

    public void SetUI()
    {
        GameManager gm = GameManager.Instance;
        if (gm.levelPoint > 0)
        {
            img.SetActive(true);
            btn.interactable = true;
            text.text = gm.levelPoint.ToString();
        }
        else
        {
            img.SetActive(false);
            btn.interactable = false;
        }
    }
}

[Serializable]
public class GameOverPannel
{
    public GameObject pannel;
    public Text title;
    public Text killEnemyNum, curLevel, gatchaKey, curWave, level;
    public Text total;

    public void SetUI(bool isClear)
    {
        GameManager gm = GameManager.Instance;

        title.text = isClear ? "클리어" : "게임오버";

        int killNum = gm.killEnemyNum;
        int expLevel = gm.expLevel;
        int gatcha = gm.gatchaKey;
        int wave = WaveManager.Instance.curWave;
        Level gameLevel = gm.level;
        int totalNum = (killNum + expLevel + gatcha + wave) * ((int)gameLevel + 1);

        killEnemyNum.text = $"처치한 적 수: +{killNum}";
        curLevel.text = $"현재 레벨: +{expLevel}";
        gatchaKey.text = $"남은 가챠 키: +{gatcha}";
        curWave.text = $"현재 웨이브: +{wave}";
        string levelTxt = "난이도: ";
        switch(gameLevel)
        {
            case Level.Easy:
                levelTxt += "쉬움(×1.0)";
                break;
            case Level.Normal:
                levelTxt = "보통(×2.0)";
                break;
            case Level.Hard:
                levelTxt = "어려움(×3.0)";
                break;
            case Level.Infinite:
                levelTxt = "무한(×4.0)";
                break;
        }
        level.text = levelTxt;

        total.text = $"= {totalNum}";

        gm.EarnMoney(totalNum);

        BtnManager.Instance.Tab(pannel);
        BtnManager.Instance.Pause();
    }
}