using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile_Flame : Projectile
{
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        return;
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!collision.CompareTag("Enemy"))
            return;
        Enemy enemy = collision.GetComponent<Enemy>();
        enemy.StartCoroutine(enemy.TakeFire((float)System.Math.Round(unitStat.dmg / 10, 1)));
    }
    public override void SetTarget(Transform start, Transform target)
    {
        base.SetTarget(start, target);
        MoveRot();
    }

    protected override void Update()
    {
    }
}
