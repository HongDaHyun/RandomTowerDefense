using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Redcode.Pools;
#pragma warning disable 0618

public class Explosion_Death : MonoBehaviour, IPoolObject
{
    public Projectile parent;
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
        enemy.Hit(parent);
    }

    public void SetParentProjectile(Projectile project)
    {
        parent = project;
        dmg = project.dmg;
    }
}
