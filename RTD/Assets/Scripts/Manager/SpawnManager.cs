using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Redcode.Pools;

public class SpawnManager : Singleton<SpawnManager>
{
    public float spawnTime;

    private void Start()
    {
        StartCoroutine(SpawnEnemy());
    }

    private IEnumerator SpawnEnemy()
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
        UIManager.Instance.clickTile.GetComponentInParent<TileGround>().isBuild = true;
        newUnit.transform.position = UIManager.Instance.clickTile.transform.position;
    }
}
