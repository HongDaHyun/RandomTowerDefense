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
        return Array.Find(unitStat, stat => EqualityComparer<UnitStat>.Default.Equals(stat, unit));
    }
    public UnitStat FindUnit(string name)
    {
        return Array.Find(unitStat, stat => stat.unitName == name);
    }
    public List<UnitStat> FindUnit(UnitType type)
    {
        return unitStat.Where(stat => stat.unitType == type).ToList();
    }
    public List<UnitStat> FindUnit(Tier tier)
    {
        return unitStat.Where(stat => stat.tier == tier).ToList();
    }
    public string FindUnit(UnitType type, Tier tier)
    {
        List<UnitStat> typeStats = FindUnit(type);
        return typeStats.Find(stat => stat.tier == tier).unitName;
    }

    public int IndexOfUnit(string name)
    {
        return Array.IndexOf(unitStat, FindUnit(name));
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
        int lenght = unitStat.Length;
        for (int i = 0; i < lenght; i++)
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
                unitStat[ID].dmg = Mathf.Round((unitStat[ID].dmg + value) * 100f) / 100f;
                break;
            case UpUnit.CritPercent:
                unitStat[ID].critPercent += (int)value;
                break;
            case UpUnit.CritDmg:
                unitStat[ID].critDmg += (int)value;
                break;
            case UpUnit.AtkSpeed:
                unitStat[ID].atkSpeed = Mathf.Round((unitStat[ID].atkSpeed + value) * 100f) / 100f;
                break;
            case UpUnit.Range:
                unitStat[ID].range = Mathf.Round((unitStat[ID].range + value) * 100f) / 100f;
                break;
            case UpUnit.All:
                unitStat[ID].dmg = Mathf.Round((unitStat[ID].dmg + value) * 100f) / 100f;
                unitStat[ID].critPercent += (int)value;
                unitStat[ID].critDmg += (int)value;
                unitStat[ID].atkSpeed = Mathf.Round((unitStat[ID].atkSpeed + value) * 100f) / 100f;
                unitStat[ID].range = Mathf.Round((unitStat[ID].range + value) * 100f) / 100f;
                break;
        }
    }
    private void SpawnUp_Unit(string unitName)
    {
        SpawnManager.Instance.unitList.FindAll(unit => unit.name == unitName).ForEach(unit => unit.SetStat());
    }
    private void SpawnUp_Unit() // All
    {
        for (int i = 0; i < SpawnManager.Instance.unitList.Count; i++)
        {
            SpawnManager.Instance.unitList[i].SetStat();
        }
    }
    #endregion

    #region Enemy
    //Find
    public EnemyStat FindEnemy(EnemyStat enemy)
    {
        return Array.Find(enemyStat, stat => EqualityComparer<EnemyStat>.Default.Equals(stat, enemy));
    }
    public List<EnemyStat> FindEnemy(EnemyType type)
    {
        return enemyStat.Where(stat => stat.enemyType == type).ToList();
    }
    public EnemyStat FindEnemy(string name)
    {
        return Array.Find(enemyStat, stat => stat.enemyName == name);
    }
    public EnemyStat FindEnemy(int id)
    {
        return enemyStat[id];
    }

    public int IndexOfEnemy(string name)
    {
        return Array.IndexOf(enemyStat, FindEnemy(name));
    }

    public void UpgradeEnemy(string enemyName, UpEnemy kind, float value)
    {
        CSVUp_Enemy(IndexOfEnemy(enemyName), kind, value);
    }

    public void SetEnemy(List<EnemyStat> stats, EnemyStat stat)
    {
        for (int i = 0; i < stats.Count; i++)
        {
            stats[i] = stat;
        }
    }

    public void SetAllEnemy()
    {
        SetEnemy(FindEnemy(EnemyType.Golem), FindEnemy("녹슨골렘"));
        SetEnemy(FindEnemy(EnemyType.Human), FindEnemy("인형탈"));
        SetEnemy(FindEnemy(EnemyType.Portion), FindEnemy("검정포션"));
        SetEnemy(FindEnemy(EnemyType.Skeleton), FindEnemy("스카스카"));
        SetEnemy(FindEnemy(EnemyType.Slime), FindEnemy("불라임"));
    }

    public void UpgradeAllEnemy(UpEnemy kind, float value)
    {
        int length = enemyStat.Length;
        for (int i = 0; i < length; i++)
            CSVUp_Enemy(i, kind, value);
    }

    private void CSVUp_Enemy(int ID, UpEnemy kind, float value)
    {
        switch (kind)
        {
            case UpEnemy.Strength:
                enemyStat[ID].strength += (int)value;
                break;
            case UpEnemy.Hp:
                enemyStat[ID].hp = Mathf.Round((enemyStat[ID].hp + value) * 100f) / 100f;
                break;
            case UpEnemy.Defense:
                enemyStat[ID].defense = Mathf.Round((enemyStat[ID].defense + value) * 100f) / 100f;
                break;
            case UpEnemy.MoveSpeed:
                enemyStat[ID].moveSpeed = Mathf.Round((enemyStat[ID].moveSpeed + value) * 100f) / 100f;
                enemyStat[ID].moveSpeed = MathF.Max(enemyStat[ID].moveSpeed, 0.3f); // 이동 속도 최소값
                break;
            case UpEnemy.All:
                enemyStat[ID].strength += (int)value;
                enemyStat[ID].hp = Mathf.Round((enemyStat[ID].hp + value) * 100f) / 100f;
                enemyStat[ID].defense = Mathf.Round((enemyStat[ID].defense + value) * 100f) / 100f;
                enemyStat[ID].moveSpeed = Mathf.Round((enemyStat[ID].moveSpeed + value) * 100f) / 100f;
                enemyStat[ID].moveSpeed = MathF.Max(enemyStat[ID].moveSpeed, 0.3f); // 이동 속도 최소값
                break;
        }
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
        attributeStat[(int)attribute].attributePower = Mathf.Round((attributeStat[(int)attribute].attributePower + value) * 100f) / 100f;
    }
    public void UpgradeAllAttribute(float value)
    {
        for(int i = 0; i < attributeStat.Length; i++)
        {
            attributeStat[i].attributePower = Mathf.Round((attributeStat[i].attributePower + value) * 100f) / 100f;
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
                stat.size = Mathf.Round((stat.size + value) * 100f) / 100f;
                break;
            case "Speed":
                stat.speed = Mathf.Round((stat.speed + value) * 100f) / 100f;
                break;
        }
    }

    public void UpgradeAllProjectile(string kind, float value)
    {
        int length = projectileStat.Length;

        switch (kind)
        {
            case "Size":
                for (int i = 0; i < length; i++)
                    projectileStat[i].size = Mathf.Round((projectileStat[i].size + value) * 100f) / 100f;
                break;

            case "Speed":
                for (int i = 0; i < length; i++)
                    projectileStat[i].speed = Mathf.Round((projectileStat[i].speed + value) * 100f) / 100f;
                break;
        }
    }

    public void ChangeProjectileAttribute(ref ProjectileStat stat, Attribute _attribute)
    {
        stat.attribute = _attribute;
    }
    #endregion
}
