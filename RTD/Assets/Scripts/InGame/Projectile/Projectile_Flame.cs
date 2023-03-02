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

    }
}
