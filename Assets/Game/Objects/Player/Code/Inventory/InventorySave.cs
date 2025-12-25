using Unity.Netcode;
using UnityEngine;
using System.IO;

public abstract class InventorySave : NetworkBehaviour
{
    private bool createsavepartbool = false;
    private string savePath ;
    public InventoryItem[] items;
    public InventoryItem[] equipeditems = new InventoryItem[10];

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;
        savePath = Path.Combine(Application.persistentDataPath, "inventory.json");
        bool issaved = loadinventory();
        if (issaved)
        {
            Debug.Log("Loaded");
        }
        else
        {
            Debug.LogWarning("Not loaded");
        }
    }
    public override void OnNetworkDespawn()
    {
        bool issaved = saveinventory();
        if (issaved)
        {
            Debug.Log("Saved");
        }
        else
        {
            Debug.LogWarning("Not saved");
        }
    }
    public bool loadinventory()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
        
            InventorySaveData data = JsonUtility.FromJson<InventorySaveData>(json);
            foreach (string itemID in data.itemIDs)
            {

                string[] splittedID = splitID(itemID);

            }
        return true;
        }
        else
        {
            if (createsavepartbool)
            {
                createsavepart();
            }
            else
            {
                return false;
            }
        }
        return false;
    }
    public bool saveinventory()
    {
        return true;
    }
    public void createsavepart()
    {
        InventorySaveData data = new InventorySaveData();
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(savePath, json);
        createsavepartbool = true;
        loadinventory();
    }
    public string[] splitID(string id)
    {
        return id.Split("_");
    }

}
