using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Projectile_Axid : Projectile
{
    public AnimationCurve animCurve;

    //public override void SetUp()
    //{
    //    base.SetUp();
    //    float dur = 5f / unitStat.projectileSpeed; //throw speed
    //    transform.DOScale(0.8f, dur).SetEase(animCurve).OnComplete(() => { Explosion(); });
    //    transform.DOMove(target.position, dur); //¿Ãµø
    //    SetRotation();
    //}

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
        SpawnManager.Instance.SpawnExplosion(transform, (float)System.Math.Round(unitStat.dmg / 10, 1), "DeathExplosion");
    }
}