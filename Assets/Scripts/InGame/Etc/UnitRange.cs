using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitRange : MonoBehaviour
{
    public void OnAtkRange(Vector3 position, float range)
    {
        gameObject.SetActive(true);

        float diameter = range * 2.0f;
        transform.localScale = Vector3.one * diameter;
        Vector2 pos = position;
        transform.position = pos + new Vector2(0, 0.2f);
    }

    public void OffAtkRange()
    {
        gameObject.SetActive(false);
    }
}
