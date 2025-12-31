using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class InventorySlot
{
    [SerializeField] private InventoryItemInstance inventoryItemInstance;
    [SerializeField] private int stacksize;
    public InventoryItemInstance InventoryItemInstance => inventoryItemInstance;
    
    public int StackSize => stacksize;

    public InventorySlot (InventoryItemInstance source, int amount)
    {
        inventoryItemInstance = source;
        stacksize = amount;
    }
    public InventorySlot ()
    {
        clearSlot();
    }
    public void clearSlot()
    {
        inventoryItemInstance = null;
        stacksize = -1;
    }

    public void UpdateInventorySlot(InventoryItemInstance data, int amount)
    {
        inventoryItemInstance = data;
        stacksize = amount;
    }
    public bool RoomLeftInStack(int ammountToAdd, out int ammountRemaining)
    {
        ammountRemaining = inventoryItemInstance.itemData.MaxStackSize - stacksize;
        return RoomLeftInStack(ammountToAdd);
    }
    public bool RoomLeftInStack(int ammountToAdd)
    {
        if (stacksize + ammountToAdd <= inventoryItemInstance.itemData.MaxStackSize) return true;
        else return false;
    }
    public void addToStack(int amount)
    {
        stacksize += amount;
    }
    public void removeFromStack(int amount)
    {
        stacksize -= amount;
    }
}