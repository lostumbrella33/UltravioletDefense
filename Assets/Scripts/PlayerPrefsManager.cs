using UnityEngine;

public static class PlayerPrefsManager
{
    public static void ClearLevelSavestate(int levelIndex)
    {
        PlayerPrefs.DeleteKey("SavestateTowersLevel" + levelIndex);
        PlayerPrefs.DeleteKey("SavestateWavesListLevel" + levelIndex);
        PlayerPrefs.DeleteKey("SavestatePowersListLevel" + levelIndex);
        PlayerPrefs.DeleteKey("SavestateCheatsEnabledLevel" + levelIndex);
        PlayerPrefs.DeleteKey("SavestateCheatStartingWaveLevel" + levelIndex);
        PlayerPrefs.DeleteKey("SavestateCheatStartingMoneyLevel" + levelIndex);
        PlayerPrefs.DeleteKey("SavestateCurrentWaveLevel" + levelIndex);
        PlayerPrefs.DeleteKey("SavestateCurrentMoneyLevel" + levelIndex);
        PlayerPrefs.Save();
        
        Debug.Log($"[prefs manager] cleared savestate for level {levelIndex}");
    }


    public static void ClearLevelSavestateWhenRestartingCheatsGame(int levelIndex)
    {
        PlayerPrefs.DeleteKey("SavestateTowersLevel" + levelIndex);
        PlayerPrefs.DeleteKey("SavestateWavesListLevel" + levelIndex);
        PlayerPrefs.DeleteKey("SavestatePowersListLevel" + levelIndex);
        PlayerPrefs.DeleteKey("SavestateCurrentWaveLevel" + levelIndex);
        PlayerPrefs.DeleteKey("SavestateCurrentMoneyLevel" + levelIndex);
        PlayerPrefs.Save();
        
        Debug.Log($"[prefs manager] cleared savestate for level {levelIndex} except cheats mode starting wave and money");
    }


    public static void ResetHighscores()
    {
        int currentIndex = 0;
        while (PlayerPrefs.HasKey("HighscoreLevel" + currentIndex))
        {
            PlayerPrefs.DeleteKey("HighscoreLevel" + currentIndex);
            currentIndex += 1;
        }
    }

    
    public static void ClearAllGameData()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("[prefs manager] fully resetted playerprefs");
    }
}
