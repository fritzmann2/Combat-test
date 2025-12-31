using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using Unity.Netcode;
using System.Linq;
using System.IO;
using System.Text;
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
    private GameControls controls;
    private StaticInventoryDisplayInv inventoryUI;
    private StaticInventoryDisplayHot hotbarUI;
    
    public InventorySystem InventorySystem => inventorySystem;

    public static UnityAction<InventorySystem> OnDynamicInventoryDisplayRequested;
    private string SavePath => Path.Combine(Application.persistentDataPath, $"inventory_{OwnerClientId}.json");


    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        hotbarUI = FindFirstObjectByType<StaticInventoryDisplayHot>(FindObjectsInactive.Include);
        inventoryUI = FindFirstObjectByType<StaticInventoryDisplayInv>(FindObjectsInactive.Include);
        if (inventoryUI == null) Debug.LogError("Kein StaticInventoryDisplayInv in der Scene gefunden!");
        if (hotbarUI == null) Debug.LogError("Kein StaticInventoryDisplayHot in der Scene gefunden!");
        if (hotbarUI != null && inventoryUI != null)
        {
            hotbarUI.ConnectToPlayer(this);
            inventoryUI.ConnectToPlayer(this);
            
        }
        else
        {
            Debug.LogError("Kein StaticInventoryDisplay in der Scene gefunden!");
        }
        if (IsServer) LoadInventory();
    }
    public override void OnNetworkDespawn()
    {
        // Speicher Inventar beim Disconnect (Server-seitig)
        if (IsServer) SaveInventory();
        base.OnNetworkDespawn();
    }
    private void Awake()
    {
        controls = new GameControls();
        if (controls == null)
        {
            Debug.LogWarning("cant create controls");
        }
        inventorySystem = new InventorySystem(inventorySize);
    }
    void OnEnable()
    {
        controls.Enable();
    }

    void OnDisable()
    {
        controls.Disable();
    }


    public void AddItemRequest(string itemID, int amount, int itemRarity)
    {
        if (IsServer) AddItemServer(itemID, amount, itemRarity);
        else AddItemServerRpc(itemID, amount, itemRarity);
    }
    
    private void AddItemServer(string itemID, int amount, int itemRarity)
    {
        InventoryItemData data = GetItemFromID(itemID);
        if (data == null) return;

        // 1. Instanz erstellen
        InventoryItemInstance newItemInstance = new InventoryItemInstance(data);
        
        // Optional: Stats hinzufügen (Beispiel)
        // newItemInstance.stats.Add(new ItemParameter("Durability", 100));

        // 2. Ins Server-Inventar
        bool success = inventorySystem.AddToInventory(newItemInstance, amount);

        if (success)
        {
            Debug.Log("Server: Item hinzugefügt.");

            // 3. WICHTIG: Liste in Array umwandeln für den Transport
            ItemParameter[] statsArray = newItemInstance.stats.ToArray();

            // 4. Jetzt können wir das Array direkt senden!
            AddItemClientRpc(itemID, amount, statsArray);
        }
    }

    [ServerRpc]
    private void AddItemServerRpc(string itemID, int amount, int itemRarity)
    {
        AddItemServer(itemID, amount, itemRarity);
    }

    [ClientRpc]
    private void AddItemClientRpc(string itemID, int amount, ItemParameter[] statsArray)
    {
        if (IsServer) return; 

        InventoryItemData data = GetItemFromID(itemID);
        
        if (data != null)
        {
            Debug.Log("Client: Item hinzugefügt " + data.ID);

            // Instanz erstellen
            InventoryItemInstance clientInstance = new InventoryItemInstance(data);

            // Array zurück in Liste wandeln
            if (statsArray != null)
            {
                clientInstance.stats = new List<ItemParameter>(statsArray);
            }

            // Ins Client-Inventar
            inventorySystem.AddToInventory(clientInstance, amount);
        }
    }
    
    public InventoryItemData GetItemFromID(string id)
    {
        return itemDatabase.FirstOrDefault(x => x.ID == id);
    }

    
 

    // Save System
    public void SaveInventory()
    {
        if (!IsServer) return;

        InventorySaveData saveData = new InventorySaveData();
        // WICHTIG: Sicherstellen, dass die Liste existiert (siehe Ursache 3)
        if (saveData.slots == null) saveData.slots = new List<InventorySlotSaveData>();

        for (int i = 0; i < inventorySystem.InventorySlots.Count; i++)
        {
            var slot = inventorySystem.InventorySlots[i];

            // 1. Prüfen: Ist der Slot überhaupt besetzt?
            if (slot.InventoryItemInstance == null) continue;

            // 2. Prüfen: Hat die Instanz überhaupt Daten? (Behebt Ursache 1)
            if (slot.InventoryItemInstance.itemData == null)
            {
//                Debug.LogWarning($"Slot {i} hat eine Instanz, aber kein ItemData (ScriptableObject fehlt)! Wird nicht gespeichert.");
                continue;
            }

            // 3. Prüfen: Sind die Stats null? (Behebt Ursache 2)
            if (slot.InventoryItemInstance.stats == null)
            {
                slot.InventoryItemInstance.stats = new List<ItemParameter>();
            }

            // Jetzt ist es sicher zu speichern
            saveData.slots.Add(new InventorySlotSaveData(
                slot.InventoryItemInstance.itemData.ID, 
                slot.StackSize, 
                i,
                slot.InventoryItemInstance.stats 
            ));
        }

        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(SavePath, json);
        Debug.Log($"Inventar gespeichert in: {SavePath}");
    }

    public void LoadInventory()
    {
        if (!IsServer || !File.Exists(SavePath)) return;

        try 
        {
            string json = File.ReadAllText(SavePath);
            InventorySaveData saveData = JsonUtility.FromJson<InventorySaveData>(json);

            foreach (var slotData in saveData.slots)
            {
                // 1. Suche ScriptableObject
                InventoryItemData template = GetItemFromID(slotData.itemID);

                if (template != null)
                {
                    // 2. Erstelle Instanz
                    InventoryItemInstance instance = new InventoryItemInstance(template);
                    // 3. Fülle Stats zurück
                    instance.stats = slotData.savedStats;

                    // 4. Ab ins Inventar
                    inventorySystem.AddToInventory(instance, slotData.amount);
                }
            }
            Debug.Log("Inventar geladen.");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Fehler beim Laden: " + e.Message);
        }
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
