using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerStats : BaseMobClass
{
    [Header("Verbindungen")]
    [SerializeField] private InventoryHolder inventoryHolder;
    private PlayerSaveHandler playerSaveHandler;

    [Header("Debugging")]
    [SerializeField] private List<equipmentStatsSlot> equipmentStats = new List<equipmentStatsSlot>();
    [SerializeField] private playerstats totalStats = new playerstats();
    
    // Basis-Stats (Stats ohne Ausrüstung)
    [SerializeField] private playerstats baseStats = new playerstats(); 
    private bool isCrit;



    public override void OnDestroy()
    {
        if(inventoryHolder != null)
        {
            inventoryHolder.EquipedSlots.OnInventorySlotChanged -= HandleEquipmentChanged;
        }
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        inventoryHolder = GetComponent<InventoryHolder>();
        // Initial einmal alles berechnen
        if(inventoryHolder != null)
        {
            // Wir hören auf das Event vom InventorySystem
            inventoryHolder.EquipedSlots.OnInventorySlotChanged += HandleEquipmentChanged;
        }
        playerSaveHandler = GetComponent<PlayerSaveHandler>();        
        playerSaveHandler.dataLoaded += Init;
    }
    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        playerSaveHandler.dataLoaded -= Init;
    }

    private void Init()
    {
        if (inventoryHolder == null || inventoryHolder.EquipedSlots == null) 
        {
            Debug.LogWarning("PlayerStats: Init fehlgeschlagen - Kein InventoryHolder oder EquipedSlots gefunden.");
            return;
        }

//        Debug.Log("PlayerStats: Initialisiere Stats aus Equipment...");

        for (int i = 0; i < inventoryHolder.EquipedSlots.InventorySlots.Count; i++)
        {
            UpdateStatsFromEquipment(i);
        }
    }

    private void HandleEquipmentChanged(InventorySlot slot)
    {
        int index = inventoryHolder.EquipedSlots.InventorySlots.IndexOf(slot);
        if (index != -1)
        {
            UpdateStatsFromEquipment(index);
        }
    }

    public void UpdateStatsFromEquipment(int slotIndex)
    {
        // 1. Hole das echte Item aus dem InventoryHolder
        InventorySlot realSlot = inventoryHolder.EquipedSlots.InventorySlots[slotIndex];
        
        // 2. Finde oder erstelle den lokalen Cache-Eintrag
        equipmentStatsSlot localSlot = equipmentStats.FirstOrDefault(x => x.slotIndex == slotIndex);
        
        if (localSlot == null)
        {
            localSlot = new equipmentStatsSlot { slotIndex = slotIndex };
            equipmentStats.Add(localSlot);
        }

        // 3. Aktualisiere die Daten im Cache
        if (realSlot != null && realSlot.InventoryItemInstance != null && realSlot.InventoryItemInstance.stats != null)
        {
            // Item ist da: Stats kopieren
            localSlot.stats = realSlot.InventoryItemInstance.stats;
        }
        else
        {
            // Slot ist leer oder Item hat keine Stats: Null setzen
            localSlot.stats = null;
        }

        // 4. Alles neu berechnen
        RecalculateTotalStats();
    }

    private void RecalculateTotalStats()
    {
        totalStats = new playerstats
        {
            strength = baseStats.strength,
            critChance = baseStats.critChance,
            critDamage = baseStats.critDamage,
            attackSpeed = baseStats.attackSpeed,
            weapondamage = baseStats.weapondamage,
        };

        foreach (var equipment in equipmentStats)
        {
            if (equipment.stats == null || equipment.stats.weaponstats == null) continue;

            totalStats.strength += equipment.stats.weaponstats.strength;
            totalStats.critChance += equipment.stats.weaponstats.critChance;
            totalStats.critDamage += equipment.stats.weaponstats.critDamage;
            totalStats.attackSpeed += equipment.stats.weaponstats.attackSpeed;
            totalStats.weapondamage += equipment.stats.weaponstats.weapondamage;
        }
        
//        Debug.Log($"Stats updated. New Strength: {totalStats.strength}, Dmg: {totalStats.weapondamage}");
    }

    // --- Kampf-Methoden ---

    public void DealotherDamage(BaseEntety mob, float attackmulti)
    {
        int damage = calculateDamage(attackmulti);
        if (damage <= 0) damage = 5; 
        mob.TakeDamage(damage, isCrit);
    }

    public int calculateDamage(float attackmulti)
    {
        
        float multiplier = 1f;
        if (getcrit() == 1)
        {
            multiplier += (totalStats.critDamage / 100f);
            isCrit = true;
        }
        else
        {
            isCrit = false;
        }
        multiplier *= (1+ totalStats.strength / 100f);

        float damage = totalStats.weapondamage * multiplier * attackmulti;
        

        return Mathf.RoundToInt(damage);
    }

    public int getcrit()
    {
        float critRoll = Random.Range(0f, 100f);
        return (critRoll <= totalStats.critChance) ? 1 : 0;
    }

    public PlayerStatsSaveData GetSaveData()
    {
        PlayerStatsSaveData playerStatsSaveData = new PlayerStatsSaveData
        {
            baseStats = this.baseStats
        };
        return playerStatsSaveData;
    }
}



