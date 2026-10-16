using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningStunEffect : MonoBehaviour
{
    private float stunTimer = 0f;
    private float stunStrength = 0f;
    private bool hasStun = false;

    private Renderer enemyRenderer;
    private Material outlineMaterial;
    private bool isGlowing = false;
    private float maxStunStrength = 1f;



    private void Awake()
    {
        enemyRenderer = GetComponent<Renderer>();
        if(enemyRenderer == null)
        {
            enemyRenderer = GetComponentInChildren<Renderer>();
        }
    }


    public void ApplyStun(float duration, float strength)
    {
        float incomingStunStrength = Mathf.Min(strength, maxStunStrength);

        if (incomingStunStrength >= stunStrength)
        {
            stunStrength = incomingStunStrength;
            stunTimer = duration;
            hasStun = true;
        }

        UpdateVisuals();
    }


    void Update()
    {
        if (hasStun)
        {
            stunTimer -= Time.deltaTime;
            if (stunTimer <= 0f)
            {
                hasStun = false;
                stunStrength = 0f;
                
                RemoveOutlineVisual();
                Destroy(this);
            }
        }
    }

    
    public float GetCurrentStunModifier()
    {
        if(!hasStun)
        {
            return 1f;
        }
        return 1f - stunStrength;
    }


    private void UpdateVisuals()
    {
        if (enemyRenderer != null && hasStun && !isGlowing)
        {
            if (outlineMaterial == null)
            {
                Shader outlineShader = Shader.Find("Custom/LightningStunOutline");
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
    }
}
