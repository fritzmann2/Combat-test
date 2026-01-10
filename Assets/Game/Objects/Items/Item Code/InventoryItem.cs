using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;


public class InventoryItem : NetworkBehaviour
{
    public InventoryItemData itemData;
    public string ItemID;
    public void Initialize(string _id)
    {        
        if (itemData != null)
        {
            itemData.ID = _id;
        }
        else
        {
            Debug.LogError("ItemData ist im Inspector nicht zugewiesen!");
        }
        
    }
}

[System.Serializable]
public class InventoryItemInstance 
{
    public InventoryItemData itemData;      // Der unveränderliche Bauplan
    public WeaponType weaponType { get; set; } = WeaponType.None;
    public ArmorType armorType { get; set; } = ArmorType.None;
    public AccessoryType accessoryType { get; set; } = AccessoryType.None;
    public EquipmentStats stats;

    // Konstruktor für ein neues Item
    public InventoryItemInstance(InventoryItemData _data)
    {
        itemData = _data;
    }
}

