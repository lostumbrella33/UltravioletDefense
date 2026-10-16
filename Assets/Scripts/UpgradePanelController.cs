using System;
using System.CodeDom.Compiler;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradePanelController : MonoBehaviour
{
    public static UpgradePanelController Instance { get; private set; }

    private TowerData towerData;
    private UpgradesData upgradesData;
    private MoneyManager moneyManager;
    [SerializeField] private SelectionData selectionData;
    [SerializeField] private RawImage towerImage;
    [SerializeField] private TextMeshProUGUI towerText;
    [SerializeField] private TextMeshProUGUI targetingText;
    [SerializeField] private TextMeshProUGUI dmgCount;

    [SerializeField] private RawImage topPathImage;
    [SerializeField] private TextMeshProUGUI topPathDesc;
    [SerializeField] private Button topPathButton;
    private TextMeshProUGUI topPathButtonText;

    [SerializeField] private RawImage middlePathImage;
    [SerializeField] private TextMeshProUGUI middlePathDesc;
    [SerializeField] private Button middlePathButton;
    private TextMeshProUGUI middlePathButtonText;

    [SerializeField] private RawImage bottomPathImage;
    [SerializeField] private TextMeshProUGUI bottomPathDesc;
    [SerializeField] private Button bottomPathButton;
    private TextMeshProUGUI bottomPathButtonText;

    [SerializeField] private TextMeshProUGUI sellingCost;
    private RectTransform panelRectTransform;
    [SerializeField] private PlacePanelController placePanelController;




    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }


    void Start()
    {
        HidePanel();
        upgradesData = gameObject.GetComponent<UpgradesData>();
        moneyManager = GameObject.Find("MoneyDisplay").GetComponent<MoneyManager>();

        topPathButtonText = topPathButton.GetComponentInChildren<TextMeshProUGUI>();
        middlePathButtonText = middlePathButton.GetComponentInChildren<TextMeshProUGUI>();
        bottomPathButtonText = bottomPathButton.GetComponentInChildren<TextMeshProUGUI>();
        panelRectTransform = GetComponent<RectTransform>();
    }


    public void ShowPanel(int left)
    {
        if(left == 1)
        {
            panelRectTransform.anchoredPosition = new Vector2(placePanelController.leftX, placePanelController.panelY);
        }
        else if(left == 0)
        {
            panelRectTransform.anchoredPosition = new Vector2(placePanelController.rightX, placePanelController.panelY);
        }
        
        gameObject.SetActive(true);
        
    }


    public void HidePanel()
    {
        gameObject.SetActive(false);
    }



    public void UpdateTargeting(TowerData.TargetingOptions targeting)
    {
        targetingText.text = targeting.ToString();
    }


    public void UpdateDmgCount()
    {
        dmgCount.text = towerData.dmgCount+" dmg done";
    }



    public void UpdateData(GameObject tower)
    {
        towerData = tower.GetComponent<TowerData>();
        if(towerData.towerName == "Empty")
        {
            return;
        }


        Texture2D towerTexture = Resources.Load<Texture2D>(
                "Textures/"+towerData.towerName+"/"+towerData.towerName+"_"
                +towerData.upgrades[0]+towerData.upgrades[1]+towerData.upgrades[2]);

        if (towerTexture == null)
        {
            Debug.LogError("texture for "+towerData.towerName+"_"
            +towerData.upgrades[0]+towerData.upgrades[1]+towerData.upgrades[2]+" not found");               
        }
        else
        {
            towerImage.texture = towerTexture;
        }


        // top
        towerTexture = Resources.Load<Texture2D>(
                "Textures/"+towerData.towerName+"/"+towerData.towerName+"_"
                +(towerData.upgrades[0]+1)+towerData.upgrades[1]+towerData.upgrades[2]);

        if (towerTexture == null && towerData.upgrades[0] < 7)
        {
            if(towerData.upgrades[0] < 6 && towerData.upgrades.Sum() < 10)
            {
                Debug.LogError("texture for "+towerData.towerName+"_"
                    +(towerData.upgrades[0]+1)+towerData.upgrades[1]+towerData.upgrades[2]+" not found");
            }     
        }
        else
        {
            topPathImage.texture = towerTexture;
        }



        // mid
        towerTexture = Resources.Load<Texture2D>(
                "Textures/"+towerData.towerName+"/"+towerData.towerName+"_"
                +towerData.upgrades[0]+(towerData.upgrades[1]+1)+towerData.upgrades[2]);

        if (towerTexture == null)
        {
            if(towerData.upgrades[1] < 6 && towerData.upgrades.Sum() < 10)
            {
                Debug.LogError("texture for "+towerData.towerName+"_"
                    +towerData.upgrades[0]+(towerData.upgrades[1]+1)+towerData.upgrades[2]+" not found");
            }
        }
        else
        {
            middlePathImage.texture = towerTexture;
        }



        // bot
        towerTexture = Resources.Load<Texture2D>(
                "Textures/"+towerData.towerName+"/"+towerData.towerName+"_"
                +towerData.upgrades[0]+towerData.upgrades[1]+(towerData.upgrades[2]+1));

        if (towerTexture == null && towerData.upgrades[2] < 7)
        {
            if(towerData.upgrades[2] < 6 && towerData.upgrades.Sum() < 10)
            {
                Debug.LogError("texture for "+towerData.towerName+"_"
                    +towerData.upgrades[0]+towerData.upgrades[1]+(towerData.upgrades[2]+1)+" not found");
            }        
        }
        else
        {
            bottomPathImage.texture = towerTexture;
        }
        



        towerText.text = towerData.towerName+" Tower "+towerData.upgrades[0]+towerData.upgrades[1]+towerData.upgrades[2];

        int numberToLookUp = upgradesData.MapUpgradesToNumber(new int[] {towerData.upgrades[0]+1,0,0});
        topPathDesc.text = upgradesData.UpgradeDescriptions[towerData.towerName][numberToLookUp].ToString();

        numberToLookUp = upgradesData.MapUpgradesToNumber(new int[] {0,towerData.upgrades[1]+1,0});
        middlePathDesc.text = upgradesData.UpgradeDescriptions[towerData.towerName][numberToLookUp].ToString();

        numberToLookUp = upgradesData.MapUpgradesToNumber(new int[] {0,0,towerData.upgrades[2]+1});
        bottomPathDesc.text = upgradesData.UpgradeDescriptions[towerData.towerName][numberToLookUp].ToString();



        int cost = (int)upgradesData.UpgradeCosts[towerData.towerName]
            [upgradesData.MapUpgradesToNumber(new int[] {towerData.upgrades[0]+1,0,0})];
        topPathButtonText.text = cost.ToString()+"$\nupgrade";
        topPathButton.GetComponent<ButtonColorController>().upgradeCost = cost;

        cost = (int)upgradesData.UpgradeCosts[towerData.towerName]
            [upgradesData.MapUpgradesToNumber(new int[] {0,towerData.upgrades[1]+1,0})];
        middlePathButtonText.text = cost.ToString()+"$\nupgrade";
        middlePathButton.GetComponent<ButtonColorController>().upgradeCost = cost;

        cost = (int)upgradesData.UpgradeCosts[towerData.towerName]
            [upgradesData.MapUpgradesToNumber(new int[] {0,0,towerData.upgrades[2]+1})];
        bottomPathButtonText.text = cost.ToString()+"$\nupgrade";
        bottomPathButton.GetComponent<ButtonColorController>().upgradeCost = cost;

        cost = (int)(towerData.moneySpent*moneyManager.sellMultiplier);
        sellingCost.text = cost.ToString()+"$";
        



        if(towerData.upgrades.Sum() >= 10)
        {
            topPathImage.gameObject.SetActive(false);
            topPathButton.gameObject.SetActive(false);
            topPathDesc.text = "Max Upgrades Reached";
            middlePathImage.gameObject.SetActive(false);
            middlePathButton.gameObject.SetActive(false);
            middlePathDesc.text = "Max Upgrades Reached";
            bottomPathImage.gameObject.SetActive(false);
            bottomPathButton.gameObject.SetActive(false);
            bottomPathDesc.text = "Max Upgrades Reached";
        }
        else 
        {
            if(towerData.upgrades[0] >= 6)
            {
                topPathImage.gameObject.SetActive(false);
                topPathButton.gameObject.SetActive(false);
                topPathDesc.text = "Max Level Reached";
            }
            else
            {
                topPathImage.gameObject.SetActive(true);
                topPathButton.gameObject.SetActive(true);
            }

            if(towerData.upgrades[1] >= 6)
            {
                middlePathImage.gameObject.SetActive(false);
                middlePathButton.gameObject.SetActive(false);
                middlePathDesc.text = "Max Level Reached";
            }
            else
            {
                middlePathImage.gameObject.SetActive(true);
                middlePathButton.gameObject.SetActive(true);
            }

            if(towerData.upgrades[2] >= 6)
            {
                bottomPathImage.gameObject.SetActive(false);
                bottomPathButton.gameObject.SetActive(false);
                bottomPathDesc.text = "Max Level Reached";
            }
            else
            {
                bottomPathImage.gameObject.SetActive(true);
                bottomPathButton.gameObject.SetActive(true);
            }
        }
    }


    public void OnClickTopPath()
    {
        TowerUpgrade towerUpgrade = selectionData.selectedTower.GetComponent<TowerUpgrade>();
        towerUpgrade.CheckUpgradeCondition(0);
    }


    public void OnClickMiddlePath()
    {
        TowerUpgrade towerUpgrade = selectionData.selectedTower.GetComponent<TowerUpgrade>();
        towerUpgrade.CheckUpgradeCondition(1);
    }


    public void OnClickBottomPath()
    {
        TowerUpgrade towerUpgrade = selectionData.selectedTower.GetComponent<TowerUpgrade>();
        towerUpgrade.CheckUpgradeCondition(2);
    }


    public void OnClickSell()
    {
        TowerSell towerSell = selectionData.selectedTower.GetComponent<TowerSell>();
        towerSell.SellTower();
    }
}
