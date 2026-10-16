using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyPathing : MonoBehaviour
{
    private Vector3[] enemyPath;
    private int targetIndex;
    private EnemyData enemyData;
    private GameObject uiCanvas;
    private GameOverPanelController gameOverPanelController;



    void Start()
    {
        enemyPath = SpawnWaves.Instance.enemyPath;
        if (enemyPath == null || enemyPath.Length == 0)
        {
            Debug.LogWarning("Enemy spawned before level path array was loaded");
            return;
        }

        targetIndex = 1;  // because index 0 is spawnpoint, 1 is the first target point
        enemyData = gameObject.GetComponent<EnemyData>();
        

        uiCanvas = GameObject.Find("UICanvas");
        Transform panelTransform = uiCanvas.transform.Find("GameOverPanel");
        GameObject panel = panelTransform.gameObject;
        gameOverPanelController = panel.GetComponent<GameOverPanelController>();
    } 


    void Update()
    {
        float step = enemyData.currentMovementSpeed * Time.deltaTime;
        Vector3 startPos = transform.position;


        // this is to make sure that blimp and dart always face towards where theyre moving
        Vector3 currentWaypointTarget = enemyPath[targetIndex];
        Vector3 travelDirection = (currentWaypointTarget - transform.position).normalized;

        Renderer enemyRenderer = GetComponentInChildren<Renderer>();
        if (enemyRenderer != null && travelDirection != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(travelDirection);

            if (enemyData.EnemyType == SpawnWaves.EnemyType.Blimp)
            {
                enemyRenderer.transform.rotation = lookRotation * Quaternion.Euler(90f, 180f, 0f);
            }

            else if (enemyData.EnemyType == SpawnWaves.EnemyType.Dart)
            {
                Quaternion trackRotation = Quaternion.LookRotation(travelDirection, Vector3.forward);
                enemyRenderer.transform.rotation = trackRotation * Quaternion.Euler(0f, 90f, 0f);
            }
        }


        // actual movement logic
        transform.position = Vector3.MoveTowards(transform.position, enemyPath[targetIndex], step);
        enemyData.distanceTravelled += Vector3.Distance(startPos,transform.position);

        if (Vector3.Distance(transform.position, enemyPath[targetIndex]) < 0.001f)
        {
            if(targetIndex == enemyPath.Length - 1)
            {
                gameOverPanelController.GameOverScreen();
            }
            else
            {
                targetIndex += 1;
            }
        }
    }
}
