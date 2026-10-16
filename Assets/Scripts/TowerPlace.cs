using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerPlace : MonoBehaviour
{
    private TowerData towerData;
    private UpgradesData upgradesData;
    int[] placeCosts;
    private TowerLinker linker;



    void Start()
    {
        linker = GetComponent<TowerLinker>();
        towerData = GetComponent<TowerData>();
        upgradesData = GameObject.Find("UpgradesData").GetComponent<UpgradesData>();
        placeCosts = new int[] {(int)upgradesData.UpgradeCosts["Lazer"][0], 
            (int)upgradesData.UpgradeCosts["Lightning"][0], (int)upgradesData.UpgradeCosts["Stellar"][0], 
            (int)upgradesData.UpgradeCosts["Ice"][0], (int)upgradesData.UpgradeCosts["Metal"][0] };

    }


    void Update()
    {
        if (towerData.selected && towerData.towerName == "Empty" && Input.GetKeyDown(KeyCode.Q) && 
            linker.MoneyManager != null && MoneyManager.moneyCount >= placeCosts[0])
        {
            PlaceTower("Lazer",0);    
        }

        if (towerData.selected && towerData.towerName == "Empty" && Input.GetKeyDown(KeyCode.W) && 
            linker.MoneyManager != null && MoneyManager.moneyCount >= placeCosts[1])
        {
            PlaceTower("Lightning",1);  
        }

        else if (towerData.selected && towerData.towerName == "Empty" && Input.GetKeyDown(KeyCode.E) && 
            linker.MoneyManager != null && MoneyManager.moneyCount >= placeCosts[2])
        {
            PlaceTower("Stellar",2);
        }

        else if (towerData.selected && towerData.towerName == "Empty" && Input.GetKeyDown(KeyCode.R) && 
            linker.MoneyManager != null && MoneyManager.moneyCount >= placeCosts[3])
        {
            PlaceTower("Ice",3);
        }

        else if (towerData.selected && towerData.towerName == "Empty" && Input.GetKeyDown(KeyCode.T) && 
            linker.MoneyManager != null && MoneyManager.moneyCount >= placeCosts[4])
        {
            PlaceTower("Metal",4);
        }

    }


    public void CheckPlacementCondition(string name)
    {
        if(name == "Lazer" && towerData.towerName == "Empty" && linker.MoneyManager != null 
            && MoneyManager.moneyCount >= placeCosts[0])
        {
            PlaceTower(name,0);
        }
        else if(name == "Lightning" && towerData.towerName == "Empty" && linker.MoneyManager != null 
            && MoneyManager.moneyCount >= placeCosts[1])
        {
            PlaceTower(name,1);
        }
        else if(name == "Stellar" && towerData.towerName == "Empty" && linker.MoneyManager != null 
            && MoneyManager.moneyCount >= placeCosts[2])
        {
            PlaceTower(name,2);
        }
        else if(name == "Ice" && towerData.towerName == "Empty" && linker.MoneyManager != null 
            && MoneyManager.moneyCount >= placeCosts[3])
        {
            PlaceTower(name,3);
        }
        else if(name == "Metal" && towerData.towerName == "Empty" && linker.MoneyManager != null 
            && MoneyManager.moneyCount >= placeCosts[4])
        {
            PlaceTower(name,4);
        }
    }


    public void PlaceTower(string name, int i)
    {
        linker.MoneyManager.ModifyAmount(-placeCosts[i]);
        towerData.moneySpent = placeCosts[i];
        towerData.towerName = name;
        towerData.damage = (int)upgradesData.TowerBaseStats[towerData.towerName][0];
        towerData.attackCD = (float)upgradesData.TowerBaseStats[towerData.towerName][1];
        towerData.RecalculateCurrentCooldown();
        towerData.pierce = (int)upgradesData.TowerBaseStats[towerData.towerName][2];
        towerData.range = (int)upgradesData.TowerBaseStats[towerData.towerName][3];
        towerData.UpdateRangeCircle();
        towerData.projectileSpeed = (float)upgradesData.TowerBaseStats[towerData.towerName][4];

        towerData.TextureOn();
        towerData.RangeOn();
        

        if(name == "Lazer")
        {
            towerData.lazerWidth = 0.3f;
        }

        if(name == "Stellar")
        {
            towerData.explosionRadius = 1f;
            towerData.projectileScale = 0.8f;
        }

        if(name == "Ice")
        {
            towerData.freezeStrength = 0.1f;
            towerData.freezeDuration = 1f;
            towerData.explosionRadius = 0.5f;
        }

        if(name == "Metal")
        {
            towerData.multiTarget = 8;
        }

        if(name == "Lightning")
        {
            towerData.maxChainAmount = 1;
            towerData.lazerWidth = 0.3f;
        }
        

        Texture2D towerTexture = Resources.Load<Texture2D>(
            "Textures/"+towerData.towerName+"/"+towerData.towerName+"_000");
        if (towerTexture == null)
        {
            Debug.LogError($"Texture for '{towerData.towerName}' 000 not found");               
        }
        else
        {
            towerData.TextureChange(towerTexture);
        }    


        GameObject projPrefab = Resources.Load<GameObject>("Prefabs/Projectiles/"+towerData.towerName+"_000");
        if(projPrefab == null && towerData.towerName != "Lazer" && towerData.towerName != "Lightning")
        {
            Debug.LogError($"Projectile prefab for '{towerData.towerName}' 000 not found");
        }
        else
        {
            towerData.projectilePrefab = projPrefab;
        }

        linker.PlacePanelController.HidePanel();
        linker.UpgradePanelController.ShowPanel(linker.PlacePanelController.currentLeft);
        linker.UpgradePanelController.UpdateData(gameObject);
        MoneyManager.ForceRefreshUI();
    }
}