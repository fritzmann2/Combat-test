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
        
        if (controls.Gameplay.SummonWeapon.triggered)
        {
            bool shouldEquip = currentWeaponObject == null;
            EquipRequestServerRpc(shouldEquip ? 0 : -1); 
        }

        if (currentWeaponScript != null)
        {
            if (controls.Gameplay.Attack1.triggered || controls.Gameplay.Attack2.triggered || controls.Gameplay.Attack3.triggered) 
            {
                currentWeaponScript.setstatsweapon( 5f, 1f, 10f, 50f);
            }
            if (controls.Gameplay.Attack1.triggered) currentWeaponScript.Attack1();
            if (controls.Gameplay.Attack2.triggered) currentWeaponScript.Attack2();
            if (controls.Gameplay.Attack3.triggered) currentWeaponScript.Attack3();
            if (controls.Gameplay.Attack1.triggered || controls.Gameplay.Attack2.triggered || controls.Gameplay.Attack3.triggered) 
            {
               animtime = currentWeaponScript.GetAnimationLength();
               Debug.Log("Animation time: " + animtime);
            }
        }
    }
    [ServerRpc]
    private void EquipRequestServerRpc(int weaponIndex)
    {
        EquipClientRpc(weaponIndex);
    }

    [ClientRpc]
    private void EquipClientRpc(int weaponIndex)
    {
        if (currentWeaponObject != null)
        {
            Destroy(currentWeaponObject);
            currentWeaponObject = null;
            currentWeaponScript = null;
        }

        if (weaponIndex >= 0)
        {
            GameObject newWeapon = Instantiate(weaponPrefabs, handHolder);
            
            newWeapon.transform.localPosition = Vector3.zero;

            currentWeaponObject = newWeapon;
            currentWeaponScript = newWeapon.GetComponent<Weapon>();
        }
        else
        {
            Debug.Log("Hand wird sichtbar (keine Waffe).");
        }
    }
}
