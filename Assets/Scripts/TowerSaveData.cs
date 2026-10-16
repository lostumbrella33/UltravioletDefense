using System.Numerics;
using UnityEngine;

[System.Serializable]
public class TowerSaveData
{

    public int towerId;
    public string towerName;
    public int[] upgrades;
    public int damage;
    public float attackCD;
    public int pierce;
    public float range;
    public float projectileSpeed;
    public GameObject projectilePrefab;
    public int multiTarget;
    public float overreach;
    public float lazerWidth;
    public int damageRamp;
    public int rampLimit;
    public float explosionRadius;
    public float projectileScale;
    public int burnDamage;
    public float burnDuration;
    public float freezeStrength;
    public float freezeDuration;
    public int maxChainAmount;
    public float chainDistance;
    public float attackCDBuff;
    public TowerData.TargetingOptions targeting;
    public int dmgCount;
    public int moneySpent;
}



[System.Serializable]
public class LevelSaveDataWrapper
{
    public System.Collections.Generic.List<TowerSaveData> savedTowers = new System.Collections.Generic.List<TowerSaveData>();
}
