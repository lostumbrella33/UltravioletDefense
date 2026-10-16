using UnityEngine;

public class TowerSell : MonoBehaviour
{
    private TowerData towerData;
    private TowerLinker linker;



    void Start()
    {
        linker = GetComponent<TowerLinker>();
        towerData = gameObject.GetComponent<TowerData>();
    }


    void Update()
    {
        if (towerData.selected && towerData.towerName != "Empty" && Input.GetKeyDown(KeyCode.LeftControl))
        {
            SellTower();
        }
    }


    public void SellTower()
    {  
        linker.MoneyManager.ModifyAmount((int)(towerData.moneySpent * linker.MoneyManager.sellMultiplier));
        towerData.ResetTower();

        linker.UpgradePanelController.HidePanel();
        linker.PlacePanelController.ShowPanel(-1);
        MoneyManager.ForceRefreshUI();
    }
}
