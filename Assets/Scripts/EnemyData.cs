using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyData : MonoBehaviour
{
    [SerializeField] private SpawnWaves.EnemyType enemyType;
    [SerializeField] private SpawnWaves.EnemyColor enemyColor;
    [SerializeField] private float movementSpeed;
    [SerializeField] private int maxHealth;
    [SerializeField] private int currentHealth;

    public SpawnWaves.EnemyType EnemyType => enemyType;
    public SpawnWaves.EnemyColor EnemyColor => enemyColor;
    public float MovementSpeed => movementSpeed;
    public int MaxHealth => maxHealth; 
    public int CurrentHealth => currentHealth; 

    public float distanceTravelled = 0;
    public bool willDropMoney = true;
    public float currentMovementSpeed;

    private bool hasCountedDeath = false;
    private MoneyManager moneyManager;
    [SerializeField] private SpawnWaves spawnWaves;
    private float starSpinSpeed = -150f;
    private float cubeSpinSpeed = -80f;



    public void InitEnemy(SpawnWaves.EnemyType type, SpawnWaves.EnemyColor color, Material material, int hp, float speed)
    {
        enemyType = type;
        enemyColor = color;
        maxHealth = hp;
        movementSpeed = speed;
        currentHealth = maxHealth;

        Renderer renderer = GetComponent<Renderer>();
        if(renderer == null)
        {
            renderer = GetComponentInChildren<Renderer>();
        }

        if (renderer != null)
        {
            if(material != null)
            {
                renderer.material = material;
            }

            switch (type)
            {
                case SpawnWaves.EnemyType.Star:
                    renderer.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
                    break;

                case SpawnWaves.EnemyType.Dart:
                    renderer.transform.localRotation = Quaternion.Euler(90f, 180f, 0f);
                    break;

                case SpawnWaves.EnemyType.Blimp:
                    renderer.transform.localRotation = Quaternion.Euler(-15f, 0f, 90f);
                    break;

                default:
                    renderer.transform.localRotation = Quaternion.identity;
                    break;
            }
        }
    }
 

    void Awake()
    {
        currentHealth = maxHealth;
        currentMovementSpeed = movementSpeed;
        moneyManager = GameObject.Find("MoneyDisplay").GetComponent<MoneyManager>();
    }


    void Update()
    {
        Renderer enemyRenderer = GetComponentInChildren<Renderer>();
        if(enemyRenderer == null)
        {
            return;
        }

        if (enemyType == SpawnWaves.EnemyType.Star)
        {
            enemyRenderer.transform.Rotate(Vector3.up * starSpinSpeed * Time.deltaTime, Space.Self);
        }

        else if (enemyType == SpawnWaves.EnemyType.Cube)
        {
            Vector3 diagonalAxis = new Vector3(1f, 1f, 1f).normalized;
            enemyRenderer.transform.Rotate(diagonalAxis * cubeSpinSpeed * Time.deltaTime, Space.Self);
        }



        float totalModifier = 1f;

        FreezeEffect freeze = GetComponent<FreezeEffect>();
        if (freeze != null)
        {
            totalModifier *= freeze.GetCurrentFreezeModifier();
        }

        LightningStunEffect stun = GetComponent<LightningStunEffect>();
        if (stun != null)
        {
            totalModifier *= stun.GetCurrentStunModifier();
        }

        if(totalModifier < 0f)
        {
            totalModifier = 0f;
        }

        currentMovementSpeed = MovementSpeed * totalModifier;
    }


    public void ResetSpeed()
    {
        currentMovementSpeed = movementSpeed;
    }


    public void TakeDamage(int amount)
    {     
        currentHealth = Mathf.Clamp(currentHealth - amount, 0, maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    
    private void OnDestroy()
    {
        if (hasCountedDeath) return;
        hasCountedDeath = true;

        if (SpawnWaves.Instance != null)
        {
            SpawnWaves.Instance.OnEnemyKilled();
        }

        if(willDropMoney)
        {
            moneyManager.ModifyAmount(Mathf.CeilToInt(maxHealth * moneyManager.moneyDropMultiplier));
        }
    }

    
    public void Die()
    {
        Destroy(gameObject);
    }

}
