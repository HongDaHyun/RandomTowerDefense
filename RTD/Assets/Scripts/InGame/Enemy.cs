using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Redcode.Pools;

public abstract class Enemy : MonoBehaviour, IPoolObject
{
    public string idName;
    public EnemyStat enemyStat;
    #region Movement
    protected Vector3 moveDirection;
    protected Transform[] wayPoints;
    protected int curWayPoint;
    #endregion

    public void OnCreatedInPool() //최초
    {
    }

    public void OnGettingFromPool() //재사용
    {
        enemyStat = CSVReader.Instance.EnemyExport(idName);
        SetWayPoint();
        ResetWayPoint();
    }

    protected virtual void FixedUpdate()
    {
        Movement();
    }

    protected abstract void Die();
    protected abstract void Movement();

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
            curWayPoint++;
            Vector3 direction = wayPoints[curWayPoint].position;
            MoveTo(direction);
        }
        else
        {
            SpawnManager.Instance.DeSpawnEnemy(this);
        }
    }
    #endregion

    public void MoveTo(Vector3 direction)
    {
        moveDirection = direction;
    }
}
