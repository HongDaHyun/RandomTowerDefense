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

    public QuestStat QuestExport(int id)
    {
        return csvList.questStat[id];
    }

    public CharacteristicStat CharacteristicExport(int id)
    {
        return csvList.characteristicStat[id];
    }

    public float AttributeExport(Attribute attribute)
    {
        for(int i = 0; i < csvList.attributeStat.Length; i++)
        {
            if (attribute == csvList.attributeStat[i].attribute)
                return csvList.attributeStat[i].attributePower;
        }
        return 0;
    }
    #endregion

    #region Stat
    public void SetUnitStat(UnitType type, string kind, float value) // 구현 중
    {
        // CSV 유닛 스탯
        int length = csvList.unitStat.Length;
        for (int i = 0; i < length; i++)
        {
            if (type == csvList.unitStat[i].unitType)
            {
                UpdateUnitStat(csvList.unitStat[i], kind, value);
            }
        }

        // 기존 소환된 유닛
        int unitListCount = SpawnManager.Instance.unitList.Count;
        for (int i = 0; i < unitListCount; i++)
        {
            if (type == SpawnManager.Instance.unitList[i].unitStat.unitType)
            {
                UpdateUnitStat(SpawnManager.Instance.unitList[i].unitStat, kind, value);
            }
        }
    }

    private void UpdateUnitStat(UnitStat unitStat, string kind, float value)
    {
        switch (kind)
        {
            case "Dmg":
                unitStat.dmg += value;
                break;
            case "CritPercent":
                unitStat.critPercent += (int)value;
                break;
            case "CritDmg":
                unitStat.critDmg += (int)value;
                break;
            case "AtkSpeed":
                unitStat.atkSpeed += value;
                break;
            case "Range":
                unitStat.range += value;
                break;
        }
    }

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