using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;



[System.Serializable]
public struct InventorySlotSaveData
{
    public string itemID;
    public int amount;
    public int slotIndex;
    public EquipmentStats savedStats;

    public InventorySlotSaveData(string _id, int _amount, int _index, EquipmentStats _stats)
    {
        itemID = _id;
        amount = _amount;
        slotIndex = _index;
        savedStats = _stats;
    }
}

[System.Serializable]
public class InventorySaveData
{
    public List<InventorySlotSaveData> slots = new List<InventorySlotSaveData>();
}