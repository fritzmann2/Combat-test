using UnityEngine;
using Unity.Netcode;

public class PlayerSaveHandler : NetworkBehaviour
{
    [SerializeField] private string playerName = "Fritzmann"; // Muss beim Login gesetzt werden
    
    private InventoryHolder inventoryHolder;
    private PlayerStats playerStats;

    private void Awake()
    {
        inventoryHolder = GetComponent<InventoryHolder>();
        playerStats = GetComponent<PlayerStats>();
    }

    public void SetPlayerName(string name)
    {
        playerName = name;
    }

    // Diese Methode packt alles zusammen
    public GameSaveData CollectAllData()
    {
        GameSaveData data = new GameSaveData();
        data.playerName = playerName;

        // 1. Inventar holen 
        if (inventoryHolder != null)
        {
            data.inventoryData = inventoryHolder.saveinventorySystem(); 
        }

        // 2. Stats holen
        if (playerStats != null)
        {
            // Du brauchst eine ähnliche Methode in PlayerStats, die die Daten zurückgibt
            data.statsData = playerStats.GetSaveData(); 
        }

        // 3. Position (Optional)
        data.position = new Vector3SaveData(transform.position);

        return data;
    }
}