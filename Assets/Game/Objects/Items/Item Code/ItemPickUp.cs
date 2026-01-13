using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class ItemPickUp : NetworkBehaviour
{
    public InventoryItemData[] ItemData;
    public int amount = 1;
    private BoxCollider2D bx;

    public void Awake()
    {
        bx = GetComponent<BoxCollider2D>();
        bx.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsServer) return;

        var holder = other.GetComponent<InventoryHolder>();
        
        if (holder != null)
        {   
            int itemRarity = 1;
            int randint = Random.Range(0, ItemData.Length);
//            Debug.Log("Item ID: " + ItemData[randint].ID + " wird aufgehoben.");
            holder.AddItemRequest(ItemData[randint].ID, amount, itemRarity);

            // Pickup entfernen
            GetComponent<NetworkObject>().Despawn();
        }
    }
}