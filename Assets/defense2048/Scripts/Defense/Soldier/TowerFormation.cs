using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerFormation : MonoBehaviour
{
    [Header("Soldier")]
    [SerializeField] private GameObject soldierPrefab;
    [SerializeField] private float respawnDelay = 3f;

    [Header("Formation")]
    [SerializeField] private float formationRadius = 0.6f;

    [Header("Tower Point Detect")]
    [SerializeField] private float detectRadius = 1.5f;
    [SerializeField] private string towerPointTag = "tower_point";
    [SerializeField] private Transform soldierSpawnPoint;


    private Transform formationAnchor;
    private readonly Dictionary<Vector3, GameObject> activeSoldiers = new Dictionary<Vector3, GameObject>();
    private readonly List<Vector3> formationOffsets = new List<Vector3>();

    private void Start()
    {
        FindFormationAnchor();
        CreateFormationOffsets();
        SpawnInitialSoldiers();
    }

    #region Find Anchor

    void FindFormationAnchor()
    {
        GameObject[] points = GameObject.FindGameObjectsWithTag(towerPointTag);

        float minDist = Mathf.Infinity;
        Transform nearest = null;

        foreach (var point in points)
        {
            float d = Vector2.Distance(transform.position, point.transform.position);
            if (d < minDist && d <= detectRadius)
            {
                minDist = d;
                nearest = point.transform;
            }
        }

        if (nearest == null)
        {
            Debug.LogError("❌ No TowerPoint found near tower!");
            return;
        }

        if (nearest.childCount == 0)
        {
            Debug.LogError("❌ TowerPoint has no anchor child!");
            return;
        }

        formationAnchor = nearest.GetChild(0);
    }

    #endregion

    #region Formation

    void CreateFormationOffsets()
    {
        formationOffsets.Clear();

        formationOffsets.Add(new Vector3(0, formationRadius, 0));
        formationOffsets.Add(new Vector3(-formationRadius, -formationRadius * 0.5f, 0));
        formationOffsets.Add(new Vector3(formationRadius, -formationRadius * 0.5f, 0));

        foreach (var offset in formationOffsets)
        {
            activeSoldiers[offset] = null;
        }
    }

    void SpawnInitialSoldiers()
    {
        if (formationAnchor == null) return;

        foreach (var offset in formationOffsets)
            SpawnSoldier(offset);
    }

    #endregion

    #region Spawn

    void SpawnSoldier(Vector3 offset)
    {
        if (activeSoldiers.ContainsKey(offset) && activeSoldiers[offset] != null)
        {
            return;
        }
        
        Vector3 spawnPos = soldierSpawnPoint != null
            ? soldierSpawnPoint.position
            : transform.position;

        GameObject soldierObj = Instantiate(
            soldierPrefab,
            spawnPos,
            Quaternion.identity
        );
        activeSoldiers[offset] = soldierObj;

        SoldierMove move = soldierObj.GetComponent<SoldierMove>();
        SoldierHealth health = soldierObj.GetComponent<SoldierHealth>();

        move.SetFormaiton(formationAnchor, offset);

        health.ownerTower = this;
        health.formationOffset = offset;
    }

    public void OnSoldierDead(SoldierHealth deadSoldier)
    {
        Vector3 deadOffset = deadSoldier.formationOffset;

        if (activeSoldiers.ContainsKey(deadOffset))
        {
            activeSoldiers[deadOffset] = null;
        }
        
        StartCoroutine(RespawnSoldier(deadSoldier.formationOffset));
    }

    IEnumerator RespawnSoldier(Vector3 offset)
    {
        yield return new WaitForSeconds(respawnDelay);
        if (activeSoldiers[offset] == null)
        {
            SpawnSoldier(offset);
        }
      
    }

    public bool IsFullCapacity()
    {
        int count = 0;
        foreach (var soldier in activeSoldiers.Values)
        {
            if (soldier != null) count++;
        }

        return count >= 3;
    }

    #endregion
}
