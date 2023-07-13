using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Redcode.Pools;
using UnityEngine.UI;
using System;

public class Enemy : MonoBehaviour, IPoolObject
{
    GameManager gm;
    CharacManager cm;
    UIManager um;
    SpawnManager sm;
    CSVReader cr;
    QuestManager qm;
    SoundManager soundm;
    WaveManager wm;

    private SpriteRenderer spriteRenderer;
    private Animator anim;
    public EnemyStat enemyStat;
    [HideInInspector] public float speed;
    private Dictionary<Attribute, bool> debuffState = new Dictionary<Attribute, bool>()
    {
        {Attribute.Fire, false },
        {Attribute.Germ, false },
        {Attribute.Ice, false },
        {Attribute.Thunder, false }
    };
    public Sprite defSprite;

    #region Movement
    [HideInInspector] public Vector3 moveDirection;
    private Transform[] wayPoints;
    [HideInInspector] public int curWayPoint;
    #endregion
    #region HP
    private int curLife;
    public int maxLife;
    [HideInInspector] public float curHp;
    public bool isDie = false; public bool isStop = false;
    private Slider hpSlider;
    #endregion

    public void OnCreatedInPool() //최초
    {
        name = name.Replace("(Clone)", "");
        defSprite = GetComponent<SpriteRenderer>().sprite;

        SetUp();

        SetWayPoint();
    }

    public void OnGettingFromPool() //재사용
    {
        SetStat();
    }

    private void FixedUpdate()
    {
        Movement();
    }

    private void SetUp()
    {
        // 외부 컴포넌트 가져오기
        gm = GameManager.Instance;
        cm = CharacManager.Instance;
        um = UIManager.Instance;
        sm = SpawnManager.Instance;
        cr = CSVReader.Instance;
        qm = QuestManager.Instance;
        soundm = SoundManager.Instance;
        wm = WaveManager.Instance;

        //자체 컴포넌트 가져오기
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        hpSlider = GetComponentInChildren<Slider>();
    }

    private void SetStat()
    {
        enemyStat = cr.csvList.FindEnemy(name); //스탯 가져오기

        //변수 초기화
        curHp = enemyStat.hp;
        curLife = maxLife;
        isDie = false;
        isStop = false;
        ResetDebuffStat();
        spriteRenderer.color = Color.white;
        hpSlider.value = 1f;

        //위치 설정
        ResetWayPoint();

        speed = enemyStat.moveSpeed / 100;
    }

    public void ResetDebuffStat()
    {
        debuffState[Attribute.Fire] = false; debuffState[Attribute.Germ] = false; debuffState[Attribute.Ice] = false; debuffState[Attribute.Thunder] = false;
    }

    #region Movement Code
    private void Movement()
    {
        if (isStop)
            return;
        transform.position = Vector2.MoveTowards(transform.position, moveDirection, speed);
    }

    private void SetWayPoint()
    {
        if (wayPoints != null)
            return;
        wayPoints = new Transform[sm.wayPointsObj.childCount];
        for (int i = 0; i < wayPoints.Length; i++)
        {
            wayPoints[i] = sm.wayPointsObj.GetChild(i);
        }
        transform.position = wayPoints[curWayPoint].position;
    }
    private void ResetWayPoint()
    {
        curWayPoint = 0;

        transform.position = wayPoints[curWayPoint].position;

        StartCoroutine(OnMove());
    }
    public IEnumerator OnMove()
    {
        NextMoveTo();

        while (true)
        {
            if (transform.position == wayPoints[curWayPoint].position)
                NextMoveTo();
            yield return null;
        }
    }
    private void NextMoveTo()
    {
        if(curWayPoint < wayPoints.Length - 1)
        {
            transform.position = wayPoints[curWayPoint].position;

            if (wayPoints[curWayPoint + 1].position.x >= wayPoints[curWayPoint].position.x) //방향 전환
                spriteRenderer.flipX = true;
            else
                spriteRenderer.flipX = false;
            curWayPoint++;
            Vector3 direction = wayPoints[curWayPoint].position;
            MoveTo(direction);
        }
        else //끝 도달
        {
            sm.DeSpawnEnemy(this);
            um.upPannel.UpdateCurWave(-1);
            gm.LoseHealth((int)enemyStat.tier);
        }
    }

    public void MoveTo(Vector3 direction)
    {
        moveDirection = direction;
    }
    #endregion

    #region Damaged
    //Damage
    private void TakeDmg(Projectile project, Color _color, float time)
    {
        if (isDie || !gameObject.activeSelf)
            return;

        float defenseIgnore = enemyStat.defense * (1f - project.projectileStat.ignoreDefense);
        float defenseRatio = Mathf.Round(100f / (100f + defenseIgnore) * 10f) / 10f;

        float totalDmg = (project.dmg + (curHp * (cm.proportionHP / 100)))
            * defenseRatio; // 총 데미지 (데미지 + 체력비례 데미지) * 방어력

        curHp -= totalDmg;
        project.unit.totalDmg += totalDmg;
        um.info.UpdateUnitUI();
        um.info.UpdateEnemyUI();

        // UI
        StartCoroutine(HPHandlerCoroutine());

        // DmgPopUp
        TMPro.TextMeshPro dmgText = sm.SpawnDmgText(totalDmg);
        dmgText.transform.position = transform.position;

        if (project.isCrit) // 크리티컬 색
            dmgText.color = Color.yellow;
        else
            dmgText.color = Color.white;

        um.PopUp(dmgText);

        StartCoroutine(HitColorAnimation(_color, time));

        // 처형
        if (curHp <= cm.execution * enemyStat.hp) // Charac 30
        {
            project.unit.totalDmg -= curHp - cm.execution * enemyStat.hp;
            SpecificDie();
        }
    }

    // 일반 공격과 데미지 공격 구분
    private IEnumerator Hitted(Projectile project)
    {
        if(project.projectileStat.attribute == Attribute.None) // 일반 공격
        {
            TakeDmg(project, Color.red, 0.05f);
        }
        else // 디버프 공격
        {
            float attributePower = cr.csvList.FindAttribute(project.projectileStat.attribute);
            Attribute kind = project.projectileStat.attribute;

            switch (kind)
            {
                case Attribute.Fire: // 불 공격
                    if (debuffState[kind])
                        project.dmg *= 2 + cm.dmgFire; // Charac 15
                    debuffState[kind] = true;

                    int dmgCount = Mathf.CeilToInt(attributePower / 1 + GetStrengthModifier()); // 반복 횟수(올림)

                    for (int i = 0; i < dmgCount; i++)
                    {
                        TakeDmg(project, Color.red, 0.1f);
                        yield return new WaitForSeconds(0.3f);
                    }
                    break;
                case Attribute.Germ: // 독 공격
                    if (debuffState[kind])
                        project.dmg *= 2 + cm.dmgGerm; // Charac 22
                    debuffState[kind] = true;

                    dmgCount = Mathf.CeilToInt(attributePower / 1 + GetStrengthModifier()); // 반복 횟수(올림)

                    for (int i = 0; i < dmgCount; i++)
                    {
                        TakeDmg(project, Color.black, 0.1f);
                        yield return new WaitForSeconds(0.3f);
                    }
                    break;

                case Attribute.Ice: // 얼음 공격

                    if (debuffState[kind])
                    {
                        TakeDmg(project, Color.cyan, 0.1f);
                        yield break;
                    }
                    debuffState[kind] = true;
                    float slowDuration = Mathf.Min(attributePower - GetStrengthModifier(), 4); // 지속 시간
                    float slowPercent = attributePower / (10 + enemyStat.strength); // 슬로우 %

                    TakeDmg(project, Color.cyan, slowDuration);

                    //슬로우
                    speed *= Mathf.Max(1 - slowPercent, 0.2f); // 느려진 속도(최대 80%)
                    yield return new WaitForSeconds(slowDuration);
                    speed = enemyStat.moveSpeed / 100; // 원래 속도

                    break;

                case Attribute.Thunder: // 번개 공격

                    if (debuffState[kind])
                    {
                        TakeDmg(project, Color.blue, 0.1f);
                        yield break;
                    }

                    debuffState[kind] = true;
                    float stunDuration = Mathf.Min(attributePower - GetStrengthModifier(), 4); // 최대 4초

                    TakeDmg(project, Color.blue, stunDuration);

                    //스턴
                    isStop = true;
                    yield return new WaitForSeconds(stunDuration); //지속 시간
                    isStop = false;

                    break;
            }
            debuffState[kind] = false;
        }
    }

    private float GetStrengthModifier()
    {
        return enemyStat.strength / 10;
    }

    public void Hit(Projectile project)
    {
        if (!gameObject.activeSelf || isDie)
            return;

        StartCoroutine(Hitted(project));
    }

    private IEnumerator HitColorAnimation(Color _color, float time)
    {
        spriteRenderer.color = _color;

        yield return new WaitForSeconds(time);

        spriteRenderer.color = Color.white;
    }

    private IEnumerator HPHandlerCoroutine()
    {
        float imsi = curHp / enemyStat.hp;
        float elapsedTime = 0f;
        float duration = 0.5f; // 변경되는 시간 간격

        float startValue = hpSlider.value; // 시작 값 저장

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            hpSlider.value = Mathf.Lerp(startValue, imsi, elapsedTime / duration);
            yield return null;
        }

        hpSlider.value = imsi; // 최종 값을 설정하여 정확히 일치하도록 합니다.
    }

    private void Die()
    {
        StopAllCoroutines();

        qm.UpQuest(4); // 퀘스트 4

        int tierNum = (int)enemyStat.tier;
        if (tierNum > 2)
        {
            if (tierNum > 3)
                qm.UpQuest(24); // 퀘스트 24
            qm.UpQuest(5); // 퀘스트 5
        }

        // 경험치 증가
        gm.GainExp((int)enemyStat.tier);

        // 웨이브 전체 몬스터
        um.upPannel.UpdateCurWave(-1);
        gm.killEnemyNum++;

        SpawnManager.Instance.DeSpawnEnemy(this);
    }
    #endregion

    #region Specific
    private void SpecificDie()
    {
        if (isDie)
            return;

        isStop = true;
        isDie = true;
        curHp = 0;

        switch (enemyStat.enemyType)
        {
            case EnemyType.Portion:
                anim.SetTrigger("Die");
                break;
            case EnemyType.Skeleton:
                if (curLife > 0)
                    anim.SetTrigger("Reborn");
                else
                    Die();
                break;
            case EnemyType.Slime:
                anim.SetTrigger("Split");
                break;
            default: // 사람, 골렘, 작라임
                Die();
                break;
        }
    }

    public void Reborn()
    {
        isStop = false;
        isDie = false;
        curLife--;
        curHp = enemyStat.hp;
        hpSlider.value = 1f;
        ResetDebuffStat();

        soundm.PlaySFX("Reborn");
    }

    public void Split()
    {
        soundm.PlaySFX("Split");

        sm.SpawnBabySlime(this);
        sm.SpawnBabySlime(this);

        Die();
    }
    #endregion
}
