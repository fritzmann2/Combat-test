using System.Collections.Generic;
using UnityEngine;

abstract public class InventoryDisplay : MonoBehaviour
{
    [SerializeField] public MouseItemData mouseInventoryItem;
    protected InventorySystem inventorySystem;
    protected Dictionary<InventorySlots_UI, InventorySlot> slotDictionary;

    public InventorySystem InventorySystem => inventorySystem;
    public Dictionary<InventorySlots_UI, InventorySlot> SlotDictionary => slotDictionary;
    protected virtual void Start()
    {
        
    }

    public abstract void AssignSlot(InventorySystem invToDisplay);

    protected virtual void UpdateSlot(InventorySlot updatedSlot)
    {
        foreach (var slot in slotDictionary)
        {
            if (slot.Value == updatedSlot)
            {
                slot.Key.UpdateUISlot();
            }
        }
    }

    public void SlotClicked(InventorySlots_UI clickedSlot, int index)
    {
        Debug.Log("slot clicked: " + clickedSlot.ToString() + " at index " + index.ToString());
        if (mouseInventoryItem != null)
        {
            mouseInventoryItem.clickedOnInventorySlot(clickedSlot, index);
        }
    }
}
