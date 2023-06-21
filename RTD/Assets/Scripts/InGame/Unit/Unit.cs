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
    public float atkSpeedCount;
    #endregion
    #region Animation
    public Animator anim;
    public bool isAnim; //공격 방식 animation or default
    #endregion
    public UnitStat unitStat;

    void IPoolObject.OnCreatedInPool()
    {
        name = name.Replace("(Clone)", ""); //(Clone) 삭제
        anim = GetComponent<Animator>();
        if (anim != null)
            isAnim = true;
        unitStat = CSVReader.Instance.csvList.FindUnit(name);
        SpawnManager.Instance.SpawnTier(unitStat.tier, transform);
    }

    void IPoolObject.OnGettingFromPool()
    {
        SetUp();
        ChangeState(UnitState.SearchTarget);
    }

    void Update()
    {
        if (attackTarget != null)
            RotateToTarget();
    }

    public void SetUp()
    {
        unitStat = CSVReader.Instance.csvList.FindUnit(name);
        atkSpeedCount = 3 / (1 + (unitStat.atkSpeed / 100)); // 초당 공격 속도 계산 공식
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

            if (attackTarget != null && attackTarget.gameObject.activeSelf)
            {
                ChangeState(UnitState.AttackToTarget);
            }
            yield return null;
        }
    }

    protected virtual IEnumerator AttackToTarget()
    {
        // 1. target이 있는지 검사(다른 발사체에 의해 제거, Goal 지점 이동해 삭제 등)
        if(attackTarget == null || !attackTarget.gameObject.activeSelf)
            ChangeState(UnitState.SearchTarget);
        
        // 2. 사거리 벗어났는지 체크
        float distance = Vector2.Distance(attackTarget.position, transform.position);
        if (distance > unitStat.range)
        {
            attackTarget = null;
            ChangeState(UnitState.SearchTarget);
        }

        // 3. 공격
        if (isAnim && !isAtkCool)
        {
            anim.SetBool("IsAtk", true);
        }
        else
        {
            isAtkCool = true;

            Shoot();

            if (unitStat.unitType == UnitType.Fire) // 불일때 remainTime도 계산해야함
            {
                yield return new WaitForSeconds(atkSpeedCount);
                ChangeState(UnitState.SearchTarget);
            }

            yield return new WaitForSeconds(atkSpeedCount);
            ChangeState(UnitState.SearchTarget);
        }
    }

    IEnumerator AtkCoolAnim()
    {
        isAtkCool = true;
        anim.SetBool("IsAtk", false);
        yield return new WaitForSeconds(atkSpeedCount);
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
