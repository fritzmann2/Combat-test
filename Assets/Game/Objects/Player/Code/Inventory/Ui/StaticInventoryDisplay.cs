using System.Collections.Generic;
using UnityEngine;

public enum InventoryDisplayType
{
    MainInventory,
    Equipment
}
public class StaticInventoryDisplay : InventoryDisplay
{
    private InventoryHolder inventoryHolder; 
    [SerializeField] private InventoryDisplayType displayType;
    [SerializeField] private InventorySlots_UI[] slots;
    [SerializeField] protected int offset = 0;
    protected InventorySystem currentSystemToDisplay;
    private bool wasactiveonce = false;

    protected override void Start()
    {
        base.Start();
    }
    private void OnEnable()
    {
        if (!wasactiveonce && inventoryHolder != null)
        {
            SetupDisplay();
            wasactiveonce = true;
        }
    }

    public void ConnectToPlayer(InventoryHolder playerHolder)
    {
        inventoryHolder = playerHolder;
        SetupDisplay();
        
    }
    private void SetupDisplay()
    {
        if (inventoryHolder != null)
        {
            inventorySystem = inventoryHolder.InventorySystem;
            equipmentSlots = inventoryHolder.EquipedSlots; 

            InventorySystem systemToDisplay = null;

            if (displayType == InventoryDisplayType.MainInventory)
            {
                systemToDisplay = inventorySystem;
            }
            else if (displayType == InventoryDisplayType.Equipment)
            {
                systemToDisplay = equipmentSlots;
            }

            if (systemToDisplay != null)
            {
                systemToDisplay.OnInventorySlotChanged -= UpdateSlot;
                systemToDisplay.OnInventorySlotChanged += UpdateSlot;

                AssignSlot(systemToDisplay);
            }
            
        }
    }

    public override void AssignSlot(InventorySystem invToDisplay)
    {
        slotDictionary = new Dictionary<InventorySlots_UI, InventorySlot>();

        // Sicherheitscheck
        if (slots.Length + offset > invToDisplay.InventorySize) 
        {
            Debug.LogError($"Inventory Display Error auf {gameObject.name}: UI hat {slots.Length} Slots, aber System '{displayType}' hat nur {invToDisplay.InventorySize} Slots!");
            return; 
        }

        for (int i = 0; i < slots.Length; i++)
        {
            int systemIndex = i + offset;

            InventorySlot slotToDisplay = invToDisplay.InventorySlots[systemIndex];

            if (!slotDictionary.ContainsKey(slots[i]))
            {
                slotDictionary.Add(slots[i], slotToDisplay);
            }
            
            slots[i].Init(slotToDisplay, i);
        }
    }
}