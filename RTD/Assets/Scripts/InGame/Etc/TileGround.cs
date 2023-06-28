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

        newUnit.ChangeState(UnitState.SearchTarget); // 위치 설정 후 유닛이 탐색 상태가 되어야함 (처음 위치 벡터(0, 0, 0)이라 적 찾는 것에 버그가 생김)
    }

    public void ResetTile()
    {
        isBuild = false;
        inUnitName = null;
    }
}