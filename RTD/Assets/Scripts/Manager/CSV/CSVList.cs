using System;

[Serializable]
public struct UnitStat
{
    public string unitName;
    public UnitType unitType;
    public Tier tier;
    public int critPercent, critDmg, projectileSpeed;
    public float dmg ,atkSpeed, range;
}

[Serializable]
public struct EnemyStat
{
    public string enemyName;
    public EnemyType enemyType;
    public int strength;
    public float hp, defense, moveSpeed;
}

[Serializable]
public struct DebuffStat
{
    public string debuffName;
    public float ticTime;
}

[Serializable]
public struct QuestStat
{
    public string questName, explanation;
    public int condition, conditionWidth;
    public int ID;
}

[Serializable]
public class CSVList
{
    public UnitStat[] unitStat;
    public EnemyStat[] enemyStat;
    public DebuffStat[] debuffStat;
    public QuestStat[] questStat;
}
