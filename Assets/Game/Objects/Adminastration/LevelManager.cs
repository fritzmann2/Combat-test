using UnityEngine;
using Unity.Netcode;

public class LevelManager : NetworkBehaviour
{
    [Header("Settings")]
    public GameObject playerPrefab; 

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        if (playerPrefab == null)
        {
            Debug.LogError(">>> LevelManager: FEHLER! Player Prefab fehlt im Inspector! <<<");
            return;
        }

        // --- HIER WAR DER FEHLER ---
        // Wir müssen dem NetworkManager sagen: "Wenn wer joint, ruf meine Methode auf!"
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        // ---------------------------

        // Spawne alle, die JETZT schon da sind (z.B. der Host selbst)
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            SpawnPlayer(clientId);
        }
    }

    public override void OnNetworkDespawn()
    {
        if (NetworkManager.Singleton != null && IsServer)
        {
            // Sauber abmelden, wenn das Level zerstört wird
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        Debug.Log($">>> Client {clientId} ist beigetreten! Spawne Player... <<<");
        SpawnPlayer(clientId);
    }

    private void SpawnPlayer(ulong clientId)
    {
        Vector3 spawnPos = new Vector3(0, 2, 0); 
        GameObject playerInstance = Instantiate(playerPrefab, spawnPos, Quaternion.identity);

        // Das ist korrekt so:
        playerInstance.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
    }
}