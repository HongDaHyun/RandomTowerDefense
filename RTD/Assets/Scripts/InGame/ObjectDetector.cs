using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Com.LuisPedroFonseca.ProCamera2D;

public class ObjectDetector : MonoBehaviour
{
    private RaycastHit2D hit;
    private Camera mainCam;

    private void Awake()
    {
        mainCam = Camera.main;
    }

    private void Update()
    {
        if(Input.GetMouseButtonUp(0) && !mainCam.GetComponent<ProCamera2DPanAndZoom>().IsPanning)
        {
            Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            hit = Physics2D.Raycast(pos, Vector2.zero, 0f);
            if (hit.collider != null)
            {
                if (hit.collider.CompareTag("TileGround")) //타워 설치 가능 땅
                {
                    if (!UIManager.Instance.clickTile.activeSelf)
                        UIManager.Instance.clickTile.SetActive(true);
                    else if (UIManager.Instance.clickTile.transform.parent == hit.transform)
                        UIManager.Instance.clickTile.SetActive(false);
                    UIManager.Instance.clickTile.transform.SetParent(hit.transform);
                    UIManager.Instance.clickTile.transform.localPosition = new Vector3(0, 0, 0);
                }
                else if (hit.collider.CompareTag("TileRoad")) //타워 설치 불가능 길
                    if (UIManager.Instance.clickTile.activeSelf)
                        UIManager.Instance.clickTile.SetActive(false);
            }
            else
                return;
        }
    }
}
