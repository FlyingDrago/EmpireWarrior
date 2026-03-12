using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerFormation : MonoBehaviour
{
    [Header("Soldier")]
    [SerializeField] private GameObject soldierPrefab;
    [SerializeField] private float respawnDelay = 10f;

    [Header("Formation")]
    [SerializeField] private float formationRadius = 0.6f;

    [Header("Tower Point Detect")]
    [SerializeField] private float detectRadius = 1.5f;
    [SerializeField] private string towerPointTag = "tower_point";
    [SerializeField] private Transform soldierSpawnPoint;

    private Transform formationAnchor;

    private List<Vector3> formationOffsets = new List<Vector3>();
    private GameObject[] activeSoldiers;
    private bool[] respawnRunning;

    void Start()
    {
        FindFormationAnchor();
        CreateFormationOffsets();

        activeSoldiers = new GameObject[formationOffsets.Count];
        respawnRunning = new bool[formationOffsets.Count];

        SpawnInitialSoldiers();
    }

    // =========================
    // FIND ANCHOR
    // =========================

    void FindFormationAnchor()
    {
        GameObject[] points = GameObject.FindGameObjectsWithTag(towerPointTag);

        float minDist = Mathf.Infinity;

        foreach (var point in points)
        {
            float d = Vector2.Distance(transform.position, point.transform.position);

            if (d < minDist && d <= detectRadius)
            {
                minDist = d;
                formationAnchor = point.transform.GetChild(0);
            }
        }

        if (formationAnchor == null)
        {
            Debug.LogError("No TowerPoint anchor found");
        }
    }

    // =========================
    // FORMATION
    // =========================

    void CreateFormationOffsets()
    {
        formationOffsets.Clear();

        formationOffsets.Add(new Vector3(0, formationRadius));
        formationOffsets.Add(new Vector3(-formationRadius, -formationRadius * 0.5f));
        formationOffsets.Add(new Vector3(formationRadius, -formationRadius * 0.5f));
    }

    void SpawnInitialSoldiers()
    {
        for (int i = 0; i < formationOffsets.Count; i++)
        {
            SpawnSoldier(i);
        }
    }

    // =========================
    // SPAWN
    // =========================

    void SpawnSoldier(int index)
    {
        if (formationAnchor == null) return;

        if (activeSoldiers[index] != null)
            return;

        Vector3 spawnPos =
            soldierSpawnPoint != null ? soldierSpawnPoint.position : transform.position;

        GameObject soldier = Instantiate(
            soldierPrefab,
            spawnPos,
            Quaternion.identity
        );

        activeSoldiers[index] = soldier;

        SoldierMove move = soldier.GetComponent<SoldierMove>();
        SoldierHealth health = soldier.GetComponent<SoldierHealth>();

        move.SetFormation(formationAnchor, formationOffsets[index]);

        health.ownerTower = this;
        health.slotIndex = index;
    }

    // =========================
    // SOLDIER DEAD
    // =========================

    public void OnSoldierDead(SoldierHealth dead)
    {
        int index = dead.slotIndex;

        if (index < 0 || index >= activeSoldiers.Length)
            return;

        activeSoldiers[index] = null;

        if (!respawnRunning[index])
            StartCoroutine(RespawnSoldier(index));
    }

    IEnumerator RespawnSoldier(int index)
    {
        respawnRunning[index] = true;

        yield return new WaitForSeconds(respawnDelay);

        if (this == null) yield break;

        if (activeSoldiers[index] == null)
        {
            SpawnSoldier(index);
        }

        respawnRunning[index] = false;
    }

    // =========================
    // CAPACITY
    // =========================

    public bool IsFullCapacity()
    {
        for (int i = 0; i < activeSoldiers.Length; i++)
        {
            if (activeSoldiers[i] == null)
                return false;
        }

        return true;
    }
}