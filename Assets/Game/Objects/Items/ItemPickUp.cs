using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class ItemPickUp : NetworkBehaviour
{
    public float PickUpRadius = 1f;
    public InventoryItemData ItemData;

    private BoxCollider2D bx;
    private void Awake()
    {
        bx = GetComponent<BoxCollider2D>();
        bx.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var inventory = other.gameObject.GetComponent<InventoryHolder>();
        if  (!inventory)
        {
            return;
        }    
        
        if (inventory.InventorySystem.AddToInventory(ItemData, 1))
        {
            DestroythisServerRpc();
        }    
    }
    [ServerRpc]
    public void DestroythisServerRpc()
    {
        DestroythisClientRpc();
    }
    [ClientRpc]
    public void DestroythisClientRpc()
    {
        Destroy(gameObject);
    }
}
