using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlacePanelController : MonoBehaviour
{
    public static PlacePanelController Instance { get; private set; }

    [SerializeField] private SelectionData selectionData;
    private UpgradesData upgradesData;


    [SerializeField] private Button LazerButton;
    private TextMeshProUGUI LazerButtonText;

    [SerializeField] private Button LightningButton;
    private TextMeshProUGUI LightningButtonText;

    [SerializeField] private Button StellarButton;
    private TextMeshProUGUI StellarButtonText;

    [SerializeField] private Button IceButton;
    private TextMeshProUGUI IceButtonText;

    [SerializeField] private Button MetalButton;
    private TextMeshProUGUI MetalButtonText;
    private RectTransform panelRectTransform;
    public float leftX = -750;
    public float rightX = 750;
    public float panelY = -50;
    public int currentLeft = 0;
    [SerializeField] private MoneyManager moneyManager;



    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    
    void Start()
    {
        upgradesData = GameObject.Find("UpgradesData").GetComponent<UpgradesData>();

        LazerButtonText = LazerButton.GetComponentInChildren<TextMeshProUGUI>();
        LightningButtonText = LightningButton.GetComponentInChildren<TextMeshProUGUI>();
        StellarButtonText = StellarButton.GetComponentInChildren<TextMeshProUGUI>();
        IceButtonText = IceButton.GetComponentInChildren<TextMeshProUGUI>();
        MetalButtonText = MetalButton.GetComponentInChildren<TextMeshProUGUI>();
        panelRectTransform = GetComponent<RectTransform>();

        HidePanel();
    }


    void Update()
    {
        int cost = (int)upgradesData.UpgradeCosts["Lazer"][0];
        LazerButtonText.text = cost.ToString()+"$";
        LazerButton.GetComponent<ButtonColorController>().upgradeCost = cost;

        cost = (int)upgradesData.UpgradeCosts["Lightning"][0];
        LightningButtonText.text = cost.ToString()+"$";
        LightningButton.GetComponent<ButtonColorController>().upgradeCost = cost;

        cost = (int)upgradesData.UpgradeCosts["Stellar"][0];
        StellarButtonText.text = cost.ToString()+"$";
        StellarButton.GetComponent<ButtonColorController>().upgradeCost = cost;

        cost = (int)upgradesData.UpgradeCosts["Ice"][0];
        IceButtonText.text = cost.ToString()+"$";
        IceButton.GetComponent<ButtonColorController>().upgradeCost = cost;

        cost = (int)upgradesData.UpgradeCosts["Metal"][0];
        MetalButtonText.text = cost.ToString()+"$";
        MetalButton.GetComponent<ButtonColorController>().upgradeCost = cost;
    }


    public void HidePanel()
    {
        gameObject.SetActive(false);
    }


    public void ShowPanel(int left)
    {
        if(left == 1)
        {
            panelRectTransform.anchoredPosition = new Vector2(leftX, panelY);
        }
        else if(left == 0)
        {
            panelRectTransform.anchoredPosition = new Vector2(rightX, panelY);
        }
        
        gameObject.SetActive(true);
        currentLeft = left;
        
    }


    public void OnClickLazer()
    {
        TowerPlace towerPlace = selectionData.selectedTower.GetComponent<TowerPlace>();
        towerPlace.CheckPlacementCondition("Lazer");
    }


    public void OnClickLightning()
    {
        TowerPlace towerPlace = selectionData.selectedTower.GetComponent<TowerPlace>();
        towerPlace.CheckPlacementCondition("Lightning");
    }


    public void OnClickStellar()
    {
        TowerPlace towerPlace = selectionData.selectedTower.GetComponent<TowerPlace>();
        towerPlace.CheckPlacementCondition("Stellar");
    }


    public void OnClickIce()
    {
        TowerPlace towerPlace = selectionData.selectedTower.GetComponent<TowerPlace>();
        towerPlace.CheckPlacementCondition("Ice");
    }


    public void OnClickMetal()
    {
        TowerPlace towerPlace = selectionData.selectedTower.GetComponent<TowerPlace>();
        towerPlace.CheckPlacementCondition("Metal");
    }
}
