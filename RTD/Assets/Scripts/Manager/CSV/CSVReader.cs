using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CSVReader : Singleton<CSVReader>
{
    public TextAsset[] textAssets;
    public CSVList csvList = new CSVList();

    #region Read
    [ContextMenu("CSV불러오기")]
    public void CSVReading()
    {
        UnitCSVReader();
        EnemyCSVReader();
    }

    public void UnitCSVReader()
    {
        int order = 0; int size = 9;
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
            csvList.unitStat[i].attribute = (Attribute)Enum.Parse(typeof(Attribute), (data[size * k + 3]));
            csvList.unitStat[i].dmg = int.Parse(data[size * k + 4]);
            csvList.unitStat[i].critPercent = int.Parse(data[size * k + 5]);
            csvList.unitStat[i].critDmg = int.Parse(data[size * k + 6]);
            csvList.unitStat[i].atkSpeed = float.Parse(data[size * k + 7]);
            csvList.unitStat[i].range = float.Parse(data[size * k + 8]);
        }
    }

    public void EnemyCSVReader()
    {
        int order = 1; int size = 6;
        string[] data = textAssets[order].text.Split(new string[] { ",", "\n" }, StringSplitOptions.None);
        int tableSize = data.Length / size - 1;
        csvList.enemyStat = new EnemyStat[tableSize];

        for (int i = 0; i < tableSize; i++)
        {
            int k = i + 1;
            csvList.enemyStat[i] = new EnemyStat();
            csvList.enemyStat[i].enemyName = data[size * k];
            csvList.enemyStat[i].enemyType = (EnemyType)Enum.Parse(typeof(EnemyType), (data[size * k + 1]));
            csvList.enemyStat[i].hp = int.Parse(data[size * k + 2]);
            csvList.enemyStat[i].strength = int.Parse(data[size * k + 3]);
            csvList.enemyStat[i].defense = float.Parse(data[size * k + 4]);
            csvList.enemyStat[i].moveSpeed = float.Parse(data[size * k + 5]);
        }
    }
    #endregion

    #region Export
    public ref UnitStat UnitExport(string name)
    {
        for (int i = 0; i < csvList.unitStat.Length; i++)
        {
            if (name == csvList.unitStat[i].unitName.ToString())
            {
                return ref csvList.unitStat[i];
            }
        }
        return ref csvList.unitStat[0];
    }
    #endregion
}