using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile_Punch : Projectile
{
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!collision.CompareTag("Enemy"))
            return;
        collision.GetComponent<Enemy>().TakeDmg(unitStat.dmg, Color.red, 0.05f);
    }
}
