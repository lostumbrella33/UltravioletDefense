using System.Collections.Generic;
using UnityEngine;

public class LevelBuilder : MonoBehaviour
{
    public static LevelBuilder Instance { get; private set; }
    [SerializeField] private GameObject tilePrefab;

    private List<GameObject> activeTilesInScene = new List<GameObject>();
    public List<GameObject> ActiveTilesInScene => activeTilesInScene;
    private float tileCoordZ = -1;

    [SerializeField] private MoneyManager moneyManager;
    [SerializeField] private SpawnWaves spawnWaves;
    [SerializeField] private WavesGenerator wavesGenerator;



    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }


    void Start() // builds a level
    {
        int currentLevel = PlayerPrefs.GetInt("SelectedLevelIndex", 0);

        if(PlayerPrefs.HasKey("SavestateWavesListLevel" + currentLevel)) // if there exists a savestate
        {
            spawnWaves.currentWavesList = spawnWaves.LoadWavesFromPlayerPrefs();
            spawnWaves.currentPowersList = spawnWaves.LoadPowersFromPlayerPrefs();
        }
        else // if there doesnt
        {
            wavesGenerator.GenerateNewWavesAndPowerList();
            spawnWaves.currentWavesList = spawnWaves.LoadWavesFromPlayerPrefs();
            spawnWaves.currentPowersList = spawnWaves.LoadPowersFromPlayerPrefs();
        }


        if (PlayerPrefs.GetInt("SavestateCheatsEnabledLevel" + currentLevel, 0) == 1)
        {
            int startingWave = PlayerPrefs.GetInt("SavestateCheatStartingWaveLevel" + currentLevel, 1);
            int startingMoney = PlayerPrefs.GetInt("SavestateCheatStartingMoneyLevel" + currentLevel, 0);
            PlayerPrefs.Save();
            int naturallyGeneratedMoney = spawnWaves.StartFromAdvancedWave(startingWave);
            int finalMoney = startingMoney;

            if(startingMoney == 0)
            {
                finalMoney = moneyManager.DefaultStartingMoney + naturallyGeneratedMoney;
                PlayerPrefs.SetInt("SavestateCheatStartingMoneyLevel" + currentLevel, finalMoney);
                PlayerPrefs.Save();
            }

            PlayerPrefs.SetInt("SavestateCurrentMoneyLevel" + currentLevel, finalMoney);
            moneyManager.RestoreMoneySnapshot(finalMoney);
        }
        
        BuildLevel(currentLevel);
        spawnWaves.Initiate();
    }


    public void BuildLevel(int levelIndex)
    {
        LevelData[] loadedProfiles = Resources.LoadAll<LevelData>("LevelsData");
        LevelData targetLevel = null;

        foreach (LevelData data in loadedProfiles)
        {
            if (data != null && data.levelIndex == levelIndex)
            {
                targetLevel = data;
                break;
            }
        }

        if (targetLevel == null)
        {
            Debug.LogError($"[Level Builder] couldn't find leveldata file with index {levelIndex}");
            return;
        }



        foreach (GameObject tile in activeTilesInScene)
        {
            if (tile != null) Destroy(tile);
        }
        activeTilesInScene.Clear();

        if (tilePrefab != null && targetLevel.tileCoordinates != null)
        {
            for(int i = 0; i < targetLevel.tileCoordinates.Length; i++)
            {
                Vector3 tilePos = targetLevel.tileCoordinates[i];
                Vector3 correctedTilePos = new Vector3(tilePos.x, tilePos.y, tileCoordZ);

                GameObject spawnedTile = Instantiate(tilePrefab, correctedTilePos, Quaternion.identity, transform);
                activeTilesInScene.Add(spawnedTile);
                spawnedTile.GetComponent<TowerData>().towerId = i;
            }
        }


        if (SpawnWaves.Instance != null && targetLevel.enemyPathNodes != null && targetLevel.enemyPathNodes.Length > 0)
        {
            Vector3[] correctedPathNodes = new Vector3[targetLevel.enemyPathNodes.Length];

            for (int i = 0; i < targetLevel.enemyPathNodes.Length; i++)
            {
                Vector3 originalNode = targetLevel.enemyPathNodes[i];
                correctedPathNodes[i] = new Vector3(originalNode.x, originalNode.y, -1f);
            }

            SpawnWaves.Instance.enemyPath = correctedPathNodes;
        }
    }
}
