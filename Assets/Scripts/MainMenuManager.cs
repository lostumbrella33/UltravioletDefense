using System.Drawing;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject levelSelectPanel;
    [SerializeField] private GameObject levelLoadOptionsPanel;
    [SerializeField] private GameObject cheatsConfigPanel;
    
    [SerializeField] private TMP_InputField waveInputField;
    [SerializeField] private TMP_InputField moneyInputField;

    [SerializeField] private TMP_Text[] levelButtonTexts;
    public static int maxWave = 300; // 1 indexed
    public static int maxStartingMoney = 100000000; // 100 million
    [SerializeField] private TextMeshProUGUI currentSaveInfo;
    [SerializeField] private NewGameConfirmPanelController newGameConfirmPanelController;
    [SerializeField] private TextMeshProUGUI[] highscoreTexts;
    [SerializeField] private SettingsPanelController settingsPanelController;
    [SerializeField] private ResetHighscoresConfirmPanelController resetHighscoresConfirmPanelController;
    


    void Awake()
    {
        if (levelSelectPanel != null)
        {
            levelSelectPanel.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        }

        if (levelLoadOptionsPanel != null)
        {
            levelLoadOptionsPanel.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        }

        if (cheatsConfigPanel != null)
        {
            cheatsConfigPanel.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        }

        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
        if (levelSelectPanel != null) levelSelectPanel.gameObject.SetActive(false);
        if (levelLoadOptionsPanel != null) levelLoadOptionsPanel.gameObject.SetActive(false);
        if (cheatsConfigPanel != null) cheatsConfigPanel.gameObject.SetActive(false);
    }


    void Start()
    {
        LoadLevelNames(); // reading level names from resources folder to make sure the buttons in level select screen know what to display
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (cheatsConfigPanel.activeSelf)
            {
                CloseCheatsConfigMenu();
            }
            else if (levelLoadOptionsPanel.activeSelf)
            {
                CloseLevelLoadMenu();
            }
            else if (levelSelectPanel.activeSelf)
            {
                OpenMainMenu();
                CloseLevelSelectMenu();
            }
        }
        
    }



    // open and close for all panels

    public void OpenMainMenu()
    {
        mainMenuPanel.SetActive(true);
    }


    public void CloseMainMenu()
    {
        mainMenuPanel.SetActive(false);
    }


    public void OpenLevelSelectMenu()
    {
        levelSelectPanel.SetActive(true);
    }


    public void CloseLevelSelectMenu()
    {
        levelSelectPanel.SetActive(false);
    }

    
    public void OpenLevelLoadMenu()
    {
        levelLoadOptionsPanel.SetActive(true);
    }


    public void CloseLevelLoadMenu()
    {
        levelLoadOptionsPanel.SetActive(false);
    }


    public void OpenCheatsConfigMenu()
    {
        cheatsConfigPanel.SetActive(true);
    }


    public void CloseCheatsConfigMenu()
    {
        cheatsConfigPanel.SetActive(false);
    }

    

    // on click functions for all buttons

    public void OnClickPlay() // play in main menu
    {
        UpdateHighscores();
        OpenLevelSelectMenu();
        CloseMainMenu();
    }


    public void OnClickSettings() // settings in main menu
    {
        settingsPanelController.ShowPanel();
    }


    public void OnClickResetHighscore() // reset highscore in main menu
    {
        resetHighscoresConfirmPanelController.ShowPanel();
    }


    public void OnClickLevel(int levelIndex) // level in level select
    {
        string modeString;
        PlayerPrefs.SetInt("SelectedLevelIndex", levelIndex);

        if(!PlayerPrefs.HasKey("SavestateCurrentWaveLevel" + levelIndex))
        {
            currentSaveInfo.text = "No save found for this level";
        }
        else
        {
            int savedWave = PlayerPrefs.GetInt("SavestateCurrentWaveLevel" + levelIndex, 0);
            int cheatsOn = PlayerPrefs.GetInt("SavestateCheatsEnabledLevel" + levelIndex, 0);
            if(cheatsOn == 0)
            {
                modeString = "Normal";
            }
            else
            {
                modeString = "Cheats";
            }

            currentSaveInfo.text = $"Current save: {modeString} mode, Wave {savedWave + 1}";  // bc its 0 indexed
        }
        
        OpenLevelLoadMenu();
    }


    public void OnClickBackFromLevelLoad() // transparent fullscreen button in level load
    {
        CloseLevelLoadMenu();
    }


    public void OnClickNewGame() // new game in level load
    {
        newGameConfirmPanelController.ShowPanel();
        newGameConfirmPanelController.cheats = false;
    }


    public void OnClickContinue() // continue in level load
    {
        LaunchLevel(cheatMode: false);
    }


    public void OnClickCheatsMode() // cheats mode in level load
    {
        OpenCheatsConfigMenu();
        CloseLevelLoadMenu();
    }


    public void OnClickBackFromCheatsConfig() // transparent fullscreen button in cheats config
    {
        OpenLevelLoadMenu();
        CloseCheatsConfigMenu();
    }


    public void OnClickStartGameCheat() // start game in cheats config
    {
        newGameConfirmPanelController.ShowPanel();
        newGameConfirmPanelController.cheats = true;
    }

    
    private void LaunchLevel(bool cheatMode)
    {
        int levelIndex = PlayerPrefs.GetInt("SelectedLevelIndex", 0);
        if (!cheatMode)
        {
            PlayerPrefs.SetInt("SavestateCheatsEnabledLevel" + levelIndex, 0);
        }
        
        PlayerPrefs.Save();

        SceneManager.LoadScene("LevelScene");
    }


    private void LoadLevelNames()
    {
        LevelData[] rawLevels = Resources.LoadAll<LevelData>("LevelsData");
        LevelData[] sortedLevels = rawLevels.OrderBy(level => level.levelIndex).ToArray();

        for (int i = 0; i < levelButtonTexts.Length; i++)
        {
            if (i < sortedLevels.Length && sortedLevels[i] != null)
            {
                levelButtonTexts[i].text = sortedLevels[i].levelName;
            }
            else
            {
                levelButtonTexts[i].text = $"Level {i + 1}"; 
            }
        }
    }


    public void StartNewGame()
    {
        int currentLevel = PlayerPrefs.GetInt("SelectedLevelIndex", 0);
        PlayerPrefsManager.ClearLevelSavestate(currentLevel);

        LaunchLevel(cheatMode: false);
    }


    public void StartNewGameCheats()
    {
        int currentLevel = PlayerPrefs.GetInt("SelectedLevelIndex", 0);
        PlayerPrefsManager.ClearLevelSavestate(currentLevel);

        int startingWave; // 0 indexed, but input field is 1 indexed
        if (int.TryParse(waveInputField.text, out int r))
        {
            if(r < 1)
            {
                startingWave = 0;
            }
            else if(r > maxWave)
            {
                startingWave = maxWave - 1;
            }
            else
            {
                startingWave = r - 1;
            }
        }
        else
        {
            startingWave = 0;
        }

        
        int startingMoney;
        if (int.TryParse(moneyInputField.text, out int m))
        {
            if(m < 0)
            {
                startingMoney = 0;
            }
            else if(m > maxStartingMoney)
            {
                startingMoney = maxStartingMoney;
            }
            else
            {
                startingMoney = m;
            }
        }
        else
        {
            startingMoney = 0;
        }

        PlayerPrefs.SetInt("SavestateCheatsEnabledLevel" + currentLevel, 1);
        PlayerPrefs.SetInt("SavestateCheatStartingWaveLevel" + currentLevel, startingWave);
        PlayerPrefs.SetInt("SavestateCheatStartingMoneyLevel" + currentLevel, startingMoney);        
        PlayerPrefs.Save();

        LaunchLevel(cheatMode: true);
    }


    public void UpdateHighscores()
    {
        for(int i = 0; i < highscoreTexts.Length; i++)
        {
            int currentHighscore = PlayerPrefs.GetInt("HighscoreLevel" + i, -1) + 1; // this number is 0 indexed but its also the wave 
            // that is about to start, aka 1 higher than the highest beaten wave which should be the highscore, so the -1 and +1 cancel out
            highscoreTexts[i].text = currentHighscore.ToString();
        }
    }
}
