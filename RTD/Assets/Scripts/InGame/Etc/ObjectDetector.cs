using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Com.LuisPedroFonseca.ProCamera2D;
using UnityEngine.EventSystems;

public class ObjectDetector : MonoBehaviour
{
    private RaycastHit2D hit;
    private ProCamera2DPanAndZoom proCam;

    private LayerMask targetLayer;

    bool isTouchingUI;

    private void Awake()
    {
        proCam = Camera.main.GetComponent<ProCamera2DPanAndZoom>();
        targetLayer = LayerMask.GetMask("Tile", "Enemy", "UI");
    }

    private void Update()
    {
        ClickTile();
    }

    private void ClickTile()
    {
        if (proCam.IsPanning)
            return;
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            // 마우스가 UI요소 위에 있을 시
            if (EventSystem.current.IsPointerOverGameObject())
            {
                isTouchingUI = true;
                return;
            }

            isTouchingUI = false;
        }
#elif UNITY_ANDROID
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    // 터치 위에 UI요소가 있을 시
                    if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
                    {
                        isTouchingUI = true;
                        return;
                    }

                    isTouchingUI = false;
                }
#endif
        if (!isTouchingUI && (Input.GetMouseButtonUp(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)))
            Tile();
    }

    private void Tile()
    {
        // RayCast 설정
        UIManager uiManager = UIManager.Instance;

        Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        hit = Physics2D.Raycast(pos, Vector2.zero, 0f, targetLayer);

        // RayCast 충돌 설정
        if (hit.collider != null)
        {
            // 땅 타일
            if (hit.collider.CompareTag("TileGround"))
            {
                SoundManager.Instance.PlaySFX("ClickGrass");

                // 클릭 타일 비활성화 시
                if (!uiManager.clickTile.activeSelf)
                {
                    uiManager.clickTile.SetActive(true); // 클릭 타일 생성
                }


                // 동일한 클릭타일 클릭 시
                else if (uiManager.clickTile.transform.parent == hit.transform)
                {
                    uiManager.CloseInfoClickTile();
                    return;
                }

                // 타일
                TileGround tileGround = hit.transform.GetComponent<TileGround>();
                // 클릭 타일에 유닛 존재
                if (tileGround.isBuild)
                    uiManager.info.SetUnitInfo(tileGround); // 유닛 정보창 생성

                // 빈 클릭 타일 && 유닛 정보 창 활성화되어 있다면
                else if (uiManager.info.unitInfo.pannel.activeSelf)
                    uiManager.info.TabInfo(false); // 정보 창 닫기

                uiManager.clickTile.transform.SetParent(hit.transform);
                uiManager.clickTile.transform.localPosition = new Vector3(0, 0, 0);
            }

            // 길 타일
            else if (hit.collider.CompareTag("TileRoad"))
            {
                uiManager.CloseInfoClickTile();
            }

            // 적
            else if (hit.collider.CompareTag("Enemy"))
            {
                if (uiManager.clickTile.activeSelf)
                    uiManager.clickTile.SetActive(false); // 클릭 타일 제거

                uiManager.info.SetEnemyInfo(hit.collider.GetComponent<Enemy>()); // 적 정보창 생성
            }
        }
    }
}
