using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Projectile_Germ : Projectile
{
    public AnimationCurve animCurve;

    public override void SetPos(Transform _start, Transform _target)
    {
        base.SetPos(_start, _target);
        float dur = 5f / projectileStat.speed; //throw speed
        transform.DOScale(projectileStat.size * 2, dur).SetEase(animCurve).OnComplete(() => { Explosion(); });
        transform.DOMove(target.position, dur); //¿Ãµø
    }

    public void Explosion()
    {
        SpawnManager.Instance.DeSpawnProjectile(this);
        SpawnManager.Instance.SpawnExplosion(transform, this, "DeathExplosion");
    }
}