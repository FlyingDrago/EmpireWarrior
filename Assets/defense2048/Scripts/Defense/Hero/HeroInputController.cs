using System;
using UnityEngine;

public class HeroInputController : MonoBehaviour
{
    private bool isSelected;
    private HeroMove heroMove;
    private HeroCombat heroCombat;

    private void Awake()
    {
        heroMove = GetComponent<HeroMove>();
        heroCombat = GetComponent<HeroCombat>();
    }

    public void SelectHero()
    {
        if (heroCombat != null && heroCombat.GetCurrentEnemy() == null)
        {
            isSelected = true;
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
            heroCombat.StopCombat();
            heroMove.MoveToPosition(worldPos);
            isSelected = false;
        }
    }

  
}