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
    public string charateristicName, explanation, maxExplanation;
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
    public float size, speed, ignoreDefense;
}

[Serializable]
public struct ShopStat // csv 추가 시 뒤로만 추가 가능
{
    public string shopName, explanation;
    public float condition;
    public int curLevel, maxLevel, index;
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
    public ShopStat[] shopStat;

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

        UIManager.Instance.info.UpdateUnitUI();
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

        UIManager.Instance.info.UpdateUnitUI();
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

        UIManager.Instance.info.UpdateUnitUI();
    }
    public void UpgradeAllUnit(UpUnit kind, float value)
    {
        int lenght = unitStat.Length;
        for (int i = 0; i < lenght; i++)
        {
            // CSV
            CSVUp_Unit(i, kind, value);
        }

        // Spawn
        SpawnUp_Unit();

        UIManager.Instance.info.UpdateUnitUI();
    }
    public void UpgradeAllUnit_Begin(UpUnit kind, float value)
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
        float percentValue = 1 + value / 100f;
        switch (kind)
        {
            case UpUnit.Dmg:
                unitStat[ID].dmg = Mathf.Round(unitStat[ID].dmg * percentValue * 100f) / 100f;
                break;
            case UpUnit.CritPercent:
                unitStat[ID].critPercent += (int)value;

                // 초과분은 크리데미지로 치환
                if (unitStat[ID].critPercent > 100)
                {
                    unitStat[ID].critDmg += unitStat[ID].critPercent - 100;
                    unitStat[ID].critPercent = 100;
                }
                break;
            case UpUnit.CritDmg:
                unitStat[ID].critDmg += (int)value;
                break;
            case UpUnit.AtkSpeed:
                unitStat[ID].atkSpeed = Mathf.Round(unitStat[ID].atkSpeed * percentValue * 100f) / 100f;
                break;
            case UpUnit.Range:
                unitStat[ID].range = Mathf.Round(unitStat[ID].range * percentValue * 100f) / 100f;
                break;
            case UpUnit.All:
                unitStat[ID].dmg = Mathf.Round(unitStat[ID].dmg * percentValue * 100f) / 100f;
                unitStat[ID].critPercent += (int)value;
                unitStat[ID].critDmg += (int)value;
                unitStat[ID].atkSpeed = Mathf.Round(unitStat[ID].atkSpeed * percentValue * 100f) / 100f;
                unitStat[ID].range = Mathf.Round(unitStat[ID].range * percentValue * 100f) / 100f;
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
    public List<EnemyStat> FindEnemy(Tier tier)
    {
        return enemyStat.Where(stat => stat.tier == tier).ToList();
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

        SpawnUp_Enemy(enemyName);

        UIManager.Instance.info.UpdateEnemyUI();
    }

    public void UpgradeEnemy(List<EnemyStat> stats, UpEnemy kind, float value)
    {
        for (int i = 0; i < stats.Count; i++)
        {
            // CSV
            CSVUp_Enemy(IndexOfEnemy(stats[i].enemyName), kind, value);

            SpawnUp_Enemy(stats[i].enemyName);
        }
        UIManager.Instance.info.UpdateEnemyUI();
    }

    public void SetAllEnemy() // 모든 유닛 동기화
    {
        SetEnemy(FindEnemy(EnemyType.Golem), "녹슨골렘");
        SetEnemy(FindEnemy(EnemyType.Human), "인형탈");
        SetEnemy(FindEnemy(EnemyType.Portion), "검정포션");
        SetEnemy(FindEnemy(EnemyType.Skeleton), "스카스카");
        SetEnemy(FindEnemy(EnemyType.Slime), "불라임");
    }

    public void UpgradeAllEnemy(UpEnemy kind, float value)
    {
        int length = enemyStat.Length;
        for (int i = 0; i < length; i++)
        {
            CSVUp_Enemy(i, kind, value);
        }
        SpawnUp_Enemy();

        UIManager.Instance.info.UpdateEnemyUI();
    }

    public void SetEnemy(List<EnemyStat> stats, string s)
    {
        for(int i = 0; i < stats.Count; i++)
        {
            enemyStat[IndexOfEnemy(stats[i].enemyName)] = enemyStat[IndexOfEnemy(s)];
        }
    }

    private void CSVUp_Enemy(int ID, UpEnemy kind, float value)
    {
        float percentValue = 1 + value / 100f;

        switch (kind)
        {
            case UpEnemy.Strength:
                enemyStat[ID].strength = Mathf.RoundToInt(enemyStat[ID].strength * percentValue);
                break;
            case UpEnemy.Hp:
                enemyStat[ID].hp = Mathf.Round(enemyStat[ID].hp * percentValue * 100f) / 100f;
                break;
            case UpEnemy.Defense:
                enemyStat[ID].defense = Mathf.Round(enemyStat[ID].defense * percentValue * 100f) / 100f;

                break;
            case UpEnemy.MoveSpeed:
                enemyStat[ID].moveSpeed = Mathf.Round(enemyStat[ID].moveSpeed * percentValue * 100f) / 100f;
                break;
            case UpEnemy.All:
                enemyStat[ID].strength += (int)value;
                enemyStat[ID].hp = Mathf.Round(enemyStat[ID].hp * percentValue * 100f) / 100f;
                enemyStat[ID].defense = Mathf.Round(enemyStat[ID].defense * percentValue * 100f) / 100f;
                enemyStat[ID].moveSpeed = Mathf.Round(enemyStat[ID].moveSpeed * percentValue * 100f) / 100f;
                break;
        }
    }
    private void SpawnUp_Enemy(string enemyName)
    {
        SpawnManager.Instance.enemyList.FindAll(enemy => enemy.name == enemyName).ForEach(enemy => enemy.SetStat());
    }
    private void SpawnUp_Enemy() // All
    {
        for (int i = 0; i < SpawnManager.Instance.enemyList.Count; i++)
        {
            SpawnManager.Instance.enemyList[i].SetStat();
        }
    }
    #endregion

    #region Quest
    //Find
    public QuestStat FindQuest(QuestStat quest)
    {
        return Array.Find(questStat, stat => EqualityComparer<QuestStat>.Default.Equals(stat, quest));
    }
    public QuestStat FindQuest(string name)
    {
        return Array.Find(questStat, stat => stat.questName == name);
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
        float percentValue = 1 + value / 100f;

        switch (kind)
        {
            case "Size":
                stat.size = Mathf.Round(stat.size * percentValue * 100f) / 100f;
                break;
            case "Speed":
                stat.speed = Mathf.Round(stat.speed * percentValue * 100f) / 100f;
                break;
        }
    }

    public void UpgradeAllProjectile(string kind, float value)
    {
        int length = projectileStat.Length;
        float percentValue = 1 + value / 100f;

        switch (kind)
        {
            case "Size":
                for (int i = 0; i < length; i++)
                    projectileStat[i].size = Mathf.Round(projectileStat[i].size * percentValue * 100f) / 100f;
                break;

            case "Speed":
                for (int i = 0; i < length; i++)
                    projectileStat[i].speed = Mathf.Round(projectileStat[i].speed * percentValue * 100f) / 100f;
                break;
        }
    }

    public void ChangeProjectileAttribute(ref ProjectileStat stat, Attribute _attribute)
    {
        stat.attribute = _attribute;
    }

    public void SetIgnoreDefense(ref ProjectileStat stat, float value)
    {
        stat.ignoreDefense = Mathf.Clamp(stat.ignoreDefense + value, 0f, 1f);
    }
    #endregion

    #region Shop
    public ShopStat FindShop(string name)
    {
        return Array.Find(shopStat, stat => stat.shopName.ToString() == name);
    }

    public ShopStat FindShop(int id)
    {
        return shopStat[id];
    }

    public int CalCost(ShopStat stat)
    {
        return 100 * (int)Mathf.Pow(2, stat.curLevel); // 등비 수열 (20, 40, 80, 160 ... 10240)
    }

    public void SaveShop(ShopStat shop)
    {
        int index = shop.index;
        ES3.Save<int>($"Shop_{index}", CSVReader.Instance.csvList.FindShop(index).curLevel , GameManager.Instance.saveDirectory); // 세이브 디렉토리
    }

    public void LoadShop()
    {
        int length = shopStat.Length;
        string saveDirectory = GameManager.Instance.saveDirectory;
        for(int i = 0; i < length; i++)
        {
            if(ES3.KeyExists($"Shop_{i}", saveDirectory)) // 세이브 디렉토리
                shopStat[i].curLevel = ES3.Load<int>($"Shop_{i}", saveDirectory); // 세이브 디렉토리
        }
    }

    public void UpgradeShop(ShopStat shop)
    {
        if (shop.curLevel >= shop.maxLevel)
            return;

        int cost = CalCost(shop);
        if (GameManager.Instance.curMoney < cost)
            return;

        GameManager.Instance.EarnMoney(-cost);
        shopStat[shop.index].curLevel++;

        SaveShop(shop);
    }
    #endregion
}
