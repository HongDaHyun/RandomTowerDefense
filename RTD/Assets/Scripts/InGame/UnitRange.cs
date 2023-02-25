using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitRange : MonoBehaviour
{
    private void Awake()
    {
        
    }

    public void OnAtkRange(Vector3 position, float range)
    {
        gameObject.SetActive(true);

        float diameter = range * 2.0f;
        transform.localScale = Vector3.one * diameter;
        transform.position = position;
    }

    public void OffAtkRange()
    {
        gameObject.SetActive(false);
    }
}
