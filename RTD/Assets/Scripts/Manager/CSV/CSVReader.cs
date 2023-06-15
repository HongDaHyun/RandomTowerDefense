using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CSVReader : Singleton<CSVReader>
{
    public TextAsset[] textAssets;
    public CSVList csvList = new CSVList();
    public Projectile[] projectiles;


    #region Read
    [ContextMenu("CSV불러오기")]
    public void CSVReading()
    {
        UnitCSVReader();
        EnemyCSVReader();
        QuestCSVReader();
        CharacterisitcReader();
    }

    public void UnitCSVReader()
    {
        int order = 0; int size = 8;
        string[] data = textAssets[order].text.Split(new string[] { ",", "\n" }, StringSplitOptions.None);
        int tableSize = data.Length / size - 1;
        csvList.unitStat = new UnitStat[tableSize];

        for (int i = 0; i < tableSize; i++)
        {
            int k = i + 1;
            csvList.unitStat[i] = new UnitStat();
            csvList.unitStat[i].unitName = data[size * k];
            csvList.unitStat[i].unitType = (UnitType)Enum.Parse(typeof(UnitType), (data[size * k + 1]));
            csvList.unitStat[i].tier = (Tier)Enum.Parse(typeof(Tier), (data[size * k + 2]));
            csvList.unitStat[i].dmg = float.Parse(data[size * k + 3]);
            csvList.unitStat[i].critPercent = int.Parse(data[size * k + 4]);
            csvList.unitStat[i].critDmg = int.Parse(data[size * k + 5]);
            csvList.unitStat[i].atkSpeed = float.Parse(data[size * k + 6]);
            csvList.unitStat[i].range = float.Parse(data[size * k + 7]);
        }
    }

    public void EnemyCSVReader()
    {
        int order = 1; int size = 7;
        string[] data = textAssets[order].text.Split(new string[] { ",", "\n" }, StringSplitOptions.None);
        int tableSize = data.Length / size - 1;
        csvList.enemyStat = new EnemyStat[tableSize];

        for (int i = 0; i < tableSize; i++)
        {
            int k = i + 1;
            csvList.enemyStat[i] = new EnemyStat();
            csvList.enemyStat[i].enemyName = data[size * k];
            csvList.enemyStat[i].enemyType = (EnemyType)Enum.Parse(typeof(EnemyType), (data[size * k + 1]));
            csvList.enemyStat[i].tier = (Tier)Enum.Parse(typeof(Tier), (data[size * k + 2]));
            csvList.enemyStat[i].hp = float.Parse(data[size * k + 3]);
            csvList.enemyStat[i].strength = int.Parse(data[size * k + 4]);
            csvList.enemyStat[i].defense = float.Parse(data[size * k + 5]);
            csvList.enemyStat[i].moveSpeed = float.Parse(data[size * k + 6]);
        }
    }

    public void QuestCSVReader()
    {
        int order = 2; int size = 4;
        string[] data = textAssets[order].text.Split(new string[] { ",", "\n" }, StringSplitOptions.None);
        int tableSize = data.Length / size - 1;
        csvList.questStat = new QuestStat[tableSize];

        for (int i = 0; i < tableSize; i++)
        {
            int k = i + 1;
            csvList.questStat[i] = new QuestStat();
            csvList.questStat[i].questName = data[size * k];
            csvList.questStat[i].explanation = data[size * k + 1];
            csvList.questStat[i].condition = int.Parse(data[size * k + 2]);
            csvList.questStat[i].conditionWidth = int.Parse(data[size * k + 3]);
            csvList.questStat[i].ID = i;
        }
    }

    public void CharacterisitcReader()
    {
        int order = 3; int size = 4;
        string[] data = textAssets[order].text.Split(new string[] { ",", "\n" }, StringSplitOptions.None);
        int tableSize = data.Length / size - 1;
        csvList.characteristicStat = new CharacteristicStat[tableSize];

        for (int i = 0; i < tableSize; i++)
        {
            int k = i + 1;
            csvList.characteristicStat[i] = new CharacteristicStat();
            csvList.characteristicStat[i].charateristicName = data[size * k];
            csvList.characteristicStat[i].explanation = data[size * k + 1];
            csvList.characteristicStat[i].condition = int.Parse(data[size * k + 2]);
            csvList.characteristicStat[i].conditionWidth = int.Parse(data[size * k + 3]);
            csvList.characteristicStat[i].level = 0;
            csvList.characteristicStat[i].ID = i;
        }
    }
    #endregion

    #region Stat

    public void SetProjectileStat(UnitType type, string kind, float value)
    {
        switch(kind)
        {
            case "Size":
                projectiles[(int)type].transform.localScale += new Vector3(value, value);
                break;
            case "Speed":
                projectiles[(int)type].speed += value;
                break;
        }
    }
    #endregion
}