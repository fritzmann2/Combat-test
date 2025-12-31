using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;


public class InventoryItem : NetworkBehaviour
{
    public InventoryItemData itemData;
    public List<ItemParameter> itemstats;
    protected virtual void Awake()
    {
        if (itemstats == null) itemstats = new List<ItemParameter>();
    }
    public void Initialize(string _id, List<ItemParameter> _stats)
    {        
        if (itemData != null)
        {
            itemData.ID = _id;
        }
        else
        {
            Debug.LogError("ItemData ist im Inspector nicht zugewiesen!");
        }
        if (_stats != null)
        {
            itemstats = new List<ItemParameter>(_stats);
        }
        else
        {
            itemstats = new List<ItemParameter>();
        }
    }
}

[System.Serializable]
public class InventoryItemInstance
{
    public InventoryItemData itemData;      // Der unveränderliche Bauplan
    public List<ItemParameter> stats;   // Die veränderlichen Werte

    // Konstruktor für ein neues Item
    public InventoryItemInstance(InventoryItemData _data)
    {
        itemData = _data;
        stats = new List<ItemParameter>();
    }
}

