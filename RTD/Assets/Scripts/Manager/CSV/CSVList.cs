using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum UnitType { Bow, Fighter, Fire, Germ, Gun, Ice, Sword, Thunder }
public enum EnemyType { Golem, Human, Portion, Skeleton, Slime }
public enum Attribute { None, Fire, Germ, Ice, Thunder, Stun }
public enum Tier { Common, Rare, Epic, Legend }

[Serializable]
public struct UnitStat
{
    public string unitName;
    public UnitType unitType;
    public Tier tier;
    public Attribute attribute;
    public int critPercent, critDmg;
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
public struct ProjectileStat
{
    public string projectileName;
    public UnitType projectileType;
    public int critPercent, critDmg;
    public float dmg, atkSpeed, range, projectileSpeed;
}

[Serializable]
public struct DebuffStat
{
    public string debuffName;
    public float ticTime;
}

[Serializable]
public struct Quest
{
    public string questName;
    public string explanation;
    public int ID;
}

[Serializable]
public class CSVList
{
    public UnitStat[] unitStat;
    public EnemyStat[] enemyStat;
    public ProjectileStat[] projectileStat;
    public DebuffStat[] debuffStat;
    public Quest[] quest;
}
