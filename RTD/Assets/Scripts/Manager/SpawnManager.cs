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
        PoolManager.Instance.TakeToPool<Enemy>(enemy.idName, enemy);
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
                switch(realCurWave.enemy[i].type)
                {
                    case EnemyType.Golem:
                        break;
                    case EnemyType.Human:
                        Enemy newEnemy = PoolManager.Instance.GetFromPool<Enemy_People>(realCurWave.enemy[i].index);
                        enemyList.Add(newEnemy);
                        break;
                    case EnemyType.Portion:
                        break;
                    case EnemyType.Skeleton:
                        break;
                    case EnemyType.Slime:
                        break;
                }
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
        PoolManager.Instance.TakeToPool<Unit>(unit.idName, unit);
    }
    #endregion

    #region SpawnProjectile
    public void SpawnProjectile(Transform transform, Transform target, UnitStat unitStat)
    {
        Projectile newProjectile = PoolManager.Instance.GetFromPool<Projectile>();
        newProjectile.transform.position = transform.position;
        newProjectile.SetTarget(target);
        newProjectile.unitStat = unitStat;
    }

    public void DeSpawnProjectile(Projectile projectile)
    {
        PoolManager.Instance.TakeToPool<Projectile>(projectile.idName, projectile);
    }
    #endregion
}