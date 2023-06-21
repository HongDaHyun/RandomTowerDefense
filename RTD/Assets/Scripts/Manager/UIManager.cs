using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using DG.Tweening;

public class UIManager : Singleton<UIManager>
{
    GameManager gm;
    [Header("MainMenu")]
    public GameObject[] mainPannel;
    public Text[] levelWaveTxt;
    [Header("InGame")]
    public GameObject[] inGamePannel;
    public GameObject clickTile;
    public GatchaPannel gatchaPannel;
    public GameObject characteristicPannel;
    public Info info;
    public UpPannel upPannel;
    public Quest[] quests;
    public CharacteristicCard[] cards;
    public GameObject slotObj;
    public Text gameSpeedTxt;
    public GameObject gameOverPannel;

    private void Start()
    {
        gm = GameManager.Instance;
    }

    private void Update()
    {
        UpdateUpPannel();
    }

    public void UpdateUpPannel()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0) // 씬이 0이면 함수 실행 x
            return;
        upPannel.gatchaKeyTxt.text = gm.gatchaKey.ToString(); // 가챠 키
        upPannel.healthTxt.text = $"{gm.curHealth}/{gm.maxHealth}"; // 체력
        upPannel.timerTxt.text = WaveManager.Instance.readyTime.ToString("00.0"); // 타이머
        upPannel.levelTxt.text = $"{gm.expLevel}Lv:"; // 레벨
        if (gm.level == Level.Infinite)
            upPannel.waveTxt.text = WaveManager.Instance.curWave.ToString(); // 웨이브
        else
            upPannel.waveTxt.text = $"{WaveManager.Instance.curWave}/{gm.maxWave}"; // 웨이브

        ExpSlider();
    }

    public void ExpSlider()
    {
        float imsi = gm.curExp / gm.maxExp;
        upPannel.expSlider.value = Mathf.Lerp(upPannel.expSlider.value, imsi, Time.deltaTime * 10);
    }

    public void DmgPopUp(float dmg, Vector3 startTrans, bool isCrit)
    {
        TMPro.TextMeshPro dmgText = SpawnManager.Instance.SpawnDmgText(dmg);

        if (isCrit) // 크리티컬 색
            dmgText.color = Color.yellow;
        else
            dmgText.color = Color.white;

        dmgText.transform.position = startTrans;
        dmgText.transform.DOMove(startTrans + new Vector3(0f, 1f, 0f), 0.5f).SetEase(Ease.OutQuad).SetDelay(0.1f).OnComplete(() => //0.5f -> moveDuration
        {
            dmgText.DOFade(0f, 0.5f) //0.5f -> moveDuration
            .OnComplete(() =>
            {
                SpawnManager.Instance.DeSpawnDmgText(dmgText);
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
}

[Serializable]
public class Info
{
    public GameObject pannel;
    public UnitInfo unitInfo;
    public EnemyInfo enemyInfo;

    public void SetUnitInfo(TileGround tile)
    {
        // 패널 활성화
        pannel.SetActive(true);
        enemyInfo.pannel.SetActive(false);
        unitInfo.pannel.SetActive(true);

        // 텍스트 동기화
        Unit unit = tile.GetComponentInChildren<Unit>();

        unitInfo.img.sprite = unit.GetComponent<SpriteRenderer>().sprite;
        unitInfo.nameTxt.text = unit.unitStat.unitName;
        unitInfo.dmgTxt.text = unit.unitStat.dmg.ToString();
        unitInfo.atkSpeedTxt.text = $"{unit.unitStat.atkSpeed}%";
        unitInfo.rangeTxt.text = unit.unitStat.range.ToString();
        unitInfo.critPercentTxt.text = $"{unit.unitStat.critPercent}%";
        unitInfo.critDmgTxt.text = $"{unit.unitStat.critDmg}%";
        unitInfo.unitRange.OnAtkRange(unit.transform.position, unit.unitStat.range);
    }

    public void SetEnemyInfo(Enemy enemy)
    {
        // 패널 활성화
        pannel.SetActive(true);
        unitInfo.unitRange.OffAtkRange();
        unitInfo.pannel.SetActive(false);
        enemyInfo.pannel.SetActive(true);

        // 텍스트 동기화
        enemyInfo.img.sprite = enemy.GetComponent<SpriteRenderer>().sprite;
        enemyInfo.nameTxt.text = enemy.enemyStat.enemyName;
        enemyInfo.hpTxt.text = enemy.enemyStat.hp.ToString();
        enemyInfo.moveSpeedTxt.text = enemy.enemyStat.moveSpeed.ToString();
        enemyInfo.defenseTxt.text = enemy.enemyStat.defense.ToString();
        enemyInfo.strengthTxt.text = enemy.enemyStat.strength.ToString();
    }

    public void CloseInfo()
    {
        pannel.SetActive(false);
        unitInfo.unitRange.OffAtkRange();
    }
}

[Serializable]
public struct UnitInfo
{
    public GameObject pannel;
    public Image img;
    public UnitRange unitRange;
    public Text nameTxt, dmgTxt, atkSpeedTxt, rangeTxt, critPercentTxt, critDmgTxt;
}

[Serializable]
public struct EnemyInfo
{
    public GameObject pannel;
    public Image img;
    public Text nameTxt, hpTxt, moveSpeedTxt, defenseTxt, strengthTxt;
}

[Serializable]
public struct UpPannel
{
    public Text gatchaKeyTxt, healthTxt, waveTxt, timerTxt, levelTxt;
    public Slider expSlider;
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