using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Redcode.Pools;
#pragma warning disable 0618

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
        StartCoroutine(ActiveTime());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Enemy"))
            return;
        Enemy enemy = collision.GetComponent<Enemy>();
        enemy.Hit(dmg, Attribute.Germ, false); // 독 크리티컬 x
    }

    IEnumerator ActiveTime()
    {
        yield return new WaitForSeconds(particle.duration);
        SpawnManager.Instance.DeSpawnExplosion(this);
    }

    public void SetDmg(float damage)
    {
        dmg = damage;
    }
}
