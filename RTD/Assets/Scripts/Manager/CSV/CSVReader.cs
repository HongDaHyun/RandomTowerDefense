using UnityEngine;
using System;
using Sirenix.OdinInspector;
using UGS;

public class CSVReader : Singleton<CSVReader>
{
    public TextAsset[] textAssets;
    public CSVList csvList = new CSVList();

    public void OnSceneLoaded()
    {
        CSVReading();

        UnitGoogle();
        EnemyGoogle();
        QuestGoogle();
        CharacteristicGoogle();
        ProjectileGoogle();
    }

    public void StartUpdateStat()
    {
        csvList.LoadShop();

        ShopStat[] shopStat = csvList.shopStat;

        csvList.UpgradeAllUnit(UpUnit.Dmg, shopStat[0].curLevel * shopStat[0].condition);
        csvList.UpgradeAllUnit(UpUnit.AtkSpeed, shopStat[1].curLevel * shopStat[1].condition);
        csvList.UpgradeAllUnit(UpUnit.Range, shopStat[2].curLevel * shopStat[2].condition);
        csvList.UpgradeAllUnit(UpUnit.CritPercent, shopStat[3].curLevel * shopStat[3].condition);
        csvList.UpgradeAllUnit(UpUnit.CritDmg, shopStat[4].curLevel * shopStat[4].condition);

        csvList.UpgradeAllProjectile("Size", shopStat[5].curLevel * shopStat[5].condition);
        csvList.UpgradeAllProjectile("Speed", shopStat[6].curLevel * shopStat[6].condition);
        csvList.UpgradeAllAttribute(shopStat[7].curLevel * shopStat[7].condition);

        GameManager.Instance.GainMaxHealth((int)(shopStat[8].curLevel * shopStat[8].condition));
        CharacManager.Instance.healthWave += (int)(shopStat[9].curLevel * shopStat[9].condition);
        CharacManager.Instance.plusExp += (int)(shopStat[10].curLevel * shopStat[10].condition);
        GameManager.Instance.probability[2] += (int)(shopStat[11].curLevel * shopStat[11].condition);
        GameManager.Instance.probability[3] += (int)(shopStat[11].curLevel * shopStat[11].condition);
    }

    #region ExcelRead
    [Button("유닛 데이터 불러오기")]
    public void UnitCSVReader()
    {
        int order = 0; int size = 8;
        string[] data = textAssets[order].text.Split(new string[] { ",", "\n" }, StringSplitOptions.None);
        int tableSize = data.Length / size - 1;
        csvList.unitStat = new UnitStat[tableSize];

        for (int i = 0; i < tableSize; i++)
        {
            int k = i + 1;
            csvList.unitStat[i] = new UnitStat
            {
                unitName = data[size * k],
                unitType = (UnitType)Enum.Parse(typeof(UnitType), data[size * k + 1]),
                tier = (Tier)Enum.Parse(typeof(Tier), data[size * k + 2]),
                dmg = float.Parse(data[size * k + 3]),
                critPercent = int.Parse(data[size * k + 4]),
                critDmg = int.Parse(data[size * k + 5]),
                atkSpeed = float.Parse(data[size * k + 6]),
                range = float.Parse(data[size * k + 7])
            };
        }
    }

    [Button("적 데이터 불러오기")]
    public void EnemyCSVReader()
    {
        int order = 1; int size = 7;
        string[] data = textAssets[order].text.Split(new string[] { ",", "\n" }, StringSplitOptions.None);
        int tableSize = data.Length / size - 1;
        csvList.enemyStat = new EnemyStat[tableSize];

        for (int i = 0; i < tableSize; i++)
        {
            int k = i + 1;
            csvList.enemyStat[i] = new EnemyStat
            {
                enemyName = data[size * k],
                enemyType = (EnemyType)Enum.Parse(typeof(EnemyType), data[size * k + 1]),
                tier = (Tier)Enum.Parse(typeof(Tier), data[size * k + 2]),
                hp = float.Parse(data[size * k + 3]),
                strength = int.Parse(data[size * k + 4]),
                defense = float.Parse(data[size * k + 5]),
                moveSpeed = float.Parse(data[size * k + 6])
            };
        }
    }

    [Button("퀘스트 데이터 불러오기")]
    public void QuestCSVReader()
    {
        int order = 2; int size = 5;
        string[] data = textAssets[order].text.Split(new string[] { ",", "\n" }, StringSplitOptions.None);
        int tableSize = data.Length / size - 1;
        csvList.questStat = new QuestStat[tableSize];

        for (int i = 0; i < tableSize; i++)
        {
            int k = i + 1;
            csvList.questStat[i] = new QuestStat
            {
                questName = data[size * k],
                explanation = data[size * k + 1],
                condition = int.Parse(data[size * k + 2]),
                conditionWidth = int.Parse(data[size * k + 3]),
                ID = int.Parse(data[size * k + 4])
            };
        }
    }

    [Button("특성 데이터 불러오기")]
    public void CharacterisiticReader()
    {
        int order = 3; int size = 5;
        string[] data = textAssets[order].text.Split(new string[] { ",", "\n" }, StringSplitOptions.None);
        int tableSize = data.Length / size - 1;
        csvList.characteristicStat = new CharacteristicStat[tableSize];

        for (int i = 0; i < tableSize; i++)
        {
            int k = i + 1;
            csvList.characteristicStat[i] = new CharacteristicStat
            {
                charateristicName = data[size * k],
                explanation = data[size * k + 1],
                condition = float.Parse(data[size * k + 2]),
                conditionWidth = float.Parse(data[size * k + 3]),
                ID = int.Parse(data[size * k + 4]),
                level = 0,

            };
        }
    }

    [Button("투사체 데이터 불러오기")]
    public void ProjectileCSVReader()
    {
        int order = 4; int size = 4;
        string[] data = textAssets[order].text.Split(new string[] { ",", "\n" }, StringSplitOptions.None);
        int tableSize = data.Length / size - 1;
        csvList.projectileStat = new ProjectileStat[tableSize];

        for (int i = 0; i < tableSize; i++)
        {
            int k = i + 1;
            csvList.projectileStat[i] = new ProjectileStat
            {
                projectileName = (UnitType)Enum.Parse(typeof(UnitType), data[size * k]),
                attribute = (Attribute)Enum.Parse(typeof(Attribute), data[size * k + 1]),
                size = float.Parse(data[size * k + 2]),
                speed = float.Parse(data[size * k + 3])
            };
        }
    }

    [Button("상점 데이터 불러오기")]
    public void ShopCSVReader()
    {
        int order = 5; int size = 5;
        string[] data = textAssets[order].text.Split(new string[] { ",", "\n" }, StringSplitOptions.None);
        int tableSize = data.Length / size - 1;
        csvList.shopStat = new ShopStat[tableSize];

        for (int i = 0; i < tableSize; i++)
        {
            int k = i + 1;
            csvList.shopStat[i] = new ShopStat
            {
                shopName = data[size * k],
                explanation = data[size * k + 1],
                condition = float.Parse(data[size * k + 2]),
                maxLevel = int.Parse(data[size * k + 3]),
                index = i
            };
        }
    }

    [Button("모든 데이터 불러오기")]
    public void CSVReading()
    {
        UnitCSVReader();
        EnemyCSVReader();
        QuestCSVReader();
        CharacterisiticReader();
        ProjectileCSVReader();
        ShopCSVReader();
    }
    #endregion

    #region GoogleRoad
    private void UnitGoogle()
    {
        UnityGoogleSheet.LoadFromGoogle<string, Test.Unit>((list, map) => {
            int length = list.Count;
            csvList.unitStat = new UnitStat[length];

            int index = 0; // 인덱스 변수 추가

            foreach (var x in list) // foreach 루프로 변경
            {
                csvList.unitStat[index] = new UnitStat
                {
                    unitName = x.Name,
                    unitType = (UnitType)Enum.Parse(typeof(UnitType), x.Type),
                    tier = (Tier)Enum.Parse(typeof(Tier), x.Tier),
                    dmg = x.Dmg,
                    critPercent = x.CritPercent,
                    critDmg = x.CritDmg,
                    atkSpeed = x.AtkSpeed,
                    range = x.Range
                };

                index++; // 인덱스 증가
            }
        }, true);
    }

    private void EnemyGoogle()
    {
        UnityGoogleSheet.LoadFromGoogle<string, Test.Enemy>((list, map) => {
            int length = list.Count;
            csvList.enemyStat = new EnemyStat[length];

            int index = 0;

            foreach (var x in list)
            {
                csvList.enemyStat[index] = new EnemyStat
                {
                    enemyName = x.Name,
                    enemyType = (EnemyType)Enum.Parse(typeof(EnemyType), x.Type),
                    tier = (Tier)Enum.Parse(typeof(Tier), x.Tier),
                    hp = x.Hp,
                    strength = x.Strength,
                    defense = x.Defense,
                    moveSpeed = x.MoveSpeed
                };

                index++;
            }
        }, true);
    }

    private void QuestGoogle()
    {
        UnityGoogleSheet.LoadFromGoogle<string, Test.Quest>((list, map) => {
            int length = list.Count;
            csvList.questStat = new QuestStat[length];

            int index = 0;

            foreach (var x in list)
            {
                csvList.questStat[index] = new QuestStat
                {
                    questName = x.Name,
                    explanation = x.Explanation,
                    condition = x.Condition,
                    conditionWidth = x.ConditionWidth,
                    ID = x.ID
                };

                index++;
            }
        }, true);
    }

    private void CharacteristicGoogle()
    {
        UnityGoogleSheet.LoadFromGoogle<string, Test.Characterisitic>((list, map) => {
            int length = list.Count;
            csvList.characteristicStat = new CharacteristicStat[length];

            int index = 0;

            foreach (var x in list)
            {
                csvList.characteristicStat[index] = new CharacteristicStat
                {
                    charateristicName = x.Name,
                    explanation = x.Contents,
                    condition = x.Condition,
                    conditionWidth = x.ConditionWidth,
                    ID = x.ID
                };

                index++;
            }
        }, true);
    }

    private void ProjectileGoogle()
    {
        UnityGoogleSheet.LoadFromGoogle<string, Test.Projectile>((list, map) => {
            int length = list.Count;
            csvList.projectileStat = new ProjectileStat[length];

            int index = 0;

            foreach (var x in list)
            {
                csvList.projectileStat[index] = new ProjectileStat
                {
                    projectileName = (UnitType)Enum.Parse(typeof(UnitType), x.Name),
                    attribute = (Attribute)Enum.Parse(typeof(Attribute), x.Attribute),
                    size = x.Size,
                    speed = x.Speed
                };

                index++;
            }
        }, true);
    }
    #endregion
}