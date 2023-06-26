using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : Singleton<WaveManager>
{
    public int curWave = 1;
    public int maxWave;
    public bool isWave;
    public List<Wave> waves;
    public float readyTime;

    private void Start()
    {
        maxWave = waves.Count;
    }

    private void Update()
    {
        EndWaveSet();
    }

    private void EndWaveSet() //웨이브 종료
    {
        if (isWave == false || SpawnManager.Instance.enemyList.Count > 0)
            return;
        CSVList cl = CSVReader.Instance.csvList;
        UIManager um = UIManager.Instance;
        CharacManager cm = CharacManager.Instance;
        GameManager gm = GameManager.Instance;

        isWave = false;

        SoundManager.Instance.PlaySFX("EndWave");

        if (curWave == gm.levelNum[(int)gm.level])
        {
            gm.GameClear();
            return;
        }

        if (curWave == maxWave)
            cl.SetAllEnemy();

        gm.gatchaKey += 2;
        curWave++; //웨이브 시작 시 curWave 증가

        // Quest Cool
        if (um.questPannel.curCool < um.questPannel.maxCool)
            um.questPannel.curCool++;

        // Charac
        gm.GainHealth(cm.healthWave); // 10
        if (curWave % 2 == 0) // 8
            gm.gatchaKey += cm.doubleWave;
        if (curWave % 5 == 0) // 34
            gm.gatchaKey += cm.wave5;

        if (cm.dmgWave > 0) // 13
            cl.UpgradeAllUnit(UpUnit.Dmg, cm.dmgWave);
        if (cm.dmgWaveGun > 0) // 24
            cl.UpgradeUnit(cl.FindUnit(UnitType.Gun), UpUnit.All, cm.dmgWaveGun);

        // 적 HP 증가
        cl.UpgradeAllEnemy(UpEnemy.Hp, (int)gm.level * curWave);

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
                readyTime = 0;
                SoundManager.Instance.PlaySFX("StartWave");
                SpawnManager.Instance.SpawnEnemy();
            }
        }
    } // 웨이브 시작

    public void RanWave(Wave wave)
    {
        CSVList cl = CSVReader.Instance.csvList;
        Wave newWave;

        newWave.spawnTime = Mathf.Round(Random.Range(0.8f, 1.3f) * 10) / 10f; // 스폰 주기
        // 적 종류
        int waveEnemyKind = Random.Range(3, 10);
        newWave.enemy = new WaveEnemy[waveEnemyKind];

        // 최대 적 수
        int maxWave = 0;
        foreach (var enemy in wave.enemy)
            maxWave += enemy.maxCount;

        // 종류 별 적 설정
        for (int i = 0; i < waveEnemyKind; i++)
        {
            newWave.enemy[i] = new WaveEnemy();
            newWave.enemy[i].name = cl.FindEnemy(Random.Range(0, cl.enemyStat.Length)).enemyName; // 이름 설정

            // 마지막 인덱스
            if (i == waveEnemyKind - 1)
                newWave.enemy[i].maxCount = maxWave; // 나머지

            // 마지막 인덱스 X
            else
            {
                int count = Random.Range(3, maxWave); // 최대 적 수 사이 랜덤 값
                maxWave -= count;
                newWave.enemy[i].maxCount = count;
            }
        }
        waves.Add(newWave);
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
