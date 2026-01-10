using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using UnityEngine.InputSystem;

public class MouseItemData : MonoBehaviour
{
    public Image ItemSprite;
    public TextMeshProUGUI itemCount;
    public InventorySlots_UI inventorySlot;
    private int indexslot;
    private bool hasitem = false;
    public event UnityAction<int, int> ItemChange;
    public event UnityAction<int, int> EquipRequest;

    void Awake() 
    {
        ItemSprite.color = Color.clear;
        itemCount.text = null;
    }

    public void clickedOnInventorySlot(InventorySlots_UI clickedSlot, int index)
    {
        if (clickedSlot is EquipmentSlot eqSlot && hasitem)
        {
            Debug.Log("Equip request sent");
            EquipRequest?.Invoke(indexslot, index);
            return;
        }
        if (clickedSlot.AssignedInventorySlot.InventoryItemInstance == null) 
        {
            Debug.Log("Error Slot is empty");
            return;
        }
        if (inventorySlot == null && clickedSlot.AssignedInventorySlot.InventoryItemInstance.itemData != null)
        {
            Debug.Log("MouseItemData: Slot aufgenommen");
            inventorySlot = clickedSlot;
            indexslot = index;
            ItemSprite.sprite = inventorySlot.AssignedInventorySlot.InventoryItemInstance.itemData.Icon;
            itemCount.text = inventorySlot.AssignedInventorySlot.StackSize.ToString();
            ItemSprite.color = Color.white;
            hasitem = true;

        }
        else
        {
            ItemChange?.Invoke(indexslot, index);
            inventorySlot = null;
            ItemSprite.sprite = null;
            ItemSprite.color = Color.clear;
            itemCount.text = null;
            hasitem = false;
        }
    }
    void Update()
    {
        if (inventorySlot != null)
        {
            followmouse();
        }
    }

    private void followmouse()
    {
        if (Mouse.current == null) return;

        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            transform.parent.GetComponent<RectTransform>(),
            mousePos, 
            null, 
            out pos);
        
        transform.localPosition = pos;
    }
}
