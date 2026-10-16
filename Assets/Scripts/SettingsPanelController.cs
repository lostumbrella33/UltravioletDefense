using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingsPanelController : MonoBehaviour
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
}