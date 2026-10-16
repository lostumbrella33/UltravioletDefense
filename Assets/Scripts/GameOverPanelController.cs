using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverPanelController : MonoBehaviour
{
    [SerializeField] private PausePanelController pausePanelController;
    [SerializeField] private RestartConfirmPanelController restartConfirmPanelController;
    [SerializeField] private GiveUpConfirmPanelController giveUpConfirmPanelController;
    [SerializeField] private TextMeshProUGUI currentWaveText;
    [SerializeField] private TextMeshProUGUI highscoreText;
    


    void Start()
    {
        HidePanel();
        
    }


    public void HidePanel()
    {
        gameObject.SetActive(false);
        pausePanelController.Unpause();
    }


    public void ShowPanel()
    {
        UpdateCurrentWaveAndHighscoreText();
        gameObject.SetActive(true);
        pausePanelController.Pause();
    }


    public void GameOverScreen()
    {
        ShowPanel();
    }


    // theres a listener for the retry last button in spawn waves so we dont need to do anything with it here


    public void OnClickRestart()
    {
        restartConfirmPanelController.ShowPanel();
    }


    public void OnClickHome() // hard quit so it must clear prefs
    {
        giveUpConfirmPanelController.ShowPanel();
    }


    public void UpdateCurrentWaveAndHighscoreText()
    {
        int currentLevel = PlayerPrefs.GetInt("SelectedLevelIndex", 0);
        int currentWave = PlayerPrefs.GetInt("SavestateCurrentWaveLevel" + currentLevel, 0);
        int currentHighscore = PlayerPrefs.GetInt("HighscoreLevel" + currentLevel, -1);
        currentWaveText.text = $"Current Wave: {currentWave + 1}";
        highscoreText.text = $"Highscore: {currentHighscore + 1}"; 
    }
}