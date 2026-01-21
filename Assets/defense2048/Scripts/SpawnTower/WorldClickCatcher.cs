using defense2048.Scripts.SpawnTower;
using UnityEngine;
using UnityEngine.EventSystems;

public class WorldClickCatcher : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Click lên UI thì bỏ qua
            if (EventSystem.current.IsPointerOverGameObject())
                return;

            // Raycast world
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray);

            // Không click TowerPoint → đóng panel
            if (!hit || !hit.collider.CompareTag("tower_point"))
            {
                TowerBuildManager.instance?.CloseBuildPanel();
                TowerUIManager.instance?.Close();
            }
        }
    }
}