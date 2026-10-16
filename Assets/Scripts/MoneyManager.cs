using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class MoneyManager : MonoBehaviour
{
    public static MoneyManager Instance { get; private set; }
    public static int moneyCount { get; private set; }
    private int defaultStartingMoney = 2000;
    public int DefaultStartingMoney => defaultStartingMoney;

    public float moneyDropMultiplier = 10f;
    public readonly float sellMultiplier = 0.5f;
    [SerializeField] private SpawnWaves spawnWaves;
    public static event Action<int> OnMoneyChanged;
    



    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        int currentLevel = PlayerPrefs.GetInt("SelectedLevelIndex", 0);

        if(PlayerPrefs.GetInt("SavestateCheatsEnabledLevel" + currentLevel, 0) == 0)
        {
            moneyCount = defaultStartingMoney;
        }
    }


    void Start()
    {
        
    }


    void Update()
    {
        gameObject.GetComponent<TextMeshProUGUI>().text = moneyCount+"$";
    }


    public void ModifyAmount(int amount)
    {
        moneyCount += amount;
        OnMoneyChanged?.Invoke(moneyCount);
    }

    
    public void RestoreMoneySnapshot(int restoredMoney)
    {
        moneyCount = restoredMoney;
        OnMoneyChanged?.Invoke(moneyCount);
    }


    public static void ForceRefreshUI()
    {
        OnMoneyChanged?.Invoke(moneyCount);
    }
}
