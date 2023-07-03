using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Redcode.Pools;

public enum UnitState { SearchTarget = 0, AttackToTarget }

public class Unit : MonoBehaviour, IPoolObject
{
    #region Attack
    private UnitState unitState = UnitState.SearchTarget;
    public Transform attackTarget = null;
    private bool isAtkCool;
    #endregion
    #region Animation
    public Animator anim;
    public bool isAnim; //공격 방식 animation or default
    private SpriteRenderer render;
    public Sprite defSprite;
    #endregion
    public UnitStat unitStat;

    void IPoolObject.OnCreatedInPool()
    {
        name = name.Replace("(Clone)", ""); //(Clone) 삭제
        anim = GetComponent<Animator>();
        render = GetComponent<SpriteRenderer>();
        defSprite = render.sprite;
        if (anim != null)
            isAnim = true;
        SetStat();
        SpawnManager.Instance.SpawnTier(unitStat.tier, transform);
    }

    void IPoolObject.OnGettingFromPool()
    {
        SetUp();
    }

    void Update()
    {
        if (attackTarget != null)
            RotateToTarget();
    }

    public void SetStat()
    {
        unitStat = CSVReader.Instance.csvList.FindUnit(name);
    }

    void SetUp()
    {
        SetStat();
        if (isAnim)
            anim.SetBool("IsAtk", false);
        render.sprite = defSprite;
    }

    #region Attack
    public void ChangeState(UnitState newState)
    {
        StopCoroutine(unitState.ToString());
        unitState = newState;
        StartCoroutine(unitState.ToString());
    }

    private void RotateToTarget()
    {
        if (transform.position.x > attackTarget.position.x)
            transform.rotation = Quaternion.Euler(0, -180, 0);
        else
            transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    private IEnumerator SearchTarget()
    {
        isAtkCool = false;
        while(true)
        {
            float closetDistSqr = Mathf.Infinity;
            for (int i = 0; i < SpawnManager.Instance.enemyList.Count; i++)
            {
                float distance = Vector2.Distance(SpawnManager.Instance.enemyList[i].transform.position,
                    transform.position);
                if(distance <= unitStat.range && distance <= closetDistSqr)
                {
                    closetDistSqr = distance;
                    attackTarget = SpawnManager.Instance.enemyList[i].transform;
                }
            }

            if (attackTarget != null && attackTarget.gameObject.activeSelf && !attackTarget.GetComponent<Enemy>().isDie)
            {
                ChangeState(UnitState.AttackToTarget);
            }
            yield return null;
        }
    }

    protected virtual IEnumerator AttackToTarget()
    {
        // 1. 사거리 벗어났는지 체크
        float distance = Vector2.Distance(attackTarget.position, transform.position);
        if (distance > unitStat.range)
        {
            attackTarget = null;
            ChangeState(UnitState.SearchTarget);
        }

        // 2. 공격
        if (isAnim && !isAtkCool)
        {
            anim.SetBool("IsAtk", true);
        }
        else
        {
            isAtkCool = true;

            Shoot();

            yield return new WaitForSeconds(1f / unitStat.atkSpeed);
            ChangeState(UnitState.SearchTarget);
        }
    }

    // 애니메이션에서 작동
    IEnumerator AtkCoolAnim()
    {
        isAtkCool = true;
        anim.SetBool("IsAtk", false);
        yield return new WaitForSeconds(1f / unitStat.atkSpeed);
        ChangeState(UnitState.SearchTarget);
    }

    Projectile Shoot()
    {
        if (attackTarget == null || !attackTarget.gameObject.activeSelf)
            return null;

        Projectile projectile = SpawnManager.Instance.SpawnProjectile(transform.GetChild(0), attackTarget, unitStat.unitType.ToString());

        projectile.SetStat(unitStat); // 투사체 공격력, 사거리 세팅

        SoundManager.Instance.PlaySFX($"Shoot{projectile.projectileStat.projectileName}"); // 효과음

        return projectile;
    }


    private float DmgCalculator(Projectile projectile)
    {
        float totalDmg = unitStat.dmg;

        // 크리티컬 계산
        int critRan = Random.Range(0, 100);

        if (critRan < unitStat.critPercent || unitStat.critPercent >= 100)
        {
            totalDmg *= unitStat.critDmg / 100;
            projectile.isCrit = true;
        }
        else
            projectile.isCrit = false;

        return totalDmg;
    }
    #endregion
}
