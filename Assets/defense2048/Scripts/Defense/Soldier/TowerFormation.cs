using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerFormation : MonoBehaviour
{
    public GameObject soldierPrefab;
    public Transform formationAnchor;
    public float formationRadius = 0.6f;
    public float respawnDelay = 3f;

    private readonly List<Vector3> formationOffsets = new();

    private void Start()
    {
        CreateTriangleOffsets();
        SpawnInitialSoldiers();
    }

    void CreateTriangleOffsets()
    {
        formationOffsets.Clear();

        formationOffsets.Add(new Vector3(0, formationRadius, 0));
        formationOffsets.Add(new Vector3(-formationRadius, -formationRadius * 0.5f, 0));
        formationOffsets.Add(new Vector3(formationRadius, -formationRadius * 0.5f, 0));
    }

    void SpawnInitialSoldiers()
    {
        foreach (var offset in formationOffsets)
        {
            SpawnSoldier(offset);
        }
    }

    void SpawnSoldier(Vector3 offset)
    {
        GameObject soldierObj = Instantiate(
            soldierPrefab,
            transform.position,
            Quaternion.identity
        );

        SoldierMove move = soldierObj.GetComponent<SoldierMove>();
        SoldierHealth health = soldierObj.GetComponent<SoldierHealth>();
        SoldierCombat combat = soldierObj.GetComponent<SoldierCombat>();
        
        

        move.SetFormaiton(formationAnchor, offset);
     

        health.ownerTower = this;
        health.formationOffset = offset;
        
        if (!combat.TryFindEnemyImmediate())
        {
            move.ReturnFormation();
        }
    }

    //  ĐƯỢC GỌI KHI SOLDIER CHẾT
    public void OnSoldierDead(SoldierHealth deadSoldier)
    {
        StartCoroutine(RespawnSoldier(deadSoldier.formationOffset));
    }

    IEnumerator RespawnSoldier(Vector3 offset)
    {
        yield return new WaitForSeconds(respawnDelay);
        SpawnSoldier(offset);
    }
}