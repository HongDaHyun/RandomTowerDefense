using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Redcode.Pools;
using UnityEngine.UI;
using System;

public class Enemy : MonoBehaviour, IPoolObject
{
    private SpriteRenderer spriteRenderer;
    private Animator anim;
    public EnemyStat enemyStat;
    private Dictionary<Attribute, bool> debuffState = new Dictionary<Attribute, bool>()
    {
        {Attribute.Fire, false },
        {Attribute.Germ, false },
        {Attribute.Ice, false },
        {Attribute.Thunder, false }
    };

    #region Movement
    [HideInInspector] public Vector3 moveDirection;
    private Transform[] wayPoints;
    [HideInInspector] public int curWayPoint;
    #endregion
    #region HP
    private int curLife;
    public int maxLife;
    [HideInInspector] public float curHp;
    private bool isDie = false, isStop = false;
    private Slider hpSlider;
    #endregion

    public void OnCreatedInPool() //최초
    {
        name = name.Replace("(Clone)", "");

        SetWayPoint();
    }

    public void OnGettingFromPool() //재사용
    {
        SetUp();
    }

    private void FixedUpdate()
    {
        Movement();
    }

    private void Update()
    {
        HpSliderUpdate();
    }

    protected void SetUp()
    {
        enemyStat = CSVReader.Instance.csvList.FindEnemy(name); //스탯 가져오기

        //컴포넌트 가져오기
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        hpSlider = GetComponentInChildren<Slider>();

        //변수 초기화
        curHp = enemyStat.hp;
        curLife = maxLife;
        isDie = false;
        isStop = false;
        debuffState[Attribute.Fire] = false; debuffState[Attribute.Germ] = false; debuffState[Attribute.Ice] = false; debuffState[Attribute.Thunder] = false;
        spriteRenderer.color = Color.white;

        //위치 설정
        ResetWayPoint();
    }

    #region Movement Code
    private void Movement()
    {
        if (isStop)
            return;
        transform.position = Vector2.MoveTowards(transform.position, moveDirection, enemyStat.moveSpeed);
    }

    private void SetWayPoint()
    {
        if (wayPoints != null)
            return;
        wayPoints = new Transform[SpawnManager.Instance.wayPointsObj.childCount];
        for (int i = 0; i < wayPoints.Length; i++)
        {
            wayPoints[i] = SpawnManager.Instance.wayPointsObj.GetChild(i);
        }
    }
    private void ResetWayPoint()
    {
        curWayPoint = 0;

        transform.position = wayPoints[curWayPoint].position;

        StartCoroutine(OnMove());
    }
    private IEnumerator OnMove()
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
            SpawnManager.Instance.DeSpawnEnemy(this);
            GameManager.Instance.LoseHealth();
        }
    }

    public void MoveTo(Vector3 direction)
    {
        moveDirection = direction;
    }
    #endregion

    #region Damaged
    //Damage
    private void TakeDmg(float dmg, Color _color, float time, bool isCrit)
    {
        if (isDie || !gameObject.activeSelf)
            return;
        
        float defenseRatio = MathF.Round(100 / (100 + enemyStat.defense), 1);
        float totalDmg = (dmg + (enemyStat.hp * (CharacManager.Instance.proportionHP / 100)) + WaveManager.Instance.dmgPerWave)
            * defenseRatio; // 총 데미지 (데미지 + 체력비례 데미지 + 웨이브 당 데미지) * 디펜스

        curHp -= totalDmg;
        UIManager.Instance.DmgPopUp(totalDmg, transform.position, isCrit);

        StartCoroutine(HitColorAnimation(_color, time));

        if (curHp <= 0)
            SpecificDie();
    }

    // 일반 공격과 데미지 공격 구분
    private IEnumerator Hitted(float dmg, Attribute attribute, bool isCrit)
    {
        if(attribute == Attribute.None) // 일반 공격
        {
            TakeDmg(dmg, Color.red, 0.05f, isCrit);
        }
        else // 디버프 공격
        {
            float attributePower = CSVReader.Instance.csvList.FindAttribute(attribute);

            switch (attribute)
            {
                case Attribute.Fire: // 불 공격
                    if (debuffState[attribute])
                        dmg *= 2 + CharacManager.Instance.dmgFire; // Charac 15
                    debuffState[attribute] = true;

                    float delayDmg = 0.3f; // 딜레이
                    int dmgCount = Mathf.CeilToInt(attributePower / 1 + GetStrengthModifier()); // 반복 횟수(올림)
                    enemyStat.defense -= CharacManager.Instance.defenseGerm;

                    for (int i = 0; i < dmgCount; i++)
                    {
                        TakeDmg(dmg, Color.red, 0.5f, isCrit);
                        yield return new WaitForSeconds(delayDmg);
                    }
                    break;
                case Attribute.Germ: // 독 공격
                    if (debuffState[attribute])
                        dmg *= 2 + CharacManager.Instance.dmgGerm; // Charac 22
                    debuffState[attribute] = true;

                    delayDmg = 0.3f; // 딜레이
                    dmgCount = Mathf.CeilToInt(attributePower / 1 + GetStrengthModifier()); // 반복 횟수(올림)

                    for (int i = 0; i < dmgCount; i++)
                    {
                        TakeDmg(dmg, Color.black, 0.5f, isCrit);
                        yield return new WaitForSeconds(delayDmg);
                    }
                    enemyStat.defense += CharacManager.Instance.defenseGerm;
                    break;

                case Attribute.Ice: // 얼음 공격
                    TakeDmg(dmg, Color.cyan, attributePower, isCrit);

                    if (debuffState[attribute])
                        yield break;
                    debuffState[attribute] = true;

                    //슬로우
                    float slowDuration = (attributePower - GetStrengthModifier()); // 지속 시간
                    float slowPercent = attributePower / (10 + enemyStat.strength); // 슬로우 %
                    // Charac 26
                    int plusSlow = CharacManager.Instance.plusIce;
                    if (plusSlow > 1)
                    {
                        slowDuration *= plusSlow;
                        slowPercent *= plusSlow;
                    }
                    float originalMoveSpeed = enemyStat.moveSpeed; // 원래 속도
                    enemyStat.moveSpeed *= 1 - slowPercent; // 느려진 속도
                    yield return new WaitForSeconds(slowDuration);
                    enemyStat.moveSpeed = originalMoveSpeed;
                    break;

                case Attribute.Thunder: // 번개 공격
                    TakeDmg(dmg, Color.blue, 1f, isCrit);

                    if (debuffState[attribute])
                        yield break;
                    debuffState[attribute] = true;

                    //스턴
                    float stunDuration = attributePower - GetStrengthModifier(); // 지속 시간
                    isStop = true;
                    yield return new WaitForSeconds(stunDuration); //지속 시간
                    isStop = false;
                    break;
            }
            debuffState[attribute] = false;
        }
    }

    private float GetStrengthModifier()
    {
        return enemyStat.strength / 10;
    }

    public void Hit(float dmg, Attribute attribute, bool isCrit)
    {
        if (!gameObject.activeSelf)
            return;

        StartCoroutine(Hitted(dmg, attribute, isCrit));
    }

    private IEnumerator HitColorAnimation(Color _color, float time)
    {
        spriteRenderer.color = _color;

        yield return new WaitForSeconds(time);

        spriteRenderer.color = Color.white;
    }

    private void HpSliderUpdate()
    {
        float imsi = curHp / enemyStat.hp;

        hpSlider.value = Mathf.Lerp(hpSlider.value, imsi, Time.deltaTime * 10);
    }

    //Die
    private void SpecificDie()
    {
        StopAllCoroutines();
        switch (enemyStat.enemyType)
        {
            case EnemyType.Portion:
                isStop = true;
                anim.SetTrigger("Die");
                break;
            case EnemyType.Skeleton:
                isStop = true;
                if (curLife > 0)
                    anim.SetTrigger("Reborn");
                else
                    Die();
                break;
            case EnemyType.Slime:
                isStop = true;
                anim.SetTrigger("Split");
                break;
            default: // 사람, 골렘, 작라임
                Die();
                break;
        }
    }

    private void Die()
    {
        isDie = true;
        QuestManager.Instance.killEnemyNum++; // 퀘스트 5

        GameManager.Instance.curExp += (int)enemyStat.tier; // 경험치 증가

        SpawnManager.Instance.DeSpawnEnemy(this);
    }
    #endregion

    #region Specific
    public void Reborn()
    {
        isStop = false;
        curLife--;
        curHp = enemyStat.hp;
    }

    public void Split()
    {
        SpawnManager.Instance.SpawnBabySlime(this);
        SpawnManager.Instance.SpawnBabySlime(this);

        Die();
    }
    #endregion
}
