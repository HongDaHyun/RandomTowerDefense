using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Redcode.Pools;

public class SpawnManager : Singleton<SpawnManager>
{
    public Transform wayPointsObj;
    public float spawnTime;

    public void SpawnEnemy()
    {
        StartCoroutine(SpawnEnemyCoroutine());
    }

    private IEnumerator SpawnEnemyCoroutine()
    {
        while(true)
        {
            Enemy newEnemy = PoolManager.Instance.GetFromPool<Enemy>(0);
            yield return new WaitForSeconds(spawnTime);
        }
    }

    public void SpawnUnit()
    {
        if (!UIManager.Instance.clickTile.activeSelf || UIManager.Instance.clickTile.GetComponentInParent<TileGround>().isBuild)
            return;
        Unit newUnit = PoolManager.Instance.GetFromPool<Unit>();
        TileGround tileGround = UIManager.Instance.clickTile.GetComponentInParent<TileGround>();
        tileGround.SetTile(newUnit);
        if (UIManager.Instance.clickTile.activeSelf)
            UIManager.Instance.clickTile.SetActive(false);
    }
}
