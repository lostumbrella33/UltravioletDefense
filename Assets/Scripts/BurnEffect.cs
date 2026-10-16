using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurnEffect : MonoBehaviour
{
    private EnemyData enemyData;
    private TowerData sourceTowerData;
    private float duration;
    private int damagePerTick;
    private Coroutine burnCoroutine;
    private float burnTick = 1f;

    private Renderer enemyRenderer;
    private Material outlineMaterial;
    private bool isGlowing = false;
    private float burnOutlineThickness = 0.15f;



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
    }


    public void ApplyBurn(float burnDuration, int burnDamage, TowerData towerData)
    {
        this.duration = burnDuration;
        this.damagePerTick = burnDamage;
        this.sourceTowerData = towerData;

        if (outlineMaterial == null)
        {
            Shader outlineShader = Shader.Find("Custom/BurnOutline");
            if (outlineShader != null)
            {
                outlineMaterial = new Material(outlineShader);

                outlineMaterial.SetFloat("_OutlineWidth", burnOutlineThickness); 
            }
            else
            {
                Debug.LogError("Custom/BurnOutline shader not found");
            }
        }


        if (burnCoroutine != null)
        {
            StopCoroutine(burnCoroutine);
        }

        burnCoroutine = StartCoroutine(BurnTickRoutine());
    }



    private IEnumerator BurnTickRoutine()
    {
        
        if (enemyRenderer != null && outlineMaterial != null && !isGlowing)
        {
            List<Material> currentMaterials = new List<Material>(enemyRenderer.sharedMaterials);
            
            currentMaterials.Add(outlineMaterial);
            enemyRenderer.materials = currentMaterials.ToArray();
            isGlowing = true;
        }


        float elapsed = 0f;

        while (elapsed < duration)
        {
            yield return new WaitForSeconds(burnTick);
            elapsed += burnTick;

            if (enemyData != null)
            {
                if (damagePerTick > enemyData.CurrentHealth)
                {
                    sourceTowerData.dmgCount += enemyData.CurrentHealth;
                }
                else
                {
                    sourceTowerData.dmgCount += damagePerTick;
                }

                enemyData.TakeDamage(damagePerTick);
            }
        }

        RemoveOutline();
        Destroy(this);
    }


    private void RemoveOutline()
    {
        if (enemyRenderer != null && isGlowing)
        {
            List<Material> currentMaterials = new List<Material>(enemyRenderer.sharedMaterials);
            
            if (currentMaterials.Contains(outlineMaterial))
            {
                currentMaterials.Remove(outlineMaterial);
            }
            
            enemyRenderer.materials = currentMaterials.ToArray();
            isGlowing = false;
        }
    }


    private void OnDisable()
    {
        RemoveOutline();
    }
}
