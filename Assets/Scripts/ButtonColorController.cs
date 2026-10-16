using UnityEngine;
using UnityEngine.UI;

public class ButtonColorController : MonoBehaviour
{
    public int upgradeCost;
    private Image buttonImage;

    public Color affordableColor = Color.green;
    public Color expensiveColor = Color.gray;


    void Start()
    {
        buttonImage = gameObject.GetComponent<Image>();
        UpdateButtonVisuals(MoneyManager.moneyCount);
    }



    private void OnEnable()
    {
        MoneyManager.OnMoneyChanged += UpdateButtonVisuals;
    }


    private void OnDisable()
    {
        MoneyManager.OnMoneyChanged -= UpdateButtonVisuals;
    }


    public void UpdateButtonVisuals(int currentMoney)
    {
        bool canAfford = currentMoney >= upgradeCost;
        if(buttonImage == null) buttonImage = gameObject.GetComponent<Image>();
        buttonImage.color = canAfford ? affordableColor : expensiveColor;
    }
}
