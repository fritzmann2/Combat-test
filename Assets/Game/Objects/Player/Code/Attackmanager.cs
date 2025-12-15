using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;


public class Attackmanager : NetworkBehaviour
{
    [Header("Setup")]
    public GameObject[] weaponPrefabs; 
    public Transform handHolder;
    public Renderer handMeshRenderer;

    [Header("Input")]
    private GameControls controls; 

    // Interner Status
    private GameObject currentWeaponObject;
    private Weapon currentWeaponScript;

    void Awake()
    {
        controls = new GameControls();
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner) controls.Enable();
    }

    public override void OnNetworkDespawn()
    {
        if (IsOwner) controls.Disable();
    }
    void Update()
    {
        if (!IsOwner) return;

        if (controls.Gameplay.SummonWeapon.triggered)
        {
            bool shouldEquip = currentWeaponObject == null;
            EquipRequestServerRpc(shouldEquip ? 0 : -1); 
        }

        if (currentWeaponScript != null)
        {
            if (controls.Gameplay.Attack1.triggered) currentWeaponScript.Attack1();
            if (controls.Gameplay.Attack2.triggered) currentWeaponScript.Attack2();
            if (controls.Gameplay.Attack3.triggered) currentWeaponScript.Attack3();
        }
    }
    [ServerRpc]
    private void EquipRequestServerRpc(int weaponIndex)
    {
        // Der Server sagt allen Clients: "Ändert die Waffe!"
        EquipClientRpc(weaponIndex);
    }

    [ClientRpc]
    private void EquipClientRpc(int weaponIndex)
    {
        // 1. Aufräumen: Altes Schwert löschen
        if (currentWeaponObject != null)
        {
            Destroy(currentWeaponObject);
            currentWeaponObject = null;
            currentWeaponScript = null;
        }

        // 2. Entscheiden: Waffe anlegen oder Hand zeigen?
        if (weaponIndex >= 0 && weaponIndex < weaponPrefabs.Length)
        {
            // --- MODUS: WAFFE ---
            
            // A. Hand unsichtbar machen (damit es aussieht, als wurde sie zum Schwert)
            if (handMeshRenderer != null) handMeshRenderer.enabled = false;

            // B. Schwert spawnen (Lokal)
            GameObject newWeapon = Instantiate(weaponPrefabs[weaponIndex], handHolder);
            
            // C. Position resetten (damit es im Griff sitzt)
            newWeapon.transform.localPosition = Vector3.zero;
            newWeapon.transform.localRotation = Quaternion.identity;

            // D. Referenzen speichern
            currentWeaponObject = newWeapon;
            currentWeaponScript = newWeapon.GetComponent<Weapon>();
        }
        else
        {
            // --- MODUS: LEERE HAND ---
            
            // Hand wieder sichtbar machen
            if (handMeshRenderer != null) handMeshRenderer.enabled = true;
        }
    }
}
