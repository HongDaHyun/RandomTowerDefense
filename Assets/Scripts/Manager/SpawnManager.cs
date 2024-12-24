using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Redcode.Pools;
using TMPro;
using System.Linq;
using DG.Tweening;

public class SpawnManager : Singleton<SpawnManager>
{
    public Transform[] stageMaps;
    public List<Enemy> enemyList;
    public List<Unit> unitList;

    protected override void Awake()
    {
        base.Awake();
        enemyList = new List<Enemy>();
        unitList = new List<Unit>();
    }

    public void SpawnTilemap(int index)
    {
        Transform stage = Instantiate(stageMaps[index]);

        SpriteRenderer[] grounds = stage.GetComponentsInChildren<SpriteRenderer>();
        for (int i = 0; i < grounds.Length; i++)
        {
            if (grounds[i].name == "Ground1")
                grounds[i].sprite = SpriteData.Instance.tileMapSprites[index].ground1;
            else if(grounds[i].name == "Ground2")
                grounds[i].sprite = SpriteData.Instance.tileMapSprites[index].ground2;
        }
    }

    #region SpawnEnemy
    public void SpawnEnemy()
    {
        if (enemyList.Count != 0)
            return;

        WaveManager wm = WaveManager.Instance;

        wm.isWave = true;

        if (wm.curWave > 21) // maxWave를 넘길 시
            wm.RanWave();

        StartCoroutine(SpawnEnemyRoutine(wm.waves[wm.curWave - 1]));
    }

    private IEnumerator SpawnEnemyRoutine(Wave wave)
    {
        CSVList cl = CSVReader.Instance.csvList;

        int totalEnemyCount = wave.enemy.Sum(enemyData => enemyData.maxCount);

        foreach (WaveEnemy enemyData in wave.enemy)
        {
            int length = enemyData.maxCount;

            // 티어에 맞는 무작위 적 정하기
            List<EnemyStat> enemyStats = cl.FindEnemy(enemyData.tier);
            string ranEnemy = enemyStats[Random.Range(0, enemyStats.Count)].enemyName;

            for (int i = 0; i < length; i++)
            {
                Enemy newEnemy = PoolManager.Instance.GetFromPool<Enemy>(ranEnemy);
                enemyList.Add(newEnemy);
                totalEnemyCount--;

                SoundManager.Instance.PlaySFX("SpawnEnemy");

                yield return new WaitForSeconds(wave.spawnTime);
            }
        }

        while (enemyList.Count > 0)
        {
            yield return null;
        }

        WaveManager.Instance.EndWaveSet(); // 웨이브 종료
    }

    public void DeSpawnEnemy(Enemy enemy)
    {
        enemyList.Remove(enemy);
        PoolManager.Instance.TakeToPool<Enemy>(enemy.enemyStat.enemyName, enemy);
    }

    public Enemy SpawnBabySlime(Enemy bigSlime)
    {
        //작라임 생성
        Enemy babySlime = PoolManager.Instance.GetFromPool<Enemy>($"작{bigSlime.enemyStat.enemyName}");
        enemyList.Add(babySlime);
        UIManager.Instance.upPannel.UpdateCurWave(1); // 전체 웨이브 몬스터 증가(새로운 슬라임이 생성되었기 때문)

        //위치 조정
        float ranPos = Random.Range(-5, 5) / 10f;
        babySlime.transform.position = new Vector2(bigSlime.transform.position.x + ranPos, bigSlime.transform.position.y + ranPos);
        babySlime.curWayPoint = bigSlime.curWayPoint;
        babySlime.moveDirection = bigSlime.moveDirection;

        //스탯 조정
        babySlime.enemyStat = bigSlime.enemyStat;
        babySlime.enemyStat.enemyName = babySlime.name; // 이름
        babySlime.enemyStat.tier = Tier.Common; // 티어(경험치)
        babySlime.enemyStat.enemyType = EnemyType.SlimeBaby; // 타입
        babySlime.enemyStat.hp /= 2; // 체력
        babySlime.curHp = babySlime.enemyStat.hp;
        babySlime.speed = babySlime.enemyStat.moveSpeed / 100;
        babySlime.ResetDebuffStat();

        return babySlime;
    }
    #endregion

    #region SpawnUnit
    public Unit FindUnit(Unit _unit)
    {
        return unitList.Find(unit => unit.unitStat.unitName == _unit.unitStat.unitName && unit != _unit);
    }

    public void SpawnUnit(UnitType type, Tier tier)
    {
        Unit newUnit = PoolManager.Instance.GetFromPool<Unit>(CSVReader.Instance.csvList.FindUnit(type, tier));

        unitList.Add(newUnit);

        // 타일
        TileGround tileGround = UIManager.Instance.clickTile.GetComponentInParent<TileGround>();
        tileGround.SetTile(newUnit);
        if (UIManager.Instance.clickTile.activeSelf)
            UIManager.Instance.clickTile.SetActive(false);

        if (newUnit.GetComponentInChildren<TierParticle>() == null)
            SpawnTier(tier, newUnit.transform);

        if ((int)tier > 2) // Epic, Legend
        {
            SpawnVFX($"{tier}VFX", newUnit.transform);
            SoundManager.Instance.PlaySFX("SpawnUnitUp");
        }
        else // Common, Rare
            SoundManager.Instance.PlaySFX("SpawnUnit");

        QuestManager qm = QuestManager.Instance;

        qm.UpQuest(9 + ((int)tier - 1)); // 퀘스트 9 ~ 12
        qm.UpQuest(14 + (int)type); // 퀘스트 14 ~ 21

        // Charac
        CharacManager cm = CharacManager.Instance;
        CSVReader.Instance.csvList.UpgradeAllUnit(UpUnit.Dmg, cm.spawnDmg); // 33
        // 35
        if(tier == Tier.Common)
        {
            int pro = Random.Range(0, 100);
            if(pro < cm.commonGatcha)
            {
                pro = Random.Range(1, 4);
                GameManager.Instance.GainGatcha(pro);
            }
        }
    }

    public void DeSpawnUnit(Unit unit)
    {
        unitList.Remove(unit);
        unit.GetComponentInParent<TileGround>().ResetTile();
        PoolManager.Instance.TakeToPool<Unit>(unit.unitStat.unitName, unit);
    }

    public void SpawnTier(Tier _tier, Transform spawnPoint)
    {
        if ((int)_tier < 3) // Rare 이하 티어 생성
            return;
        Transform trans = PoolManager.Instance.GetFromPool<Transform>(_tier.ToString());
        trans.SetParent(spawnPoint);
        trans.localPosition = new Vector3(0, 0, 0);
        trans.localScale = new Vector3(0.15f, 0.15f, 0.1f);
    }
    #endregion

    #region SpawnProjectile
    public Projectile SpawnProjectile(Unit _unit, string _name)
    {
        Projectile newProjectile = PoolManager.Instance.GetFromPool<Projectile>(_name);
        newProjectile.SetPos(_unit.transform.GetChild(0), _unit.attackTarget);
        newProjectile.unit = _unit;
        return newProjectile;
    }

    public void DeSpawnProjectile(Projectile projectile)
    {
        PoolManager.Instance.TakeToPool<Projectile>(projectile.name, projectile);
    }

    public Explosion_Death SpawnExplosion(Transform start, Projectile germ, string name)
    {
        Explosion_Death newProjectile = PoolManager.Instance.GetFromPool<Explosion_Death>(name);
        newProjectile.transform.position = start.position;
        newProjectile.transform.localScale = start.localScale;
        newProjectile.SetParentProjectile(germ);
        return newProjectile;
    }

    public void DeSpawnExplosion(Explosion_Death explosion)
    {
        PoolManager.Instance.TakeToPool<Explosion_Death>(explosion.name, explosion);
    }
    #endregion

    #region SpawnUI
    public PopUpText SpawnDmgText(Enemy enemy, float dmg, bool isCrit)
    {
        PopUpText dmgText = PoolManager.Instance.GetFromPool<PopUpText>("DmgText");
        dmgText.PopUp_DMG_New(enemy, dmg, isCrit);
        return dmgText;
    }

    public void DeSpawnDmgText(PopUpText text)
    {
        if (text == null)
            return;

        PoolManager.Instance.TakeToPool<PopUpText>(text);
    }

    public Particle SpawnVFX(string name, Transform trans)
    {
        Particle particle = PoolManager.Instance.GetFromPool<Particle>(name);
        particle.transform.position = trans.position;
        return particle;
    }

    public void DeSpawnVFX(string name, Particle particle)
    {
        PoolManager.Instance.TakeToPool<Particle>(name, particle);
    }

    public Bubble SpawnTxtBubble(Transform parent, string s)
    {
        // 이미 말풍선이 있다면
        if (parent.GetComponentInChildren<Bubble>() != null)
            return null;

        SoundManager.Instance.PlaySFX("Pop"); // 사운드

        // 말풍선 소환
        Bubble bubble = PoolManager.Instance.GetFromPool<Bubble>("TxtBubble");
        bubble.transform.SetParent(parent);
        bubble.transform.localPosition = Vector3.zero;
        bubble.text.text = s;

        // 닷트윈()
        BubblePop(bubble);

        return bubble;
    }

    public Bubble SpawnCharacBubble(RectTransform parent, CharacSlot slot)
    {
        // 이미 말풍선이 있다면
        if (slot.isShow)
            return null;

        SoundManager.Instance.PlaySFX("Pop"); // 사운드

        // 말풍선 소환
        Bubble bubble = PoolManager.Instance.GetFromPool<Bubble>("CharacBubble");
        bubble.transform.SetParent(slot.transform);
        bubble.transform.localPosition = Vector3.zero;
        bubble.text.text = slot.s;
        bubble.transform.SetParent(parent);

        // 닷트윈()
        BubblePop_Charac(slot, bubble);

        return bubble;
    }

    private void BubblePop(Bubble bubble)
    {
        bubble.transform.localScale = Vector3.zero;

        Sequence seq = DOTween.Sequence();
        seq.Append(bubble.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBounce));
        seq.AppendInterval(2f);
        seq.Append(bubble.transform.DOScale(0f, 0.25f).SetEase(Ease.InBack).OnComplete(() => PoolManager.Instance.TakeToPool<Bubble>("TxtBubble", bubble)));
        seq.Play();
    }

    private void BubblePop_Charac(CharacSlot slot, Bubble bubble)
    {
        bubble.transform.localScale = Vector3.zero;

        Sequence seq = DOTween.Sequence();
        seq.Append(bubble.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBounce).OnStart(() => slot.isShow = true));
        seq.AppendInterval(2f);
        seq.Append(bubble.transform.DOScale(0f, 0.25f).SetEase(Ease.InBack).OnComplete(() => {
            PoolManager.Instance.TakeToPool<Bubble>("CharacBubble", bubble);
            slot.isShow = false;
        }));
        seq.Play();
    }

    public Transform SpawnUI(string name)
    {
        return PoolManager.Instance.GetFromPool<Transform>(name);
    }

    public void DeSpawnUI(string name, Transform trans)
    {
        PoolManager.Instance.TakeToPool<Transform>(name, trans);
    }
    #endregion

    #region Find
    public int NumUnit(UnitType type)
    {
        List<Unit> units = unitList.FindAll(unit => unit.unitStat.unitType == type);
        return units.Count;
    }
    #endregion
}