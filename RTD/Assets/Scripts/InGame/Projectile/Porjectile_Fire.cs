using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Porjectile_Fire : Projectile
{
    Transform childTrans;
    BoxCollider2D hitbox;
    float defaultRange = 3.2f; // 불마법사를 기본으로 불 크기 설정

    private void Awake()
    {
        childTrans = transform.GetChild(0);
        hitbox = GetComponent<BoxCollider2D>();
    }

    public void FireRange()
    {
        if (range == defaultRange)
            return;

        float plusSize = (range - defaultRange) * 0.5f;

        // Collider
        hitbox.offset += new Vector2(plusSize, 0);
        hitbox.size = new Vector2(hitbox.offset.x * 2, 1);
        // Scale
        childTrans.localScale += new Vector3(0, 0, plusSize);

        // 사거리 기본값 변경
        defaultRange = range;
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
