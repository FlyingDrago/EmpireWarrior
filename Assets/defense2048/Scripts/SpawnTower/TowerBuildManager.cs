using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerBuildManager : MonoBehaviour
{

    public static TowerBuildManager instance;
    [Header("UI")] public RectTransform buildPanel;
    public GameObject clickBlocker;

    private TowerPoint currentPoint;
    private BuildTowerButton currentConfirmButton;

    private void Awake()
    {
        if (instance == null) instance = this;
        else
        {
            Destroy(gameObject);
        }

        buildPanel.gameObject.SetActive(false);
    }

    public void OpenBuildPanel(TowerPoint point)
    {
        if (!buildPanel || !clickBlocker) return;

        currentPoint = point;

        Vector2 screenPos = Camera.main.WorldToScreenPoint(point.transform.position);

        RectTransform canvasRect = buildPanel.GetComponentInParent<Canvas>().GetComponent<RectTransform>();

        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPos, null, out Vector2 localPos);

        buildPanel.localPosition = localPos;

        clickBlocker.SetActive(true);
        buildPanel.gameObject.SetActive(true);
        ResetConfirm();
    }

    public void OnBuildButtonClicked(BuildTowerButton button)
    {
        // LẦN 2 → BUILD
        if (currentConfirmButton == button && button.IsConfirming())
        {
            BuildTower(button.towerPrefab);
            CloseBuildPanel();
            return;
        }

        // LẦN 1 → CHỌN CONFIRM
        ResetConfirm();
        currentConfirmButton = button;
        button.SetConfirm(true);
    }

    void ResetConfirm()
    {
        if (currentConfirmButton != null)
        {
            currentConfirmButton.SetConfirm(false);
            currentConfirmButton = null;
        }
    }
    public void BuildTower(GameObject towerPrefab)
    {
        if (currentPoint == null || towerPrefab == null)
            return;

      GameObject towerGO=  Instantiate(towerPrefab, currentPoint.transform.position, Quaternion.identity);

      Tower tower = towerGO.GetComponentInParent<Tower>();
      if (tower != null)
      {
          tower.ownerPoint = currentPoint;
      }
      
        currentPoint.SetOccupied(true);
        CloseBuildPanel();
    }

    public void CloseBuildPanel()
    {
        currentPoint = null; 
        buildPanel.gameObject.SetActive(false);
        clickBlocker.SetActive(false);
        ResetConfirm();
    }        
}

         
