using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class InventorySlots_UI : MonoBehaviour
{
    [SerializeField] private Image itemSprite;
    [SerializeField] private TextMeshProUGUI itemCount;
    [SerializeField] private InventorySlot assignedInventorySlot;
    public static event UnityAction OnClear;

    private int index {get; set; }

    private Button button;

    public InventorySlot AssignedInventorySlot => assignedInventorySlot;
    public InventoryDisplay ParentDisplay{ get; private set; }

    private void Awake()
    {
        ClearSlot();

        ParentDisplay = transform.parent.GetComponent<InventoryDisplay>();

        button = GetComponent<Button>();
        button?.onClick.AddListener(OnUISlotClick);

    }


    public virtual void Init(InventorySlot slot, int index)
    {
        assignedInventorySlot = slot;
        this.index = index;
        UpdateUISlot(slot);
    }

    public void UpdateUISlot(InventorySlot slot)
    {
        if (slot == null)
        {
            Debug.Log("Slot ist null");
        }
        if (slot.InventoryItemInstance == null)
        {
            ClearSlot();
            return;
        }
        if (slot.InventoryItemInstance.itemData == null)
        {
            Debug.Log("InventoryItemData ist null");
        }
        
        if (slot.InventoryItemInstance.itemData != null)
        {
            Debug.Log("Try to update slot");
            itemSprite.sprite = slot.InventoryItemInstance.itemData.Icon;
            itemSprite.color = Color.white;
            if (slot.StackSize > 1) itemCount.text = slot.StackSize.ToString();
            else itemCount.text = "";
        }

        else
        {
            ClearSlot();
        }
    }

    public virtual void UpdateUISlot()
    {
        if (assignedInventorySlot != null) 
        {
            OnClear.Invoke();
            Debug.Log("UpdateUISlot called");
            UpdateUISlot(assignedInventorySlot);
        }
    }

    public void ClearSlot()
    {
        //assignedInventorySlot?.clearSlot();
        itemSprite.sprite = null;
        itemSprite.color = Color.clear;
        itemCount.text = "";
    }
    public void OnUISlotClick()
    {
//        Debug.Log("UI Slot clicked");
        if( ParentDisplay == null) Debug.Log("ParentDisplay is null");
        ParentDisplay?.SlotClicked(this, index);
    }
}
