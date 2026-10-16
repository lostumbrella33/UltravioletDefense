using UnityEngine;


// script to cache references to all manager objects so that all scripts within this tower have know it
public class TowerLinker : MonoBehaviour 
{
    public PlacePanelController PlacePanelController { get; private set; }
    public UpgradePanelController UpgradePanelController { get; private set; }
    public MoneyManager MoneyManager { get; private set; }
    public SelectionData SelectionData { get; private set; }



    void Start()
    {
        PlacePanelController = PlacePanelController.Instance;
        UpgradePanelController = UpgradePanelController.Instance;
        MoneyManager = MoneyManager.Instance;
        SelectionData = SelectionData.Instance;


        if (PlacePanelController == null)
        {
            Debug.LogWarning($"[tower linker] place panel missing");
        }
        if (UpgradePanelController == null)
        {
            Debug.LogWarning($"[tower linker] upgrade panel missing");
        }
        if (MoneyManager == null)
        {
            Debug.LogWarning($"[tower linker] money missing");
        }
        if (SelectionData == null)
        {
            Debug.LogWarning($"[tower linker] selection missing");
        }
    }
}
