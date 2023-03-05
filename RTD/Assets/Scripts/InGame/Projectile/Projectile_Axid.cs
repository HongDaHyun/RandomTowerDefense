using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Projectile_Axid : Projectile
{
    Vector2 targetPos;
    public AnimationCurve animCurve;

    public override void SetTarget(Transform start, Transform target)
    {
        base.SetTarget(start, target);
        targetPos = target.position;
        float dur = 5f / projectileStat.projectileSpeed; //throw speed
        transform.DOScale(0.8f, dur).SetEase(animCurve).OnComplete(() => { Explosion(); });
        transform.DOMove(targetPos, dur);//¿Ãµø
        MoveRot();
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        return;
    }

    protected override void Update()
    {
        transform.Rotate(new Vector3(0, 0, -600f * Time.deltaTime));
    }

    public void Explosion()
    {
        SpawnManager.Instance.DeSpawnProjectile(this);
        SpawnManager.Instance.SpawnExplosion(transform, unitStat.dmg + projectileStat.dmg, "DeathExplosion");
    }
}