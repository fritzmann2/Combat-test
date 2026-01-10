using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class PlayerStats : BaseMobClass
{
    public movement playerMovement;
    
    private List<equipmentStatsSlot> equipmentStats;
    private playerstats totalStats = new playerstats();
    
    private void Init()
    {
        foreach (var equipment in equipmentStats)
        {

            totalStats.strength += equipment.stats.weaponstats.strength;
            totalStats.critChance += equipment.stats.weaponstats.critChance;
            totalStats.critDamage += equipment.stats.weaponstats.critDamage;
            totalStats.attackSpeed += equipment.stats.weaponstats.attackSpeed;
            totalStats.weapondamage += equipment.stats.weaponstats.weapondamage;
        }

    }

    public void DealotherDamage(BaseEntety mob)
    {
        int damage = calculateDamage();
        if (damage == 0) damage = 10;
        mob.TakeDamage(damage);
    }
    public void UpdateStatsFromEquipment(int slotindex)
    {
        updateslotstat(slotindex);
    }
    private void updateslotstat(int slotnum)
    {
        equipmentStatsSlot eqstat = equipmentStats.Find(e => e.slotIndex == slotnum);
        if (eqstat != null)
        {
            Init();
        }
    }
    public int calculateDamage()
    {
        float damage = totalStats.weapondamage * (1 + getcrit() * (totalStats.critDamage / 100));
        return Mathf.RoundToInt(damage);
    }
    public int getcrit()
    {
        float critRoll = Random.Range(0f, 100f);
        if (critRoll <= totalStats.critChance)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }

}

[System.Serializable]
public class equipmentStatsSlot
{
    public int slotIndex;
    public EquipmentStats stats;
}


public class playerstats
{
    public float movementSpeed;
    public float weapondamage;
    public float attackSpeed;
    public float critChance;
    public float critDamage;
    public float strength;
    public float defense;
    public float spellresistance;
}