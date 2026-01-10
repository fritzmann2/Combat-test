using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;


public class Attackmanager : NetworkBehaviour
{
    [Header("Setup")]
    public GameObject weaponPrefabs; 
    public Transform handHolder;

    [Header("Input")]
    private GameControls controls; 

    private GameObject currentWeaponObject;
    private Weapon currentWeaponScript;
    private float animtime;

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
        // Waffe ausrüsten/ablegen
        if (controls.Gameplay.SummonWeapon.triggered)
        {
            bool shouldEquip = currentWeaponObject == null;
            EquipRequestServerRpc(shouldEquip ? 0 : -1); 
        }
        // Angriffe ausführen und abfrage welche
        if (currentWeaponScript != null)
        {
            if (controls.Gameplay.Attack1.triggered || controls.Gameplay.Attack2.triggered || controls.Gameplay.Attack3.triggered || controls.Gameplay.Attack4.triggered) 
            {
                currentWeaponScript.weaponstats = new Weaponstats
                {
                    weapondamage = 10f,
                    strength = 5f,
                    critChance = 20f,
                    critDamage = 50f
                };
            }
            if (controls.Gameplay.Attack1.triggered) currentWeaponScript.Attack1();
            if (controls.Gameplay.Attack2.triggered) currentWeaponScript.Attack2();
            if (controls.Gameplay.Attack3.triggered) currentWeaponScript.Attack3();
            if (controls.Gameplay.Attack4.triggered) currentWeaponScript.Attack4();
            if (controls.Gameplay.Attack1.triggered || controls.Gameplay.Attack2.triggered || controls.Gameplay.Attack3.triggered || controls.Gameplay.Attack4.triggered) 
            {
               animtime = currentWeaponScript.GetAnimationLength();
               Debug.Log("Animation time: " + animtime);
            }
        }
    }
    [ServerRpc]
    //Server sagen das eine Waffe ausgerüstet/abgelegt werden soll
    private void EquipRequestServerRpc(int weaponIndex)
    {
        if (currentWeaponObject != null)
        {
            Destroy(currentWeaponObject);
            currentWeaponObject = null;
            currentWeaponScript = null;
        }

        if (weaponIndex >= 0)
        {
            //Waffe spawnen
            GameObject newWeapon = Instantiate(weaponPrefabs, handHolder);
            var netObj = newWeapon.GetComponent<NetworkObject>();
            netObj.Spawn();
            netObj.TrySetParent(this.NetworkObject);
            netObj.transform.localPosition = handHolder.localPosition;


            currentWeaponObject = netObj.gameObject;
            currentWeaponScript = netObj.GetComponent<Weapon>();
            EquipClientRpc(netObj.NetworkObjectId);

        }
        else
        {
            Debug.Log("Hand wird sichtbar (keine Waffe).");
        }

    }

    [ClientRpc]
    private void EquipClientRpc(ulong weaponNetworkId)
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(weaponNetworkId, out NetworkObject weaponNetObj))
        {            
            currentWeaponObject = weaponNetObj.gameObject;
            currentWeaponScript = weaponNetObj.GetComponent<Weapon>();
            currentWeaponScript.SetFollowTarget(this.handHolder);
        }
    }

}
