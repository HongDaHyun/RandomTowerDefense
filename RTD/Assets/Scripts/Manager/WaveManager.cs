using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : Singleton<WaveManager>
{
    public int curWave;
    public bool isWave;
    public Wave[] waves;

    private void Update()
    {
        EndWaveSet();
    }

    private void EndWaveSet() //웨이브 끝났을 때 설정들
    {
        if (isWave == false || SpawnManager.Instance.enemyList.Count > 0)
            return;
        isWave = false;
        GameManager.Instance.curExp++;
        StartCoroutine(UIManager.Instance.CountingKey(2));
        StartCoroutine(UIManager.Instance.WaveTimer());
    }
}

[System.Serializable]
public struct Wave
{
    public float spawnTime;
    public WaveEnemy[] enemy;
}

[System.Serializable]
public class WaveEnemy
{
    public string name;
    public int maxCount;
}
