using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Redcode.Pools;
using UnityEngine.UI;

public abstract class Enemy : MonoBehaviour, IPoolObject
{
    protected SpriteRenderer spriteRenderer;
    public EnemyStat enemyStat;
    public bool isFire, isGerm, isIce, isThunder;
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
        enemyStat = CSVReader.Instance.EnemyExport(name);
        spriteRenderer = GetComponent<SpriteRenderer>();
        hpSlider = GetComponentInChildren<Slider>();
        curHp = enemyStat.hp;
        isDie = false; isFire = false; isGerm = false; isIce = false; isThunder = false;
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
    public void TakeDmg(float dmg, Color _color, float time)
    {
        if (isDie || !gameObject.activeSelf)
            return;
        curHp -= dmg;

        StopCoroutine(HitColorAnimation(_color, time));
        StartCoroutine(HitColorAnimation(_color, time));

        if(curHp <= 0)
            Die();
    }

    public IEnumerator TakeFire(float dmg)
    {
        if (isFire)
            yield break;
        isFire = true;
        for(int i = 0; i < enemyStat.strength; i++)
        {
            TakeDmg(dmg, Color.red, 0.05f);
            yield return new WaitForSeconds(CSVReader.Instance.DebuffExport("Fire").ticTime);
        }
        isFire = false;
    }

    public IEnumerator TakeGerm(float dmg)
    {
        if (isGerm)
            yield break;
        isGerm = true;
        for (int i = 0; i < enemyStat.strength; i++)
        {
            TakeDmg(dmg, Color.black, 0.05f);
            yield return new WaitForSeconds(CSVReader.Instance.DebuffExport("Germ").ticTime);
        }
        isGerm = false;
    }

    public IEnumerator TakeIce(float dmg)
    {
        DebuffStat ice = CSVReader.Instance.DebuffExport("Ice");
        TakeDmg(dmg, Color.cyan, ice.ticTime);

        if (isIce) //슬로우
            yield break;
        isIce = true;
        float temp = enemyStat.moveSpeed;
        enemyStat.moveSpeed *= 0.7f;
        yield return new WaitForSeconds(ice.ticTime);
        enemyStat.moveSpeed = temp;

        isIce = false;
    }

    public IEnumerator TakeThunder(float dmg)
    {
        DebuffStat thunder = CSVReader.Instance.DebuffExport("Thunder");
        TakeDmg(dmg, Color.blue, thunder.ticTime);

        if (isThunder)
            yield break;
        isThunder = true;
        float temp = enemyStat.moveSpeed;
        enemyStat.moveSpeed = 0;
        yield return new WaitForSeconds(thunder.ticTime);
        enemyStat.moveSpeed = temp;

        isThunder = false;
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
