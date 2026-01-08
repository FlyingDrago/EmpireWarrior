using UnityEngine;
using UnityEngine.UI;

public class BuildTowerButton : MonoBehaviour
{
    public GameObject towerPrefab;

    [Header("UI")]
    public Image icon;
    public Sprite normalSprite;
    public Sprite confirmSprite;

    private bool isConfirming;

    public void OnClick()
    {
        TowerBuildManager.instance.OnBuildButtonClicked(this);
    }

    public void SetConfirm(bool value)
    {
        isConfirming = value;
        icon.sprite = isConfirming ? confirmSprite : normalSprite;
    }

    public bool IsConfirming()
    {
        return isConfirming;
    }
}