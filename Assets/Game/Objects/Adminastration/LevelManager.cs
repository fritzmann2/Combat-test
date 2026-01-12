using UnityEngine;
using Unity.Netcode;
using System.Collections; // Wichtig für Coroutines

public class LevelManager : NetworkBehaviour
{
    [Header("Settings")]
    public GameObject playerPrefab;
    public float clientSpawnDelay = 0.5f;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        if (playerPrefab == null)
        {
            Debug.LogError(">>> LevelManager: FEHLER! Player Prefab fehlt im Inspector! <<<");
            return;
        }

        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;

        // Für Spieler, die beim Server-Start schon da sind (z.B. der Host)
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            // Host spawnen wir meistens sofort, aber du kannst auch hier das Delay nutzen
            // Wenn du beim Host KEIN Delay willst, nutze direkt SpawnPlayer(clientId)
            StartCoroutine(SpawnPlayerWithDelay(clientId)); 
        }
    }

    public override void OnNetworkDespawn()
    {
        if (NetworkManager.Singleton != null && IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        Debug.Log($">>> Client {clientId} verbunden. Starte Verzögerung... <<<");
        // Hier starten wir die Warteschleife
        StartCoroutine(SpawnPlayerWithDelay(clientId));
    }

    // --- DIE NEUE COROUTINE ---
    private IEnumerator SpawnPlayerWithDelay(ulong clientId)
    {
        // 1. Warten
        yield return new WaitForSeconds(clientSpawnDelay);

        // 2. Sicherheits-Check: Ist der Client nach der Wartezeit überhaupt noch da?
        // Es kann passieren, dass er disconnected, während wir warten.
        if (NetworkManager.Singleton.ConnectedClients.ContainsKey(clientId))
        {
//            Debug.Log($">>> Spawne jetzt Spieler für Client {clientId} <<<");
            SpawnPlayer(clientId);
        }
        else
        {
            Debug.LogWarning($"Client {clientId} hat während des Delays die Verbindung verloren.");
        }
    }

    private void SpawnPlayer(ulong clientId)
    {
        Vector3 spawnPos = new Vector3(0, 2, 0); 
        GameObject playerInstance = Instantiate(playerPrefab, spawnPos, Quaternion.identity);

        playerInstance.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
    }
    
}