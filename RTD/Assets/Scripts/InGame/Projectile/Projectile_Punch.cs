using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile_Punch : Projectile
{
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Enemy"))
            return;
        collision.GetComponent<Enemy>().TakeDmg(unitStat.dmg + projectileStat.dmg);
    }

    public override void SetTarget(Transform start, Transform target)
    {
        base.SetTarget(start, target);
        MoveRot();
    }

    protected override void Update()
    {
        if (Vector2.Distance(startTrans.position, transform.position) > unitStat.range + projectileStat.range)
        {
            SpawnManager.Instance.DeSpawnProjectile(this);
            return;
        }

        MovePos();
    }
}
