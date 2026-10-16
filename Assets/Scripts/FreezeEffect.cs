using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeEffect : MonoBehaviour
{
    private EnemyData enemyData;
    
    private float attackSlowTimer = 0f;
    private float attackSlowStrength = 0f;
    private bool hasAttackSlow = false;
    
    private float areaSlowStrength = 0f;
    private bool hasAreaSlow = false;

    private Renderer enemyRenderer;
    private Material outlineMaterial;
    private bool isGlowing = false;
    private float maxFreezeStrength = 0.7f;



    private void Awake()
    {
        enemyData = GetComponent<EnemyData>();
        if(enemyData == null)
        {
            enemyData = GetComponentInParent<EnemyData>();
        }

        enemyRenderer = GetComponent<Renderer>();
        if(enemyRenderer == null)
        {
            enemyRenderer = GetComponentInChildren<Renderer>();
        }

        FreezeEffect[] existingScripts = gameObject.GetComponents<FreezeEffect>();
        foreach (FreezeEffect script in existingScripts)
        {
            if (script != this)
            {
                if (this.hasAreaSlow)
                {
                    script.ApplyAreaSlow(this.areaSlowStrength);
                }
                
                Destroy(this);
                return;
            }
        }
    }


    public void ApplyAttackSlow(float duration, float strength)
    {
        float incomingFreezeStrength = Mathf.Min(strength, maxFreezeStrength);

        if (incomingFreezeStrength >= attackSlowStrength)
        {
            attackSlowStrength = incomingFreezeStrength;
            attackSlowTimer = duration;
            hasAttackSlow = true;
        }
    
        UpdateVisuals();
    }


    public void ApplyAreaSlow(float strength)
    {
        float incomingFreezeStrength = Mathf.Min(strength, maxFreezeStrength);

        if (incomingFreezeStrength >= areaSlowStrength)
        {
            areaSlowStrength = incomingFreezeStrength;
            hasAreaSlow = true;
        }

        UpdateVisuals();
    }


    public void RemoveAreaSlow()
    {
        hasAreaSlow = false;
        areaSlowStrength = 0f;

        float currentModifier = 1f;
        if(hasAttackSlow)
        {
            currentModifier *= 1f - attackSlowStrength;
        }
        
        enemyData.currentMovementSpeed = enemyData.MovementSpeed * currentModifier;
        
        UpdateVisuals();
    }


    private void Update()
    {
        if (hasAttackSlow)
        {
            attackSlowTimer -= Time.deltaTime;
            if (attackSlowTimer <= 0f)
            {
                hasAttackSlow = false;
                attackSlowStrength = 0f;
            }
        }

        if (!hasAttackSlow && !hasAreaSlow)
        {
            RemoveOutlineVisual();
            Destroy(this);
        }
    }


    public float GetCurrentFreezeModifier()
    {
        float currentModifier = 1f;

        if(hasAttackSlow)
        {
            currentModifier *= 1f - attackSlowStrength;
        }
        if(hasAreaSlow)
        {
            currentModifier *= 1f - areaSlowStrength;
        }
        
        float modifierCap = (1 - maxFreezeStrength)*(1 - maxFreezeStrength);
        if (currentModifier < modifierCap) 
        {
            currentModifier = modifierCap;
        }

        return currentModifier;
    }


    private void UpdateVisuals()
    {
        if (enemyRenderer != null && (hasAttackSlow || hasAreaSlow) && !isGlowing)
        {
            if (outlineMaterial == null)
            {
                Shader outlineShader = Shader.Find("Custom/FreezeOutline");
                if (outlineShader != null) outlineMaterial = new Material(outlineShader);
            }

            if (outlineMaterial != null)
            {
                List<Material> currentMaterials = new List<Material>(enemyRenderer.sharedMaterials);
                currentMaterials.Add(outlineMaterial);
                enemyRenderer.materials = currentMaterials.ToArray();
                isGlowing = true;
            }
        }
    }


    private void RemoveOutlineVisual()
    {
        if (enemyRenderer != null && isGlowing && outlineMaterial != null)
        {
            List<Material> currentMaterials = new List<Material>(enemyRenderer.sharedMaterials);
            if (currentMaterials.Contains(outlineMaterial)) currentMaterials.Remove(outlineMaterial);
            enemyRenderer.materials = currentMaterials.ToArray();
            isGlowing = false;
        }
    }


    private void OnDestroy()
    {
        RemoveOutlineVisual();
        
        if(enemyData != null && gameObject.GetComponent<FreezeEffect>() == null)
        {
            enemyData.currentMovementSpeed = enemyData.MovementSpeed;
        }
    }


    private void OnDisable()
    {
        if(enemyData != null)
        {
            enemyData.ResetSpeed();
        }
        RemoveOutlineVisual();
    }
}
