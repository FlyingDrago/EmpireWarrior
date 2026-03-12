using System;
using UnityEngine;

public class SoldierHealth : DefenseHealth
{
    public static event Action<SoldierHealth> OnAnySoldierDead;

    public TowerFormation ownerTower;
    public EnemyShooter enemyShooter;

    public int slotIndex;

    protected override void Awake()
    {
        base.Awake();

        enemyShooter = GetComponentInChildren<EnemyShooter>();
    }

    protected override void Die()
    {
        // Clear combat first
        if (enemyShooter != null)
        {
            enemyShooter.ExitMelee();
        }

        // Global event
        OnAnySoldierDead?.Invoke(this);

        // Notify tower
        if (ownerTower != null)
        {
            ownerTower.OnSoldierDead(this);
        }

        Destroy(gameObject);
    }
}