using Unity.Netcode;
using UnityEngine;

abstract class BaseMobClass : NetworkBehaviour
{
    public int health { get; set; }
    public int maxHealth { get; set;}
    public float movementSpeed { get; set; }
    public float attackSpeed { get; set; }
    public float critChance { get; set; }
    public float critDamage { get; set; }
    public float strength { get; set; }
    public float defense { get; set; }
    public float spellresistance { get; set; }
    public bool TakeDamage(float damage)
    {
        Debug.Log("Mob took " + damage + " damage.");
        AmIDead();
        return true;
    }
    public void AmIDead()
    {
        if(health <= 0)
        {
            Debug.Log("Mob is dead.");
        }
        if (this.tag == "Entety")
        {
            Destroy(this.gameObject);
        }
    }
}
