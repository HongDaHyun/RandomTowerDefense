using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGround : MonoBehaviour
{
    public string inUnitName;
    public bool isBuild;

    public void SetTile(Unit newUnit)
    {
        newUnit.transform.SetParent(transform);
        isBuild = true;
        inUnitName = newUnit.unitStat.unitName;
        newUnit.transform.localPosition = new Vector2(0, -0.2f);
    }

    public void ResetTile()
    {
        isBuild = false;
        inUnitName = null;
    }
}