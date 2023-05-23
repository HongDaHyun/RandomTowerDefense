using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Projectile_Germ : Projectile
{
    public AnimationCurve animCurve;

    public override void SetUp(Transform _start, Transform _target)
    {
        base.SetUp(_start, _target);
        float dur = 5f / speed; //throw speed
        transform.DOScale(0.8f, dur).SetEase(animCurve).OnComplete(() => { Explosion(); });
        transform.DOMove(target.position, dur); //¿Ãµø
    }

    public void Explosion()
    {
        SpawnManager.Instance.DeSpawnProjectile(this);
        SpawnManager.Instance.SpawnExplosion(transform, (float)System.Math.Round(dmg / 10, 1), "DeathExplosion");
    }
}