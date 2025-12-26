using System.Collections.Generic;
using UnityEngine;

public class StaticInventoryDisplay : InventoryDisplay
{
    private InventoryHolder inventoryHolder; 
    [SerializeField] private InventorySlots_UI[] slots;

    protected override void Start()
    {
        base.Start();
    }

    public void ConnectToPlayer(InventoryHolder playerHolder)
    {
        inventoryHolder = playerHolder;

        if (inventoryHolder != null)
        {
            inventorySystem = inventoryHolder.InventorySystem;
            
            inventorySystem.OnInventorySlotChanged -= UpdateSlot;
            inventorySystem.OnInventorySlotChanged += UpdateSlot;

            AssignSlot(inventorySystem);
            
            Debug.Log("Inventar erfolgreich mit lokalem Spieler verknüpft.");
        }
    }

    public override void AssignSlot(InventorySystem invToDisplay)
    {
        slotDictionary = new Dictionary<InventorySlots_UI, InventorySlot>();

        if (slots.Length != inventorySystem.InventorySize) 
            Debug.Log($"Inventory slots out of sync on {this.gameObject}");
        
        for (int i = 0; i < inventorySystem.InventorySize; i++)
        {
            slotDictionary.Add(slots[i], inventorySystem.InventorySlots[i]);
            slots[i].Init(inventorySystem.InventorySlots[i]);
        }
    }
}