using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Redcode.Pools;

public abstract class Unit : MonoBehaviour, IPoolObject
{
    public UnitStat unitStat;
    void IPoolObject.OnCreatedInPool()
    {
    }

    void IPoolObject.OnGettingFromPool()
    {
        unitStat = CSVReader.Instance.UnitExport(name);
    }
}
