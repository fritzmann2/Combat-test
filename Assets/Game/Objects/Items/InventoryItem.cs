using UnityEngine;
using Unity.Netcode;


[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class InventoryItemData : ScriptableObject
{
    public string ID;           // Z.B. "sword_01" (WICHTIG für Save/Load!)
    public string ItemName;
    public Sprite Icon;
    public Itemtype Type;
    public bool IsStackable;
    public int MaxStackSize;
}

abstract public class InventoryItem : NetworkBehaviour
{
    public InventoryItemData itemData;
    public string Name { get; set;}
    public Itemtype ItemType{ get; set;}
    public string itemID { get; set;}
    public Sprite basesprite;
    public bool isStackable { get; set;}
    protected virtual void Awake()
    {
        //setupData();
    }
    virtual public void setupData()
    {
        Name = itemData.ItemName;
        ItemType = itemData.Type;
        itemID = itemData.ID;
        basesprite = itemData.Icon;
        isStackable = itemData.IsStackable;
    }
}
