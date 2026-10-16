using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SelectionData : MonoBehaviour
{
    public static SelectionData Instance { get; private set; }

    public GameObject selectedTower;
    private TowerData towerData;
    [SerializeField] private UpgradePanelController upgradePanelController;
    [SerializeField] private PlacePanelController placePanelController;
    public int isLeft = 0;




    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }


    void Start()
    {
        selectedTower = null;
    }


    void Update()
    {
        if(selectedTower != null)
        {
            upgradePanelController.UpdateDmgCount();

            if (Input.GetKeyDown(KeyCode.Tab))
            {  
                towerData.targeting = TowerData.NextTargeting(towerData.targeting);
                upgradePanelController.UpdateTargeting(towerData.targeting);
            }
            else if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                towerData.targeting = TowerData.PrevTargeting(towerData.targeting);
                upgradePanelController.UpdateTargeting(towerData.targeting);
            }
        }
    }


    public void ChangeSelected(GameObject newTower, int left)  
    {
        if(selectedTower != null)
        {
            towerData = selectedTower.GetComponent<TowerData>();
            towerData.selected = false;
            towerData.RangeOff();
        }

        selectedTower = newTower;
        towerData = newTower.GetComponent<TowerData>();
        towerData.selected = true;
        towerData.RangeOn();
        upgradePanelController.UpdateData(newTower);
        
        if(towerData.towerName == "Empty")
        {
            placePanelController.ShowPanel(left);
            MoneyManager.ForceRefreshUI();
            upgradePanelController.HidePanel();
        }
        else
        {
            upgradePanelController.ShowPanel(left);
            MoneyManager.ForceRefreshUI();
            placePanelController.HidePanel();
        }
        
    }

    public void ClearSelection()
    {
        if(selectedTower != null)
        {
            towerData = selectedTower.GetComponent<TowerData>();
            towerData.selected = false;
            towerData.RangeOff();

            selectedTower = null;
            towerData = null;
            
            upgradePanelController.HidePanel();
            placePanelController.HidePanel();
        }
    }
}
