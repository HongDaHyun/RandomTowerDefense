using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#pragma warning disable 0618

public class Projectile_SwordCup : Projectile
{
    ParticleSystem particle;

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Enemy"))
            return;
        collision.GetComponent<Enemy>().Hit(unitStat.dmg, Attribute.None);
    }

    protected override void Update()
    {
    }

    //public override void SetUp()
    //{
    //    base.SetUp();
    //    particle = GetComponent<ParticleSystem>();
    //    remainTime = particle.duration;
    //}
}
