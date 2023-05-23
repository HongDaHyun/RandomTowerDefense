using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Redcode.Pools;

public class Projectile : MonoBehaviour, IPoolObject
{
    public UnitType type;
    public bool isCrit;
    public float dmg, range, speed;
    public Transform target, startTrans;
    public float remainTime = 2f;

    void IPoolObject.OnCreatedInPool()
    {
        name = name.Replace("(Clone)", "");
    }

    void IPoolObject.OnGettingFromPool()
    {
        StartCoroutine(Remain());
    }

    private void Update()
    {
        switch (type)
        {
            case UnitType.Bow:
            case UnitType.Gun:
            case UnitType.Ice:
            case UnitType.Thunder:
                if (!target.gameObject.activeSelf)
                    SpawnManager.Instance.DeSpawnProjectile(this);
                ShootFollow();
                break;
            case UnitType.Fighter:
                if (range < Vector2.Distance(transform.position, startTrans.position))
                    SpawnManager.Instance.DeSpawnProjectile(this);

                ShootStraight();
                break;
            case UnitType.Germ:
                transform.Rotate(new Vector3(0, 0, -600f * Time.deltaTime));
                break;
            default:
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Enemy"))     return;

        Enemy enemy = collision.GetComponent<Enemy>();
        switch(type)
        {
            case UnitType.Bow:
            case UnitType.Gun:
                if (collision.transform != target)  return;
                SpawnManager.Instance.DeSpawnProjectile(this);
                enemy.Hit(dmg, Attribute.None, isCrit);
                break;

            case UnitType.Fighter:
            case UnitType.Sword:
                enemy.Hit(dmg, Attribute.None, isCrit);
                break;

            case UnitType.Fire:
                enemy.Hit((float)System.Math.Round(dmg / 10, 1), Attribute.Fire, false); // 불 크리티컬 x
                break;

            case UnitType.Ice:
                if (collision.transform != target)  return;
                SpawnManager.Instance.DeSpawnProjectile(this);
                enemy.Hit(dmg, Attribute.Ice, isCrit);
                break;

            case UnitType.Thunder:
                if (collision.transform != target) return;
                SpawnManager.Instance.DeSpawnProjectile(this);
                enemy.Hit(dmg, Attribute.Thunder, isCrit);
                break;

            default:
                break;

        }
    }

    public virtual void SetUp(Transform _start, Transform _target) //SpawnManager에서 사용
    {
        startTrans = _start;    target = _target;
        transform.position = startTrans.position;
        SetRotation(0);
    }

    #region ShootType
    private void ShootStraight()
    {
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }

    private void ShootFollow()
    {
        SetRotation(0);
        ShootStraight();
    }

    protected virtual void SetRotation(int ang)
    {
        Vector3 direction = target.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle - ang, Vector3.forward);
    }
    #endregion

    private IEnumerator Remain()
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
