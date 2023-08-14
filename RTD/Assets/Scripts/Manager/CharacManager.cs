using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacManager : Singleton<CharacManager>
{
    public bool isLevelUping;

    [HideInInspector] public int doubleWave, plusReward, wave5; // 8, 12, 34
    [HideInInspector]
    public float proportionHP, dmgWave, dmgFire, dmgSword, dmgGerm, dmgWaveGun, iceGatcha,
        execution, spawnDmg, commonGatcha, madDmg; // 0, 13, 15, 17, 22, 24, 27, 30, 33, 34, 36

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
                list.UpgradeAttribute(Attribute.Fire, value);
                list.UpgradeAttribute(Attribute.Germ, value);
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
                GameManager.Instance.GainMaxHealth((int)value);
                break;
            case 10:
                GameManager.Instance.healthRegen += (int)value;
                break;
            case 11:
                GameManager.Instance.GainGatcha((int)value);
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
                list.ChangeProjectileAttribute(ref list.FindProjectile_Ref(UnitType.Fighter), Attribute.Ice);
                list.UpgradeProjectile(ref list.FindProjectile_Ref(UnitType.Fighter), "Speed", value);
                break;
            case 21:
                list.UpgradeProjectile(ref list.FindProjectile_Ref(UnitType.Fighter), "Size", value);
                list.UpgradeUnit(list.FindUnit(UnitType.Fighter), UpUnit.Range, value);
                break;
            case 22:
                dmgGerm += value / 100;
                break;
            case 23:
                list.SetIgnoreDefense(ref list.FindProjectile_Ref(UnitType.Germ), value / 100f);
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
                list.UpgradeAttribute(Attribute.Ice, value / 10);
                list.UpgradeAttribute(Attribute.Thunder, value / 100);
                break;
            case 27:
                iceGatcha = value;
                GameManager.Instance.GainGatcha(Mathf.RoundToInt(SpawnManager.Instance.NumUnit(UnitType.Ice) * iceGatcha));
                break;
            case 28:
                stats = list.FindUnit(UnitType.Thunder);
                list.UpgradeUnit(stats, UpUnit.CritPercent, value);
                break;
            case 29:
                list.UpgradeUnit(list.FindUnit(UnitType.Thunder), UpUnit.Dmg, value);
                break;
            case 30:
                execution = value / 100f;
                break;
            case 31:
                list.UpgradeAllUnit(UpUnit.Range, value);
                break;
            case 32:
                list.UpgradeAllProjectile("Size", value);
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
                GameManager.Instance.plusExp += value;
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

    public void DoCharac_Max(CharacteristicStat characStat)
    {
        CSVList list = CSVReader.Instance.csvList;
        GameManager gm = GameManager.Instance;

        switch (characStat.ID)
        {
            case 0:
                list.UpgradeAllEnemy(UpEnemy.All, -30f);
                break;
            case 1:
                list.UpgradeUnit(list.FindUnit(Tier.Common), UpUnit.AtkSpeed, 200f);
                list.UpgradeUnit(list.FindUnit(Tier.Rare), UpUnit.AtkSpeed, 200f);
                break;
            case 2:
                list.UpgradeProjectile(ref list.FindProjectile_Ref(UnitType.Fire), "Size", 100f);
                list.UpgradeProjectile(ref list.FindProjectile_Ref(UnitType.Germ), "Size", 100f);
                break;
            case 3:
            case 4:
            case 5:
            case 6:
                list.UpgradeAllUnit(UpUnit.Dmg, 33f);
                list.UpgradeAllUnit(UpUnit.AtkSpeed, 33f);
                list.UpgradeAllUnit(UpUnit.CritPercent, 33f);
                break;
            case 7:
                gm.probability[3] += 20;
                break;
            case 8:
                WaveManager.Instance.waveGatcha++;
                break;
            case 9:
                list.UpgradeAllUnit(UpUnit.Dmg, GameManager.Instance.maxHealth);
                break;
            case 10:
                gm.healthRegen += 20;
                break;
            case 11:
                gm.LoseHealth(5);
                gm.GainGatcha(20);
                break;
            case 12:
                UIManager um = UIManager.Instance;
                um.questPannel.maxCool = 1;
                um.questPannel.curCool = 0;

                um.questPannel.QuestCool();
                break;
            case 13:
                dmgWave *= 2;
                break;
            case 14:
                list.UpgradeUnit(list.FindUnit(UnitType.Fire), UpUnit.Dmg, 100f);
                break;
            case 15:
                list.UpgradeAttribute(Attribute.Fire, 10);
                break;
            case 16:
                list.ChangeProjectileAttribute(ref list.FindProjectile_Ref(UnitType.Sword), Attribute.Ice);
                list.UpgradeProjectile(ref list.FindProjectile_Ref(UnitType.Sword), "Size", 100f);
                break;
            case 17:
                gm.healthReduce++;
                break;
            case 18:
                list.UpgradeProjectile(ref list.FindProjectile_Ref(UnitType.Bow), "Speed", 200f);
                break;
            case 19:
                list.UpgradeUnit(list.FindUnit(UnitType.Bow), UpUnit.CritDmg, 100f);
                break;
            case 20:
                list.UpgradeUnit(list.FindUnit(UnitType.Fighter), UpUnit.Range, 150f);
                break;
            case 21:
                list.UpgradeUnit(list.FindUnit(UnitType.Fighter), UpUnit.CritPercent, 100f);
                break;
            case 22:
                list.UpgradeAttribute(Attribute.Germ, 10);
                break;
            case 23:
                list.UpgradeAllEnemy(UpEnemy.Strength, -80f);
                break;
            case 24:
                list.ChangeProjectileAttribute(ref list.FindProjectile_Ref(UnitType.Gun), Attribute.Germ);
                list.UpgradeUnit(list.FindUnit(UnitType.Gun), UpUnit.Range, 100f);
                break;
            case 25:
                list.SetIgnoreDefense(ref list.FindProjectile_Ref(UnitType.Gun), 20f);
                break;
            case 26:
                gm.playerDefense++;
                break;
            case 27:
                list.SetIgnoreDefense(ref list.FindProjectile_Ref(UnitType.Ice), 30f);
                break;
            case 28:
                list.UpgradeUnit(list.FindUnit(UnitType.Thunder), UpUnit.CritPercent, 100f);
                break;
            case 29:
                gm.LoseHealth(5);
                list.UpgradeAllUnit(UpUnit.All, 20f);
                break;
            case 30:
                proportionHP += 3f;
                break;
            case 31:
                list.UpgradeAllProjectile("Size", 80f);
                break;
            case 32:
                list.UpgradeAllUnit(UpUnit.Range, 50f);
                break;
            case 33:
                list.UpgradeAllUnit(UpUnit.Dmg, SpawnManager.Instance.enemyList.Count * 3f);
                break;
            case 34:
                WaveManager.Instance.waveGatcha += 2;
                break;
            case 35:
                list.UpgradeUnit(list.FindUnit(Tier.Common), UpUnit.All, 100f);
                break;
            case 36:
                gm.limitExp = 60;
                break;
            case 37:
                gm.healthReduce += 2;
                break;
            case 38:
                gm.probability[2] += 10;
                gm.probability[3] += 10;
                break;
            case 39:
                list.UpgradeAllAttribute(5);
                break;
        }
        GPGSBinder.Inst.CharacAchievement(characStat.ID); // 특성 업적
    }
}
