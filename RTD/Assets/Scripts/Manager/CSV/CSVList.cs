using System;
using System.Collections.Generic;
using System.Linq;
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
        UnitStat foundStat = Array.Find(unitStat, stat => EqualityComparer<UnitStat>.Default.Equals(stat, unit));
        return foundStat;
    }
    public UnitStat FindUnit(string name)
    {
        UnitStat foundStat = Array.Find(unitStat, stat => stat.unitName == name);
        return foundStat;
    }
    public List<UnitStat> FindUnit(UnitType type)
    {
        List<UnitStat> foundStats = unitStat.Where(stat => stat.unitType == type).ToList();
        return foundStats;
    }
    public List<UnitStat> FindUnit(Tier tier)
    {
        List<UnitStat> foundStats = unitStat.Where(stat => stat.tier == tier).ToList();
        return foundStats;
    }
    public string FindUnit(UnitType type, Tier tier)
    {
        List<UnitStat> typeStats = FindUnit(type);
        UnitStat foundStat = typeStats.Find(stat => stat.tier == tier);
        return foundStat.unitName;
    }

    public int IndexOfUnit(string name)
    {
        UnitStat target = FindUnit(name);
        return Array.IndexOf(unitStat, target);
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
            case UpUnit.All:
                unitStat[ID].dmg += value;
                unitStat[ID].critPercent += (int)value;
                unitStat[ID].critDmg += (int)value;
                unitStat[ID].atkSpeed += value;
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
        EnemyStat foundStat = Array.Find(enemyStat, stat => EqualityComparer<EnemyStat>.Default.Equals(stat, enemy));
        return foundStat;
    }
    public EnemyStat FindEnemy(string name)
    {
        EnemyStat foundStat = Array.Find(enemyStat, stat => stat.enemyName == name);
        return foundStat;
    }
    #endregion

    #region Quest
    //Find
    public QuestStat FindQuest(QuestStat quest)
    {
        QuestStat foundStat = Array.Find(questStat, stat => EqualityComparer<QuestStat>.Default.Equals(stat, quest));
        return foundStat;
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
        CharacteristicStat foundStat = Array.Find(characteristicStat, stat => EqualityComparer<CharacteristicStat>.Default.Equals(stat, charac));
        return foundStat;
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
        AttributeStat foundStat = Array.Find(attributeStat, stat => stat.attribute == attribute);
        return foundStat.attributePower;
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
        ProjectileStat foundStat = Array.Find(projectileStat, stat => stat.projectileName.ToString() == name);
        return foundStat;
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
