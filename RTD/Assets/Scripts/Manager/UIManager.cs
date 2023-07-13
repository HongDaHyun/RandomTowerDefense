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
    public Button[] selectLevelBtn;

    [Title("인 게임", titleAlignment: TitleAlignments.Centered)]
    public GameObject clickTile;
    public GatchaPannel gatchaPannel;
    public GameObject characteristicPannel;
    public List<CharacSlot> characSlots;
    public Info info;
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

    public void CharacInfoPannel(CharacteristicCard card)
    {
        CharacSlot slot = characSlots.Find(img => img.icon.sprite == card.cardPannel.cardIconImg.sprite);

        // 기존에 존재하는 특성 얻을 시
        if (slot != null)
            slot.SetUI(card);
        // 새로운 특성 얻을 시
        else
        {
            Transform characIcon = SpawnManager.Instance.SpawnUI("CharacIcon");
            slot = characIcon.GetComponent<CharacSlot>();
            slot.SetUI(card);
            characSlots.Add(slot);
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
                shopLevel[shopIndex].text = "Max";
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
public struct CardPannel
{
    public Text cardNameTxt, explanationTxt;
    public Image cardIconImg;
    public Image[] starImg;
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