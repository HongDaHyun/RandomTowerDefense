using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit_Germ : Unit
{
    protected override void Shoot()
    {
        SpawnManager.Instance.SpawnProjectile(transform.GetChild(0), attackTarget, unitStat, "Axid");
    }
}
