using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using Unity.Netcode;
using System.Linq;
using System.IO;


#if UNITY_EDITOR
using UnityEditor; 
#endif
[System.Serializable]
public class InventoryHolder : NetworkBehaviour
{
    [Header("Datenbank")]
    public List<InventoryItemData> itemDatabase;
    [SerializeField] private int inventorySize;
    [SerializeField] private int equipmentSize;

    [SerializeField] protected InventorySystem inventorySystem;
    [SerializeField] protected InventorySystem equipedSlots;
    private GameControls controls;
    private MouseItemData mouseItemData;
    
    public InventorySystem InventorySystem => inventorySystem;
    public InventorySystem EquipedSlots => equipedSlots;

    public static UnityAction<InventorySystem> OnDynamicInventoryDisplayRequested;
    public static event UnityAction OnEquipmentChanged;

    private string SavePath => Path.Combine(Application.persistentDataPath, $"inventory_{OwnerClientId}.json");


    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        StaticInventoryDisplay[] allDisplays = FindObjectsByType<StaticInventoryDisplay>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        if (allDisplays.Length > 0)
        {
            foreach (var display in allDisplays)
            {
                display.ConnectToPlayer(this);
            }
        }
        else
        {
            Debug.LogWarning("Keine StaticInventoryDisplays in der Scene gefunden!");
        }
        //if (IsServer) LoadInventory();
        mouseItemData = FindFirstObjectByType<MouseItemData>(FindObjectsInactive.Include);
        mouseItemData.ItemChange += MoveItem;
    }
    public override void OnNetworkDespawn()
    {
        // Speicher Inventar beim Disconnect (Server-seitig)
        //if (IsServer) SaveInventory();
        base.OnNetworkDespawn();
    }
    private void Awake()
    {
        controls = new GameControls();
        if (controls == null)
        {
            Debug.LogWarning("cant create controls");
        }
        equipedSlots = new InventorySystem(equipmentSize);
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
        if (itemRarity == 0) Debug.LogError("Item Rarity is null!");
        if (amount == 0) Debug.LogWarning("Amount is null");
        // 1. Instanz erstellen
        InventoryItemInstance newItemInstance = new InventoryItemInstance(data);
        
        // Optional: Stats hinzufügen (Beispiel)
        if (newItemInstance.itemData.Type.ToString() == "Weapon")
        {
            newItemInstance.stats = new EquipmentStats();
            newItemInstance.stats.weaponstats = new Weaponstats();
            newItemInstance.stats.weaponstats.weapondamage = getStatbyrarity(itemRarity);
            newItemInstance.stats.weaponstats.strength = getStatbyrarity(itemRarity);
            newItemInstance.stats.weaponstats.critChance = getStatbyrarity(itemRarity);
            newItemInstance.stats.weaponstats.critDamage = getStatbyrarity(itemRarity);
        }        

        // 2. Ins Server-Inventar
        bool success = inventorySystem.AddToInventory(newItemInstance, amount);

        if (success)
        {
//            Debug.Log("Server: Item hinzugefügt.");
            
            AddItemClientRpc(itemID, amount, newItemInstance.stats);
        }
    }

    [ServerRpc]
    private void AddItemServerRpc(string itemID, int amount, int itemRarity)
    {
        AddItemServer(itemID, amount, itemRarity);
    }

    [ClientRpc]
    private void AddItemClientRpc(string itemID, int amount, EquipmentStats stats)
    {
        if (IsServer) return; 

        InventoryItemData data = GetItemFromID(itemID);
        
        if (data != null)
        {
            Debug.Log("Client: Item hinzugefügt " + data.ID);

            InventoryItemInstance clientInstance = new InventoryItemInstance(data);
            clientInstance.stats = stats;

            // Ins Client-Inventar
            inventorySystem.AddToInventory(clientInstance, amount);
        }
    }
    
    public InventoryItemData GetItemFromID(string id)
    {
        return itemDatabase.FirstOrDefault(x => x.ID == id);
    }

    private int getStatbyrarity(int itemRarity)
    {
        switch (itemRarity)
        {
            case 0:
                return Random.Range(1, 6); // Common
            case 1:
                return Random.Range(5, 11); // Uncommon
            case 2:
                return Random.Range(10, 16); // Rare
            case 3:
                return Random.Range(15, 21); // Epic
            case 4:
                return Random.Range(20, 26); // Legendary
            default:
                return 0; // Invalid rarity
        }
    }
 
    private void MoveItem(int index1, int index2, bool isSourceEq, bool isTargetEq)
{
    // 1. BESTIMMEN DER SYSTEME
    InventorySystem sourceSystem = isSourceEq ? equipedSlots : inventorySystem;
    InventorySystem targetSystem = isTargetEq ? equipedSlots : inventorySystem;

    // 2. SLOTS HOLEN
    InventorySlot slot1 = sourceSystem.InventorySlots[index1];
    InventorySlot slot2 = targetSystem.InventorySlots[index2];

    Debug.Log($"MoveItem: Von {sourceSystem} [{index1}] nach {targetSystem} [{index2}]");

    // 3. DATEN SICHERN
    InventoryItemInstance tempItem = slot1.InventoryItemInstance;
    int tempCount = slot1.StackSize;

    // 4. TAUSCH-LOGIK
    if (slot2.InventoryItemInstance != null)
    {
        InventoryItemInstance targetItem = slot2.InventoryItemInstance;
        int targetStack = slot2.StackSize;

        slot2.UpdateInventorySlot(tempItem, tempCount);  
        slot1.UpdateInventorySlot(targetItem, targetStack); 
    }
    else
    {
        slot2.UpdateInventorySlot(tempItem, tempCount);   
        slot1.clearSlot();                                
    }
    if (isSourceEq || isTargetEq)
        {
            OnEquipmentChanged?.Invoke();
        }
    // 5. EVENTS FEUERN
    
    sourceSystem.OnInventorySlotChanged?.Invoke(slot1);
    targetSystem.OnInventorySlotChanged?.Invoke(slot2);
}
    
// Save Methode
    public InventorySaveData saveinventorySystem()
    {       
        InventorySaveData saveData = new InventorySaveData();
        if (saveData.invslots == null) saveData.invslots = new List<InventorySlotSaveData>();
        for (int i = 0; i < inventorySystem.InventorySlots.Count; i++)
        {
            var slot = inventorySystem.InventorySlots[i];

            // 1. Prüfen: Ist der Slot überhaupt besetzt?
            if (slot.InventoryItemInstance == null) continue;

            // 2. Prüfen: Hat die Instanz überhaupt Daten?
            if (slot.InventoryItemInstance.itemData == null)
            {
                continue;
            }

            // 3. Prüfen: Sind die Stats null?
            if (slot.InventoryItemInstance.stats == null)
            {
                slot.InventoryItemInstance.stats = new EquipmentStats();
            }

            // Jetzt ist es sicher zu speichern
            saveData.invslots.Add(new InventorySlotSaveData(
                slot.InventoryItemInstance.itemData.ID, 
                slot.StackSize, 
                i,
                slot.InventoryItemInstance.stats 
            ));
        }
        if (saveData.equipmentSlots == null) saveData.equipmentSlots = new List<InventorySlotSaveData>();
        for (int i = 0; i < equipedSlots.InventorySlots.Count; i++)
        {
            var slot = equipedSlots.InventorySlots[i];

            // 1. Prüfen: Ist der Slot überhaupt besetzt?
            if (slot.InventoryItemInstance == null) continue;

            // 2. Prüfen: Hat die Instanz überhaupt Daten?
            if (slot.InventoryItemInstance.itemData == null)
            {
                continue;
            }

            // 3. Prüfen: Sind die Stats null?
            if (slot.InventoryItemInstance.stats == null)
            {
                slot.InventoryItemInstance.stats = new EquipmentStats();
            }

            // Jetzt ist es sicher zu speichern
            saveData.equipmentSlots.Add(new InventorySlotSaveData(
                slot.InventoryItemInstance.itemData.ID, 
                slot.StackSize, 
                i,
                slot.InventoryItemInstance.stats 
            ));
        }
        return saveData;
    }


    public void loadinventorySystem(InventorySaveData saveData)
    {
        // --- Lade Hauptinventar ---
        foreach (var slotData in saveData.invslots)
        {
            InventoryItemData template = GetItemFromID(slotData.itemID);

            if (template != null)
            {
                InventoryItemInstance instance = new InventoryItemInstance(template);
                instance.stats = slotData.savedStats;

                if (slotData.slotIndex < inventorySystem.InventorySize)
                {
                    inventorySystem.InventorySlots[slotData.slotIndex].UpdateInventorySlot(instance, slotData.amount);
                    
                }
            }
        }

        // --- Lade Equipment ---
        foreach (var slotData in saveData.equipmentSlots)
        {
            InventoryItemData template = GetItemFromID(slotData.itemID);

            if (template != null)
            {
                InventoryItemInstance instance = new InventoryItemInstance(template);
                instance.stats = slotData.savedStats;

                if (slotData.slotIndex < equipedSlots.InventorySize)
                {
                    equipedSlots.InventorySlots[slotData.slotIndex].UpdateInventorySlot(instance, slotData.amount);

                }
            }
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
