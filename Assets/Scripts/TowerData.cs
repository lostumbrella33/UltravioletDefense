using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using UnityEngine;

public class TowerData : MonoBehaviour
{
    public int towerId;
    public string towerName = "Empty";
    public int[] upgrades = {0,0,0};
    public int damage = 0;
    public float attackCD = 0f;
    public int pierce = 0;
    public float range = 0f;
    public float projectileSpeed = 0f;
    public GameObject projectilePrefab;
    public int multiTarget = 1;
    public float overreach = 0;
    public float lazerWidth = 0f;
    public int damageRamp = 0;
    public int rampLimit = 0;
    public float explosionRadius = 0f;
    public float projectileScale = 0.5f;
    public int burnDamage = 0;
    public float burnDuration = 0f;
    public float freezeStrength = 0f;
    public float freezeDuration = 0f;
    public int maxChainAmount = 0;
    public float chainDistance = 0f;
    public float attackCDBuff = 0f;
    public float currentAttackCD = 0f;
    private Dictionary<LightningBuffController, float> activeCooldownBuffs = new Dictionary<LightningBuffController, float>();


    public bool selected = false;
    public int dmgCount = 0;
    public int moneySpent = 0;
    private Renderer textureRenderer;
    public GameObject rangeCircle;
    private Transform rangeCircleTransform;
    private Renderer rangeCircleRenderer;
    private float rangeMultiplier = 0.1f;
    private CheckEnemiesInRange checkEnemiesInRange;

    public enum TargetingOptions 
    {
        First, Last, Strong, Fast
    }
    public TargetingOptions targeting = TargetingOptions.First;


    public static int totalTargetings = Enum.GetValues(typeof(TowerData.TargetingOptions)).Length;

    public static TargetingOptions NextTargeting(TargetingOptions x)
    {
        return (TargetingOptions)(((int)x + 1) % totalTargetings);
    }

    public static TargetingOptions PrevTargeting(TargetingOptions x)
    {
        return (TargetingOptions)(((int)x - 1 + totalTargetings) % totalTargetings);
    }

    


    public void Start()
    {
        UpdateLinks();
    }


    public void UpdateLinks()
    {
        textureRenderer = transform.Find("TowerTexture")?.GetComponent<Renderer>();
        textureRenderer.enabled = false;

        rangeCircleTransform = transform.Find("RangeCircle");
        rangeCircle = rangeCircleTransform.gameObject;
        rangeCircleRenderer = rangeCircle.GetComponent<Renderer>();
        rangeCircleRenderer.enabled = false;
        currentAttackCD = attackCD;

        checkEnemiesInRange = GetComponentInChildren<CheckEnemiesInRange>();
        UpdateTexture();
    }


    public void RegisterCooldownBuff(LightningBuffController source, float reductionPercentage)
    {
        if (activeCooldownBuffs.ContainsKey(source))
        {
            activeCooldownBuffs[source] = reductionPercentage;
        }
        else
        {
            activeCooldownBuffs.Add(source, reductionPercentage);
        }
        RecalculateCurrentCooldown();
    }


    public void UnregisterCooldownBuff(LightningBuffController source)
    {
        if (activeCooldownBuffs.ContainsKey(source))
        {
            activeCooldownBuffs.Remove(source);
        }
        RecalculateCurrentCooldown();
    }


    public void RecalculateCurrentCooldown()
    {
        float highestReduction = 0f;

        foreach (float reduction in activeCooldownBuffs.Values)
        {
            if (reduction > highestReduction)
            {
                highestReduction = reduction;
            }
        }

        float calculatedCD = attackCD * (1f - highestReduction);

        if (calculatedCD < 0.1f)
        {
            calculatedCD = 0.1f;
        }

        currentAttackCD = calculatedCD;
    }


    public void TextureChange(Texture2D newTexture)
    {
        textureRenderer.material.mainTexture = newTexture;
    }


    public void TextureOn()
    {
        textureRenderer.enabled = true;
    }


    public void TextureOff()
    {
        textureRenderer.enabled = false;
    }


    public void RangeOn()
    {
        rangeCircleRenderer.enabled = true;
    }


    public void RangeOff()
    {
        rangeCircleRenderer.enabled = false;
    }


    public void ClearEnemiesInRange()
    {
        checkEnemiesInRange.ClearEnemiesInRange();
    }


    public void UpdateRangeCircle()
    {
        rangeCircleTransform.localScale = new Vector3(range * rangeMultiplier, 0.001f, range * rangeMultiplier);
    }


    public void UpdateTexture()
    {
        if(towerName == "Empty")
        {
            TextureOff();
        }
        else
        {
            TextureOn();
            Texture2D towerTexture = Resources.Load<Texture2D>("Textures/"+towerName+"/"+towerName+"_"+
                                                                upgrades[0]+upgrades[1]+upgrades[2]);
            if (towerTexture == null)
            {
                Debug.LogError("texture for "+towerName+upgrades[0]+upgrades[1]+upgrades[2]+" not found");               
            }
            else
            {
                TextureChange(towerTexture);
            }
        }
    }


    public void ResetTower()
    {
        UpdateLinks();
        towerName = "Empty";
        upgrades = new int[] {0,0,0};
        damage = 0;
        attackCD = 0f;
        pierce = 0;
        range = 0f;
        UpdateRangeCircle();
        multiTarget = 1;
        overreach = 0;
        lazerWidth = 0.3f;
        damageRamp = 0;
        rampLimit = 0;
        projectileSpeed = 0f;
        projectilePrefab = null;
        projectileScale = 0.5f;
        explosionRadius = 0f;
        burnDamage = 0;
        burnDuration = 0f;
        freezeStrength = 0f;
        freezeDuration = 0f;
        maxChainAmount = 0;
        chainDistance = 0f;
        attackCDBuff = 0f;
        currentAttackCD = 0f;
        if (activeCooldownBuffs != null)
        {
            activeCooldownBuffs.Clear();
        }
        targeting = TargetingOptions.First;

        selected = false;
        dmgCount = 0;
        moneySpent = 0;

        TextureOff();
        RangeOff();
        ClearEnemiesInRange();
        RecalculateCurrentCooldown();
    }

}
