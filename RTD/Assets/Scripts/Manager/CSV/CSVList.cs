using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct UnitStat
{
    public string unitName;
    public UnitType unitType;
    public Tier tier;
    public int critPercent, critDmg;
    public float dmg ,atkSpeed, range;
}

[Serializable]
public struct EnemyStat
{
    public string enemyName;
    public EnemyType enemyType;
    public Tier tier;
    public int strength;
    public float hp, defense, moveSpeed;
}

[Serializable]
public struct QuestStat
{
    public string questName, explanation;
    public int condition, conditionWidth;
    public int ID;
}

[Serializable]
public struct CharacteristicStat
{
    public string charateristicName, explanation;
    public float condition, conditionWidth;
    public int level, ID;
}

[Serializable]
public struct AttributeStat
{
    public Attribute attribute;
    public float attributePower;
}

[Serializable]
public struct ProjectileStat
{
    public UnitType projectileName;
    public Attribute attribute;
    public float size, speed;
}

[Serializable]
public class CSVList
{
    public UnitStat[] unitStat;
    public EnemyStat[] enemyStat;
    public QuestStat[] questStat;
    public CharacteristicStat[] characteristicStat;
    public AttributeStat[] attributeStat;
    public ProjectileStat[] projectileStat;

    #region Unit
    //Find
    public UnitStat FindUnit(UnitStat unit)
    {
        foreach (UnitStat stat in unitStat)
        {
            if (EqualityComparer<UnitStat>.Default.Equals(stat, unit))
                return stat;
        }
        return default(UnitStat);
    }
    public UnitStat FindUnit(string name)
    {
        foreach (UnitStat stat in unitStat)
        {
            if (stat.unitName == name)
                return stat;
        }
        return default(UnitStat);
    }
    public List<UnitStat> FindUnit(UnitType type)
    {
        List<UnitStat> unitStats = new List<UnitStat>();
        foreach (UnitStat stat in unitStat)
        {
            if (type == stat.unitType)
                unitStats.Add(stat);
        }
        return unitStats;
    }
    public List<UnitStat> FindUnit(Tier tier)
    {
        List<UnitStat> unitStats = new List<UnitStat>();
        foreach (UnitStat stat in unitStat)
        {
            if (tier == stat.tier)
                unitStats.Add(stat);
        }
        return unitStats;
    }
    public string FindUnit(UnitType type, Tier tier)
    {
        List<UnitStat> unitStats = FindUnit(type);
        foreach (UnitStat stat in unitStats)
        {
            if (tier == stat.tier)
            {
                return stat.unitName;
            }
        }
        return null;
    }

    public int IndexOfUnit(string name)
    {
        int length = unitStat.Length;
        for (int i = 0; i < length; i++)
        {
            if (unitStat[i].unitName == name)
                return i;
        }
        return 0;
    }

    //Upgrade
    public void UpgradeUnit(string unitName, UpUnit kind, float value)
    {
        // CSV
        CSVUp_Unit(IndexOfUnit(unitName), kind, value);

        // Spawn
        SpawnUp_Unit(unitName);
    }
    public void UpgradeUnit(List<string> unitName, UpUnit kind, float value)
    {
        for (int i = 0; i < unitName.Count; i++)
        {
            // CSV
            CSVUp_Unit(IndexOfUnit(unitName[i]), kind, value);

            // Spawn
            SpawnUp_Unit(unitName[i]);
        }
    }
    public void UpgradeUnit(List<UnitStat> stats, UpUnit kind, float value)
    {
        for (int i = 0; i < stats.Count; i++)
        {
            // CSV
            CSVUp_Unit(IndexOfUnit(stats[i].unitName), kind, value);

            // Spawn
            SpawnUp_Unit(stats[i].unitName);
        }
    }
    public void UpgradeAllUnit(UpUnit kind, float value)
    {
        for (int i = 0; i < unitStat.Length; i++)
        {
            // CSV
            CSVUp_Unit(i, kind, value);

            // Spawn
            SpawnUp_Unit();
        }
    }

    private void CSVUp_Unit(int ID, UpUnit kind, float value)
    {
        switch (kind)
        {
            case UpUnit.Dmg:
                unitStat[ID].dmg += value;
                break;
            case UpUnit.CritPercent:
                unitStat[ID].critPercent += (int)value;
                break;
            case UpUnit.CritDmg:
                unitStat[ID].critDmg += (int)value;
                break;
            case UpUnit.AtkSpeed:
                unitStat[ID].atkSpeed += value;
                break;
            case UpUnit.Range:
                unitStat[ID].range += value;
                break;
        }
    }
    private void SpawnUp_Unit(string unitName)
    {
        SpawnManager.Instance.unitList.FindAll(unit => unit.name == unitName).ForEach(unit => unit.SetUp());
    }
    private void SpawnUp_Unit() // All
    {
        for (int i = 0; i < SpawnManager.Instance.unitList.Count; i++)
        {
            SpawnManager.Instance.unitList[i].SetUp();
        }
    }
    #endregion

    #region Enemy
    //Find
    public EnemyStat FindEnemy(EnemyStat enemy)
    {
        foreach (EnemyStat stat in enemyStat)
        {
            if (EqualityComparer<EnemyStat>.Default.Equals(stat, enemy))
                return stat;
        }
        return default(EnemyStat);
    }
    public EnemyStat FindEnemy(string name)
    {
        foreach (EnemyStat stat in enemyStat)
        {
            if (name == stat.enemyName)
            {
                return stat;
            }
        }
        return default(EnemyStat);
    }
    #endregion

    #region Quest
    //Find
    public QuestStat FindQuest(QuestStat quest)
    {
        foreach (QuestStat stat in questStat)
        {
            if (EqualityComparer<QuestStat>.Default.Equals(stat, quest))
                return stat;
        }
        return default(QuestStat);
    }
    public QuestStat FindQuest(int id)
    {
        return questStat[id];
    }
    #endregion

    #region Character
    //Find
    public CharacteristicStat FindCharac(CharacteristicStat charac)
    {
        foreach (CharacteristicStat stat in characteristicStat)
        {
            if (EqualityComparer<CharacteristicStat>.Default.Equals(stat, charac))
                return stat;
        }
        return default(CharacteristicStat);
    }
    public CharacteristicStat FindCharac(int id)
    {
        return characteristicStat[id];
    }

    public void ResizeCharac(int id)
    {
        for (int i = id - 1; i < characteristicStat.Length - 1; i++)
        {
            characteristicStat[i] = characteristicStat[i + 1];
        }
    }
    #endregion

    #region Attribute
    // Find
    public float FindAttribute(Attribute attribute)
    {
        foreach (AttributeStat stat in attributeStat)
        {
            if (attribute == stat.attribute)
                return stat.attributePower;
        }
        return 0;
    }

    // Upgrade
    public void UpgradeAttribute(Attribute attribute, float value)
    {
        attributeStat[(int)attribute].attributePower += value;
    }
    public void UpgradeAllAttribute(float value)
    {
        for(int i = 0; i < attributeStat.Length; i++)
        {
            attributeStat[i].attributePower += value;
        }
    }
    #endregion

    #region Projectile
    public ProjectileStat FindProjectile(string name)
    {
        foreach (ProjectileStat stat in projectileStat)
        {
            if (stat.projectileName.ToString() == name)
                return stat;
        }

        return default(ProjectileStat);
    }

    public ref ProjectileStat FindProjectile_Ref(UnitType type)
    {
        int length = projectileStat.Length;
        for(int i = 0; i < length; i++)
        {
            if (projectileStat[i].projectileName == type)
                return ref projectileStat[i];
        }
        return ref projectileStat[0];
    }
    public void UpgradeProjectile(ref ProjectileStat stat, string kind, float value)
    {
        switch (kind)
        {
            case "Size":
                stat.size += value;
                break;
            case "Speed":
                stat.speed += value;
                break;
        }
    }

    public void ChangeProjectileAttribute(ref ProjectileStat stat, Attribute _attribute)
    {
        stat.attribute = _attribute;
    }
    #endregion
}
