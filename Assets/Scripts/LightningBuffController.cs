using System.Collections.Generic;
using UnityEngine;

public class LightningBuffController : MonoBehaviour
{
    private TowerData towerData;
    private List<TowerData> buffedNeighbors = new List<TowerData>();



    void Start()
    {
        towerData = GetComponent<TowerData>();
    }


    void Update()
    {
        if (towerData == null || towerData.towerName == "Empty" || towerData.attackCDBuff <= 0)
        {
            ClearMyBuffFromEveryone();
            return;
        }

        float currentBuffRange = towerData.range * 0.1f; 
        Collider[] structuralHits = Physics.OverlapSphere(transform.position, currentBuffRange);  
        HashSet<TowerData> currentNeighborsInRange = new HashSet<TowerData>();

        foreach (Collider col in structuralHits)
        {
            TowerData neighborData = col.GetComponent<TowerData>();
            if (neighborData == null) neighborData = col.GetComponentInParent<TowerData>();

            if (neighborData != null && neighborData != towerData && neighborData.towerName != "Empty")
            {
                currentNeighborsInRange.Add(neighborData);
            }
        }

        
        foreach (TowerData neighbor in currentNeighborsInRange)
        {
            neighbor.RegisterCooldownBuff(this, towerData.attackCDBuff);
            
            if (!buffedNeighbors.Contains(neighbor))
            {
                buffedNeighbors.Add(neighbor);
            }
        }


        List<TowerData> toRemove = new List<TowerData>();
        foreach (TowerData checkedTower in buffedNeighbors)
            if (!currentNeighborsInRange.Contains(checkedTower))
                toRemove.Add(checkedTower);

        foreach (TowerData oldNeighbor in toRemove)
        {
            if (oldNeighbor != null)
            {
                oldNeighbor.UnregisterCooldownBuff(this);
            }
            buffedNeighbors.Remove(oldNeighbor);
        }
    }


    private void ClearMyBuffFromEveryone()
    {
        foreach (TowerData neighbor in buffedNeighbors)
        {
            if (neighbor != null)
            {
                neighbor.UnregisterCooldownBuff(this);
            }
        }
        buffedNeighbors.Clear();
    }


    private void OnDisable()
    {
        ClearMyBuffFromEveryone();
    }
}
