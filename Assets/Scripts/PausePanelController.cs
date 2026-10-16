using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PausePanelController : MonoBehaviour
{
    [SerializeField] private GameOverPanelController gameOverPanelController;
    [SerializeField] private RestartConfirmPanelController restartConfirmPanelController;

    private bool gamePaused = false;
    public bool GamePaused => gamePaused;

    [SerializeField] private TextMeshProUGUI currentWaveText;
    [SerializeField] private TextMeshProUGUI highscoreText;



    void Start()
    {
        HidePanel();
        GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
    }


    void Update()
    {
        if (gamePaused && !restartConfirmPanelController.isActiveAndEnabled && Input.GetKeyDown(KeyCode.Escape))
        {
            OnClickClose();
        }
    }


    public void Pause()
    {
        Time.timeScale = 0f;
        gamePaused = true;
    }


    public void Unpause()
    {
        Time.timeScale = 1f;
        gamePaused = false;
    }


    public void HidePanel()
    {
        gameObject.SetActive(false);
        Unpause();
    }


    public void ShowPanel()
    {
        UpdateCurrentWaveAndHighscoreText();
        gameObject.SetActive(true);
        Pause();
    }


    public void OnClickClose()
    {
        HidePanel();
        Unpause();
    }


    public void OnClickResume()
    {
        HidePanel();
        Unpause();
    }


    // theres a listener for the retry last button in spawn waves so we dont need to do anything with it here


    public void OnClickRestart()
    {
        restartConfirmPanelController.ShowPanel();
    }


    public void OnClickHome() // save and quit so it doesnt clear prefs
    {
        GoToMainMenu();
    }


    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
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