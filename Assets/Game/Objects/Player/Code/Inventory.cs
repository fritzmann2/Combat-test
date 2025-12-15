using UnityEngine;

public class Inventory : MonoBehaviour
{
    private GameControls controls;
    public GameObject[] inventory;
    private GameObject currentWeapon;
    public GameObject Handpoint;

    void OnEnable()
    {
        controls.Enable();
    }

    void OnDisable()
    {
        controls.Disable();
    }

    void Awake()
    {
        controls = new GameControls();
    }

    void FixedUpdate()
    {
        if (controls.Gameplay.OpenInventory.triggered)
        {
            ToggleInventory();
        }
        if (controls.Gameplay.SummonWeapon.triggered)
        {
            SummonWeapon();
        }
    }

    private void ToggleInventory()
    {
        Debug.Log("Toggling Inventory");
    }
    private void SummonWeapon()
    {
        if (currentWeapon == null && inventory.Length > 0)
        {
            currentWeapon = Instantiate(inventory[0], Handpoint.transform.position, Quaternion.identity, transform);
        }
        else if (currentWeapon != null)
        {
            Destroy(currentWeapon);
            currentWeapon = null;
        }
    }

}
