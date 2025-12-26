using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class InventorySlot
{
    [SerializeField] private InventoryItemData itemData;
    [SerializeField] private int stacksize;
    public InventoryItemData ItemData => itemData;
    public int StackSize => stacksize;

    public InventorySlot (InventoryItemData source, int amount)
    {
        itemData = source;
        stacksize = amount;
    }
    public InventorySlot ()
    {
        clearSlot();
    }
    public void clearSlot()
    {
        itemData = null;
        stacksize = -1;
    }

    public void UpdateInventorySlot(InventoryItemData data, int amount)
    {
        itemData = data;
        stacksize = amount;
    }
    public bool RoomLeftInStack(int ammountToAdd, out int ammountRemaining)
    {
        ammountRemaining = itemData.MaxStackSize - stacksize;
        return RoomLeftInStack(ammountToAdd);
    }
    public bool RoomLeftInStack(int ammountToAdd)
    {
        if (stacksize + ammountToAdd <= itemData.MaxStackSize) return true;
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