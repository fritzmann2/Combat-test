using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

[System.Serializable]
public struct ItemParameter : INetworkSerializable, System.IEquatable<ItemParameter>
{
    public Unity.Collections.FixedString64Bytes parameterName; 
    public float value;

    public ItemParameter(string name, float val) : this()
    {
        parameterName = name; 
        value = val;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref parameterName);
        serializer.SerializeValue(ref value);
    }

    public bool Equals(ItemParameter other)
    {
        return parameterName == other.parameterName && value == other.value;
    }
}

[System.Serializable]
public struct InventorySlotSaveData
{
    public string itemID;
    public int amount;
    public int slotIndex;
    public List<ItemParameter> savedStats;

    public InventorySlotSaveData(string _id, int _amount, int _index, List<ItemParameter> _stats)
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