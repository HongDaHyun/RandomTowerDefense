using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Redcode.Pools;
using UnityEngine.UI;

public abstract class Enemy : MonoBehaviour, IPoolObject
{
    protected SpriteRenderer spriteRenderer;
    public EnemyStat enemyStat;
    #region Movement
    protected Vector3 moveDirection;
    protected Transform[] wayPoints;
    protected int curWayPoint;
    #endregion
    #region HP
    protected int curHp;
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

    protected virtual void FixedUpdate()
    {
        Movement();
    }

    protected virtual void Update()
    {
        HpSliderUpdate();
    }

    protected virtual void Die()
    {
        isDie = true;
        SpawnManager.Instance.DeSpawnEnemy(this);
    }
    protected virtual void Movement()
    {
        transform.position = Vector2.MoveTowards(transform.position, moveDirection, enemyStat.moveSpeed);
    }
    protected virtual void SetUp()
    {
        name = name.Replace("(Clone)", "");
        enemyStat = CSVReader.Instance.EnemyExport(name); //(Clone) 삭제
        spriteRenderer = GetComponent<SpriteRenderer>();
        hpSlider = GetComponentInChildren<Slider>();
        curHp = enemyStat.hp;
        isDie = false;
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
    public void TakeDmg(int dmg)
    {
        if (isDie)
            return;
        curHp -= dmg;

        StopCoroutine(HitColorAnimation());
        StartCoroutine(HitColorAnimation());

        if(curHp <= 0)
            Die();
    }

    public IEnumerator TakeFire(int dmg)
    {
        if (isDie)
            yield break;

    }

    private IEnumerator HitColorAnimation()
    {
        spriteRenderer.color = Color.red;

        yield return new WaitForSeconds(0.05f);

        spriteRenderer.color = Color.white;
    }

    private void HpSliderUpdate()
    {
        float imsi = (float)curHp / enemyStat.hp;

        hpSlider.value = Mathf.Lerp(hpSlider.value, imsi, Time.deltaTime * 10);
    }
    #endregion
}
