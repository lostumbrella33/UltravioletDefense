using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartConfirmPanelController : MonoBehaviour
{
    

    
    void Start()
    {
        HidePanel();
        GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
    }


    void Update()
    {
        if (gameObject.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            OnClickClose();
        }
    }


    public void HidePanel()
    {
        gameObject.SetActive(false);
    }


    public void ShowPanel()
    {
        gameObject.SetActive(true);
    }


    public void OnClickClose()
    {
        HidePanel();
    }


    public void OnClickYes()
    {
        RestartLevel();
    }


    public void OnClickNo()
    {
        HidePanel();
    }


    public void RestartLevel()
    {
        int currentLevel = PlayerPrefs.GetInt("SelectedLevelIndex", 0);
        int cheats = PlayerPrefs.GetInt("SavestateCheatsEnabledLevel" + currentLevel, 0);

        if (cheats == 0)  // normal mode clear fully
        {
            PlayerPrefsManager.ClearLevelSavestate(currentLevel);
        }
        else // dont clear cheats bool and starting wave + money so that it reverts to those values
        {
            PlayerPrefsManager.ClearLevelSavestateWhenRestartingCheatsGame(currentLevel);
        }
        

        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }
}