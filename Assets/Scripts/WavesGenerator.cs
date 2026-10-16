using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class WavesGenerator : MonoBehaviour
{
    [System.Serializable] public class SpawnGroup
    {
        public SpawnWaves.EnemyType enemyType;
        public SpawnWaves.EnemyColor enemyColor;
        public int count;
        public float start;
        public float interval;
    }

    [System.Serializable] public class WaveData // one wave
    {
        public List<SpawnGroup> spawnGroups = new List<SpawnGroup>();
    }

    [System.Serializable] public class MasterWaveDataWrapper // entire waves list
    {
        public List<WaveData> allWaves = new List<WaveData>();
    }

    [System.Serializable] public class PowersSaveWrapper
    {
        public List<float> allPowers = new List<float>();
    }

    private float initialPowerLevel = 200;
    private float powerLevelIncrement = 1200;
    private int maxGroupAmount = 50;
    private float maxGroupDuration = 20f;
    private float maxGroupInterval = 3f;
    private float skewedRandomExponent = 2f;

    private float[] strengthMultipliers = new float[] { 1f, 1.5f, 2f, 2f, 4f };
    private int[] colorUnlockedAtWave = new int[] { 0, 1, 2, 4, 6, 8, 11, 14, 17, 20, 24 };

    private int[] colorWeights = new int[] { 17,17,17,17,17, 17,17,17,17,17, 5 };
    private int[] typeWeights = new int[] { 4, 2, 1, 2, 1 };
    [SerializeField] private SpawnWaves spawnWaves;


    public List<List<SpawnGroup>> wavesList = new() {};
    public List<float> powerLevelsList = new() {};




    public void GenerateNewWavesAndPowerList()
    {
        int currentLevel = PlayerPrefs.GetInt("SelectedLevelIndex", 0);
        string saveKey = "SavestateWavesListLevel" + currentLevel;

        if (!PlayerPrefs.HasKey(saveKey)) // if theres no currently savestated list for this level generate a new one
        {
            for(int i = 0; i < MainMenuManager.maxWave; i++)
            {
                (List<SpawnGroup> wave, float power) = GenerateWave(i);
                wavesList.Add(wave);
                powerLevelsList.Add(power);
            }

            SaveWavesListToPlayerPrefs(wavesList);
            SavePowersListToPlayerPrefs(powerLevelsList);
        }
    }


    public (List<SpawnGroup> wave, float powerLevel) GenerateWave(int waveIndex)
    {
        int maxUnlockedIndex = GetHighestSpawnableColorIndex(waveIndex);
        float targetWavePower = initialPowerLevel + powerLevelIncrement * waveIndex;

        float currentWavePower = 0;
        List<SpawnGroup> generatedWave = new List<SpawnGroup>();

        int failedIterations = 0;
        while (true)
        {
            int chosenColorIndex = ChooseWeightedRandomIndex(colorWeights, maxUnlockedIndex);
            int chosenTypeIndex = ChooseWeightedRandomIndex(typeWeights, typeWeights.Length - 1);
            int chosenAmount = (int)ChooseSkewedRandomFloat(maxGroupAmount) + 1;
            
            float chosenStart = 0f;
            if(generatedWave.Count != 0)
            {
                chosenStart = ChooseSkewedRandomFloat(maxGroupDuration - 0.1f);
            }

            float chosenInterval = ChooseSkewedRandomFloat(maxGroupInterval);

            if(chosenStart + chosenInterval*chosenAmount <= maxGroupDuration)
            {
                int baseHp = spawnWaves.baseHpByColor[chosenColorIndex];
                float baseSpeed = spawnWaves.baseSpeedByColor[chosenColorIndex];
                float enemyPowerLevel = baseHp * baseSpeed * strengthMultipliers[chosenTypeIndex];
                float groupPowerLevel = enemyPowerLevel * chosenAmount;
                
                

                // if total power is still less after adding the newly generated group continue making more
                if(currentWavePower + groupPowerLevel < targetWavePower) 
                {
                    currentWavePower += groupPowerLevel;

                    generatedWave.Add(new() { enemyType = (SpawnWaves.EnemyType)chosenTypeIndex, enemyColor = (SpawnWaves.
                        EnemyColor)chosenColorIndex, count = chosenAmount, start = chosenStart, interval = chosenInterval});

                    failedIterations = 0;
                }
                
                else 
                // if it reached target power, remove enemies from the new group one by one until reaching breakpoint,
                // after which add the last one back and return entire wave 
                {
                    float tempGroupPower = groupPowerLevel; 

                    for(int i = 0; i < chosenAmount; i++)
                    {
                        chosenAmount -= 1;
                        tempGroupPower -= enemyPowerLevel;

                        if(currentWavePower + tempGroupPower < targetWavePower)
                        {
                            chosenAmount += 1;
                            tempGroupPower += enemyPowerLevel;

                            currentWavePower += tempGroupPower; 
                            
                            generatedWave.Add(new() { enemyType = (SpawnWaves.EnemyType)chosenTypeIndex, enemyColor = (SpawnWaves.
                                EnemyColor)chosenColorIndex, count = chosenAmount, start = chosenStart, interval = chosenInterval});

                            
                            return (CompactWave(generatedWave), currentWavePower);                   
                        }
                    }          
                }
            }
            
            failedIterations += 1;
            if(failedIterations >= 50)
            {
                Debug.Log($"Failed to generate wave {waveIndex + 1}, targeted power: {targetWavePower}, generated power: {currentWavePower}");
                return (generatedWave, currentWavePower);
            }
        }
    }


    // compacts the wave until theres no more timeframes where nothing spawns for more than max allowed interval (3 seconds rn)
    private List<SpawnGroup> CompactWave(List<SpawnGroup> wave) 
    {
        if (wave == null || wave.Count <= 1) return wave;
        int safetyCounter = 0;
        bool gapFound = true;

        while (gapFound && safetyCounter < 50)
        {
            safetyCounter++;
            gapFound = false;

            List<EnemyTimestamp> sortedTimeline = new List<EnemyTimestamp>();

            foreach (SpawnGroup group in wave)
            {
                for (int i = 0; i < group.count; i++)
                {
                    float spawnTime = group.start + (i * group.interval);
                    sortedTimeline.Add(new EnemyTimestamp { time = spawnTime, sourceGroup = group });
                }
            }
            sortedTimeline.Sort((a, b) => a.time.CompareTo(b.time));


            for (int i = 1; i < sortedTimeline.Count; i++)
            {
                float currentGap = sortedTimeline[i].time - sortedTimeline[i - 1].time;

                if (currentGap > 3.0f)
                {
                    SpawnGroup offendingGroup = sortedTimeline[i].sourceGroup;
                    float overshoot = currentGap - 3.0f;
                    offendingGroup.start -= overshoot;

                    if (offendingGroup.start < 0f)
                    {
                        offendingGroup.start = 0f;
                    }

                    gapFound = true;
                    break; 
                }
            }
        }

        return wave;
    }

    
    private struct EnemyTimestamp
    {
        public float time;
        public SpawnGroup sourceGroup;
    }


    public float ChooseSkewedRandomFloat(float givenNumber)
    {
        float uniformRoll = Random.value;
        float skewedFraction = Mathf.Pow(1f - uniformRoll, skewedRandomExponent);

        return skewedFraction * givenNumber;
    }


    public int ChooseWeightedRandomIndex(int[] weights, int maxUnlockedIndex)
    {
        int totalWeightSum = 0;
        int checkLimit = Mathf.Min(maxUnlockedIndex, weights.Length - 1);

        for (int i = 0; i <= checkLimit; i++)
        {
            totalWeightSum += weights[i];
        }

        int randomRoll = Random.Range(0, totalWeightSum);

        int cumulativeWeight = 0;
        for (int i = 0; i <= checkLimit; i++)
        {
            cumulativeWeight += weights[i];
            
            if (randomRoll < cumulativeWeight)
            {
                return i;
            }
        }

        return 0;
    }


    public int GetHighestSpawnableColorIndex(int currentWave)
    {
        int unlockedCount = 0;

        for (int i = 0; i < colorUnlockedAtWave.Length; i++)
        {
            if (currentWave >= colorUnlockedAtWave[i])
            {
                unlockedCount++;
            }
            else
            {
                break; 
            }
        }

        return unlockedCount - 1;
    }


    public void SaveWavesListToPlayerPrefs(List<List<SpawnGroup>> wavesList)
    {
        MasterWaveDataWrapper masterWrapper = new MasterWaveDataWrapper();

        foreach (List<SpawnGroup> wave in wavesList)
        {
            WaveData waveWrapper = new WaveData();
            waveWrapper.spawnGroups.AddRange(wave);
            masterWrapper.allWaves.Add(waveWrapper);
        }

        string wavesListJson = JsonUtility.ToJson(masterWrapper);

        int currentLevel = PlayerPrefs.GetInt("SelectedLevelIndex", 0);
        PlayerPrefs.SetString("SavestateWavesListLevel" + currentLevel, wavesListJson);
        PlayerPrefs.Save();
    }


    public void SavePowersListToPlayerPrefs(List<float> powersList)
    {
        PowersSaveWrapper wrapper = new PowersSaveWrapper();
        foreach (float power in powersList)
        {
           wrapper.allPowers.AddRange(powersList);
        }

        string powersListJson = JsonUtility.ToJson(wrapper);

        int currentLevel = PlayerPrefs.GetInt("SelectedLevelIndex", 0);
        PlayerPrefs.SetString("SavestatePowersListLevel" + currentLevel, powersListJson);
        PlayerPrefs.Save();
    }
}
