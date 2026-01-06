using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class InventorySlots_UI : MonoBehaviour
{
    [SerializeField] private Image itemSprite;
    [SerializeField] private TextMeshProUGUI itemCount;
    [SerializeField] private InventorySlot assignedInventorySlot;
    private int index {get; set; }

    private Button button;

    public InventorySlot AssignedInventorySlot => assignedInventorySlot;
    public InventoryDisplay ParentDisplay{ get; private set; }

    private void Awake()
    {
        //ClearSlot();

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
            Debug.Log("InventoryItemInstance ist null");
            ClearSlot();
            return;
        }
        if (slot.InventoryItemInstance.itemData == null)
        {
            Debug.Log("InventoryItemData ist null");
        }
        
        Debug.Log("Update Slot");
        if (slot.InventoryItemInstance.itemData != null)
        {
            itemSprite.sprite = slot.InventoryItemInstance.itemData.Icon;
            itemSprite.color = Color.white;
            if (slot.StackSize > 1) itemCount.text = slot.StackSize.ToString();
            else itemCount.text = "";
            Debug.Log("item slot set");
        }

        else
        {
            ClearSlot();
        }
    }

    public void UpdateUISlot()
    {
        if (assignedInventorySlot.InventoryItemInstance.itemData != null) UpdateUISlot(assignedInventorySlot);
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
        ParentDisplay?.SlotClicked(this, index);
    }
}
