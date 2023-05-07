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
        DebuffCSVReader();
        QuestCSVReader();
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
            csvList.unitStat[i].dmg = float.Parse(data[size * k + 3]);
            csvList.unitStat[i].critPercent = int.Parse(data[size * k + 4]);
            csvList.unitStat[i].critDmg = int.Parse(data[size * k + 5]);
            csvList.unitStat[i].atkSpeed = float.Parse(data[size * k + 6]);
            csvList.unitStat[i].range = float.Parse(data[size * k + 7]);
            csvList.unitStat[i].projectileSpeed = int.Parse(data[size * k + 8]);
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
            csvList.enemyStat[i].hp = float.Parse(data[size * k + 2]);
            csvList.enemyStat[i].strength = int.Parse(data[size * k + 3]);
            csvList.enemyStat[i].defense = float.Parse(data[size * k + 4]);
            csvList.enemyStat[i].moveSpeed = float.Parse(data[size * k + 5]);
        }
    }

    public void DebuffCSVReader()
    {
        int order = 2; int size = 2;
        string[] data = textAssets[order].text.Split(new string[] { ",", "\n" }, StringSplitOptions.None);
        int tableSize = data.Length / size - 1;
        csvList.debuffStat = new DebuffStat[tableSize];

        for (int i = 0; i < tableSize; i++)
        {
            int k = i + 1;
            csvList.debuffStat[i] = new DebuffStat();
            csvList.debuffStat[i].debuffName = data[size * k];
            csvList.debuffStat[i].ticTime = float.Parse(data[size * k + 1]);
        }
    }

    public void QuestCSVReader()
    {
        int order = 3; int size = 4;
        string[] data = textAssets[order].text.Split(new string[] { ",", "\n" }, StringSplitOptions.None);
        int tableSize = data.Length / size - 1;
        csvList.quest = new Quest[tableSize];

        for (int i = 0; i < tableSize; i++)
        {
            int k = i + 1;
            csvList.quest[i] = new Quest();
            csvList.quest[i].questName = data[size * k];
            csvList.quest[i].explanation = data[size * k + 1];
            csvList.quest[i].condition = int.Parse(data[size * k + 2]);
            csvList.quest[i].conditionWidth = int.Parse(data[size * k + 3]);
            csvList.quest[i].ID = i;
        }
    }
    #endregion

    #region Export
    public UnitStat UnitExport(string name) // 이름으로 유닛 정보
    {
        for (int i = 0; i < csvList.unitStat.Length; i++)
        {
            if (name == csvList.unitStat[i].unitName.ToString())
            {
                return csvList.unitStat[i];
            }
        }
        return csvList.unitStat[0];
    }

    public List<UnitStat> UnitExport_Type(UnitType type) // 타입으로 유닛 정보
    {
        List<UnitStat> unitStats = new List<UnitStat>();
        for (int i = 0; i < csvList.unitStat.Length; i++)
        {
            if (type == csvList.unitStat[i].unitType)
                unitStats.Add(csvList.unitStat[i]);
        }
        return unitStats;
    }

    public string UnitExport_forSpawn(UnitType type, Tier tier) // 타입과 티어로 유닛 정보
    {
        List<UnitStat> unitStats = UnitExport_Type(type);
        for(int i = 0; i < unitStats.Count; i++)
        {
            if(tier == unitStats[i].tier)
            {
                return unitStats[i].unitName;
            }
        }
        return unitStats[0].unitName;
    }    

    public EnemyStat EnemyExport(string name)
    {
        for (int i = 0; i < csvList.enemyStat.Length; i++)
        {
            if (name == csvList.enemyStat[i].enemyName.ToString())
            {
                return csvList.enemyStat[i];
            }
        }
        return csvList.enemyStat[0];
    }

    public DebuffStat DebuffExport(string name)
    {
        for(int i = 0; i < csvList.debuffStat.Length; i++)
        {
            if (name == csvList.debuffStat[i].debuffName.ToString())
                return csvList.debuffStat[i];
        }
        return csvList.debuffStat[0];
    }
    
    public void DebuffUp(int count)
    {
        for(int i = 0; i < csvList.debuffStat.Length; i++)
        {
            csvList.debuffStat[i].ticTime += count;
        }
    }

    public Quest QuestExport(int id)
    {
        for (int i = 0; i < csvList.quest.Length; i++)
        {
            if (id == csvList.quest[i].ID)
                return csvList.quest[i];
        }
        return csvList.quest[0];
    }
    #endregion

    #region Stat
    public void SetUnitStat(string kind, float value) // 구현 중
    {
        switch(kind)
        {
            case "Dmg":
                break;
            case "CritPercent":
                break;
            case "CritDmg":
                break;
            case "AtkSpeed":
                break;
            case "Range":
                break;
            case "ProjectileSpeed":
                break;
        }
    }
    #endregion
}