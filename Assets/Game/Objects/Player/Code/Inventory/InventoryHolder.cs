using NUnit.Framework;
using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using Unity.Netcode;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor; 
#endif
[System.Serializable]
public class InventoryHolder : NetworkBehaviour
{
    [Header("Datenbank")]
    public List<InventoryItemData> itemDatabase;
    [SerializeField] private int inventorySize;
    [SerializeField] protected InventorySystem inventorySystem;
    public InventorySystem InventorySystem => inventorySystem;

    public static UnityAction<InventorySystem> OnDynamicInventoryDisplayRequested;


    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        var display = FindFirstObjectByType<StaticInventoryDisplay>();
        if (display != null)
        {
            display.ConnectToPlayer(this);
        }
        else
        {
            Debug.LogError("Kein StaticInventoryDisplay in der Scene gefunden!");
        }
    }
    private void Awake()
    {
        inventorySystem = new InventorySystem(inventorySize);
    }

    public void AddItemRequest(string itemID, int amount)
    {
        if (IsServer)
        {
            AddItemServer(itemID, amount);
        }
        else
        {
            AddItemServerRpc(itemID, amount);
        }
    }
    
    private void AddItemServer(string itemID, int amount)
    {
        InventoryItemData data = GetItemFromID(itemID);
        if (data == null) return;

        bool success = inventorySystem.AddToInventory(data, amount);

        if (success)
        {
            Debug.Log("Server: Item hinzugefügt.");
            AddItemClientRpc(itemID, amount);
        }
    }

    [ServerRpc]
    private void AddItemServerRpc(string itemID, int amount)
    {
        AddItemServer(itemID, amount);
    }

    [ClientRpc]
    private void AddItemClientRpc(string itemID, int amount)
    {
        if (IsServer) return;

        InventoryItemData data = GetItemFromID(itemID);
        
        if (data != null)
        {
            Debug.Log("Client: Item hinzugefügt." + data.ID);
            inventorySystem.AddToInventory(data, amount);
        }
    }
    
    public InventoryItemData GetItemFromID(string id)
    {
        return itemDatabase.FirstOrDefault(x => x.ID == id);
    }

    #if UNITY_EDITOR
    [ContextMenu("Auto Fill Database")]
    public void AutoFillDatabase()
    {
        int i = 0;
        itemDatabase = new List<InventoryItemData>();
        string[] guids = AssetDatabase.FindAssets("t:InventoryItemData");
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            InventoryItemData item = AssetDatabase.LoadAssetAtPath<InventoryItemData>(path);
            if(item != null) itemDatabase.Add(item);
            i++;
        }
        Debug.Log("Added " + i + " items to database.");
        EditorUtility.SetDirty(this);
    }
    #endif
}
