using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Projectile_Germ : Projectile
{
    public AnimationCurve animCurve;

    public override void SetUp(Transform _start, Transform _target, UnitStat _unitStat)
    {
        base.SetUp(_start, _target, _unitStat);
        float dur = 5f / unitStat.projectileSpeed; //throw speed
        transform.DOScale(0.8f, dur).SetEase(animCurve).OnComplete(() => { Explosion(); });
        transform.DOMove(target.position, dur); //¿Ãµø
    }

    public void Explosion()
    {
        SpawnManager.Instance.DeSpawnProjectile(this);
        SpawnManager.Instance.SpawnExplosion(transform, (float)System.Math.Round(unitStat.dmg / 10, 1), "DeathExplosion");
    }
}