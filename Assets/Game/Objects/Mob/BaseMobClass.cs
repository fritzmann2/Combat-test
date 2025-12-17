using Unity.Netcode;
using UnityEngine;

abstract public class BaseEntety : NetworkBehaviour
{
    public NetworkVariable<float> health = new NetworkVariable<float>();

    public int maxHealth;
    public BoxCollider2D bx;

    virtual public void Awake()
    {
        bx = GetComponent<BoxCollider2D>();
    }
    // Schaden nehmen
    public virtual void TakeDamage(float damage)
    {
        // 1. Check: Läuft das Netzwerk überhaupt schon?
        if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsListening)
        {
            Debug.LogWarning("Kein Server/Client gestartet! Schlag ignoriert.");
            return;
        }

        // 2. Check: Ist DIESES Map-Objekt schon synchronisiert?
        if (!IsSpawned) 
        {
            Debug.LogWarning("Map-Objekt ist noch nicht gespawnt! (Warte auf Sync)");
            return;
        }
        TakeDamageServerRpc((int)damage);
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    public void TakeDamageServerRpc(int damage)
    {
        Debug.Log("Mob took " + damage + " damage.");
        health.Value -= damage;
        AmIDead();
        Debug.Log(health + " HP remains");
    }
    //Todesabfrage
    public void AmIDead()
    {
        if(health.Value <= 0)
        {
            Debug.Log("Mob is dead.");
        }
        if (this.tag == "Entety")
        {
            GetComponent<NetworkObject>().Despawn();
        }
    }
}

abstract class BaseMobClass : BaseEntety
{
    public float movementSpeed { get; set; }
    public float attackSpeed { get; set; }
    public float critChance { get; set; }
    public float critDamage { get; set; }
    public float strength { get; set; }
    public float defense { get; set; }
    public float spellresistance { get; set; }    
}
