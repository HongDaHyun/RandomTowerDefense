using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Redcode.Pools;

public class Explosion_Death : MonoBehaviour, IPoolObject
{
    public float dmg;
    ParticleSystem particle;

    void IPoolObject.OnCreatedInPool()
    {
        particle = GetComponent<ParticleSystem>();
        name = name.Replace("(Clone)", "");
    }

    void IPoolObject.OnGettingFromPool()
    {
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Enemy"))
            return;
        Enemy enemy = collision.GetComponent<Enemy>();
        enemy.StartCoroutine(enemy.TakeGerm(dmg));
    }

    private void Update()
    {
        if (!particle.IsAlive())
            SpawnManager.Instance.DeSpawnExplosion(this);
    }

    public void SetDmg(float damage)
    {
        dmg = damage;
    }
}
