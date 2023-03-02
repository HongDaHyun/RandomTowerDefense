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
    public int dmg, critPercent, critDmg;
    public float atkSpeed, range;
}

[Serializable]
public struct EnemyStat
{
    public string enemyName;
    public EnemyType enemyType;
    public int hp, strength;
    public float defense, moveSpeed;
}

[Serializable]
public struct ProjectileStat
{
    public string projectileName;
    public UnitType projectileType;
    public int dmg, critPercent, critDmg;
    public float atkSpeed, range, projectileSpeed;
}

[Serializable]
public struct DebuffStat
{
    public string debuffName;
    public bool isActive;
    public float activeTime;
}

[Serializable]
public class CSVList
{
    public UnitStat[] unitStat;
    public EnemyStat[] enemyStat;
    public ProjectileStat[] projectileStat;
    public DebuffStat[] debuffStat;
}
