using UnityEngine;
using Unity.Netcode;
using UnityEngine.Events;

public class PlayerSaveHandler : NetworkBehaviour
{
    [SerializeField] private string playerName = "Fritzmann"; 
    public event UnityAction dataLoaded;   
    private InventoryHolder inventoryHolder;
    // private PlayerStats playerStats; // Stats einkommentieren wenn fertig

    private void Awake()
    {
        inventoryHolder = GetComponent<InventoryHolder>();
        // playerStats = GetComponent<PlayerStats>();
    }

    public void SetPlayerName(string name)
    {
        playerName = name;
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {

            //Debug.Log($"Client Login: Fordere Daten an für '{playerName}'");
            
            // WICHTIG: Wir schicken den Namen an den Server!
            RequestLoadDataServerRpc(playerName);
        }
    }

    // Server-Methode: Wird vom Client aufgerufen
    [ServerRpc]
    private void RequestLoadDataServerRpc(string nameSentByClient)
    {
//        Debug.Log($"Server: Login Anfrage erhalten für '{nameSentByClient}'");

        // Ohne das würde OnNetworkDespawn später "Fritzmann" speichern statt "Hans".
        this.playerName = nameSentByClient;
        
        // Name zur Übersicht im Editor ändern
        gameObject.name = $"Player_{nameSentByClient}";

        // 2. Daten laden
        if (SaveManager.Instance != null)
        {
            GameSaveData loadedData = SaveManager.Instance.LoadPlayerData(nameSentByClient);
            if (loadedData != null)
            {
                ApplyData(loadedData);
            }
        }
    }

    private void ApplyData(GameSaveData data)
    {
        // Daten in die Komponenten schreiben
        if (inventoryHolder != null && data.inventoryData != null)
        {
            inventoryHolder.loadinventorySystem(data.inventoryData);
        }
        
        
        transform.position = new Vector3(data.position.x, data.position.y, data.position.z);
        dataLoaded?.Invoke();
    }

    public override void OnNetworkDespawn()
    {
        // Server-Only Logic
        if (IsServer && !string.IsNullOrEmpty(playerName))
        {
            Debug.Log($"Spieler '{playerName}' despawnt. Speichere...");
            
            GameSaveData dataToSave = CollectAllData();

            if (SaveManager.Instance != null)
            {
                SaveManager.Instance.SaveGameData(dataToSave);
            }
        }

        base.OnNetworkDespawn();
    }

    public GameSaveData CollectAllData()
    {
        GameSaveData data = new GameSaveData();
        data.playerName = playerName; // Hier wird jetzt der korrekte Name genommen!

        if (inventoryHolder != null)
        {
            data.inventoryData = inventoryHolder.saveinventorySystem(); 
        }
        
        // Stats und Position hinzufügen wie gehabt
        return data;
    }
}