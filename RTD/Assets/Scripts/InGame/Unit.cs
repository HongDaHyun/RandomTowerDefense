using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Redcode.Pools;

public abstract class Unit : MonoBehaviour, IPoolObject
{
    void IPoolObject.OnCreatedInPool()
    {
    }

    void IPoolObject.OnGettingFromPool()
    {
    }
}
