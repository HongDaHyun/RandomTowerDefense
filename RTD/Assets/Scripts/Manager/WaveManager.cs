using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : Singleton<WaveManager>
{
    public int curWave = 1;
    public int maxWaveEnemy;
    public bool isWave;
    public List<Wave> waves;
    public float readyTime;

    private void Start()
    {
        StartCoroutine(TimeWave());
    }

    public void EndWaveSet() //웨이브 종료
    {
        CSVList cl = CSVReader.Instance.csvList;
        UIManager um = UIManager.Instance;
        CharacManager cm = CharacManager.Instance;
        GameManager gm = GameManager.Instance;

        isWave = false;

        SoundManager.Instance.PlaySFX("EndWave"); // 사운드

        // 클리어
        if (curWave == gm.levelNum[(int)gm.level])
        {
            gm.GameClear();
            return;
        }

        // 최대 웨이브 도달 시 모든 적 동기화
        if (curWave == waves.Count)
            cl.SetAllEnemy();

        // 웨이브 종료 시 변수 설정
        gm.GainGatcha(2);
        curWave++; //웨이브 시작 시 curWave 증가
        um.upPannel.SetWaveTxt(gm); // UI
        cl.UpgradeAllEnemy(UpEnemy.Hp, (int)gm.level); // 적 HP 증가
        um.upPannel.ToggleCurWavePannel(false);

        // Quest Cool
        if (um.questPannel.curCool < um.questPannel.maxCool)
            um.questPannel.curCool++;
        else
            um.questPannel.refreshBtn.interactable = true;

        // 퀘스트
        QuestManager.Instance.UpQuest(0); // 퀘스트 0

        // Charac
        gm.GainHealth(gm.healthWave); // 10
        if (curWave % 3 == 0) // 8
            gm.GainGatcha(cm.doubleWave);
        if (curWave % 5 == 0) // 34
            gm.GainGatcha(cm.wave5);
        if (cm.dmgWave > 0) // 13
            cl.UpgradeAllUnit(UpUnit.Dmg, cm.dmgWave);
        if (cm.dmgWaveGun > 0) // 24
            cl.UpgradeUnit(cl.FindUnit(UnitType.Gun), UpUnit.All, cm.dmgWaveGun);

        // 타이머 시작
        StartCoroutine(TimeWave());
    }

    public IEnumerator TimeWave()
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
                CountEnemyWave(); // 최대 적 수 세기
                UIManager.Instance.upPannel.ToggleCurWavePannel(true); // UI
            }

            UIManager.Instance.upPannel.timerTxt.text = readyTime.ToString("00.0"); // UI
        }
        // 웨이브 시작
    }

    public void CountEnemyWave()
    {
        maxWaveEnemy = 0;
        foreach (var enemy in waves[curWave - 1].enemy)
            maxWaveEnemy += enemy.maxCount;
    }

    public void RanWave()
    {
        CSVList cl = CSVReader.Instance.csvList;
        Wave newWave;

        newWave.spawnTime = Mathf.Round(Random.Range(0.8f, 1.3f) * 10) / 10f; // 스폰 주기
        // 적 종류
        int waveEnemyKind = Random.Range(3, 10);
        newWave.enemy = new WaveEnemy[waveEnemyKind];

        // 최대 적 수
        int countNewWave = maxWaveEnemy + Random.Range(10, 21);
        int countNum = countNewWave / waveEnemyKind;
        int countNumRes = countNewWave % waveEnemyKind;

        for (int i = 0; i < waveEnemyKind; i++)
        {
            newWave.enemy[i] = new WaveEnemy();
            newWave.enemy[i].name = cl.FindEnemy(Random.Range(0, cl.enemyStat.Length)).enemyName; // 이름 설정

            newWave.enemy[i].maxCount = countNum;
        }

        for(int i = 0; i < countNumRes; i++)
        {
            newWave.enemy[i].maxCount++;

            if (i >= waveEnemyKind)
                i = 0;
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
