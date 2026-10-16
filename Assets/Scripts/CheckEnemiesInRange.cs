using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class CheckEnemiesInRange : MonoBehaviour
{
    public List<GameObject> enemiesInRange { get; private set; } = new List<GameObject>();


    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Enemy")) {
            enemiesInRange.Add(other.gameObject);
        }
    }

    void OnTriggerExit(Collider other) {
        if (other.CompareTag("Enemy")) {
            enemiesInRange.Remove(other.gameObject);
        }
    }

    
    void Update()
    {
        enemiesInRange.RemoveAll(item => item == null);
    }


    public void ClearEnemiesInRange()
    {
        enemiesInRange.Clear();
    }
}
