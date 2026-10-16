using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using UnityEngine;

public class TowerAttack : MonoBehaviour
{
    private float nextAttackTime = 0f;
    private List<GameObject> targets;
    public enum TargetingPriority
    {
        First,Last,Strong,Fast
    }
    private CheckEnemiesInRange checkEnemiesInRange;
    private TowerData towerData;

    private Vector3 projectileSpawnpoint;
    public LayerMask enemyLayer;
    private LineRenderer lineRenderer;

    public Material laserMaterial;
    public Color laserColor = Color.purple;
    public float laserFade = 0.15f;

    public Color lightningFirst; 
    public Color lightningSecond = Color.white;

    private Dictionary<EnemyData, int> consecutiveHitsTracker = new Dictionary<EnemyData, int>();
    private HashSet<EnemyData> enemiesHitThisAttack = new HashSet<EnemyData>();

    
    

    void Start() 
    {
        checkEnemiesInRange = GetComponentInChildren<CheckEnemiesInRange>();
        towerData = gameObject.GetComponent<TowerData>();
        projectileSpawnpoint = gameObject.transform.position;
    }


    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }
        lineRenderer.useWorldSpace = true;

        if (laserMaterial != null)
        {
            lineRenderer.material = laserMaterial;
        }
        
        lineRenderer.positionCount = 0;
    }
    
    

    void Update()
    {

        if(towerData.towerName != "Empty")
        {
            if(Time.time >= nextAttackTime)
            {
                if(checkEnemiesInRange.enemiesInRange.Count > 0)
                {
                    Attack();
                    nextAttackTime = Time.time + towerData.currentAttackCD;
                }
                else
                {
                    lineRenderer.positionCount = 0;
                }        
            }
        }
    }

    void Attack()
    {
        targets = PickTarget(checkEnemiesInRange.enemiesInRange);

        if(targets.Count > 0 && targets[0] != null)
        {

            // lazer
            if(towerData.towerName == "Lazer") 
            {
                enemiesHitThisAttack.Clear();

                foreach(GameObject target in targets)
                {
                    ShootLazer(target.transform);
                }
            }



            // stellar
            else if(towerData.towerName == "Stellar") 
            {
                GameObject mainTarget = targets[0];
                Vector3 origin = projectileSpawnpoint;
                
                Vector3 baseDirection = (mainTarget.transform.position - origin).normalized;
                float targetDistance = Vector3.Distance(origin, mainTarget.transform.position);

                int bombCount = towerData.multiTarget;
                float angleStep = 360f / bombCount;


                for (int i = 0; i < bombCount; i++)
                {
                    float currentAngle = i * angleStep;
                    
                    Vector3 uniqueDirection = Quaternion.Euler(0, 0, currentAngle) * baseDirection;
                    Quaternion lookRotation = Quaternion.LookRotation(uniqueDirection);

                    GameObject projectile = Instantiate(towerData.projectilePrefab, origin, lookRotation);
                    projectile.transform.localScale = Vector3.one * towerData.projectileScale;

                    ProjectilePathing projScript = projectile.GetComponent<ProjectilePathing>();
                    projScript.Init(gameObject, mainTarget);
                    projScript.SetupExplosion(towerData.explosionRadius, towerData.pierce, towerData.damage);

                    if (i == 0)
                    {
                        projScript.SetMovementType(true, Vector3.zero);
                    }
                    else
                    {
                        Vector3 targetDestinationPoint = origin + (uniqueDirection * targetDistance);
                        projScript.SetMovementType(false, targetDestinationPoint);
                    }
                    
                }
            }



            // ice
            else if(towerData.towerName == "Ice") 
            {
                GameObject target = targets[0];
                Vector3 origin = projectileSpawnpoint;
                
                Vector3 direction = (target.transform.position - origin).normalized;
                Quaternion lookRotation = Quaternion.LookRotation(direction);

                GameObject projectile = Instantiate(towerData.projectilePrefab, origin, lookRotation);
                projectile.transform.localScale = Vector3.one * towerData.projectileScale;

                ProjectilePathing projScript = projectile.GetComponent<ProjectilePathing>();
                projScript.Init(gameObject, target);
                projScript.SetupExplosion(towerData.explosionRadius, towerData.pierce, towerData.damage);
                projScript.SetMovementType(true, Vector3.zero);
            }



            // metal
            else if(towerData.towerName == "Metal")
            {
                Vector3 origin = projectileSpawnpoint;
                int spikeCount = towerData.multiTarget;
                float angleStep = spikeCount > 1 ? 360f / spikeCount : 0f;

                for (int i = 0; i < spikeCount; i++)
                {
                    float currentAngle = i * angleStep;
                    
                    Vector3 spikeDirection = Quaternion.Euler(0, 0, currentAngle) * Vector3.up;
                    Quaternion lookRotation = Quaternion.LookRotation(spikeDirection);

                    GameObject spike = Instantiate(towerData.projectilePrefab, origin, lookRotation);
                    spike.transform.localScale = Vector3.one * towerData.projectileScale;

                    SpikePathing spikeScript = spike.GetComponent<SpikePathing>();
                    if (spikeScript == null) spikeScript = spike.AddComponent<SpikePathing>();

                    if (spikeScript != null)
                    {
                        spikeScript.InitSpike(gameObject, towerData.damage, towerData.pierce, 
                            (towerData.range + towerData.overreach) * 0.1f, spikeDirection);
                    }
                }
            }



            // lightning
            else if(towerData.towerName == "Lightning")
            {
                enemiesHitThisAttack.Clear();
                GameObject firstTarget = targets[0];

                if (firstTarget != null)
                {
                    ExecuteLightningChain(firstTarget);
                }
            }
        }
        


        List<EnemyData> keysToReset = new List<EnemyData>();
        foreach (EnemyData enemy in consecutiveHitsTracker.Keys)
        {
            if (!enemiesHitThisAttack.Contains(enemy))
            {
                keysToReset.Add(enemy);
            }
        }
        foreach (EnemyData enemy in keysToReset)
        {
            consecutiveHitsTracker.Remove(enemy);
        }
        
    }


    List<GameObject> PickTarget(List<GameObject> enemiesInRange)
    {
        GameObject[] enemiesSorted = new GameObject[0];

        for (int i = enemiesInRange.Count - 1; i >= 0; i--)
        {
            if (enemiesInRange[i] == null)
            {
                enemiesInRange.RemoveAt(i);
            }
        }
        

        if(towerData.targeting == TowerData.TargetingOptions.First)
        {
            enemiesSorted = enemiesInRange.OrderByDescending(enemy => enemy.GetComponent<EnemyData>().distanceTravelled).ToArray();
        }

        else if(towerData.targeting == TowerData.TargetingOptions.Last)
        {
            enemiesSorted = enemiesInRange.OrderBy(enemy => enemy.GetComponent<EnemyData>().distanceTravelled).ToArray();
        }

        else if(towerData.targeting == TowerData.TargetingOptions.Strong)
        {
            enemiesSorted = enemiesInRange.OrderByDescending(enemy => enemy.GetComponent<EnemyData>().MaxHealth).
                                            ThenByDescending(enemy => enemy.GetComponent<EnemyData>().distanceTravelled).ToArray();
        }

        else if(towerData.targeting == TowerData.TargetingOptions.Fast)
        {
            enemiesSorted = enemiesInRange.OrderByDescending(enemy => enemy.GetComponent<EnemyData>().MovementSpeed).
                                            ThenByDescending(enemy => enemy.GetComponent<EnemyData>().distanceTravelled).ToArray();
        }

        return enemiesSorted.Take(towerData.multiTarget).ToList();
    }



    private void ExecuteLightningChain(GameObject firstEnemy)
    {
        Vector3 currentSourcePosition = projectileSpawnpoint;
        GameObject currentTarget = firstEnemy;

        int maxBounces = towerData.maxChainAmount; 
        float bounceRadius = towerData.chainDistance;

        HashSet<EnemyData> hitInThisChain = new HashSet<EnemyData>();


        for (int i = 0; i < maxBounces; i++)
        {
            if (currentTarget == null)
            {
                break;
            }

            EnemyData enemyData = currentTarget.GetComponent<EnemyData>();

            if(enemyData != null && !hitInThisChain.Contains(enemyData))
            {
                hitInThisChain.Add(enemyData);
                enemiesHitThisAttack.Add(enemyData);
 
                int damage = towerData.damage;
                if (damage > enemyData.CurrentHealth)
                {
                    towerData.dmgCount += enemyData.CurrentHealth;
                }
                else
                {
                    towerData.dmgCount += damage;
                }

                enemyData.TakeDamage(damage);


                // bottom path stun effect
                if (towerData.freezeDuration > 0 && towerData.freezeStrength > 0)
                {
                    LightningStunEffect stunComponent = currentTarget.GetComponent<LightningStunEffect>();
                    if(stunComponent == null)
                    {
                        stunComponent = currentTarget.AddComponent<LightningStunEffect>();
                    }
                    
                    stunComponent.ApplyStun(towerData.freezeDuration, towerData.freezeStrength);
                }

                bool flipColorPattern = i % 2 != 0;
                CreateVisualLightningSegment(currentSourcePosition, currentTarget.transform.position, flipColorPattern);
            
                currentSourcePosition = currentTarget.transform.position;
                currentTarget = FindNextChainTarget(currentSourcePosition, bounceRadius, hitInThisChain);
            }
            else
            {
                break;
            }
        }
    }


    private GameObject FindNextChainTarget(Vector3 sourcePos, float chainDistance, HashSet<EnemyData> alreadyHit)
    {
        Collider[] colliders = Physics.OverlapSphere(sourcePos, chainDistance, enemyLayer);
        GameObject closest = null;
        float minDistance = float.MaxValue;

        foreach (Collider col in colliders)
        {
            EnemyData enemy = col.GetComponent<EnemyData>();
            if (enemy == null) enemy = col.GetComponentInParent<EnemyData>();

            if (enemy != null && !alreadyHit.Contains(enemy))
            {
                float dist = Vector3.Distance(sourcePos, col.transform.position);
                if (dist < minDistance)
                {
                    minDistance = dist;
                    closest = col.gameObject;
                }
            }
        }
        return closest;
    }


    private void CreateVisualLightningSegment(Vector3 start, Vector3 end, bool flipPattern)
    {
        GameObject visualBeam = new GameObject("LightningSegment");
        visualBeam.transform.SetParent(transform);

        LineRenderer line = visualBeam.AddComponent<LineRenderer>();
        line.useWorldSpace = true;
        
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.startWidth = towerData.lazerWidth;
        line.endWidth = towerData.lazerWidth;

        Color startColor = flipPattern ? lightningFirst : lightningSecond;
        Color endColor = flipPattern ? lightningSecond : lightningFirst;

        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(startColor, 0.0f), new GradientColorKey(endColor, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(1.0f, 1.0f) }
        );
        line.colorGradient = gradient;

        StartCoroutine(FadeLazer(line, visualBeam, start, end));
    }


    void ShootLazer(Transform target)
    {
        HashSet<EnemyData> damagedByThisBeam = new HashSet<EnemyData>();
        int currentPierceCount = 0;

        float scaledRange = (towerData.range + towerData.overreach + 1f) * 0.1f;
        Vector3 origin = projectileSpawnpoint;
        Vector3 direction = (target.position - origin).normalized;
        Vector3 maxRangePoint = origin + (direction * scaledRange);

        RaycastHit[] hits = Physics.SphereCastAll(origin, towerData.lazerWidth * 0.5f, direction, scaledRange, enemyLayer);
        
        System.Array.Sort(hits, (a, b) => Vector3.Distance(origin, a.transform.position)
                .CompareTo(Vector3.Distance(origin, b.transform.position)));


        foreach (RaycastHit hit in hits)
        {
            if (currentPierceCount >= towerData.pierce) 
            {
                break; 
            }

            if ((enemyLayer.value & (1 << hit.collider.gameObject.layer)) != 0)
            {
                EnemyData enemy = hit.collider.GetComponent<EnemyData>();
                if (enemy != null && !damagedByThisBeam.Contains(enemy))
                {
                    ProcessEnemy(enemy, damagedByThisBeam);
                    currentPierceCount++;
                }
            }

            
        }


        GameObject temporaryBeam = new GameObject("LazerBeam");
        
        temporaryBeam.transform.SetParent(transform);
        temporaryBeam.transform.localScale = Vector3.one; 

        LineRenderer runtimeLine = temporaryBeam.AddComponent<LineRenderer>();
        runtimeLine.useWorldSpace = true;
        runtimeLine.material = laserMaterial;

        runtimeLine.startWidth = towerData.lazerWidth;
        runtimeLine.endWidth = towerData.lazerWidth;

        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(laserColor, 0.0f), new GradientColorKey(laserColor, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(laserColor.a, 0.0f), new GradientAlphaKey(laserColor.a, 1.0f) }
        );
        runtimeLine.colorGradient = gradient;


        StartCoroutine(FadeLazer(runtimeLine, temporaryBeam, origin, maxRangePoint));
    }



    void ProcessEnemy(EnemyData enemy, HashSet<EnemyData> damagedByThisBeam)
    {
        damagedByThisBeam.Add(enemy);
        enemiesHitThisAttack.Add(enemy);
        float rampedDamage = 0f;

        if (consecutiveHitsTracker.ContainsKey(enemy))
        {
            consecutiveHitsTracker[enemy]++;
            rampedDamage = consecutiveHitsTracker[enemy] * towerData.damageRamp;
            if(rampedDamage > towerData.rampLimit)
            {
                rampedDamage = towerData.rampLimit;
            }
        }
        else
        {
            consecutiveHitsTracker[enemy] = 0;
        }

        
        int finalDamage = (int)(towerData.damage + rampedDamage);

        if(finalDamage > enemy.CurrentHealth)
        {
            towerData.dmgCount += enemy.CurrentHealth;
        }
        else
        {
            towerData.dmgCount += finalDamage;
        }

        enemy.TakeDamage(finalDamage);
    }


    
    IEnumerator FadeLazer(LineRenderer line, GameObject beamObject, Vector3 start, Vector3 end)
    {
        line.positionCount = 2;
        line.SetPosition(0, start);
        line.SetPosition(1, end);

        float elapsed = 0f;
        while (elapsed < laserFade)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / laserFade;
            
            float currentWidth = Mathf.Lerp(towerData.lazerWidth, 0f, progress);
            line.startWidth = currentWidth;
            line.endWidth = currentWidth;

            yield return null;
        }

        Destroy(beamObject);
    }


}
