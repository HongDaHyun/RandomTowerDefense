using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_People : Enemy
{
    protected override void Die()
    {
    }
    protected override void Movement()
    {
        transform.position = Vector2.MoveTowards(transform.position, moveDirection, moveSpeed);
    }
}
