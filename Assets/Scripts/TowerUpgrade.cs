using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TowerUpgrade : MonoBehaviour
{
    private TowerData towerData;
    private UpgradesData upgradesData;
    private int[] costs;
    private TowerLinker linker;



    void Start()
    {
        linker = GetComponent<TowerLinker>();
        upgradesData = GameObject.Find("UpgradesData").GetComponent<UpgradesData>();
        costs = new int[] {0,0,0};

        towerData = gameObject.GetComponent<TowerData>();
    } 
    



    public void Update()
    {
        if(towerData.towerName != "Empty")
        {
            costs[0] = (int)upgradesData.UpgradeCosts[towerData.towerName][
                upgradesData.MapUpgradesToNumber(new int[] {towerData.upgrades[0]+1,0,0})];
            costs[1] = (int)upgradesData.UpgradeCosts[towerData.towerName][
                upgradesData.MapUpgradesToNumber(new int[] {0,towerData.upgrades[1]+1,0})];
            costs[2] = (int)upgradesData.UpgradeCosts[towerData.towerName][
                upgradesData.MapUpgradesToNumber(new int[] {0,0,towerData.upgrades[2]+1})];
        }
        

        // top path
        if (towerData.selected && towerData.towerName != "Empty" && towerData.upgrades[0] < 6 && towerData.upgrades.Sum() < 10
            && Input.GetKeyDown(KeyCode.A) && MoneyManager.moneyCount >= costs[0])
        {
            UpgradeTower(0);
        }


        // middle path
        if (towerData.selected && towerData.towerName != "Empty" && towerData.upgrades[1] < 6 && towerData.upgrades.Sum() < 10 
            && Input.GetKeyDown(KeyCode.S) && MoneyManager.moneyCount >= costs[1])
        {
            UpgradeTower(1);
        }


        // bottom path
        if (towerData.selected && towerData.towerName != "Empty" && towerData.upgrades[2] < 6 && towerData.upgrades.Sum() < 10
            && Input.GetKeyDown(KeyCode.D) && MoneyManager.moneyCount >= costs[2])
        {
            UpgradeTower(2);
        }
    
    }


    public void CheckUpgradeCondition(int path)
    {
        if (path == 0 && towerData.towerName != "Empty" && towerData.upgrades[0] < 6 && towerData.upgrades.Sum() < 10
            && MoneyManager.moneyCount >= costs[0])
        {
            UpgradeTower(0);
        }

        if (path == 1 && towerData.towerName != "Empty" && towerData.upgrades[1] < 6 && towerData.upgrades.Sum() < 10 
            && MoneyManager.moneyCount >= costs[1])
        {
            UpgradeTower(1);
        }

        if (path == 2 && towerData.towerName != "Empty" && towerData.upgrades[2] < 6 && towerData.upgrades.Sum() < 10
            && MoneyManager.moneyCount >= costs[2])
        {
            UpgradeTower(2);
        }
    }


    public void UpgradeTower(int path)
    {
        if(path == 0)
        {
            linker.MoneyManager.ModifyAmount(-costs[0]);
            towerData.moneySpent += costs[0];
            towerData.upgrades[0] += 1;
            upgradesData.ApplyStatChanges(gameObject,0,towerData.upgrades[0]);
            towerData.UpdateRangeCircle();
            towerData.RecalculateCurrentCooldown();


            Texture2D towerTexture = Resources.Load<Texture2D>("Textures/"+towerData.towerName+"/"+towerData.towerName+"_"+
                towerData.upgrades[0]+towerData.upgrades[1]+towerData.upgrades[2]);
            if (towerTexture == null)
            {
                Debug.LogError("texture for "+towerData.towerName+
                towerData.upgrades[0]+towerData.upgrades[1]+towerData.upgrades[2]+" not found");               
            }
            else
            {
                towerData.TextureChange(towerTexture);
            }       
        
            linker.UpgradePanelController.UpdateData(gameObject);
        }

        else if(path == 1)
        {
            linker.MoneyManager.ModifyAmount(-costs[1]);
            towerData.moneySpent += costs[1];
            towerData.upgrades[1] += 1;
            upgradesData.ApplyStatChanges(gameObject,1,towerData.upgrades[1]);
            towerData.UpdateRangeCircle();
            towerData.RecalculateCurrentCooldown();


            Texture2D towerTexture = Resources.Load<Texture2D>("Textures/"+towerData.towerName+"/"+towerData.towerName+"_"+
                towerData.upgrades[0]+towerData.upgrades[1]+towerData.upgrades[2]);
            if (towerTexture == null)
            {
                Debug.LogError("texture for "+towerData.towerName+
                towerData.upgrades[0]+towerData.upgrades[1]+towerData.upgrades[2]+" not found");               
            }
            else
            {
                towerData.TextureChange(towerTexture);
            }    

            linker.UpgradePanelController.UpdateData(gameObject);
        }
        
        else if(path == 2)
        {
            linker.MoneyManager.ModifyAmount(-costs[2]);
            towerData.moneySpent += costs[2];
            towerData.upgrades[2] += 1;
            upgradesData.ApplyStatChanges(gameObject,2,towerData.upgrades[2]);
            towerData.UpdateRangeCircle();
            towerData.RecalculateCurrentCooldown();
            

            Texture2D towerTexture = Resources.Load<Texture2D>("Textures/"+towerData.towerName+"/"+towerData.towerName+"_"+
                towerData.upgrades[0]+towerData.upgrades[1]+towerData.upgrades[2]);
            if (towerTexture == null)
            {
                Debug.LogError("texture for "+towerData.towerName+
                towerData.upgrades[0]+towerData.upgrades[1]+towerData.upgrades[2]+" not found");               
            }
            else
            {
                towerData.TextureChange(towerTexture);
            }
        
            linker.UpgradePanelController.UpdateData(gameObject);
        }
    }

}
