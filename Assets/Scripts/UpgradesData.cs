using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UpgradesData : MonoBehaviour
{
    private Dictionary<string,object[]> towerBaseStats = new()
    {
        // damage attackcd pierce range projectlespeed 
        {"Lazer", new object[] {1,1f,1,50,30f}},
        {"Lightning", new object[] {2,1f,1,70,30f}},
        {"Stellar", new object[] {3,1.4f,4,60,30f}},
        {"Ice", new object[] {1,1.2f,1,40,30f}},
        {"Metal", new object[] {1,0.8f,1,30,50f}}
    };
    public Dictionary<string,object[]> TowerBaseStats => towerBaseStats;


    [SerializeField] private Dictionary<string, object[]> upgradeCosts = new()
    {
        {"Lazer", new object[] {100,
        110,500,5000,30000,120000,500000,
        120,300,3500,15000,85000,300000,
        130,400,1500,12000,60000,180000,0} },

        {"Lightning", new object[] {150,
        150,700,7000,45000,180000,750000,
        150,500,6000,40000,110000,400000,
        130,400,4500,20000,70000,200000,0} },

        {"Stellar", new object[] {300,
        200,1000,9000,75000,240000,1200000,
        150,700,7000,50000,150000,700000,
        150,600,5000,40000,90000,480000,0} },

        {"Ice", new object[] {150,
        130,700,3000,30000,130000,600000,
        130,600,2500,25000,105000,480000,
        150,800,5500,50000,200000,850000,0} },

        {"Metal", new object[] {300,
        200,800,9000,60000,210000,900000,
        150,600,6000,30000,160000,550000,
        180,700,8500,45000,180000,750000,0} }
    };
    public Dictionary<string, object[]> UpgradeCosts => upgradeCosts;


    [SerializeField] private Dictionary<string, object[]> upgradeDescriptions = new()
    {
        {"Lazer", new object[] {"Base Lazer Tower",

        "+20 range",
        "+2 pierce",
        "+3 pierce, lazer overshoots for 40 range beyond the tower's range",
        "+40 range, +4 pierce",
        "+20 range, +10 pierce, +60 overshoot range",
        "+40 range, +80 pierce",

        "+20% attack speed",
        "+20% more attack speed",
        "Damage ramps by 1 after each consecutive hit on the same target, stacks up to 10",
        "+20% attack speed, damage ramps up to 20",
        "+20% attack speed, damage ramps by 3 each attack and up to 100",
        "+10% attack speed, damage ramps by 5 and up to 1000",

        "+1 base damage",
        "+1 more damage",
        "Now shoots 2 lazers per attack",
        "3 lazers per attack and +2 damage",
        "+10 damage",
        "6 lazers per attack and +20 damage",

        ""} },

        {"Lightning", new object[] {"Base Lightning Tower",

        "+2 damage",
        "+3 damage",
        "+7 damage, +20 range",
        "+21 damage",
        "+65 damage, +30 range",
        "+900 damage, +50 range",

        "Lightning bounces to a new target once, within 5 range",
        "Bounces up to 3 times and up to 10 range",
        "+20% attack speed, and bounces up to 8 times and up to 20 range",
        "Bounces up to 20 times and up to 30 range",
        "+20% attack speed, and bounces up to 50 times",
        "+20% attack speed, and bounces up to 300 times and up to 50 range",

        "Attacks stun enemies, reducing their movement speed by 40% for 0.2 seconds",
        "+0.2s stun duration",
        "+20% stun strength and +0.1 duration, tower also passively increases attack speed of other towers in its range by 10% (doesnt stack)",
        "Attack speed buff increased to 30% (only the strongest version of the buff is applied)",
        "+20% stun strength and +0.1 duration",
        "+20% stun strength and +0.1 duration, attack speed buff increased to 50%",

        ""} },

        {"Stellar", new object[] {"Base Stellar Tower",

        "+1 explosion radius, +2 pierce",
        "+1 damage, +3 pierce",
        "+2 explosion radius, +2 damage, +5 pierce",
        "+4 explosion radius, +16 pierce",
        "+18 damage, +20 pierce",
        "+4 explosion radius, +25 damage, +50 pierce",

        "+30 range, +20% attack speed",
        "Adds a secondary bomb, which flies backwards from the main target and doesnt home",
        "Adds 2 more secondary bombs which now fly at 90 degree angles, +20% attack speed",
        "Fires 6 bombs in total, +30 range",
        "Fires 10 bombs, +30% attack speed",
        "Fires 17 bombs, +40 range, +30% attack speed",

        "Applies burn effect on hit, which does 1 damage for 3 seconds",
        "+2 burn damage per tick",
        "+4 burn damage, +4 seconds duration",
        "+10 burn damage",
        "+33 burn damage, +5 seconds duration",
        "+250 burn damage, +48 seconds duration",
        ""} },

        {"Ice", new object[] {"Base Ice Tower",

        "+20% attack speed",
        "+20 range, +20% attack speed",
        "+30 range, +20% attack speed",
        "+20% attack speed",
        "+20 range, +10% attack speed",
        "+20 range, +20% attack speed",

        "+2 pierce",
        "+0.8 explosion radius",
        "+3 pierce, +0.7 explosion radius",
        "+8 pierce",
        "+16 pierce, +1.5 explosion radius",
        "+70 pierce, +3 explosion radius",

        "Slows enemies by 20%",
        "3 seconds freeze duration",
        "Slows enemies by 30% and +2 seconds duration",
        "40% slow strength",
        "60% slow strength, +5s duration",
        "80% slow strength, +50s duration",

        ""} },

        {"Metal", new object[] {"Base Metal Tower",

        "+1 damage",
        "+10 range",
        "+2 damage, +10 range",
        "+6 damage",
        "+10 damage, +10 range",
        "+30 damage, +20 range",

        "+20% attack speed",
        "+2 pierce",
        "+20% attack speed, +4 pierce, spikes overshoot for extra 10 range beyond the tower's range",
        "+10% attack speed, +13 pierce, +20 overshoot range",
        "+10% attack speed, +30 pierce",
        "+10% attack speed, +100 pierce, +40 overshoot range",

        "+4 spikes per attack",
        "+6 spikes per attack",
        "+10 spikes per attack",
        "+16 spikes per attack",
        "+24 spikes per attack",
        "+60 spikes per attack",

        ""} }
    };
    public Dictionary<string, object[]> UpgradeDescriptions => upgradeDescriptions;
                                      
    

    void Start()
    {
        
    }


    public int MapUpgradesToNumber(int[] array)
    {
        if(array.Length != 3)
        {
            return -1;
        }
        else
        {
            int x = array.Sum();
            if(array[1] != 0)
            {
                x += 6;
            }
            else if(array[2] != 0)
            {
                x += 12;
            }
            return x;
        }
    }



    public void ApplyStatChanges(GameObject tower, int path, int level)
    {
        TowerData towerData = tower.GetComponent<TowerData>();
        
        switch(towerData.towerName)
        {
            case "Lazer": // 1, 1f, 1, 50, 30f
                switch(path)
                {
                    case 0: // top path: thicker lazer with more pierce and longer range
                        switch(level)
                        {
                            case 1:
                                towerData.range += 20; // 70
                                break;
                            case 2:
                                towerData.pierce += 2; // 3  
                                towerData.lazerWidth += 0.1f; // 0.4            
                                break;
                            case 3:
                                towerData.pierce += 3; // 6
                                towerData.overreach += 40; // 40
                                towerData.lazerWidth += 0.2f; // 0.6
                                break;
                            case 4:
                                towerData.range += 40; // 110
                                towerData.pierce += 4; // 10
                                towerData.lazerWidth += 0.2f; // 0.8
                                break;
                            case 5:
                                towerData.range += 20; // 130
                                towerData.pierce += 10; // 20
                                towerData.overreach += 60; // 100
                                towerData.lazerWidth += 0.4f; // 1.2
                                break;
                            case 6:
                                towerData.range += 40; // 170
                                towerData.pierce += 80; // 100
                                towerData.lazerWidth += 0.8f; // 2
                                break;
                            default:
                                break;
                        }
                        break;

                    case 1: // middle path: faster attack speed with damage ramping over time when focused on same target
                        switch(level)
                        {
                            case 1:
                                towerData.attackCD -= 0.2f; // 0.8
                                break;
                            case 2:
                                towerData.attackCD -= 0.2f; // 0.6
                                break;
                            case 3:
                                towerData.damageRamp += 1; // +1 ramp
                                towerData.rampLimit += 10; // 10 max
                                break;
                            case 4:
                                towerData.attackCD -= 0.2f; // 0.4
                                towerData.rampLimit += 10; // 20 max
                                break;
                            case 5:
                                towerData.attackCD -= 0.2f;
                                towerData.damageRamp += 2; // +3 ramp
                                towerData.rampLimit += 80; // 100 max
                                break;
                            case 6:
                                towerData.attackCD -= 0.1f; // 0.1
                                towerData.damageRamp += 2; // +5 ramp
                                towerData.rampLimit += 900; // 1000 max
                                break;
                            default:
                                break;
                        }
                        break;

                    case 2: // bottom path: more base damage per hit and firing multiple lazers in parallel
                        switch(level)
                        {
                            case 1:
                                towerData.damage += 1; // 2
                                break;
                            case 2:
                                towerData.damage += 1; // 3
                                break;
                            case 3:
                                towerData.multiTarget += 1; // 2-shooter
                                break;
                            case 4:
                                towerData.multiTarget += 1; // 3-shooter 
                                towerData.damage += 2; // 5
                                break;
                            case 5:
                                towerData.damage += 10; // 15
                                break;
                            case 6:
                                towerData.multiTarget += 3; // 6-shooter 
                                towerData.damage += 20; // 35
                                break;
                            default:
                                break;
                        }
                        break;
                    default:
                        break;
                }
                break;
    
            



            case "Lightning": // 2, 1f, 1, 70, 30f
                switch(path)
                {
                    case 0: // top path: single target damage + range
                        switch(level)
                        {
                            case 1:
                                towerData.damage += 2; // 4
                                towerData.lazerWidth += 0.1f; // 0.4 
                                break;
                            case 2:
                                towerData.damage += 3; // 7
                                towerData.lazerWidth += 0.1f; // 0.5       
                                break;
                            case 3:
                                towerData.damage += 7; // 14
                                towerData.range += 20; // 90
                                towerData.lazerWidth += 0.1f; // 0.6
                                break;
                            case 4:
                                towerData.damage += 21; // 35
                                towerData.lazerWidth += 0.1f; // 0.7
                                break;
                            case 5:
                                towerData.damage += 65; // 100
                                towerData.range += 30; // 120
                                towerData.lazerWidth += 0.1f; // 0.8
                                break;
                            case 6:
                                towerData.damage += 900; // 1000
                                towerData.range += 50; // 170
                                towerData.lazerWidth += 0.2f; // 1 
                                break;
                            default:
                                break;
                        }
                        break;

                    case 1: // middle path: lightning bouncing between enemies
                        switch(level)
                        {
                            case 1:
                                towerData.maxChainAmount += 1; // 1
                                towerData.chainDistance += 5; // 5 
                                break;
                            case 2:
                                towerData.maxChainAmount += 2; // 3
                                towerData.chainDistance += 5; // 10
                                break;
                            case 3:
                                towerData.maxChainAmount += 5; // 8
                                towerData.chainDistance += 10; // 20
                                towerData.attackCD -= 0.2f; // 0.8
                                break;
                            case 4:
                                towerData.maxChainAmount += 12; // 20
                                towerData.chainDistance += 10; // 30
                                break;
                            case 5:
                                towerData.maxChainAmount += 30; // 50
                                towerData.attackCD -= 0.2f; // 0.6
                                break;
                            case 6:
                                towerData.maxChainAmount += 250; // 300
                                towerData.chainDistance += 20; // 50
                                towerData.attackCD -= 0.2f; // 0.4
                                break;
                            default:
                                break;
                        }
                        break;

                    case 2: // bottom path: freeze / stun effect which is much stronger than ice but lasts very briefly
                            // + passive attack speed buff to nearby towers
                        switch(level)
                        {
                            case 1:
                                towerData.freezeStrength += 0.4f; // 0.4
                                towerData.freezeDuration += 0.2f; // 0.2
                                break;
                            case 2:
                                towerData.freezeDuration += 0.2f; // 0.4
                                break;
                            case 3:
                                towerData.freezeStrength += 0.2f; // 0.6
                                towerData.freezeDuration += 0.1f; // 0.5
                                towerData.attackCDBuff += 0.1f; // 0.1
                                break;
                            case 4:
                                towerData.attackCDBuff += 0.2f; // 0.3
                                break;
                            case 5:
                                towerData.freezeStrength += 0.2f; // 0.8
                                towerData.freezeDuration += 0.1f; // 0.6
                                break;
                            case 6:
                                towerData.freezeStrength += 0.2f; // 1
                                towerData.freezeDuration += 0.1f; // 0.7
                                towerData.attackCDBuff += 0.2f; // 0.5
                                break;
                            default:
                                break;
                        }
                        break;
                    default:
                        break;
                }
                break;





            case "Stellar": // 3, 1.4f, 4, 60, 30f, 1f
                switch(path)
                {
                    case 0: // top path: larger explosions with more radius pierce and base damage
                        switch(level)
                        {
                            case 1:
                                towerData.explosionRadius += 1; // 2
                                towerData.projectileScale += 0.2f; // 1
                                towerData.pierce += 2; // 6
                                break;
                            case 2:
                                towerData.pierce += 3; // 9
                                towerData.damage += 1; // 5     
                                break;
                            case 3:
                                towerData.explosionRadius += 2; // 4
                                towerData.projectileScale += 0.4f; // 1.4
                                towerData.pierce += 5; // 14
                                towerData.damage += 2; // 7
                                break;
                            case 4:
                                towerData.explosionRadius += 4; // 8
                                towerData.projectileScale += 0.8f; // 2.2
                                towerData.pierce += 16; // 30
                                break;
                            case 5:
                                towerData.pierce += 20; // 50
                                towerData.damage += 18; // 25
                                break;
                            case 6:
                                towerData.explosionRadius += 4; // 12
                                towerData.projectileScale += 1.1f; // 3.3
                                towerData.pierce += 50; // 100
                                towerData.damage += 25; // 50
                                break;
                            default:
                                break;
                        }
                        break;

                    case 1: // middle path: shooting multiple bombs radially and more attack speed
                        switch(level)
                        {
                            case 1:
                                towerData.attackCD -= 0.2f; // 1.2
                                towerData.range += 30; // 80
                                break;
                            case 2:
                                towerData.multiTarget += 1; // 2
                                break;
                            case 3:
                                towerData.multiTarget += 2; // 4
                                towerData.attackCD -= 0.2f; // 1
                                break;
                            case 4:
                                towerData.multiTarget += 2; // 6
                                towerData.range += 30; // 110
                                break;
                            case 5:
                                towerData.multiTarget += 4; // 10
                                towerData.attackCD -= 0.3f; // 0.7
                                break;
                            case 6:
                                towerData.multiTarget += 7; // 17
                                towerData.attackCD -= 0.3f; // 0.4
                                towerData.range += 40; // 150
                                break;
                            default:
                                break;
                        }
                        break;

                    case 2: // bottom path: applies burn effect for residual damage
                        switch(level)
                        {
                            case 1:
                                towerData.burnDamage += 1; // 1
                                towerData.burnDuration += 3f; // 3
                                break;
                            case 2:
                                towerData.burnDamage += 2; // 3
                                break;
                            case 3:
                                towerData.burnDamage += 4; // 7
                                towerData.burnDuration += 4f; // 7
                                break;
                            case 4:
                                towerData.burnDamage += 10; // 17
                                break;
                            case 5:
                                towerData.burnDamage += 33; // 50
                                towerData.burnDuration += 5f; // 12
                                break;
                            case 6:
                                towerData.burnDamage += 250; // 300
                                towerData.burnDuration += 48f; // 60
                                break;
                            default:
                                break;
                        }
                        break;
                    default:
                        break;
                }
                break;





            case "Ice":
                switch(path) // 1, 1.2f, 1, 40, 30f
                {
                    case 0: // top path: range and attack speed
                        switch(level)
                        {
                            case 1:
                                towerData.attackCD -= 0.2f; // 1
                                towerData.projectileSpeed += 10f; // 40
                                break;
                            case 2:
                                towerData.range += 20; // 60
                                towerData.attackCD -= 0.2f; // 0.8   
                                break;
                            case 3:
                                towerData.range += 30; // 90
                                towerData.attackCD -= 0.2f; // 0.6
                                towerData.projectileSpeed += 20f; // 60
                                towerData.projectileScale -= 0.1f; // 0.4
                                break;
                            case 4:
                                towerData.attackCD -= 0.2f; // 0.4
                                towerData.projectileSpeed += 30f; // 90
                                break;
                            case 5:
                                towerData.range += 20; // 110
                                towerData.attackCD -= 0.1f; // 0.3
                                towerData.projectileSpeed += 20f; // 110
                                break;
                            case 6:
                                towerData.range += 20; // 130
                                towerData.attackCD -= 0.2f; // 0.1
                                towerData.projectileSpeed += 50f; // 160
                                towerData.projectileScale -= 0.1f; // 0.2
                                break;
                            default:
                                break;
                        }
                        break;

                    case 1: // middle path: explosion size and pierce
                        switch(level)
                        {
                            case 1:
                                towerData.pierce += 2; // 3
                                break;
                            case 2:
                                towerData.explosionRadius += 0.8f; // 1.3
                                towerData.projectileScale += 0.3f; // 0.8
                                break;
                            case 3:
                                towerData.explosionRadius += 0.7f; // 2
                                towerData.pierce += 3; // 6
                                towerData.projectileScale += 0.4f; // 1.2
                                break;
                            case 4:
                                towerData.pierce += 8; // 14
                                break;
                            case 5:
                                towerData.explosionRadius += 1.5f; // 3.5
                                towerData.pierce += 16; // 30
                                towerData.projectileScale += 0.3f; // 1.5
                                break;
                            case 6:
                                towerData.explosionRadius += 3f; // 6.5
                                towerData.pierce += 70; // 100
                                towerData.projectileScale += 0.5f; // 2
                                break;
                            default:
                                break;
                        }
                        break;

                    case 2: // bottom path: slowdown strength and duration
                        switch(level)
                        {
                            case 1:
                                towerData.freezeStrength += 0.1f; // 0.2
                                break;
                            case 2:
                                towerData.freezeDuration += 2; // 3
                                break;
                            case 3:
                                towerData.freezeStrength += 0.1f; // 0.3
                                towerData.freezeDuration += 2; // 5
                                break;
                            case 4:
                                towerData.freezeStrength += 0.1f; // 0.4
                                break;
                            case 5:
                                towerData.freezeStrength += 0.2f; // 0.6
                                towerData.freezeDuration += 5; // 10
                                break;
                            case 6:
                                towerData.freezeStrength += 0.2f; // 0.8
                                towerData.freezeDuration += 50; // 60
                                break;
                            default:
                                break;
                        }
                        break;
                    default:
                        break;
                }
                break;





            case "Metal": 
                switch(path) // 1, 0.8f, 1, 30, 50f
                {
                    case 0: // top path: dmg per hit and range
                        switch(level)
                        {
                            case 1:
                                towerData.damage += 1; // 2
                                towerData.projectileScale += 0.1f; // 0.6
                                break;
                            case 2:
                                towerData.range += 10; // 40    
                                break;
                            case 3:
                                towerData.damage += 2; // 4
                                towerData.projectileScale += 0.1f; // 0.7
                                towerData.projectileSpeed -= 10f; // 40
                                towerData.range += 10; // 50
                                break;
                            case 4:
                                towerData.damage += 6; // 10
                                towerData.projectileSpeed -= 15f; // 25
                                break;
                            case 5:
                                towerData.damage += 10; // 20
                                towerData.projectileScale += 0.1f; // 0.8
                                towerData.range += 10; // 60
                                break;
                            case 6:
                                towerData.damage += 30; // 50
                                towerData.projectileScale += 0.2f; // 1
                                towerData.projectileSpeed -= 15f; // 10
                                towerData.range += 20; // 80
                                break;
                            default:
                                break;
                        }
                        break;

                    case 1: // middle path: attack speed and pierce
                        switch(level)
                        {
                            case 1:
                                towerData.attackCD -= 0.2f; // 0.6
                                towerData.projectileSpeed += 10f; // 60
                                break;
                            case 2:
                                towerData.pierce += 2; // 3
                                break;
                            case 3:
                                towerData.attackCD -= 0.2f; // 0.4
                                towerData.projectileSpeed += 20f; // 80
                                towerData.pierce += 4; // 7
                                towerData.overreach += 10; // 10
                                break;
                            case 4:
                                towerData.attackCD -= 0.1f; // 0.3
                                towerData.pierce += 13; // 20
                                towerData.overreach += 20; // 30
                                break;
                            case 5:
                                towerData.attackCD -= 0.1f; // 0.2
                                towerData.projectileSpeed += 20f; // 100
                                towerData.pierce += 30; // 50
                                break;
                            case 6:
                                towerData.attackCD -= 0.1f; // 0.1
                                towerData.projectileSpeed += 40f; // 140
                                towerData.pierce += 100; // 150
                                towerData.overreach += 40; // 70
                                break;
                            default:
                                break;
                        }
                        break;

                    case 2: // bottom path: more spikes per attack
                        switch(level)
                        {
                            case 1:
                                towerData.multiTarget += 4; // 12
                                break;
                            case 2:
                                towerData.multiTarget += 6; // 18
                                break;
                            case 3:
                                towerData.multiTarget += 10; // 28
                                break;
                            case 4:
                                towerData.multiTarget += 16; // 44 
                                break;
                            case 5:
                                towerData.multiTarget += 24; // 68
                                break;
                            case 6:
                                towerData.multiTarget += 60; // 128
                                break;
                            default:
                                break;
                        }
                        break;
                    default:
                        break;
                }
                break;

            
            default:
                break;
        }
    }
}
