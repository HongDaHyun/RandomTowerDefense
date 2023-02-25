using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Com.LuisPedroFonseca.ProCamera2D;
using UnityEngine.EventSystems;

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
            if (hit.collider != null && !EventSystem.current.IsPointerOverGameObject())
            {
                if (hit.collider.CompareTag("TileGround")) //타워 설치 가능 땅
                {
                    if (!UIManager.Instance.clickTile.activeSelf) //클릭 타일 없으면 생성
                        UIManager.Instance.clickTile.SetActive(true);

                    else if (UIManager.Instance.clickTile.transform.parent == hit.transform) //동일한 클릭타일 있을 시 제거
                    {
                        UIManager.Instance.clickTile.SetActive(false);
                        return;
                    }

                    TileGround tileGround = hit.transform.GetComponent<TileGround>();
                    if (tileGround.isBuild) //클릭타일에 유닛있을 시 정보창 생성
                        UIManager.Instance.SetUnitInfo(tileGround);
                    else //클릭타일에 유닛 없는 빈 타일일 때 정보창 제거
                        if (UIManager.Instance.unitInfoPannel.activeSelf)
                            CloseUnitInfo();

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
        if (!UIManager.Instance.clickTile.activeSelf) //클릭타일 없을 때 유닛정보패널 비활성화
            CloseUnitInfo();
    }

    private void CloseUnitInfo()
    {
        UIManager.Instance.unitInfoPannel.SetActive(false);
        UIManager.Instance.unitRange.OffAtkRange();
    }
}
