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
        ProjectileCSVReader();
        DebuffCSVReader();
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
            csvList.unitStat[i].dmg = float.Parse(data[size * k + 4]);
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
            csvList.enemyStat[i].hp = float.Parse(data[size * k + 2]);
            csvList.enemyStat[i].strength = int.Parse(data[size * k + 3]);
            csvList.enemyStat[i].defense = float.Parse(data[size * k + 4]);
            csvList.enemyStat[i].moveSpeed = float.Parse(data[size * k + 5]);
        }
    }

    public void ProjectileCSVReader()
    {
        int order = 2; int size = 8;
        string[] data = textAssets[order].text.Split(new string[] { ",", "\n" }, StringSplitOptions.None);
        int tableSize = data.Length / size - 1;
        csvList.projectileStat = new ProjectileStat[tableSize];

        for (int i = 0; i < tableSize; i++)
        {
            int k = i + 1;
            csvList.projectileStat[i] = new ProjectileStat();
            csvList.projectileStat[i].projectileName = data[size * k];
            csvList.projectileStat[i].projectileType = (UnitType)Enum.Parse(typeof(UnitType), (data[size * k + 1]));
            csvList.projectileStat[i].dmg = float.Parse(data[size * k + 2]);
            csvList.projectileStat[i].critPercent = int.Parse(data[size * k + 3]);
            csvList.projectileStat[i].critDmg = int.Parse(data[size * k + 4]);
            csvList.projectileStat[i].atkSpeed = float.Parse(data[size * k + 5]);
            csvList.projectileStat[i].range = float.Parse(data[size * k + 6]);
            csvList.projectileStat[i].projectileSpeed = float.Parse(data[size * k + 7]);
        }
    }

    public void DebuffCSVReader()
    {
        int order = 3; int size = 2;
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

    public ref string UnitExport_forSpawn(UnitType type, Tier tier)
    {
        for(int i = 0; i < csvList.unitStat.Length; i++)
        {
            if(type == csvList.unitStat[i].unitType && tier == csvList.unitStat[i].tier)
            {
                return ref csvList.unitStat[i].unitName;
            }
        }
        return ref csvList.unitStat[0].unitName;
    }    

    public ref EnemyStat EnemyExport(string name)
    {
        for (int i = 0; i < csvList.enemyStat.Length; i++)
        {
            if (name == csvList.enemyStat[i].enemyName.ToString())
            {
                return ref csvList.enemyStat[i];
            }
        }
        return ref csvList.enemyStat[0];
    }

    public ref ProjectileStat ProjectileExport(string name)
    {
        for (int i = 0; i < csvList.projectileStat.Length; i++)
        {
            if (name == csvList.projectileStat[i].projectileName.ToString())
            {
                return ref csvList.projectileStat[i];
            }
        }
        return ref csvList.projectileStat[0];
    }

    public ref DebuffStat DebuffExport(string name)
    {
        for(int i = 0; i < csvList.debuffStat.Length; i++)
        {
            if (name == csvList.debuffStat[i].debuffName.ToString())
                return ref csvList.debuffStat[i];
        }
        return ref csvList.debuffStat[0];
    }
    #endregion
}