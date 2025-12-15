using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class Inventory : NetworkBehaviour
{
    private GameControls controls;
    
    [Header("Settings")]
    public GameObject[] inventory; 
    public GameObject Handpoint;

    private NetworkObject currentWeaponNetObj; 

    void Awake()
    {
        controls = new GameControls();
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            controls.Enable();
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsOwner)
        {
            controls.Disable();
        }
    }

    void Update()
    {
        if (!IsOwner) return;

        if (controls.Gameplay.OpenInventory.triggered)
        {
            ToggleInventory();
        }
        
        if (controls.Gameplay.SummonWeapon.triggered)
        {
            RequestWeaponToggleServerRpc();
        }
    }

    private void ToggleInventory()
    {
        Debug.Log("Toggling Inventory (Lokal)");
    }


    [ServerRpc]
    private void RequestWeaponToggleServerRpc()
    {

        if (currentWeaponNetObj == null)
        {
            if (inventory.Length > 0 && inventory[0] != null)
            {
                Debug.Log("Server: Spawne Waffe...");
                
                GameObject weaponInstance = Instantiate(inventory[0], Handpoint.transform.position, Handpoint.transform.rotation);
                
                NetworkObject netObj = weaponInstance.GetComponent<NetworkObject>();
                
                netObj.SpawnWithOwnership(OwnerClientId);

                netObj.TrySetParent(Handpoint.transform);

                currentWeaponNetObj = netObj;
            }
        }
        else
        {
            Debug.Log("Server: Entferne Waffe...");
            
            currentWeaponNetObj.Despawn(); 
            currentWeaponNetObj = null;
        }
    }
}