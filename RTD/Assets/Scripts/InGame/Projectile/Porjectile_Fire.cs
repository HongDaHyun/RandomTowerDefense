using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Porjectile_Fire : Projectile
{
    Transform childTrans;
    BoxCollider2D hitbox;
    const float DEFAULT_RANGE = 3.2f; // 불마법사를 기본으로 불 크기 설정

    private void Awake()
    {
        childTrans = transform.GetChild(0);
        hitbox = GetComponent<BoxCollider2D>();
    }

    public void FireRange()
    {
        float plusSize = (range - DEFAULT_RANGE) * 0.5f;

        // Collider
        hitbox.offset = new Vector2(hitbox.offset.x + plusSize, 0);
        hitbox.size = new Vector2(hitbox.offset.x * 2, 1);
        // Scale
        childTrans.localScale = new Vector3(childTrans.localScale.x, childTrans.localScale.y, childTrans.localScale.z + plusSize);
    }

    public override void SetStat(UnitStat unit)
    {
        base.SetStat(unit);

        dmg = (float)System.Math.Round(dmg / 10, 1);
        isCrit = false;

        FireRange();
    }

    public override void SetPos(Transform _start, Transform _target)
    {
        base.SetPos(_start, _target);
    }
}
