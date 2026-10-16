using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class SpawnWaves : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI waveDisplay;
    public int currentWaveIndex = 0;
    [SerializeField] private int enemiesAlive = 0;
    private List<bool> isSpawning = new() {false};
    public static SpawnWaves Instance {get; private set;}
    [SerializeField] private Button retryLastButtonGameOver;
    [SerializeField] private Button retryLastButtonPause;

    private Coroutine waveCoroutine;
    [SerializeField] private MoneyManager moneyManager;
    [SerializeField] private SelectionData selectionData;


    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private Material[] enemyMaterials;

    public enum EnemyType { Ball, Star, Dart, Cube, Blimp }  //  1,  1/2,  1/5,  4,    20    hp multipliers
                                                             //  1,  2,    5,    1/2,  1/5   speed multipliers
    public enum EnemyColor {Red, Orange, Yellow, Green, Cyan, Blue, Purple, Pink, Chrome, Silver, Antimatter }

    public readonly int[] baseHpByColor = new int[] {1, 2, 3, 5, 8, 12, 18, 27, 41, 62, 27};
    public readonly float[] baseSpeedByColor = new float[] {5, 10, 15, 20, 25, 30, 35, 40, 45, 50, 80};

    public readonly float[] hpMultiplierByType = new float[] { 1f, 0.5f, 0.25f, 4f, 20f };
    public readonly float[] speedMultiplierByType = new float[] { 1f, 2f, 4f, 0.5f, 0.2f };

    private bool isWaveActive = false;
    private bool requiresManualStart = true;
    private bool triggerNextWaveOnNextFrame = false;
    public Vector3[] enemyPath = new Vector3[0];
    [SerializeField] private GameOverPanelController gameOverPanelController;
    [SerializeField] private PausePanelController pausePanelController;
    [SerializeField] private LevelBuilder levelBuilder;
    public List<List<WavesGenerator.SpawnGroup>> currentWavesList;
    public List<float> currentPowersList;
    [SerializeField] private WavesGenerator wavesGenerator;




    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }


    public void Initiate()
    {
        int selectedLevel = PlayerPrefs.GetInt("SelectedLevelIndex", 0);

        if(PlayerPrefs.HasKey("SavestateCurrentWaveLevel" + selectedLevel)) // if there exists a savestate
        {
            RevertAll();
        }
        else // if there doesnt aka we just erased it bc we clicked new game
        {
            SnapshotAll();
        }
        
        retryLastButtonGameOver.onClick.AddListener(RetryLastWaveListener);
        retryLastButtonPause.onClick.AddListener(RetryLastWaveListener);

        // startfromadvancedwave is called in level builder

        requiresManualStart = true;
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (requiresManualStart && !isWaveActive && enemiesAlive == 0 && IsListFullyFalse(isSpawning))
            {
                SendWave();
            }
        }

    
        if (triggerNextWaveOnNextFrame)
        {
            triggerNextWaveOnNextFrame = false;
            SendWave();
        }
    }


    private void SendWave()
    {
        if (isWaveActive) return;

        if (currentWaveIndex >= currentWavesList.Count)
        {
            Debug.Log("!!! All waves beaten !!!");
            return;
        }

        isWaveActive = true;
        requiresManualStart = false;
        waveDisplay.text = $"Wave {currentWaveIndex + 1}";

        StartCoroutine(SpawnWaveRoutine());
    }


    // cheaty function to start a game from a higher round, auto acquiring all the money that you would get
    // if you did kill the enemies normally, mostly useful for showcase / testing purposes
    public int StartFromAdvancedWave(int targetWave) // targetWave(0 indexed) will be about to start after this
    {
        if (targetWave <= 1)
        {
            return 0;
        }


        int totalAddedMoney = 0;
        
        for(int waveIndex = 0; waveIndex < targetWave; waveIndex++)
        {

            List <WavesGenerator.SpawnGroup> currentWave = currentWavesList[waveIndex];

            for(int groupIndex = 0; groupIndex < currentWave.Count; groupIndex++)
            {
                WavesGenerator.SpawnGroup currentGroup = currentWave[groupIndex];
                int enemyHp = Mathf.CeilToInt(baseHpByColor[(int)currentGroup.enemyColor] * hpMultiplierByType[(int)currentGroup.enemyType]);
                
                int moneyGainedFromGroup = Mathf.CeilToInt(enemyHp * moneyManager.moneyDropMultiplier) * currentGroup.count;
                totalAddedMoney += moneyGainedFromGroup;
                }

            }

        
        this.currentWaveIndex = targetWave;

        waveDisplay.text = $"Wave {currentWaveIndex + 1} is about to start, press Space to send it";
        //SnapshotAll();
        requiresManualStart = true;

        Debug.LogFormat($"fast forwarded to wave: {targetWave + 1}, simulated extra money: {totalAddedMoney}");
        return totalAddedMoney;
    }



    private void SpawnEnemy(EnemyType chosenType, EnemyColor chosenColor)
    {
        if (enemyPath == null || enemyPath.Length == 0)
        {
            Debug.LogError("[spawn waves] tried to spawn enemy when path wasnt loaded");
            return;
        }


        int prefabIndex = (int)chosenType;
        GameObject prefabBlueprint = enemyPrefabs[prefabIndex];

        int baseHP = baseHpByColor[(int)chosenColor];
        float baseSpeed = baseSpeedByColor[(int)chosenColor];

        int hp = 0;
        float speed = 0;

        switch (chosenType)
        {
            case EnemyType.Ball:
                hp = baseHP;
                speed = baseSpeed;
                break;

            case EnemyType.Star:
                hp = Mathf.CeilToInt(baseHP * 0.5f);
                speed = baseSpeed * 2f;
                break;

            case EnemyType.Dart:
                hp = Mathf.CeilToInt(baseHP * 0.2f);
                speed = baseSpeed * 5f;
                break;

            case EnemyType.Cube:
                hp = baseHP * 4;
                speed = baseSpeed * 0.5f;
                break;

            case EnemyType.Blimp:
                hp = baseHP * 20;
                speed = baseSpeed * 0.2f;
                break;
        }


        Vector3 spawnpoint = enemyPath[0];
        GameObject liveEnemy = Instantiate(prefabBlueprint, spawnpoint, Quaternion.identity);

        EnemyData enemyData = liveEnemy.GetComponent<EnemyData>();
        if (enemyData != null)
        {
            Material chosenMaterial = enemyMaterials[Mathf.Clamp((int)chosenColor, 0, enemyMaterials.Length - 1)];
            enemyData.InitEnemy(chosenType, chosenColor, chosenMaterial, hp, speed);
        }
    }


    IEnumerator SpawnWaveRoutine()
    {

        Debug.Log($"Started wave {currentWaveIndex + 1}, Power level: {currentPowersList[currentWaveIndex]}");

        waveDisplay.text = $"Wave {currentWaveIndex + 1}";
        List<WavesGenerator.SpawnGroup> currentWave = currentWavesList[currentWaveIndex];


        while(isSpawning.Count < currentWave.Count)
        {
            isSpawning.Add(false);
        }

        List<Coroutine> activeGroups = new List<Coroutine>();
        for(int i = 0; i < currentWave.Count; i++)
        {
            WavesGenerator.SpawnGroup currentGroup = currentWave[i]; 
            Coroutine groupCoroutine = StartCoroutine(SpawnGroupRoutine(currentGroup, i));
            activeGroups.Add(groupCoroutine);
        }
        
        foreach(Coroutine group in activeGroups)
        {
            yield return group;
        }


        while (enemiesAlive > 0 || !IsListFullyFalse(isSpawning))
        {
            yield return null; 
        }

        Debug.Log($"Wave {currentWaveIndex + 1} cleared");

        currentWaveIndex++;
        SnapshotAll();

        
        isWaveActive = false;

        if (!requiresManualStart)
        {
            yield return new WaitForSeconds(0.5f); 
        
            triggerNextWaveOnNextFrame = true;
        }
        
    }
            

    IEnumerator SpawnGroupRoutine(WavesGenerator.SpawnGroup currentGroup, int index)
    {
        Debug.Log($"Spawning group: {currentGroup.enemyColor} {currentGroup.enemyType}, amount: {currentGroup.count}");

        isSpawning[index] = true;

        yield return new WaitForSeconds(currentGroup.start);
        for (int i = 0; i < currentGroup.count; i++)
        {
            SpawnEnemy(currentGroup.enemyType, currentGroup.enemyColor);
            enemiesAlive++;
            if(i < currentGroup.count - 1)
            {
                yield return new WaitForSeconds(currentGroup.interval);
            }
        }
        yield return null;

        isSpawning[index] = false;
    }


    public void OnEnemyKilled()
    {
        enemiesAlive--;
        if (enemiesAlive < 0)
        {
            enemiesAlive = 0;
        }
    }


    private void ClearActiveEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            EnemyData enemyData = enemy.GetComponent<EnemyData>();
            enemyData.willDropMoney = false;
            Destroy(enemy);
        }
    }


    bool IsListFullyFalse(List<bool> list)
    {
        foreach (bool value in list)
        {
            if (value) return false;
        }
        return true; 
    }


    public List<List<WavesGenerator.SpawnGroup>> LoadWavesFromPlayerPrefs()
    {
        int currentLevel = PlayerPrefs.GetInt("SelectedLevelIndex", 0);
        string saveKey = "SavestateWavesListLevel" + currentLevel;

        if (!PlayerPrefs.HasKey(saveKey))
        {
            Debug.LogWarning("no waves save data found");
            return new List<List<WavesGenerator.SpawnGroup>>();
        }

        string wavesJson = PlayerPrefs.GetString(saveKey);
        WavesGenerator.MasterWaveDataWrapper loadedMaster = JsonUtility.FromJson<WavesGenerator.MasterWaveDataWrapper>(wavesJson);

        List<List<WavesGenerator.SpawnGroup>> restoredWavesList = new List<List<WavesGenerator.SpawnGroup>>();

        foreach (WavesGenerator.WaveData waveData in loadedMaster.allWaves)
        {
            List<WavesGenerator.SpawnGroup> cleanWave = new List<WavesGenerator.SpawnGroup>(waveData.spawnGroups);
            restoredWavesList.Add(cleanWave);
        }

        return restoredWavesList;
    }


    public List<float> LoadPowersFromPlayerPrefs()
    {
        int currentLevel = PlayerPrefs.GetInt("SelectedLevelIndex", 0);
        string saveKey = "SavestatePowersListLevel" + currentLevel;

        if (!PlayerPrefs.HasKey(saveKey))
        {
            Debug.LogWarning("no powers save data found");
            return new List<float>();
        }

        string powersJson = PlayerPrefs.GetString(saveKey);
        WavesGenerator.PowersSaveWrapper loadedWrapper = JsonUtility.FromJson<WavesGenerator.PowersSaveWrapper>(powersJson);

        return new List<float>(loadedWrapper.allPowers);
    }




    // save and revert logic
    public void SnapshotAll()
    {
        LevelSaveDataWrapper masterSave = new LevelSaveDataWrapper();

        TowerData[] activeTowers = FindObjectsByType<TowerData>();

        foreach (TowerData tower in activeTowers)
        {
            string tempTowerJson = JsonUtility.ToJson(tower);

            TowerSaveData data = JsonUtility.FromJson<TowerSaveData>(tempTowerJson);

            masterSave.savedTowers.Add(data);
        }

        string masterJson = JsonUtility.ToJson(masterSave);

        int currentLevel = PlayerPrefs.GetInt("SelectedLevelIndex", 0);
        PlayerPrefs.SetString("SavestateTowersLevel" + currentLevel, masterJson);
        PlayerPrefs.SetInt("SavestateCurrentMoneyLevel" + currentLevel, MoneyManager.moneyCount);
        PlayerPrefs.SetInt("SavestateCurrentWaveLevel" + currentLevel, currentWaveIndex);

        int currentHighscore = PlayerPrefs.GetInt("HighscoreLevel" + currentLevel, -1);
        if(PlayerPrefs.GetInt("SavestateCheatsEnabledLevel" + currentLevel, 0) == 0 && currentWaveIndex - 1 > currentHighscore)
        {
            PlayerPrefs.SetInt("HighscoreLevel" + currentLevel, currentWaveIndex - 1);
        }

        PlayerPrefs.Save();
    }


    private void RevertAll() // this is used for retrying last wave
    {
        int currentLevel = PlayerPrefs.GetInt("SelectedLevelIndex", 0);
        string saveKey = "SavestateTowersLevel" + currentLevel;

        string masterJson = PlayerPrefs.GetString(saveKey);
        LevelSaveDataWrapper loadedData = JsonUtility.FromJson<LevelSaveDataWrapper>(masterJson);

        List<TowerData> activeTowersData = new List<TowerData>();
        foreach(GameObject tower in levelBuilder.ActiveTilesInScene)
        {
            activeTowersData.Add(tower.GetComponent<TowerData>());
        }


        foreach (TowerSaveData towerSaveData in loadedData.savedTowers)
        {
            TowerData matchingSceneTower = FindSceneTowerByIndex(activeTowersData, towerSaveData.towerId);

            if (matchingSceneTower != null)
            {
                string singleTowerJson = JsonUtility.ToJson(towerSaveData);
                JsonUtility.FromJsonOverwrite(singleTowerJson, matchingSceneTower);
                matchingSceneTower.UpdateLinks();
                matchingSceneTower.UpdateTexture();
                matchingSceneTower.UpdateRangeCircle();
                matchingSceneTower.ClearEnemiesInRange();
                matchingSceneTower.RecalculateCurrentCooldown();
            }        
        }

        int savestatedMoney = PlayerPrefs.GetInt("SavestateCurrentMoneyLevel" + currentLevel, 0);
        moneyManager.RestoreMoneySnapshot(savestatedMoney);

        currentWaveIndex = PlayerPrefs.GetInt("SavestateCurrentWaveLevel" + currentLevel, 0);
        waveDisplay.text = $"Wave {currentWaveIndex + 1} is about to start, press Space to send it";

        gameOverPanelController.HidePanel();
        pausePanelController.HidePanel();
    }


    private TowerData FindSceneTowerByIndex(List<TowerData> sceneTowers, int targetIndex)
    {
        foreach (TowerData tower in sceneTowers)
        {
            if (tower.towerId == targetIndex) 
            {
                return tower;
            }
        }
        return null; 
    }


    public void RetryLastWave()
    {

        if(waveCoroutine != null)
        {
            StopCoroutine(waveCoroutine);
            waveCoroutine = null;
        }

        StopAllCoroutines();
        ClearActiveEnemies();
        enemiesAlive = 0;
        for(int i = 0; i < isSpawning.Count; i++)
        {
            isSpawning[i] = false;
        }

        selectionData.ClearSelection();
        RevertAll();
        waveDisplay.text = $"Wave {currentWaveIndex + 1} is about to start, press Space to send it";

        CheckEnemiesInRange[] allCheckers = FindObjectsByType<CheckEnemiesInRange>();
        foreach (CheckEnemiesInRange checker in allCheckers)
        {
            if (checker.enemiesInRange != null) checker.enemiesInRange.Clear();
        }

        waveCoroutine = null;
    }


    private void RetryLastWaveListener()
    {
        requiresManualStart = true;
        isWaveActive = false;
        
        RetryLastWave();
    }

    
    

    


    public void SaveAllTowers()
    {
        
    }


    public void RestoreAllTowers()
    {
        
    }
}
