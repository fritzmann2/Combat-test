using UnityEngine;


[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class InventoryItemData : ScriptableObject
{
    public string ID;           // Z.B. "sword_01" (WICHTIG für Save/Load!)
    public string ItemName;
    public Sprite Icon;
    public Itemtype Type;
    public bool IsStackable;
    public int MaxStackSize;
    public GameObject itemObject;
}
