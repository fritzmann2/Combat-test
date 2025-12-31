using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

[System.Serializable]
public class InventorySystem 
{
    [SerializeField] private List<InventorySlot> inventorySlots;
    public List<InventorySlot> InventorySlots => inventorySlots;
    public int InventorySize => InventorySlots.Count;
    public UnityAction<InventorySlot> OnInventorySlotChanged;


    public InventorySystem(int size)
    {
        inventorySlots = new List<InventorySlot>(size);

        for (int i = 0; i < size; i++)
        {
            inventorySlots.Add(new InventorySlot());
        }
    }

    public bool AddToInventory(InventoryItemInstance itemToAdd, int amount)
    {
        // Guckt nach ob das item im inventar existiert
        if (ContainsItem(itemToAdd, out List<InventorySlot> invSlot)) 
        {
            Debug.Log("try to add to existing slot");
            foreach (var slot in invSlot)
            {
                if (slot.RoomLeftInStack(amount))
                {
                    slot.addToStack(amount);
                    OnInventorySlotChanged?.Invoke(slot);
                    return true;
                }
            }
        }
        //Holt den ersten verfügbaren slot
        if (HasFreeSlot(out InventorySlot freeSlot))
        {
            Debug.Log("try to add to free slot");
            freeSlot.UpdateInventorySlot(itemToAdd, amount);
            OnInventorySlotChanged?.Invoke(freeSlot);
            return true;
        }
        return false;
    }

    public bool ContainsItem(InventoryItemInstance itemToAdd, out List<InventorySlot> invSlot)
    {
        invSlot = InventorySlots.Where(i => i.InventoryItemInstance == itemToAdd).ToList();

        return invSlot.Count > 0;
    }

    public bool HasFreeSlot(out InventorySlot freeSlot)
    {
        freeSlot = InventorySlots.FirstOrDefault(i => i.InventoryItemInstance == null);
        return freeSlot != null;
    }
}
