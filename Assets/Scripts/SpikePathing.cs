using System.Collections.Generic;
using UnityEngine;

public class SpikePathing : MonoBehaviour
{
    private TowerData towerData;
    private int projDamage;
    private int projPierce;
    private float maxDistance;
    
    private Vector3 startPosition;
    private int currentPierceCount = 0;
    private HashSet<EnemyData> hitEnemies = new HashSet<EnemyData>();
    private Vector3 moveDirection;



    public void InitSpike(GameObject sourceTower, int damage, int pierce, float maxTravelDistance, Vector3 direction)
    {
        towerData = sourceTower.GetComponent<TowerData>();
        projDamage = damage;
        projPierce = pierce;
        maxDistance = maxTravelDistance;
        startPosition = transform.position;
        moveDirection = direction.normalized;
    }

    void Update()
    {
        transform.position += moveDirection * towerData.projectileSpeed * Time.deltaTime;

        if (moveDirection != Vector3.zero)
        {
            Quaternion lookHeading = Quaternion.LookRotation(moveDirection);
            transform.rotation = lookHeading * Quaternion.Euler(90f, 0f, 0f);
        }


        if (Vector3.Distance(startPosition, transform.position) >= maxDistance)
        {
            Destroy(gameObject);
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        int otherLayerMask = 1 << other.gameObject.layer;

        if (other.CompareTag("Enemy") || (towerData.gameObject.GetComponent<TowerAttack>(
            ).enemyLayer.value & otherLayerMask) != 0)
        {
            EnemyData enemy = other.GetComponent<EnemyData>();
            if (enemy == null) enemy = other.GetComponentInParent<EnemyData>();

            if (enemy != null && !hitEnemies.Contains(enemy))
            {
                if (currentPierceCount >= projPierce)
                {
                    Destroy(gameObject);
                    return;
                }

                hitEnemies.Add(enemy);

                if (projDamage > enemy.CurrentHealth)
                {
                    towerData.dmgCount += enemy.CurrentHealth;
                }
                else
                {
                    towerData.dmgCount += projDamage;
                }

                enemy.TakeDamage(projDamage);
                currentPierceCount++;


                if (currentPierceCount >= projPierce)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}
