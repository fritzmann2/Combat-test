using System.Collections.Generic;
using UnityEngine;
using System.Linq; // Wichtig für .FirstOrDefault()

public class PlayerStats : BaseMobClass
{
    [Header("Verbindungen")]
    [SerializeField] private InventoryHolder inventoryHolder;

    [Header("Debugging")]
    [SerializeField] private List<equipmentStatsSlot> equipmentStats = new List<equipmentStatsSlot>();
    [SerializeField] private playerstats totalStats = new playerstats();
    
    // Basis-Stats (Stats ohne Ausrüstung)
    [SerializeField] private playerstats baseStats = new playerstats(); 

    private void Start()
    {
        inventoryHolder = GetComponent<InventoryHolder>();
        // Initial einmal alles berechnen
        if(inventoryHolder != null)
        {
            // Wir hören auf das Event vom InventorySystem
            inventoryHolder.EquipedSlots.OnInventorySlotChanged += HandleEquipmentChanged;
        }
        RecalculateTotalStats();
    }

    public override void OnDestroy()
    {
        if(inventoryHolder != null)
        {
            inventoryHolder.EquipedSlots.OnInventorySlotChanged -= HandleEquipmentChanged;
        }
    }

    // Diese Methode wird automatisch aufgerufen, wenn das InventorySystem ein Event feuert
    private void HandleEquipmentChanged(InventorySlot slot)
    {
        // Wir müssen herausfinden, welchen Index dieser Slot hat
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
        // WICHTIG: Erst alles auf die Basis-Werte zurücksetzen!
        // Sonst addierst du bei jedem Update unendlich weiter.
        totalStats = new playerstats
        {
            strength = baseStats.strength,
            critChance = baseStats.critChance,
            critDamage = baseStats.critDamage,
            attackSpeed = baseStats.attackSpeed,
            weapondamage = baseStats.weapondamage,
            // ... andere Stats hier kopieren
        };

        // Jetzt alle Ausrüstungsgegenstände addieren
        foreach (var equipment in equipmentStats)
        {
            // Sicherheitschecks: Ist da überhaupt ein Item drin?
            if (equipment.stats == null || equipment.stats.weaponstats == null) continue;

            totalStats.strength += equipment.stats.weaponstats.strength;
            totalStats.critChance += equipment.stats.weaponstats.critChance;
            totalStats.critDamage += equipment.stats.weaponstats.critDamage;
            totalStats.attackSpeed += equipment.stats.weaponstats.attackSpeed;
            totalStats.weapondamage += equipment.stats.weaponstats.weapondamage;
        }
        
        Debug.Log($"Stats updated. New Strength: {totalStats.strength}, Dmg: {totalStats.weapondamage}");
    }

    // --- Kampf-Methoden ---

    public void DealotherDamage(BaseEntety mob)
    {
        int damage = calculateDamage();
        if (damage <= 0) damage = 1; 
        mob.TakeDamage(damage);
    }

    public int calculateDamage()
    {
        
        float multiplier = 1f;
        if (getcrit() == 1)
        {
            multiplier += (totalStats.critDamage / 100f);
        }

        float damage = totalStats.weapondamage * multiplier;
        

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



