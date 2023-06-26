using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;
using UnityEngine.Networking;
using UGS;

public class CSVReader : Singleton<CSVReader>
{
    public bool isTest_Google;
    public TextAsset[] textAssets;
    public CSVList csvList = new CSVList();

    void Start()
    {
        if (!isTest_Google)
            return;
        UnitGoogle();
        EnemyGoogle();
        QuestGoogle();
        CharacteristicGoogle();
        ProjectileGoogle();
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
        int order = 2; int size = 4;
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
                ID = i
            };
        }
    }

    [Button("특성 데이터 불러오기")]
    public void CharacterisiticReader()
    {
        int order = 3; int size = 4;
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
                level = 0,
                ID = i
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

    [Button("모든 데이터 불러오기")]
    public void CSVReading()
    {
        UnitCSVReader();
        EnemyCSVReader();
        QuestCSVReader();
        CharacterisiticReader();
        ProjectileCSVReader();
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
                    ID = index
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
                    ID = index
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