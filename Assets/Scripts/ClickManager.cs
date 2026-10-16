using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickManager : MonoBehaviour
{
    private Camera mainCamera;
    public SelectionData selectionData;
    private float screenCenterX;
    [SerializeField] private GameObject gameOverPanel;
    private int left;



    void Awake()
    {
        mainCamera = Camera.main; 
        screenCenterX = Screen.width / 2f;
    }


    void Update()
    {
        if (!gameOverPanel.activeInHierarchy && Input.GetMouseButtonDown(0)) 
        {
            if (EventSystem.current.IsPointerOverGameObject()) // prevent processing tower clicks if player clicks on a UI menu
            {
                return; 
            }

            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject.CompareTag("Tower"))
                {
                    if (Input.mousePosition.x < screenCenterX)
                    {
                        left = 0;
                    }
                    else
                    {
                        left = 1;
                    }

                    OnTowerClicked(hit.collider.gameObject,left);
                }
                else
                {
                    OnEmptyClicked();
                }
            }
        }
    }


    void OnTowerClicked(GameObject clickedTower, int left)
    {
        selectionData.ChangeSelected(clickedTower, left);
    }


    void OnEmptyClicked()
    {
        selectionData.ClearSelection();
    }
}
