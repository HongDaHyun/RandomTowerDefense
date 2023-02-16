using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Redcode.Pools;

public enum UnitState { SearchTarget =0, AttackToTarget }

public abstract class Unit : MonoBehaviour, IPoolObject
{
    public string idName;
    #region Attack
    private UnitState unitState = UnitState.SearchTarget;
    private Transform attackTarget = null;
    #endregion
    public UnitStat unitStat;
    void IPoolObject.OnCreatedInPool()
    {
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
        unitStat = CSVReader.Instance.UnitExport(idName);
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
                ChangeState(UnitState.AttackToTarget);
            yield return null;
        }
    }

    private IEnumerator AttackToTarget()
    {
        SpawnManager.Instance.SpawnProjectile(transform.GetChild(0), attackTarget, unitStat);
        yield return new WaitForSeconds(unitStat.atkSpeed);
        ChangeState(UnitState.SearchTarget);
    }
    #endregion
}
