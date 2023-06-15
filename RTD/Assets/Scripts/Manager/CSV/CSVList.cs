using System;
using System.Collections.Generic;

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
    public int condition, conditionWidth, level;
    public int ID;
}

[Serializable]
public struct AttributeStat
{
    public Attribute attribute;
    public float attributePower;
}

[Serializable]
public class CSVList
{
    public UnitStat[] unitStat;
    public EnemyStat[] enemyStat;
    public QuestStat[] questStat;
    public CharacteristicStat[] characteristicStat;
    public AttributeStat[] attributeStat;

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
        for(int i = 0; i < length; i++)
        {
            if (unitStat[i].unitName == name)
                return i;
        }
        return 0;
    }

    //Upgrade
    public void UpgradeUnit(string unitName, UpUnit kind, float value)
    {
        int ID = IndexOfUnit(unitName);
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
        UnitStat targetUnitStat = unitStat[ID];
        SpawnManager.Instance.unitList.FindAll(unit => unit.name == targetUnitStat.unitName).ForEach(unit => unit.SetUp());
    }
    public void UpgradeUnit(List<string> unitName, UpUnit kind, float value)
    {
        for(int i = 0; i < unitName.Count; i++)
        {
            int ID = IndexOfUnit(unitName[i]);

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

            UnitStat targetUnitStat = unitStat[ID];
            SpawnManager.Instance.unitList.FindAll(unit => unit.name == targetUnitStat.unitName).ForEach(unit => unit.SetUp());
        }
    }
    public void UpgradeUnit(List<UnitStat> stats, UpUnit kind, float value)
    {
        for (int i = 0; i < stats.Count; i++)
        {
            int ID = IndexOfUnit(stats[i].unitName);

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

            UnitStat targetUnitStat = unitStat[ID];
            SpawnManager.Instance.unitList.FindAll(unit => unit.name == targetUnitStat.unitName).ForEach(unit => unit.SetUp());
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
    #endregion

    #region Attribute
    public float FindAttribute(Attribute attribute)
    {
        foreach (AttributeStat stat in attributeStat)
        {
            if (attribute == stat.attribute)
                return stat.attributePower;
        }
        return 0;
    }
    #endregion
}
