using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Redcode.Pools;
using UnityEngine.UI;
using System;

public class Enemy : MonoBehaviour, IPoolObject
{
    protected SpriteRenderer spriteRenderer;
    public EnemyStat enemyStat;
    private Dictionary<Attribute, bool> debuffState = new Dictionary<Attribute, bool>()
    {
        {Attribute.Fire, false },
        {Attribute.Germ, false },
        {Attribute.Ice, false },
        {Attribute.Thunder, false }
    };

    #region Movement
    protected Vector3 moveDirection;
    protected Transform[] wayPoints;
    protected int curWayPoint;
    #endregion
    #region HP
    protected float curHp;
    protected bool isDie = false;
    protected Slider hpSlider;
    #endregion

    public void OnCreatedInPool() //최초
    {
        name = name.Replace("(Clone)", "");
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

    private void Die()
    {
        isDie = true;
        QuestManager.Instance.killEnemyNum++; // 퀘스트 5

        GameManager.Instance.curExp += (int)enemyStat.tier; // 경험치 증가

        SpawnManager.Instance.DeSpawnEnemy(this);
    }
    private void Movement()
    {
        transform.position = Vector2.MoveTowards(transform.position, moveDirection, enemyStat.moveSpeed);
    }
    private void SetUp()
    {
        name = name.Replace("(Clone)", "");
        enemyStat = CSVReader.Instance.EnemyExport(name);
        spriteRenderer = GetComponent<SpriteRenderer>();
        hpSlider = GetComponentInChildren<Slider>();
        curHp = enemyStat.hp;
        isDie = false;

        debuffState[Attribute.Fire] = false; debuffState[Attribute.Germ] = false; debuffState[Attribute.Ice] = false; debuffState[Attribute.Thunder] = false;

        spriteRenderer.color = Color.white;

        SetWayPoint();
        ResetWayPoint();
    }

    #region Movement Code
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
            GameManager.Instance.curHealth--;
        }
    }

    public void MoveTo(Vector3 direction)
    {
        moveDirection = direction;
    }
    #endregion

    #region Damaged
    private void TakeDmg(float dmg, Color _color, float time, bool isCrit)
    {
        if (isDie || !gameObject.activeSelf)
            return;

        float totalDmg = MathF.Round(dmg / (dmg + enemyStat.defense), 1); // 총 데미지 계산

        curHp -= totalDmg;
        UIManager.Instance.DmgPopUp(totalDmg, transform.position, isCrit);

        StopCoroutine(HitColorAnimation(_color, time));
        StartCoroutine(HitColorAnimation(_color, time));

        if(curHp <= 0)
            Die();
    }

    private IEnumerator Hitted(float dmg, Attribute attribute, bool isCrit)
    {
        if(attribute == Attribute.None) // 일반 공격
        {
            TakeDmg(dmg, Color.red, 0.05f, isCrit);
        }
        else // 디버프 공격
        {
            switch (attribute)
            {
                case Attribute.Fire: // 불 공격
                case Attribute.Germ: // 독 공격
                    if (debuffState[attribute])
                        yield break;
                    debuffState[attribute] = true;

                    float delayDmg = 0.3f; // 딜레이
                    int dmgCount = Mathf.RoundToInt(CSVReader.Instance.AttributeExport(attribute) / 1 + GetStrengthModifier()); // 반복 횟수(올림)

                    for (int i = 0; i < dmgCount; i++)
                    {
                        TakeDmg(dmg, attribute == Attribute.Fire ? Color.red : Color.black, 0.5f, isCrit); // Fire -> red, Germ -> black
                        yield return new WaitForSeconds(delayDmg); // 0.5초당 데미지
                    }
                    break;

                case Attribute.Ice: // 얼음 공격
                    TakeDmg(dmg, Color.cyan, CSVReader.Instance.AttributeExport(attribute), isCrit);

                    if (debuffState[attribute])
                        yield break;
                    debuffState[attribute] = true;

                    //슬로우
                    float slowDuration = CSVReader.Instance.AttributeExport(attribute) - GetStrengthModifier(); // 지속 시간
                    float originalMoveSpeed = enemyStat.moveSpeed; // 원래 속도
                    enemyStat.moveSpeed -= CSVReader.Instance.AttributeExport(attribute) / (10 + enemyStat.strength); // 느려진 속도
                    yield return new WaitForSeconds(slowDuration);
                    enemyStat.moveSpeed = originalMoveSpeed;
                    break;

                case Attribute.Thunder: // 번개 공격
                    TakeDmg(dmg, Color.blue, 1f, isCrit);

                    if (debuffState[attribute])
                        yield break;
                    debuffState[attribute] = true;

                    //스턴
                    float stunDuration = CSVReader.Instance.AttributeExport(attribute) - GetStrengthModifier(); // 지속 시간
                    originalMoveSpeed = enemyStat.moveSpeed; // 원래 속도
                    enemyStat.moveSpeed = 0;
                    yield return new WaitForSeconds(stunDuration); //지속 시간
                    enemyStat.moveSpeed = originalMoveSpeed;
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
    #endregion
}
