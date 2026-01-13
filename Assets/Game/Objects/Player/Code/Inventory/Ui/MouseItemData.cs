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
    private bool eqfirst = false;
    private bool eqsecond = false;
    private bool invoke = false;
    public event UnityAction<int, int, bool, bool> ItemChange;
    private void OnEnable()
    {
        InventorySlots_UI.OnClear += ClearSlot;
    }

    private void OnDisable()
    {
        InventorySlots_UI.OnClear -= ClearSlot;
    }

    void Awake() 
    {
        ItemSprite.color = Color.clear;
        itemCount.text = null;
    }

    public void clickedOnInventorySlot(InventorySlots_UI clickedSlot, int index)
    {
        if (hasitem)
        {
            invoke = true;
        }
        if (clickedSlot is EquipmentSlot && hasitem)
        {
            eqsecond = true;
        }
        else if (!(clickedSlot is EquipmentSlot) && hasitem)
        {
            eqsecond = false;
        }
        else if (clickedSlot is EquipmentSlot && !hasitem)
        {
            Setitem(clickedSlot, index);
            eqfirst = true;
            hasitem = true;
        }
        else if (!(clickedSlot is EquipmentSlot) && !hasitem)
        {
            Setitem(clickedSlot, index);
            eqfirst = false;
            hasitem = true;
        }
        if (invoke)
        {
            ItemChange?.Invoke(indexslot, index, eqfirst, eqsecond);
            return;
        }
    }


    private void ClearSlot()
    {
        eqfirst = false;
        eqsecond = false;
        invoke = false;
        inventorySlot = null;
        ItemSprite.sprite = null;
        ItemSprite.color = Color.clear;
        itemCount.text = null;
        hasitem = false;
    }

    private void Setitem(InventorySlots_UI slot, int index)
    {
        inventorySlot = slot;
        indexslot = index;
        
        ItemSprite.sprite = inventorySlot.AssignedInventorySlot.InventoryItemInstance.itemData.Icon;
        itemCount.text = inventorySlot.AssignedInventorySlot.StackSize.ToString();
        ItemSprite.color = Color.white;
        hasitem = true;
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
