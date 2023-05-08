using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Redcode.Pools;

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

    private void Start()
    {
        StartCoroutine(UIManager.Instance.WaveTimer());
    }

    #region SpawnEnemy
    public void SpawnEnemy()
    {
        if (enemyList.Count != 0)
            return;
        if (WaveManager.Instance.curWave >= GameManager.Instance.maxWave)
        {
            if(GameManager.Instance.level != Level.Infinite) //무한모드일 때 웨이브 초기화 (적 스탯 모두 증가)
                return;
            WaveManager.Instance.curWave = 0;
        }
        StartCoroutine(SpawnEnemyCoroutine());
    }

    public void DeSpawnEnemy(Enemy enemy)
    {
        enemyList.Remove(enemy);
        PoolManager.Instance.TakeToPool<Enemy>(enemy.enemyStat.enemyName, enemy);
    }

    private IEnumerator SpawnEnemyCoroutine()
    {
        WaveManager.Instance.isWave = true;
        WaveManager.Instance.curWave++; //웨이브 시작 시 curWave 증가
        Wave realCurWave = WaveManager.Instance.waves[WaveManager.Instance.curWave - 1]; //증가된 상태이므로 1 감소시킨 웨이브 등장

        for(int i = 0; i < realCurWave.enemy.Length; i++)
        {
            int spawnEnemyCount = 0;
            while (spawnEnemyCount < realCurWave.enemy[i].maxCount)
            {
                Enemy newEnemy = PoolManager.Instance.GetFromPool<Enemy>(realCurWave.enemy[i].name);
                enemyList.Add(newEnemy);
                spawnEnemyCount++;
                yield return new WaitForSeconds(realCurWave.spawnTime);
            }
        }
        //모든 적 소환 완료
    }
    #endregion

    #region SpawnUnit
    public void SpawnUnit(UnitType type, Tier tier)
    {
        Unit newUnit = PoolManager.Instance.GetFromPool<Unit>(CSVReader.Instance.UnitExport_forSpawn(type, tier));
        unitList.Add(newUnit);
        TileGround tileGround = UIManager.Instance.clickTile.GetComponentInParent<TileGround>();
        tileGround.SetTile(newUnit);
        if (UIManager.Instance.clickTile.activeSelf)
            UIManager.Instance.clickTile.SetActive(false);
    }

    public void DeSpawnUnit(Unit unit)
    {
        unitList.Remove(unit);
        unit.GetComponentInParent<TileGround>().ResetTile();
        PoolManager.Instance.TakeToPool<Unit>(unit.unitStat.unitName, unit);
    }

    public void SpawnTier(Tier _tier, Transform spawnPoint)
    {
        Transform trans = PoolManager.Instance.GetFromPool<Transform>(_tier.ToString());
        trans.SetParent(spawnPoint);
        trans.localPosition = new Vector3(0, 0, 0);
        trans.localScale = new Vector2(0.2f, 0.2f);
    }
    #endregion

    #region SpawnProjectile
    public Projectile SpawnProjectile(Transform start, Transform target, UnitStat unitStat, string _name)
    {
        Projectile newProjectile = PoolManager.Instance.GetFromPool<Projectile>(_name);
        newProjectile.SetUp(start, target, unitStat);
        return newProjectile;
    }

    public void DeSpawnProjectile(Projectile projectile)
    {
        PoolManager.Instance.TakeToPool<Projectile>(projectile.name, projectile);
    }

    public Explosion_Death SpawnExplosion(Transform start, float damage, string name)
    {
        Explosion_Death newProjectile = PoolManager.Instance.GetFromPool<Explosion_Death>(name);
        newProjectile.transform.position = start.position;
        newProjectile.SetDmg(damage);
        return newProjectile;
    }

    public void DeSpawnExplosion(Explosion_Death explosion)
    {
        PoolManager.Instance.TakeToPool<Explosion_Death>(explosion.name, explosion);
    }
    #endregion
}