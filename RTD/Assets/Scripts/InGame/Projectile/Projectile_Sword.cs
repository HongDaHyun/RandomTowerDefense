using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile_Sword : Projectile
{
    public override void SetPos(Transform _start, Transform _target)
    {
        startTrans = _start;    target = _target;

        SetRotation(90);
        transform.position = target.position;
    }

    protected override void SetRotation(int ang)
    {
        Vector3 direction = target.position - startTrans.position;
        direction.Normalize();
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle - ang, Vector3.forward);
    }
}
