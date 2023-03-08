using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile_Thunderball : Projectile
{
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Enemy"))
            return;
        if (collision.transform != target)
            return;
        SpawnManager.Instance.DeSpawnProjectile(this);
        Enemy enemy = collision.GetComponent<Enemy>();
        enemy.StartCoroutine(enemy.TakeThunder(unitStat.dmg + projectileStat.dmg));
    }
}
