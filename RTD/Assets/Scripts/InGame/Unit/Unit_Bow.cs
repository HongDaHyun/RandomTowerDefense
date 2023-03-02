using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit_Bow : Unit
{
    protected override Projectile Shoot()
    {
        Projectile projectile = SpawnManager.Instance.SpawnProjectile(transform.GetChild(0), attackTarget, unitStat, "Arrow");
        return projectile;
    }
}
