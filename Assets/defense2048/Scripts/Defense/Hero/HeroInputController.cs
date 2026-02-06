using System;
using UnityEngine;

public class HeroInputController : MonoBehaviour
{
  
    private bool isSelected;
    private HeroMove heroMove;
    private HeroCombat heroCombat;

    [Header("Selection Effects")] 
    public GameObject selectionPrefab;
    public GameObject ClickEffectPrefab;

    private GameObject currentSelectionObj;
    private GameObject currentClickEffectObj;

    private void Awake()
    {
        heroMove = GetComponent<HeroMove>();
        heroCombat = GetComponent<HeroCombat>();
        if (selectionPrefab != null)
        {
            currentSelectionObj = Instantiate(selectionPrefab, transform.position, Quaternion.identity,transform);
            currentSelectionObj.SetActive(false);
        }

        if (ClickEffectPrefab != null)
        {
            currentClickEffectObj = Instantiate(ClickEffectPrefab);
            currentClickEffectObj.SetActive(false);
        }
    }

    public void SelectHero()
    {
        if (heroCombat != null && heroCombat.GetCurrentEnemy() == null)
        {
            isSelected = true;
            if (currentSelectionObj != null)
            {
                currentSelectionObj.SetActive(true);
            }
            Debug.Log("hero selected");
        }
        else
        {
            Debug.Log("Hero is busy in combat and cannot be selected");
        }
       

    }

    private void Update()
    {
        if (!isSelected) return;

        if (Input.GetMouseButtonDown(0))
        {
            Vector2 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            
            if (currentSelectionObj != null)
            {
                currentSelectionObj.SetActive(false);
            }

            if (currentClickEffectObj != null)
            {
                currentClickEffectObj.SetActive(false);
                currentClickEffectObj.transform.position = worldPos;
                currentClickEffectObj.SetActive(true);
            }
            heroCombat.StopCombat();
            heroMove.MoveToPosition(worldPos);
            isSelected = false;
        }
    }

  
}