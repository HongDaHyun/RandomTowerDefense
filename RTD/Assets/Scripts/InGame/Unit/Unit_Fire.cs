using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit_Fire : Unit
{
    protected override void SetUp()
    {
        base.SetUp();
        weapon = "Flame";
    }

    protected override IEnumerator AttackToTarget()
    {
        isAtk = true;
        Projectile projectile = Shoot();
        if (isAnim)
            anim.SetBool("IsAtk", false);
        yield return new WaitForSeconds(unitStat.atkSpeed + projectile.remainTime);
        ChangeState(UnitState.SearchTarget);
    }
}
