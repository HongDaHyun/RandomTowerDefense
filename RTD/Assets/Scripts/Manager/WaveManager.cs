using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : Singleton<WaveManager>
{
    public int curWave;
    public bool isWave;
    public Wave[] waves;
    public float dmgPerWave;

    GameManager gm;
    CharacManager cm;
    CSVList cl;

    private void Start()
    {
        gm = GameManager.Instance;
        cm = CharacManager.Instance;
        cl = CSVReader.Instance.csvList;
    }

    private void Update()
    {
        EndWaveSet();
    }

    private void EndWaveSet() //웨이브 끝났을 때 설정들
    {
        if (isWave == false || SpawnManager.Instance.enemyList.Count > 0)
            return;
        isWave = false;

        if(curWave == gm.levelNum[(int)gm.level])
        {
            // Clear
        }

        gm.gatchaKey += 2;

        // Charac
        gm.GainHealth(cm.healthWave); // 10
        if (curWave % 2 == 0) // 8
            gm.gatchaKey += cm.doubleWave;
        if (curWave % 5 == 0) // 34
            gm.gatchaKey += cm.wave5;

        if(cm.dmgWave > 0) // 13
            dmgPerWave = curWave * cm.dmgWave;
        if (cm.dmgWaveGun > 0) // 24
            cl.UpgradeUnit(cl.FindUnit(UnitType.Fighter), UpUnit.All, cm.dmgWaveGun);

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
