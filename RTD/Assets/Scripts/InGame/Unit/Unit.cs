using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Redcode.Pools;

public enum UnitState { SearchTarget = 0, AttackToTarget }

public class Unit : MonoBehaviour, IPoolObject
{
    #region Attack
    private UnitState unitState = UnitState.SearchTarget;
    protected Transform attackTarget = null;
    #endregion
    #region Animation
    protected Animator anim;
    public bool isAnim; //공격 방식 animation or default
    #endregion
    public UnitStat unitStat;

    void IPoolObject.OnCreatedInPool()
    {
        name = name.Replace("(Clone)", ""); //(Clone) 삭제
        anim = GetComponent<Animator>();
        SetUp();
        SpawnManager.Instance.SpawnTier(unitStat.tier, transform);
    }

    void IPoolObject.OnGettingFromPool()
    {
        SetUp();
    }

    protected virtual void Update()
    {
        if (attackTarget != null)
            RotateToTarget();
    }

    protected virtual void SetUp()
    {
        unitStat = CSVReader.Instance.UnitExport(name);
        ChangeState(UnitState.SearchTarget);
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
        if (isAnim)
            anim.SetBool("IsAtk", false);
        while(true)
        {
            float closetDistSqr = Mathf.Infinity;
            for (int i = 0; i < SpawnManager.Instance.enemyList.Count; i++)
            {
                float distance = Vector3.Distance(SpawnManager.Instance.enemyList[i].transform.position,
                    transform.position);
                if(distance <= unitStat.range && distance <= closetDistSqr)
                {
                    closetDistSqr = distance;
                    attackTarget = SpawnManager.Instance.enemyList[i].transform;
                }
            }

            if (attackTarget != null && attackTarget.gameObject.activeSelf)
            {
                if (isAnim)
                    anim.SetBool("IsAtk", true);
                else
                    ChangeState(UnitState.AttackToTarget);
            }
            yield return null;
        }
    }

    protected virtual IEnumerator AttackToTarget()
    {
        Projectile projectile = Shoot();

        if(unitStat.unitType == UnitType.Fire) // 불일때 remainTime도 계산해야함
        {
            yield return new WaitForSeconds(unitStat.atkSpeed + projectile.remainTime);
            ChangeState(UnitState.SearchTarget);
        }

        yield return new WaitForSeconds(unitStat.atkSpeed);
        ChangeState(UnitState.SearchTarget);
    }

    protected virtual Projectile Shoot()
    {
        Projectile projectile = SpawnManager.Instance.SpawnProjectile(transform.GetChild(0), attackTarget, unitStat, unitStat.unitType.ToString());
        return projectile;
    }
    #endregion
}
