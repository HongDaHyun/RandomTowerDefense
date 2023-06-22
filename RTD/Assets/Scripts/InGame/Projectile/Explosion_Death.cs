using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Redcode.Pools;
#pragma warning disable 0618

public class Explosion_Death : MonoBehaviour, IPoolObject
{
    public float dmg;

    void IPoolObject.OnCreatedInPool()
    {
        name = name.Replace("(Clone)", "");
    }

    void IPoolObject.OnGettingFromPool()
    {
        SoundManager.Instance.PlaySFX("ExplosionGerm");
    }

    private void OnParticleSystemStopped()
    {
        SpawnManager.Instance.DeSpawnExplosion(this);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Enemy"))
            return;
        Enemy enemy = collision.GetComponent<Enemy>();
        enemy.Hit(dmg, Attribute.Germ, false); // 독 크리티컬 x
    }

    public void SetDmg(float damage)
    {
        dmg = damage;
    }
}
