using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ProjectilePathing : MonoBehaviour
{
    private GameObject target;
    private TowerData towerData;

    private float explosionRadius;
    private int projPierce;
    private int projDamage;
    private bool hasExploded = false;

    public float turnSpeed = 50f;
    public float detonationDistance = 0.6f;
    private bool isHoming = true;
    private Vector3 staticDestination;

    
    

    public void Init(GameObject tower, GameObject targetObj)
    {
        towerData = tower.GetComponent<TowerData>();
        target = targetObj;
    }


    public void SetMovementType(bool homing, Vector3 destinationIfStatic)
    {
        isHoming = homing;
        staticDestination = destinationIfStatic;
    }


    public void SetupExplosion(float radius, int pierce, int damage)
    {
        explosionRadius = radius;
        projPierce = pierce;
        projDamage = damage;
    }


    void Update()
    {
        if(isHoming) //main bomb attack, and also all ice attacks
        {
            if (target == null)
            {
                Explode();
                return;
            }

            float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);
            if (distanceToTarget <= detonationDistance) 
            {
                Explode();
                return;
            }

            Vector3 targetDirection = (target.transform.position - transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
            transform.Translate(Vector3.forward * towerData.projectileSpeed * Time.deltaTime, Space.Self);
        }

        else // secondary bombs
        {
            float distanceToDestination = Vector3.Distance(transform.position, staticDestination); 
            if (distanceToDestination <= detonationDistance)
            {
                Explode();
                return;
            }

            Vector3 moveDir = (staticDestination - transform.position).normalized;
            transform.position += moveDir * towerData.projectileSpeed * Time.deltaTime;
        }
    }

    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy") && !hasExploded)
        {
            Explode();
        }
    }


    private void Explode()
    {
        hasExploded = true;
        Vector3 explosionCenter = transform.position;

        Collider[] collidersInRadius = Physics.OverlapSphere(explosionCenter, explosionRadius);
        List<EnemyData> validEnemies = new List<EnemyData>();

        foreach (Collider col in collidersInRadius)
        {
            EnemyData enemy = col.GetComponent<EnemyData>();
            if (enemy == null) enemy = col.GetComponentInParent<EnemyData>();

            if (enemy != null && !validEnemies.Contains(enemy))
            {
                validEnemies.Add(enemy);
            }
        }

        validEnemies.Sort((a, b) => Vector3.Distance(explosionCenter, a.transform.position)
            .CompareTo(Vector3.Distance(explosionCenter, b.transform.position)));

        
        int currentPierceCount = 0;
        foreach (EnemyData enemy in validEnemies)
        {
            if (currentPierceCount >= projPierce)
            {
                break;
            }

            if (projDamage > enemy.CurrentHealth)
            {
                towerData.dmgCount += enemy.CurrentHealth;
            }
            else
            {
                towerData.dmgCount += projDamage;
            }

            enemy.TakeDamage(projDamage);


            // burn effect
            if (towerData.burnDuration > 0 && towerData.burnDamage > 0)
            {
                BurnEffect activeBurn = enemy.gameObject.GetComponent<BurnEffect>();
                
                if (activeBurn == null)
                {
                    activeBurn = enemy.gameObject.AddComponent<BurnEffect>();
                }

                activeBurn.ApplyBurn(towerData.burnDuration, towerData.burnDamage, towerData);
            }


            // freeze effect for ice towers 
            if (towerData.freezeDuration > 0 && towerData.freezeStrength > 0)
            {
                FreezeEffect freeze = enemy.gameObject.GetComponent<FreezeEffect>();
                if (freeze == null) freeze = enemy.gameObject.AddComponent<FreezeEffect>();
                
                freeze.ApplyAttackSlow(towerData.freezeDuration, towerData.freezeStrength);
            }

            currentPierceCount++;
        }

        Destroy(gameObject);
    }

}
