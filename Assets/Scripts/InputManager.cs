using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] private PausePanelController pausePanelController;



    void Update()
    {
        if (!pausePanelController.GamePaused && Input.GetKeyDown(KeyCode.Escape))
        {
            pausePanelController.ShowPanel();
        }
    }

}
