using System.Collections.Generic;
using UnityEngine;

public class StaticInventoryDisplay : InventoryDisplay
{
    private InventoryHolder inventoryHolder; 
    [SerializeField] private InventorySlots_UI[] slots;
    [SerializeField] protected int offset = 0;
    private bool wasactiveonce = false;

    protected override void Start()
    {
        base.Start();
    }
    private void OnEnable()
    {
        if(wasactiveonce == false)
        {
            if (inventoryHolder != null)
            {
                inventorySystem = inventoryHolder.InventorySystem;
                AssignSlot(inventorySystem);
                
                Debug.Log("Inventar beim öffnen geladen.");
                wasactiveonce = true;
            }
        }
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
            
            //Debug.Log("Inventar erfolgreich mit lokalem Spieler verknüpft.");
        }
    }

    public override void AssignSlot(InventorySystem invToDisplay)
    {
        slotDictionary = new Dictionary<InventorySlots_UI, InventorySlot>();

        if (slots.Length + offset > invToDisplay.InventorySize) 
        {
            Debug.LogError($"Inventory Display Error auf {gameObject.name}: UI benötigt Index bis {slots.Length + offset}, aber System hat nur {invToDisplay.InventorySize} Slots!");
            return; 
        }

        for (int i = 0; i < slots.Length; i++)
        {
            int systemIndex = i + offset;

            InventorySlot slotToDisplay = invToDisplay.InventorySlots[systemIndex];

            // Dictionary füllen
            if (!slotDictionary.ContainsKey(slots[i]))
            {
                slotDictionary.Add(slots[i], slotToDisplay);
            }
            
            // UI Slot initialisieren
            slots[i].Init(slotToDisplay, i);
        }
    }
    
}