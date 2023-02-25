using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Redcode.Pools;

public enum UnitState { SearchTarget = 0, AttackToTarget }

public abstract class Unit : MonoBehaviour, IPoolObject
{
    #region Attack
    private UnitState unitState = UnitState.SearchTarget;
    private Transform attackTarget = null;
    private bool isAtk;
    #endregion
    #region Animation
    protected Animator anim;
    protected bool isAnim;
    #endregion
    public UnitStat unitStat;

    void IPoolObject.OnCreatedInPool()
    {
        name = name.Replace("(Clone)", "");
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

    void SetUp()
    {
        unitStat = CSVReader.Instance.UnitExport(name); //(Clone) 삭제
        anim = GetComponent<Animator>();
        isAnim = (anim != null) ? true : false;
        ChangeState(UnitState.SearchTarget);
    }

    #region Attack
    public void ChangeState(UnitState newState)
    {
        StopCoroutine(unitState.ToString());
        unitState = newState; ;
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
        isAtk = false;
        if (isAnim)
            anim.SetBool("IsAtk", false);
        while(true)
        {
            float closetDistSqr = Mathf.Infinity;
            attackTarget = null;
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

            if (attackTarget != null)
            {
                if(isAnim && !isAtk)
                    anim.SetBool("IsAtk", true);
                else
                    ChangeState(UnitState.AttackToTarget);
            }
            yield return null;
        }
    }

    private IEnumerator AttackToTarget()
    {
        if(!isAtk) // 한번만 작동
            SpawnManager.Instance.SpawnProjectile(transform.GetChild(0), attackTarget, unitStat);
        isAtk = true;
        if (isAnim)
            anim.SetBool("IsAtk", false);
        yield return new WaitForSeconds(unitStat.atkSpeed);
        ChangeState(UnitState.SearchTarget);
    }
    #endregion
}
