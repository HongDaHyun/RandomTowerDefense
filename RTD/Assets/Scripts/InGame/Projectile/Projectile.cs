using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Redcode.Pools;

public abstract class Projectile : MonoBehaviour, IPoolObject
{
    public ProjectileStat projectileStat;
    public UnitStat unitStat;
    public Transform target;

    void IPoolObject.OnCreatedInPool()
    {
        name = name.Replace("(Clone)", "");
    }

    void IPoolObject.OnGettingFromPool()
    {
        SetUp();
    }

    private void Update()
    {
        if (target == null)
        {
            SpawnManager.Instance.DeSpawnProjectile(this);
            return;
        }
        if (target.gameObject.activeSelf)
        {
            Vector3 direction = target.position - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            transform.position = Vector2.MoveTowards(transform.position, target.position, projectileStat.projectileSpeed);
        }
        else
            SpawnManager.Instance.DeSpawnProjectile(this);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Enemy"))
            return;
        if (collision.transform != target)
            return;
        SpawnManager.Instance.DeSpawnProjectile(this);
        collision.GetComponent<Enemy>().TakeDmg(unitStat.dmg + projectileStat.dmg);
    }

    public void SetTarget(Transform target) //rotationµµ
    {
        this.target = target;
    }

    public void SetUp()
    {
        projectileStat = CSVReader.Instance.ProjectileExport(name);
    }
}
