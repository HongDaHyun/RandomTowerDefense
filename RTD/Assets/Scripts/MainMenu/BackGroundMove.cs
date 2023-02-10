using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundMove: MonoBehaviour
{
    public Vector2 L, R;

    bool isLeft;

    private void FixedUpdate()
    {
        if(!isLeft)
            transform.position = Vector2.MoveTowards(transform.position, R, 0.01f);
        else
            transform.position = Vector2.MoveTowards(transform.position, L, 0.01f);

        if (transform.position.x == L.x)
            isLeft = false;
        else if (transform.position.x == R.x)
            isLeft = true;
    }

    
}
