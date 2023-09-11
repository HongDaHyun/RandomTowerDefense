using UnityEngine;
using System;
using Sirenix.OdinInspector;
using UnityEngine.SceneManagement;

public class CSVReader : Singleton<CSVReader>
{
    public TextAsset[] textAssets;
    public CSVList csvList = new CSVList();

    public void StartUpdateStat() // QuestManager에서 처리
    {
        GameManager gm = GameManager.Instance;

        ShopStat[] shopStat = csvList.shopStat;

        // 상점 스탯
        csvList.UpgradeAllUnit_Begin(UpUnit.Dmg, shopStat[0].curLevel * shopStat[0].condition); // 데미지
        csvList.UpgradeAllUnit_Begin(UpUnit.AtkSpeed, shopStat[1].curLevel * shopStat[1].condition); // 공속
        csvList.UpgradeAllUnit_Begin(UpUnit.Range, shopStat[2].curLevel * shopStat[2].condition); // 사거리
        csvList.UpgradeAllUnit_Begin(UpUnit.CritPercent, shopStat[3].curLevel * shopStat[3].condition); // 크리티컬 확률
        csvList.UpgradeAllUnit_Begin(UpUnit.CritDmg, shopStat[4].curLevel * shopStat[4].condition); // 크리티컬 데미지

        csvList.UpgradeAllProjectile("Size", shopStat[5].curLevel * shopStat[5].condition); // 투사체 크기
        csvList.UpgradeAllProjectile("Speed", shopStat[6].curLevel * shopStat[6].condition); // 이동속도
        csvList.UpgradeAllAttribute(shopStat[7].curLevel * shopStat[7].condition); // 디버프

        gm.maxHealth += (int)(shopStat[8].curLevel * shopStat[8].condition); // 최대 체력
        gm.curHealth = gm.maxHealth; // 싱글톤 순서 오류로 QuestManager에 접근하지 않게 GainMaxHealth 함수 사용하지 않음
        gm.healthRegen = (int)(shopStat[9].curLevel * shopStat[9].condition); // 체력 재생
        gm.plusExp = (int)(shopStat[10].curLevel * shopStat[10].condition); // 추가 경험치

        // 확률 초기화
        gm.probability = new int[] { (int)ProTier.Common, (int)ProTier.Rare, (int)ProTier.Epic, (int)ProTier.Legend };
        gm.probability[2] += (int)(shopStat[11].curLevel * shopStat[11].condition); // 에픽 확률
        gm.probability[3] += (int)(shopStat[11].curLevel * shopStat[11].condition); // 레전드 확률

        float enemyUpStat = gm.increase_enemy * ((int)gm.stage + 1);

        // 레벨에 따른 적 체력 증가
        csvList.UpgradeEnemy(csvList.FindEnemy(EnemyType.Human), UpEnemy.Hp, enemyUpStat); // 사람형 체력 증가
        csvList.UpgradeEnemy(csvList.FindEnemy(EnemyType.Golem), UpEnemy.Defense, enemyUpStat / 2f ); // 골렘형 방어력 증가
        csvList.UpgradeEnemy(csvList.FindEnemy(EnemyType.Portion), UpEnemy.MoveSpeed, enemyUpStat / 2f); // 포션형 이속 증가
        csvList.UpgradeEnemy(csvList.FindEnemy(EnemyType.Skeleton), UpEnemy.Hp, enemyUpStat); // 스켈레톤형 체력 증가
        csvList.UpgradeEnemy(csvList.FindEnemy(EnemyType.Slime), UpEnemy.Hp, enemyUpStat); // 슬라임형 체력 증가
        csvList.UpgradeEnemy(csvList.FindEnemy(EnemyType.BOSS), UpEnemy.Hp, enemyUpStat / 2f); // 보스형 체력 증가
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
    public void CharacterisiticCSVReader()
    {
        int order = 3; int size = 6;
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
                maxExplanation = data[size * k + 2],
                condition = float.Parse(data[size * k + 3]),
                conditionWidth = float.Parse(data[size * k + 4]),
                ID = int.Parse(data[size * k + 5]),
                level = 0,

            };
        }
    }

    [Button("디버프 데이터 불러오기")]
    public void AttributeCSVReader()
    {
        int order = 4; int size = 2;
        string[] data = textAssets[order].text.Split(new string[] { ",", "\n" }, StringSplitOptions.None);
        int tableSize = data.Length / size - 1;
        csvList.attributeStat = new AttributeStat[tableSize];

        for (int i = 0; i < tableSize; i++)
        {
            int k = i + 1;
            csvList.attributeStat[i] = new AttributeStat
            {
                attribute = (Attribute)Enum.Parse(typeof(Attribute), data[size * k]),
                attributePower = float.Parse(data[size * k + 1])
            };
        }
    }

    [Button("투사체 데이터 불러오기")]
    public void ProjectileCSVReader()
    {
        int order = 5; int size = 5;
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
                speed = float.Parse(data[size * k + 3]),
                ignoreDefense = float.Parse(data[size * k + 4])
            };
        }
    }

    [Button("상점 데이터 불러오기")]
    public void ShopCSVReader()
    {
        int order = 6; int size = 5;
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
        CharacterisiticCSVReader();
        AttributeCSVReader();
        ProjectileCSVReader();
    }
    #endregion
}