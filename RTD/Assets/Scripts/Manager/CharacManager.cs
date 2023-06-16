using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacManager : Singleton<CharacManager>
{
    [HideInInspector] public int doubleWave, healthWave, plusReward; // 8, 10, 12
    [HideInInspector] public float proportionHP, dmgWave, dmgFire, dmgSword; // 0, 13, 15, 17
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
            case 11:
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
                dmgFire += value;
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
                break;
            case 20:
                list.ChangeProjectileAttribute(ref list.FindProjectile_Ref(UnitType.Fighter), Attribute.Thunder);
                list.UpgradeAttribute(Attribute.Thunder, value);
                break;
            case 21:
                list.UpgradeProjectile(ref list.FindProjectile_Ref(UnitType.Fighter), "Size", value);
                list.UpgradeUnit(list.FindUnit(UnitType.Fighter), UpUnit.Range, value);
                break;
        }
    }
}
