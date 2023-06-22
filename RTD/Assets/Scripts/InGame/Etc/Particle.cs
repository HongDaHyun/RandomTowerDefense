using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Redcode.Pools;

public class Particle : MonoBehaviour, IPoolObject
{
    void IPoolObject.OnCreatedInPool()
    {
        name = name.Replace("(Clone)", "");
    }

    void IPoolObject.OnGettingFromPool()
    {
    }

    private void OnParticleSystemStopped()
    {
        SpawnManager.Instance.DeSpawnVFX(name, this);
    }
}
