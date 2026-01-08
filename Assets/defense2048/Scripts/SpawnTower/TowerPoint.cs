using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TowerPoint : MonoBehaviour,IPointerClickHandler
{
   [HideInInspector] public bool isOccupied = false;

   public void OnPointerClick(PointerEventData eventData)
   {
      if(isOccupied)return;
      TowerBuildManager.instance.OpenBuildPanel(this);
   }

   public void SetOccupied(bool value)
   {
      isOccupied = value;

      GetComponent<Collider2D>().enabled = !value;
   }
}
