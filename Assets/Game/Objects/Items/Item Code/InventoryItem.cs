using UnityEngine;
using Unity.Netcode;




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
    /*
    virtual public void setupData()
    {
        Name = itemData.ItemName;
        ItemType = itemData.Type;
        itemID = itemData.ID;
        basesprite = itemData.Icon;
        isStackable = itemData.IsStackable;
    }
    */
}
