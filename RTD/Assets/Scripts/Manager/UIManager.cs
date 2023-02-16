using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class UIManager : Singleton<UIManager>
{
    [Header("MainMenu")]
    public GameObject[] mainPannel;
    [Header("InGame")]
    public GameObject[] inGamePannel;
    public GameObject clickTile;
    public GameObject unitInfoPannel;
    public GameObject enemyInfoPannel;
    public UnitInfo unitInfoContents;
    public UpPannel upPannel;
    public GameObject gatchaPannel;
    public GameObject slotObj;

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
    }

    public void UpdateUpPannel()
    {
        upPannel.gatchaKeyTxt.text = GameManager.Instance.gatchaKey.ToString();
        upPannel.healthTxt.text = $"{GameManager.Instance.curHealth}/{GameManager.Instance.maxHealth}";
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
    public Text gatchaKeyTxt, healthTxt;
}
