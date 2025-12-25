using UnityEngine;

[System.Serializable]
public class InventorySaveData
{
    public string[] itemIDs;
}
public class Inventory : InventorySave
{
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
    }
    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
    }
}
