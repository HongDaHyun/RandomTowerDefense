using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Redcode.Pools;

public abstract class Projectile : MonoBehaviour, IPoolObject
{
    public ProjectileStat projectileStat;
    public UnitStat unitStat;
    public Transform startTrans, target;
    public float remainTime = 2f;

    void IPoolObject.OnCreatedInPool()
    {
        name = name.Replace("(Clone)", "");
    }

    void IPoolObject.OnGettingFromPool()
    {
        SetUp();
        StartCoroutine(Remain());
    }

    protected virtual void Update()
    {
        if (target == null)
        {
            SpawnManager.Instance.DeSpawnProjectile(this);
            return;
        }
        if (target.gameObject.activeSelf)
        {
            MoveRot();
            MovePos();
        }
        else
            SpawnManager.Instance.DeSpawnProjectile(this);
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Enemy"))
            return;
        if (collision.transform != target)
            return;
        SpawnManager.Instance.DeSpawnProjectile(this);
        collision.GetComponent<Enemy>().TakeDmg(unitStat.dmg + projectileStat.dmg);
    }

    public virtual void SetTarget(Transform start, Transform target)
    {
        startTrans = start;
        this.target = target;
    }

    public virtual void SetUp()
    {
        projectileStat = CSVReader.Instance.ProjectileExport(name);
    }

    protected virtual void MovePos()
    {
        transform.Translate(Vector2.right * projectileStat.projectileSpeed * Time.deltaTime);
    }

    protected virtual void MoveRot()
    {
        Vector3 direction = target.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    public float time;
    protected virtual IEnumerator Remain()
    {
        time = 0f;
        while(time <= remainTime)
        {
            time += Time.deltaTime;
            yield return null;
        }
        SpawnManager.Instance.DeSpawnProjectile(this);
    }
}
