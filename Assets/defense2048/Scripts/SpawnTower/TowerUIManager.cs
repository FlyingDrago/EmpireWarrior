using System;
using UnityEngine;

namespace defense2048.Scripts.SpawnTower
{
    public class TowerUIManager : MonoBehaviour
    {
        public static TowerUIManager instance;

        public RectTransform panel;
        public GameObject clickBlocker;

        private Tower currentTower;

        private void Awake()
        {
            instance = this;
            panel.gameObject.SetActive(false);
        }

        public void Open(Tower tower)
        {
            currentTower = tower;
            Vector2 screenPos = Camera.main.WorldToScreenPoint(tower.transform.position);
            Canvas canvas = panel.GetComponentInParent<Canvas>();
            RectTransform canvasRect = canvas.GetComponent<RectTransform>();

            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPos, null, out Vector2 localPos);
            
            panel.localPosition=localPos;
            panel.gameObject.SetActive(true);
            clickBlocker.SetActive(true);
        }

        public void Upgrade()
        {
            currentTower?.Upgrade();
            Close();
        }

        public void Sell()
        {
            currentTower?.Sell();
            Close();
        }

        public void Close()
        {
            currentTower = null;
            panel.gameObject.SetActive(false);
            clickBlocker.SetActive(false);
        }
    }
}