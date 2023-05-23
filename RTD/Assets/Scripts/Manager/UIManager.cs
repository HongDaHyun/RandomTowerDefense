using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using DG.Tweening;

public class UIManager : Singleton<UIManager>
{
    [Header("MainMenu")]
    public GameObject[] mainPannel;
    public Text[] levelWaveTxt;
    [Header("InGame")]
    public GameObject[] inGamePannel;
    public GameObject clickTile;
    public UnitRange unitRange;
    public GameObject unitInfoPannel;
    public GameObject enemyInfoPannel;
    public GatchaPannel gatchaPannel;
    public GameObject characteristicPannel;
    public UnitInfo unitInfoContents;
    public UpPannel upPannel;
    public Quest[] quests;
    public CharacteristicCard[] cards;
    public GameObject slotObj;
    public Text gameSpeedTxt;

    private void Update()
    {
        UpdateUpPannel();
    }

    public void SetUnitInfo(TileGround tile)
    {
        if (!unitInfoPannel.activeSelf)
            unitInfoPannel.SetActive(true);
        Unit unit = tile.GetComponentInChildren<Unit>();
        unitInfoContents.img.sprite = unit.GetComponent<SpriteRenderer>().sprite;
        unitInfoContents.texts[0].text = unit.unitStat.unitName;
        unitInfoContents.texts[1].text = unit.unitStat.dmg.ToString();
        unitInfoContents.texts[2].text = $"{unit.unitStat.atkSpeed}%";
        unitInfoContents.texts[3].text = unit.unitStat.range.ToString();
        unitInfoContents.texts[4].text = $"{unit.unitStat.critPercent}%";
        unitInfoContents.texts[5].text = $"{unit.unitStat.critDmg}%";
        unitRange.OnAtkRange(unit.transform.position, unit.unitStat.range);
    }

    public void UpdateUpPannel()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0) //씬이 0이면 함수 실행 x
            return;
        upPannel.gatchaKeyTxt.text = GameManager.Instance.gatchaKey.ToString();
        upPannel.healthTxt.text = $"{GameManager.Instance.curHealth}/{GameManager.Instance.maxHealth}";
        upPannel.waveTxt.text = $"{WaveManager.Instance.curWave}/{GameManager.Instance.maxWave}";

        ExpSlider();
    }

    public void ExpSlider()
    {
        float imsi = (float)GameManager.Instance.curExp / GameManager.Instance.maxExp;
        upPannel.expSlider.value = Mathf.Lerp(upPannel.expSlider.value, imsi, Time.deltaTime * 10);

        LevelUp();
    }

    private void LevelUp()
    {
        if (GameManager.Instance.curExp >= GameManager.Instance.maxExp)
        {
            GameManager.Instance.curExp = 0;
            GameManager.Instance.expLevel++;
            upPannel.levelTxt.text = $"{GameManager.Instance.expLevel}Lv:"; // 레벨UI

            characteristicPannel.SetActive(true); // 특성 패널 활성화
            InitCard(); // 카드 생성

            BtnManager.Instance.Pause(); //정지
        }
    }

    public IEnumerator WaveTimer()
    {
        float readyTime = 10f;

        while (readyTime > 0) // 타이머 시작
        {
            readyTime -= Time.deltaTime;
            upPannel.timerTxt.text = readyTime.ToString("00.0");
            yield return null;

            if (readyTime <= 0) //웨이브 시작(타이머 0)
            {
                SpawnManager.Instance.SpawnEnemy();
            }
        }
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
        for (int i = 0; i < cards.Length; i++)
        {
            ranID = UnityEngine.Random.Range(0, CSVReader.Instance.csvList.characteristicStat.Length);
            if (IDs.Contains(ranID)) // 중복이면 다시
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
public struct UnitInfo
{
    public Image img;
    public Text[] texts;
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