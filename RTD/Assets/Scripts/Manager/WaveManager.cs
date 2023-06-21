using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : Singleton<WaveManager>
{
    public int curWave;
    public bool isWave;
    public Wave[] waves;
    public float dmgPerWave, readyTime;

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

        SoundManager.Instance.PlaySFX("EndWave");

        if(curWave == gm.levelNum[(int)gm.level])
        {
            // GameClear
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

        // 적 HP 증가
        cl.UpgradeAllEnemy(UpEnemy.Hp, (int)gm.level + 1);

        StartCoroutine(StartWave());
    }

    public IEnumerator StartWave()
    {
        readyTime = 10f;

        while (readyTime > 0) // 타이머 시작
        {
            readyTime -= Time.deltaTime;
            yield return null;

            if (readyTime <= 0) //웨이브 시작(타이머 0)
            {
                SoundManager.Instance.PlaySFX("StartWave");
                SpawnManager.Instance.SpawnEnemy();
            }
        }
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
