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
        {Attribute.Stun, false }
    };
    const int MAX_STRENGTH = 20;

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

        debuffState[Attribute.Fire] = false; debuffState[Attribute.Germ] = false; debuffState[Attribute.Ice] = false; debuffState[Attribute.Stun] = false;

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
    private void TakeDmg(float dmg, Color _color, float time)
    {
        if (isDie || !gameObject.activeSelf)
            return;
        curHp -= dmg;
        UIManager.Instance.DmgPopUp(dmg, transform.position);

        StartCoroutine(HitColorAnimation(_color, time));

        if(curHp <= 0)
            Die();
    }

    private IEnumerator Hitted(float dmg, Attribute attribute)
    {
        if(attribute == Attribute.None) // 일반 공격
        {
            TakeDmg(dmg, Color.red, 0.05f);
        }
        else // 디버프 공격
        {
            DebuffStat debuff;
            switch (attribute)
            {
                case Attribute.Fire:
                case Attribute.Germ:
                    if (debuffState[attribute])
                        yield break;
                    debuffState[attribute] = true;

                    int limitStrength = Mathf.Clamp(enemyStat.strength, 0, 17); // 강인함 17로 제한
                    debuff = CSVReader.Instance.DebuffExport(attribute.ToString());

                    for (int i = 0; i < MAX_STRENGTH - limitStrength; i++)
                    {
                        TakeDmg(dmg, attribute == Attribute.Fire ? Color.red : Color.black, 0.05f); // Fire -> red, Germ -> black
                        yield return new WaitForSeconds(debuff.ticTime);
                    }
                    break;

                case Attribute.Ice:
                    debuff = CSVReader.Instance.DebuffExport(attribute.ToString());
                    TakeDmg(dmg, Color.cyan, debuff.ticTime);

                    if (debuffState[attribute]) // 슬로우
                        yield break;
                    debuffState[attribute] = true;

                    float originalMoveSpeed = enemyStat.moveSpeed;
                    enemyStat.moveSpeed *= 0.7f; // 나중에 변수 넣어서 개선 (지금은 default 0.7f)
                    yield return new WaitForSeconds(debuff.ticTime);
                    enemyStat.moveSpeed = originalMoveSpeed;
                    break;

                case Attribute.Stun:
                    debuff = CSVReader.Instance.DebuffExport("Thunder");
                    TakeDmg(dmg, Color.blue, debuff.ticTime);

                    if (debuffState[attribute]) //스턴
                        yield break;
                    debuffState[attribute] = true;

                    originalMoveSpeed = enemyStat.moveSpeed;
                    enemyStat.moveSpeed = 0;
                    yield return new WaitForSeconds(debuff.ticTime);
                    enemyStat.moveSpeed = originalMoveSpeed;
                    break;
            }
            debuffState[attribute] = false;
        }
    }

    public void Hit(float dmg, Attribute attribute)
    {
        StartCoroutine(Hitted(dmg, attribute));
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
