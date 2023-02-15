using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Redcode.Pools;

public class SpawnManager : Singleton<SpawnManager>
{
    public Transform wayPointsObj;
    public float spawnTime;
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
        StartCoroutine(SpawnEnemyCoroutine());
    }

    public void DeSpawnEnemy(Enemy enemy)
    {
        enemyList.Remove(enemy);
        PoolManager.Instance.TakeToPool<Enemy>(enemy.idName, enemy);
    }

    private IEnumerator SpawnEnemyCoroutine()
    {
        while(true)
        {
            Enemy newEnemy = PoolManager.Instance.GetFromPool<Enemy>(0);
            enemyList.Add(newEnemy);
            yield return new WaitForSeconds(spawnTime);
        }
    }
    #endregion

    #region SpawnUnit
    public void SpawnUnit()
    {
        Unit newUnit = PoolManager.Instance.GetFromPool<Unit>();
        unitList.Add(newUnit);
        TileGround tileGround = UIManager.Instance.clickTile.GetComponentInParent<TileGround>();
        tileGround.SetTile(newUnit);
        if (UIManager.Instance.clickTile.activeSelf)
            UIManager.Instance.clickTile.SetActive(false);
    }

    public void DeSpawnUnit(Unit unit)
    {
        unitList.Remove(unit);
        PoolManager.Instance.TakeToPool<Unit>(unit.idName, unit);
    }
    #endregion

    #region SpawnProjectile
    public void SpawnProjectile(Transform transform, Transform target)
    {
        Projectile newProjectile = PoolManager.Instance.GetFromPool<Projectile>();
        newProjectile.transform.position = transform.position;
        newProjectile.SetTarget(target);
    }

    public void DeSpawnProjectile(Projectile projectile)
    {
        PoolManager.Instance.TakeToPool<Projectile>(projectile.idName, projectile);
    }
    #endregion
}