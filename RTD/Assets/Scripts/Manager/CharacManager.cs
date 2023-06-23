using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacManager : Singleton<CharacManager>
{
    [HideInInspector] public int doubleWave, healthWave, plusReward, plusIce = 1, wave5; // 8, 10, 12, 26
    [HideInInspector]
    public float proportionHP, dmgWave, dmgFire, dmgSword, dmgGerm, defenseGerm, dmgWaveGun, iceGatcha,
        execution, spawnDmg, commonGatcha, plusExp, madDmg; // 0, 13, 15, 17, 22, 23, 24, 27, 30, 33, 34, 35, 36
    public void DoCharac(CharacteristicStat characStat)
    {
        CSVList list = CSVReader.Instance.csvList;
        float value = characStat.condition;

        switch (characStat.ID)
        {
            case 0:
                proportionHP += value;
                break;
            case 1:
                list.UpgradeUnit(list.FindUnit(Tier.Common), UpUnit.AtkSpeed, value);
                list.UpgradeUnit(list.FindUnit(Tier.Rare), UpUnit.AtkSpeed, value);
                break;
            case 2:
                list.UpgradeAllAttribute(value);
                break;
            case 3:
                list.UpgradeAllUnit(UpUnit.Dmg, value);
                break;
            case 4:
                list.UpgradeAllUnit(UpUnit.AtkSpeed, value);
                break;
            case 5:
                list.UpgradeAllUnit(UpUnit.CritDmg, value);
                break;
            case 6:
                list.UpgradeAllUnit(UpUnit.CritPercent, value);
                break;
            case 7:
                GameManager.Instance.probability[2] += (int)value;
                GameManager.Instance.probability[3] += (int)value;
                break;
            case 8:
                doubleWave += (int)value;
                break;
            case 9:
                GameManager.Instance.maxHealth += (int)value;
                GameManager.Instance.GainHealth((int)value);
                break;
            case 10:
                healthWave += (int)value;
                break;
            case 11: // 제작 중
                break;
            case 12:
                plusReward += (int)value;
                break;
            case 13:
                dmgWave = value;
                break;
            case 14:
                list.UpgradeUnit(list.FindUnit(UnitType.Fire), UpUnit.Dmg, value);
                break;
            case 15:
                dmgFire += value / 100f;
                break;
            case 16:
                list.UpgradeUnit(list.FindUnit(UnitType.Sword), UpUnit.Range, value);
                break;
            case 17:
                dmgSword = value;
                break;
            case 18:
                list.UpgradeUnit(list.FindUnit(UnitType.Bow), UpUnit.AtkSpeed, value);
                list.UpgradeUnit(list.FindUnit(UnitType.Bow), UpUnit.CritDmg, value);
                break;
            case 19:
                list.ChangeProjectileAttribute(ref list.FindProjectile_Ref(UnitType.Bow), Attribute.Fire);
                list.UpgradeProjectile(ref list.FindProjectile_Ref(UnitType.Bow), "Speed", value);
                break;
            case 20:
                list.ChangeProjectileAttribute(ref list.FindProjectile_Ref(UnitType.Fighter), Attribute.Thunder);
                list.UpgradeAttribute(Attribute.Thunder, value);
                break;
            case 21:
                list.UpgradeProjectile(ref list.FindProjectile_Ref(UnitType.Fighter), "Size", value);
                list.UpgradeUnit(list.FindUnit(UnitType.Fighter), UpUnit.Range, value);
                break;
            case 22:
                dmgGerm += value / 100;
                break;
            case 23:
                defenseGerm += value;
                break;
            case 24:
                dmgWaveGun = value;
                break;
            case 25:
                List<UnitStat> stats = list.FindUnit(UnitType.Gun);
                list.UpgradeUnit(stats, UpUnit.CritPercent, value);
                list.UpgradeUnit(stats, UpUnit.CritDmg, value);
                break;
            case 26:
                plusIce = (int)value;
                break;
            case 27:
                iceGatcha = value;
                GameManager.Instance.gatchaKey += Mathf.RoundToInt(SpawnManager.Instance.NumUnit(UnitType.Ice) * iceGatcha);
                break;
            case 28:
                stats = list.FindUnit(UnitType.Thunder);
                list.UpgradeUnit(stats, UpUnit.CritPercent, value);

                // 크리 확률 초과 분 크리 데미지로 치환
                foreach (UnitStat stat in stats)
                {
                    if (stat.critPercent > 100)
                    {
                        list.UpgradeUnit(stat.unitName, UpUnit.CritDmg, stat.critPercent - 100);
                    }
                }
                break;
            case 29:
                stats = list.FindUnit(UnitType.Thunder);
                list.UpgradeUnit(stats, UpUnit.AtkSpeed, value);
                list.UpgradeUnit(stats, UpUnit.CritDmg, -value);
                break;
            case 30:
                execution = value / 100f;
                break;
            case 31:
                list.UpgradeAllUnit(UpUnit.Range, value);
                break;
            case 32:
                list.UpgradeAllProjectile("size", value);
                break;
            case 33:
                spawnDmg = value;
                break;
            case 34:
                wave5 = (int)value;
                break;
            case 35:
                commonGatcha = value;
                break;
            case 36:
                plusExp = value / 100;
                break;
            case 37:
                madDmg = value;
                break;
            case 38:
                list.UpgradeUnit(list.FindUnit(Tier.Epic), UpUnit.Dmg, value);
                list.UpgradeUnit(list.FindUnit(Tier.Legend), UpUnit.Dmg, value);
                break;
            case 39:
                list.UpgradeAllEnemy(UpEnemy.MoveSpeed, -value);
                break;
        }
    }
}
