using System;
using UnityEngine;

public class HeroInputController : MonoBehaviour
{
    private bool isSelected;
    private HeroMove heroMove;

    private void Awake()
    {
        heroMove = GetComponent<HeroMove>();
    }

    public void SelectHero()
    {
        isSelected = true;

    }

    private void Update()
    {
        if (!isSelected) return;

        if (Input.GetMouseButtonDown(0))
        {
            Vector2 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            heroMove.MoveToPosition(worldPos);
            isSelected = false;
        }
    }

  
}