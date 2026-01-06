using UnityEngine;

public class EquipmentSlot : InventorySlots_UI
{
    [SerializeField] private Itemtype equipmentType;
    [SerializeField] private WeaponType weaponType;
    [SerializeField] private ArmorType armorType;
    public override void Init(InventorySlot slot, int index)
    {
        if (equipmentType == slot.InventoryItemInstance.itemData.Type && equipmentType == Itemtype.Weapon)
        {
            base.Init(slot, index);
        }
        else if (equipmentType == slot.InventoryItemInstance.itemData.Type && equipmentType == Itemtype.Armor)
        {
            base.Init(slot, index);    
        }
        else if (equipmentType == slot.InventoryItemInstance.itemData.Type && equipmentType == Itemtype.Accessory)
        {
            base.Init(slot, index);
        }
        else
        {
            Debug.Log("Wrong item type for this equipment slot");
        }
    }
}
