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
        collision.GetComponent<Enemy>().TakeDmg(unitStat.dmg, Color.red, 0.05f);
    }

    protected override void Update()
    {
    }

    public override void SetUp()
    {
        base.SetUp();
        particle = GetComponent<ParticleSystem>();
        remainTime = particle.duration;
    }

    public override void SetTarget(Transform start, Transform target)
    {
        base.SetTarget(start, target);
        MoveRot();
        MovePos();
    }

    protected override void MoveRot()
    {
        Vector3 direction = target.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
    }

    protected override void MovePos()
    {
        transform.position = target.position;
    }
}
