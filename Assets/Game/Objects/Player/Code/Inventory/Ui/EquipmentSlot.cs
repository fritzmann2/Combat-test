using UnityEngine;

public class EquipmentSlot : InventorySlots_UI
{
    public int slotIndex;
    [SerializeField] public Itemtype equipmentType;
    [SerializeField] public WeaponType weaponType;
    [SerializeField] public ArmorType armorType;
    [SerializeField] public AccessoryType accessoryType;

    
    public override void Init(InventorySlot slot, int index)
    {
        if (slot.InventoryItemInstance == null || slot.InventoryItemInstance.itemData == null)
        {
            Debug.Log("Slot ist leer");
            base.Init(slot, index); 
            return;
        }
        if (equipmentType == slot.InventoryItemInstance.itemData.Type)
        {
            if (equipmentType == Itemtype.Weapon)
            {
                overide(slot, index);     
            }
            if (equipmentType == Itemtype.Armor)
            {
                if (armorType == slot.InventoryItemInstance.armorType)
                {
                    overide(slot, index);
                }
            }
            if (equipmentType == Itemtype.Accessory)
            {
                if (accessoryType == slot.InventoryItemInstance.accessoryType)
                {
                    overide(slot, index);
                }
            }
        }
        
        else
        {
            Debug.Log("Wrong item type for this equipment slot");
            InventorySlot emptySlot = new InventorySlot(); 
            base.Init(emptySlot, index);
        }
    }

    private void overide(InventorySlot slot, int index)
    {
        Debug.Log("Equipment slot set");
        
        base.Init(slot, index);
        trigerstatupdate();
    }

    public override void UpdateUISlot()
    {
        
        base.UpdateUISlot();
        //trigerstatupdate();
    }

    private void trigerstatupdate()
    {
        PlayerStats playerStats = FindFirstObjectByType<PlayerStats>();
        if (playerStats != null)
        {
            playerStats.UpdateStatsFromEquipment(slotIndex);
        }
    }
}
