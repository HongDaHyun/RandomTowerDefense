using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Redcode.Pools;
using TMPro;
using System.Linq;

public class SpawnManager : Singleton<SpawnManager>
{
    public Transform wayPointsObj;
    public List<Enemy> enemyList;
    public List<Unit> unitList;

    protected override void Awake()
    {
        base.Awake();
        enemyList = new List<Enemy>();
        unitList = new List<Unit>();
    }

    #region SpawnEnemy
    public void SpawnEnemy()
    {
        if (enemyList.Count != 0)
            return;

        WaveManager wm = WaveManager.Instance;

        wm.isWave = true;

        if (wm.curWave > wm.waves.Count) // maxWave를 넘길 시
        {
            wm.RanWave(); // 바로 전 wave데이터를 바탕으로 새로운 웨이브 생성
        }

        StartCoroutine(SpawnEnemyRoutine(wm.waves[wm.curWave - 1]));
    }

    private IEnumerator SpawnEnemyRoutine(Wave wave)
    {
        int totalEnemyCount = wave.enemy.Sum(enemyData => enemyData.maxCount);

        foreach (WaveEnemy enemyData in wave.enemy)
        {
            int length = enemyData.maxCount;
            for (int i = 0; i < length; i++)
            {
                Enemy newEnemy = PoolManager.Instance.GetFromPool<Enemy>(enemyData.name);
                enemyList.Add(newEnemy);
                totalEnemyCount--;

                SoundManager.Instance.PlaySFX("SpawnEnemy");

                yield return new WaitForSeconds(wave.spawnTime);
            }
        }

        while (enemyList.Count > 0)
        {
            yield return null;
        }

        WaveManager.Instance.EndWaveSet(); // 웨이브 종료
    }

    public void DeSpawnEnemy(Enemy enemy)
    {
        enemyList.Remove(enemy);
        PoolManager.Instance.TakeToPool<Enemy>(enemy.enemyStat.enemyName, enemy);
    }

    public Enemy SpawnBabySlime(Enemy bigSlime)
    {
        //작라임 생성
        Enemy babySlime = PoolManager.Instance.GetFromPool<Enemy>($"작{bigSlime.enemyStat.enemyName}");
        enemyList.Add(babySlime);
        WaveManager.Instance.maxWaveEnemy++; // 전체 웨이브 몬스터 증가(새로운 슬라임이 생성되었기 때문)
        GameManager.Instance.killEnemyNum--;

        //위치 조정
        float ranPos = Random.Range(-5, 5) / 10f;
        babySlime.transform.position = new Vector2(bigSlime.transform.position.x + ranPos, bigSlime.transform.position.y + ranPos);
        babySlime.curWayPoint = bigSlime.curWayPoint;
        babySlime.moveDirection = bigSlime.moveDirection;

        //스탯 조정
        babySlime.enemyStat = bigSlime.enemyStat;
        babySlime.enemyStat.enemyName = babySlime.name; // 이름
        babySlime.enemyStat.tier = Tier.Common; // 티어(경험치)
        babySlime.enemyStat.enemyType = EnemyType.SlimeBaby; // 타입
        babySlime.enemyStat.hp /= 2; // 체력
        babySlime.curHp = babySlime.enemyStat.hp;
        babySlime.speed = babySlime.enemyStat.moveSpeed / 100;
        babySlime.ResetDebuffStat();

        return babySlime;
    }
    #endregion

    #region SpawnUnit
    public Unit FindUnit(Unit _unit)
    {
        return unitList.Find(unit => unit.unitStat.unitName == _unit.unitStat.unitName && unit != _unit);
    }

    public void SpawnUnit(UnitType type, Tier tier)
    {
        Unit newUnit = PoolManager.Instance.GetFromPool<Unit>(CSVReader.Instance.csvList.FindUnit(type, tier));

        unitList.Add(newUnit);

        // 타일
        TileGround tileGround = UIManager.Instance.clickTile.GetComponentInParent<TileGround>();
        tileGround.SetTile(newUnit);
        if (UIManager.Instance.clickTile.activeSelf)
            UIManager.Instance.clickTile.SetActive(false);

        if (newUnit.GetComponentInChildren<TierParticle>() == null)
            SpawnTier(tier, newUnit.transform);

        if ((int)tier > 2) // Epic, Legend
        {
            SpawnVFX($"{tier}VFX", newUnit.transform);
            SoundManager.Instance.PlaySFX("SpawnUnitUp");
        }
        else // Common, Rare
            SoundManager.Instance.PlaySFX("SpawnUnit");

        QuestManager qm = QuestManager.Instance;

        qm.UpQuest(9 + ((int)tier - 1)); // 퀘스트 9 ~ 12
        qm.UpQuest(14 + (int)type); // 퀘스트 14 ~ 21

        // Charac
        CharacManager cm = CharacManager.Instance;
        CSVReader.Instance.csvList.UpgradeAllUnit(UpUnit.Dmg, cm.spawnDmg); // 33
        // 35
        if(tier == Tier.Common)
        {
            int pro = Random.Range(0, 100);
            if(pro < cm.commonGatcha)
            {
                pro = Random.Range(1, 4);
                GameManager.Instance.GainGatcha(pro);
            }
        }
    }

    public void DeSpawnUnit(Unit unit)
    {
        unitList.Remove(unit);
        unit.GetComponentInParent<TileGround>().ResetTile();
        PoolManager.Instance.TakeToPool<Unit>(unit.unitStat.unitName, unit);
    }

    public void SpawnTier(Tier _tier, Transform spawnPoint)
    {
        if ((int)_tier < 3) // Rare 이하 티어 생성
            return;
        Transform trans = PoolManager.Instance.GetFromPool<Transform>(_tier.ToString());
        trans.SetParent(spawnPoint);
        trans.localPosition = new Vector3(0, 0, 0);
        trans.localScale = new Vector3(0.15f, 0.15f, 0.1f);
    }
    #endregion

    #region SpawnProjectile
    public Projectile SpawnProjectile(Unit _unit, string _name)
    {
        Projectile newProjectile = PoolManager.Instance.GetFromPool<Projectile>(_name);
        newProjectile.SetPos(_unit.transform.GetChild(0), _unit.attackTarget);
        newProjectile.unit = _unit;
        return newProjectile;
    }

    public void DeSpawnProjectile(Projectile projectile)
    {
        PoolManager.Instance.TakeToPool<Projectile>(projectile.name, projectile);
    }

    public Explosion_Death SpawnExplosion(Transform start, Projectile germ, string name)
    {
        Explosion_Death newProjectile = PoolManager.Instance.GetFromPool<Explosion_Death>(name);
        newProjectile.transform.position = start.position;
        newProjectile.transform.localScale = start.localScale;
        newProjectile.SetParentProjectile(germ);
        return newProjectile;
    }

    public void DeSpawnExplosion(Explosion_Death explosion)
    {
        PoolManager.Instance.TakeToPool<Explosion_Death>(explosion.name, explosion);
    }
    #endregion

    #region SpawnUI
    public TextMeshPro SpawnDmgText(float dmg)
    {
        TextMeshPro dmgText = PoolManager.Instance.GetFromPool<TextMeshPro>("DmgText");
        dmgText.alpha = 1f;
        dmgText.SetText(dmg.ToString("F1")); // 소수점 한자리까지만 표기
        return dmgText;
    }

    public void DeSpawnDmgText(TextMeshPro tmpro)
    {
        PoolManager.Instance.TakeToPool<TextMeshPro>("DmgText", tmpro);
    }

    public Particle SpawnVFX(string name, Transform trans)
    {
        Particle particle = PoolManager.Instance.GetFromPool<Particle>(name);
        particle.transform.position = trans.position;
        return particle;
    }

    public void DeSpawnVFX(string name, Particle particle)
    {
        PoolManager.Instance.TakeToPool<Particle>(name, particle);
    }

    public Transform SpawnUI(string name)
    {
        return PoolManager.Instance.GetFromPool<Transform>(name);
    }

    public void DeSpawnUI(string name, Transform trans)
    {
        PoolManager.Instance.TakeToPool<Transform>(name, trans);
    }
    #endregion

    #region Find
    public int NumUnit(UnitType type)
    {
        List<Unit> units = unitList.FindAll(unit => unit.unitStat.unitType == type);
        return units.Count;
    }
    #endregion
}