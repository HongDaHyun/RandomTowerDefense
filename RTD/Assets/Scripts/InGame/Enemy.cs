using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    #region Movement
    protected float moveSpeed = 0.01f;
    protected Vector3 moveDirection = Vector3.zero;
    protected Transform[] wayPoints;
    protected int curWayPoint;
    #endregion

    protected virtual void Awake()
    {
        SetWayPoint();
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
        wayPoints = new Transform[GameManager.Instance.wayPointsObj.childCount];
        for (int i = 0; i < wayPoints.Length; i++)
        {
            wayPoints[i] = GameManager.Instance.wayPointsObj.GetChild(i);
        }

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
    }
    #endregion

    public void MoveTo(Vector3 direction)
    {
        moveDirection = direction;
    }
}
