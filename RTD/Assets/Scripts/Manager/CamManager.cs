using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamManager : MonoBehaviour
{
    private Vector3 origin;
    private Vector3 difference;
    private Vector3 resetCam;

    private bool drag = false;

    private void Start()
    {
        resetCam = Camera.main.transform.position;
    }

    private void LateUpdate()
    {
        if (Camera.main.transform.position.x < -0.5f || Camera.main.transform.position.x > 9.5f || Camera.main.transform.position.y > 2 || Camera.main.transform.position.y < -1)
            return;
        if (GameManager.Instance.isStart)
            CamMove();
    }

    void CamMove()
    {
        if (Input.GetMouseButton(0))
        {
            difference = (Camera.main.ScreenToWorldPoint(Input.mousePosition)) - Camera.main.transform.position;
            if (drag == false)
            {
                drag = true;
                origin = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
        }
        else
        {
            drag = false;
        }

        if (drag)
            Camera.main.transform.position = origin - difference;

        if (Input.GetMouseButton(1))
            Camera.main.transform.position = resetCam;
    }
}
