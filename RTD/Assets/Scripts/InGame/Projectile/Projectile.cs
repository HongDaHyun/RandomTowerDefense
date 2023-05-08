using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Redcode.Pools;

public class Projectile : MonoBehaviour, IPoolObject
{
    public UnitStat unitStat;
    public Transform target, startTrans;
    public float remainTime = 2f;

    void IPoolObject.OnCreatedInPool()
    {
        name = name.Replace("(Clone)", "");
    }

    void IPoolObject.OnGettingFromPool()
    {
        //SetUp 관련된 것들은 SpawnManager에 명시
        StartCoroutine(Remain());
    }

    protected virtual void Update()
    {
        if (target.gameObject.activeSelf)
        {
            switch(unitStat.unitType)
            {
                case UnitType.Bow:
                case UnitType.Gun:
                    ShootFollow();
                    break;
                case UnitType.Fighter:
                    if (unitStat.range < Vector2.Distance(transform.position, startTrans.position))
                    {
                        SpawnManager.Instance.DeSpawnProjectile(this);
                    }
                    ShootStraight();
                    break;
            }
        }
        else
            SpawnManager.Instance.DeSpawnProjectile(this);
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Enemy"))     return;
        if (collision.transform != target)      return;
        SpawnManager.Instance.DeSpawnProjectile(this);
        collision.GetComponent<Enemy>().TakeDmg(unitStat.dmg, Color.red, 0.05f);
    }

    public virtual void SetUp(Transform _start, Transform _target, UnitStat _unitStat)
    {
        startTrans = _start;    target = _target;   unitStat = _unitStat;
        transform.position = _start.position;
        SetRotation();
    }

    #region ShootType
    protected virtual void ShootStraight()
    {
        transform.Translate(Vector2.right * unitStat.projectileSpeed * Time.deltaTime);
    }

    protected virtual void ShootFollow()
    {
        SetRotation();
        ShootStraight();
    }

    protected virtual void SetRotation()
    {
        Vector3 direction = target.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
    #endregion

    #region Hit
    #endregion

    protected virtual IEnumerator Remain()
    {
        float time = 0f;
        while(time <= remainTime)
        {
            time += Time.deltaTime;
            yield return null;
        }
        SpawnManager.Instance.DeSpawnProjectile(this);
    }
}
