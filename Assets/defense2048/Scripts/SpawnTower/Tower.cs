using defense2048.Scripts.SpawnTower;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tower : MonoBehaviour, IPointerClickHandler
{
    public int level = 1;
    public TowerPoint ownerPoint;

    public void OnPointerClick(PointerEventData eventData)
    {
        // Click lên UI thì bỏ qua
        if (eventData.pointerEnter == null)
            return;

        TowerUIManager.instance.Open(this);
    }

    public void Upgrade()
    {
        level++;
        Debug.Log("Upgrade tower to level " + level);
    }

    public void Sell()
    {
        if (ownerPoint != null)
        {
            ownerPoint.SetOccupied(false);
            
        }
        Destroy(gameObject);
    }
}