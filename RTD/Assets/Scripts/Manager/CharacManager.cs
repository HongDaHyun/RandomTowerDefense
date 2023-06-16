using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacManager : Singleton<CharacManager>
{
    [HideInInspector] public int proportionHP; // 0
    [HideInInspector] public int doubleWave, healthWave; // 8, 10
    public void DoCharac(CharacteristicStat characStat)
    {
        CSVList list = CSVReader.Instance.csvList;
        float value = characStat.condition;

        switch (characStat.ID)
        {
            case 0:
                proportionHP += (int)value;
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
                break;
            case 13:
                break;

        }
    }
}
