using System.Collections.Generic;
using UnityEngine;

public class AreaSlowController : MonoBehaviour
{
    private TowerData towerData;
    private CheckEnemiesInRange rangeChecker;
    private List<FreezeEffect> currentlySlowedEnemies = new List<FreezeEffect>();



    void Start()
    {
        towerData = GetComponent<TowerData>();
        rangeChecker = GetComponentInChildren<CheckEnemiesInRange>();
    }


    void Update()
    {
        if (rangeChecker == null || towerData.towerName != "Ice") 
        {
            ClearAllAuras();
            return;
        }

        List<GameObject> enemiesInRange = new List<GameObject>(rangeChecker.enemiesInRange);
        enemiesInRange.RemoveAll(item => item == null);

        Vector3 towerPos = transform.position;
        enemiesInRange.Sort((a, b) => Vector3.Distance(towerPos, a.transform.position)
            .CompareTo(Vector3.Distance(towerPos, b.transform.position)));

        HashSet<FreezeEffect> enemiesToFreeze = new HashSet<FreezeEffect>();

        
        int activeAuraCount = 0;
        foreach (GameObject enemyObj in enemiesInRange)
        {
            if (activeAuraCount >= towerData.pierce) break;

            FreezeEffect freeze = enemyObj.GetComponent<FreezeEffect>();
            if(freeze == null)
            {
                freeze = enemyObj.AddComponent<FreezeEffect>();
            }

            freeze.ApplyAreaSlow(towerData.freezeStrength);
            enemiesToFreeze.Add(freeze);
            activeAuraCount++;
        }

        
        List<FreezeEffect> toRemove = new List<FreezeEffect>();
        foreach (FreezeEffect activeSlow in currentlySlowedEnemies)
        {
            if (activeSlow == null) continue;
            
            if (!enemiesToFreeze.Contains(activeSlow))
            {
                activeSlow.RemoveAreaSlow();
                toRemove.Add(activeSlow);
            }
        }

        foreach (FreezeEffect oldEnemy in toRemove) currentlySlowedEnemies.Remove(oldEnemy);
        foreach (FreezeEffect newEnemy in enemiesToFreeze)
        {
            if (!currentlySlowedEnemies.Contains(newEnemy)) currentlySlowedEnemies.Add(newEnemy);
        }
    }


    private void ClearAllAuras()
    {
        foreach (FreezeEffect freeze in currentlySlowedEnemies)
        {
            if(freeze != null)
            {
                freeze.RemoveAreaSlow();
            }
        }
        currentlySlowedEnemies.Clear();
    }


    private void OnDisable()
    {
        ClearAllAuras();
    }
}
