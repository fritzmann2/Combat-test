using UnityEngine;
using Unity.Netcode;
using System.IO;

public class SaveManager : NetworkBehaviour
{
    public static SaveManager Instance { get; private set; }

    private void Awake()
    {
        // Singleton Setup (etwas robuster für Clients)
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Wird aufgerufen, wenn das Objekt zerstört wird (z.B. Server stoppt / Spiel beendet)
    public override void OnNetworkDespawn()
    {
        if (IsServer)
        {
            Debug.Log("Server fährt herunter. Speichere alle Spieler...");
            SaveAllConnectedPlayers();
        }
        base.OnNetworkDespawn();
    }

    // Alternativ: Wenn man das Fenster schließt
    private void OnApplicationQuit()
    {
        if (IsServer || (NetworkManager.Singleton != null && NetworkManager.Singleton.IsServer))
        {
            SaveAllConnectedPlayers();
        }
    }

    [ContextMenu("Save All Manually")] // Zum Testen im Editor
    public void SaveAllConnectedPlayers()
    {
        if (NetworkManager.Singleton == null) return;

        // Wir iterieren durch alle verbundenen Clients
        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            if (client.PlayerObject == null) continue;

            // Wir suchen unser Handler-Script auf dem Player
            PlayerSaveHandler playerHandler = client.PlayerObject.GetComponent<PlayerSaveHandler>();

            if (playerHandler != null)
            {
                // Daten einsammeln
                GameSaveData completeData = playerHandler.CollectAllData();
                
                // Speichern
                WriteToFile(completeData);
            }
        }
    }

    private void WriteToFile(GameSaveData data)
    {
        // Dateinamen bereinigen, damit keine ungültigen Zeichen drin sind
        string safeName = string.Join("_", data.playerName.Split(Path.GetInvalidFileNameChars()));
        
        string fileName = $"save_{safeName}.json";
        string path = Path.Combine(Application.persistentDataPath, fileName);

        try 
        {
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(path, json);
            Debug.Log($"<color=green>Gespeichert:</color> {data.playerName} unter {path}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Fehler beim Speichern von {data.playerName}: {e.Message}");
        }
    }
}

[System.Serializable]
public class GameSaveData
{
    public string playerName;
    public InventorySaveData inventoryData;
    public PlayerStatsSaveData statsData; 
    public Vector3SaveData position;
}

[System.Serializable]
public struct Vector3SaveData
{
    public float x, y, z;
    public Vector3SaveData(UnityEngine.Vector3 vec) { x = vec.x; y = vec.y; z = vec.z; }
}