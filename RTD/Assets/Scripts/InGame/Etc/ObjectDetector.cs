using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Com.LuisPedroFonseca.ProCamera2D;
using UnityEngine.EventSystems;

public class ObjectDetector : MonoBehaviour
{
    private RaycastHit2D hit;
    private ProCamera2DPanAndZoom proCam;
    private UIManager uiManager;
    private GameManager gm;

    private LayerMask targetLayer;

    bool isTouchingUI;

    private void Awake()
    {
        proCam = Camera.main.GetComponent<ProCamera2DPanAndZoom>();
        uiManager = UIManager.Instance;
        gm = GameManager.Instance;
        targetLayer = LayerMask.GetMask("Tile", "Enemy", "UI");
    }

    private void Update()
    {
        ClickTile(gm.device);

        // 클릭타일 없고, 적 상태창 아닐 때 클릭 타일 비활성화
        if (!uiManager.clickTile.activeSelf && !uiManager.info.enemyInfo.pannel.activeSelf)
            uiManager.info.CloseInfo();
    }

    private void ClickTile(DEVICE device)
    {
        if (proCam.IsPanning)
            return;

        switch(device)
        {
            case DEVICE.UNITY:
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
                break;
            case DEVICE.ANDROID:
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
                break;
        }

        if (!isTouchingUI && (Input.GetMouseButtonUp(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)))
            Tile();
    }

    private void Tile()
    {
        // RayCast 설정
        Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        hit = Physics2D.Raycast(pos, Vector2.zero, 0f, targetLayer);

        // RayCast 충돌 설정
        if (hit.collider != null)
        {
            // 땅 타일
            if (hit.collider.CompareTag("TileGround"))
            {
                SoundManager.Instance.PlaySFX("ClickGrass");
                // 클릭 타일 없으면 생성
                if (!uiManager.clickTile.activeSelf)
                    uiManager.clickTile.SetActive(true);

                // 동일한 클릭타일 클릭 시 제거
                else if (uiManager.clickTile.transform.parent == hit.transform)
                {
                    uiManager.clickTile.SetActive(false);
                    return;
                }

                TileGround tileGround = hit.transform.GetComponent<TileGround>();
                // 클릭 타일에 유닛 존재
                if (tileGround.isBuild)
                    uiManager.info.SetUnitInfo(tileGround);

                // 빈 클릭 타일
                else if (uiManager.info.unitInfo.pannel.activeSelf)
                    uiManager.info.CloseInfo();

                // 클릭 타일 포지션 설정
                uiManager.clickTile.transform.SetParent(hit.transform);
                uiManager.clickTile.transform.localPosition = new Vector3(0, 0, 0);
            }

            // 길 타일
            else if (hit.collider.CompareTag("TileRoad"))
            {
                if (uiManager.clickTile.activeSelf)
                    uiManager.clickTile.SetActive(false);
            }

            // 적
            else if (hit.collider.CompareTag("Enemy"))
            {
                if (uiManager.clickTile.activeSelf)
                    uiManager.clickTile.SetActive(false);

                uiManager.info.SetEnemyInfo(hit.collider.GetComponent<Enemy>());
            }
        }
    }
}
