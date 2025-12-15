using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using Unity.Netcode;
using Unity.VisualScripting;


public enum AttackType
{
    Stab,   
    Slash,
    Charge
}
abstract class Weapon : NetworkBehaviour
{
    public Animator anim;
    public float weapondamage { get; set; }
    public float attacktypemultiplier { get; set; }
    public float damage { get; set; }
    public float strength { get; set; }
    public float attackSpeed { get; set; }
    public float critChance { get; set; }
    public float critDamage { get; set; }
    public Collider2D bx;
 
    virtual public void Attack1()
    {}
    virtual public void Attack2()
    {}
    virtual public void Attack3()
    {}

    protected virtual void Awake()
    {
        bx = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
    }
    public void setstatsweapon(float weapondmg, float str, float atkspeed, float critchc, float critdmg)
    {
        weapondamage = weapondmg;
        strength = str;
        attackSpeed = atkspeed;
        critChance = critchc;
        critDamage = critdmg;
    }
    public bool performattack(AttackType attacktype)
    {        
        if(anim != null)
        {
            int crit = IsCrit();
            damage = (weapondamage + 0.1f * strength) * (1 + critDamage * 0.01f * crit) * attacktypemultiplier;
            anim.SetTrigger(attacktype.ToString());
            Debug.Log("Sword Slash executed.");
            return true;
        }
        else
        {
            return false;
        }
    }
    public void EnableHitbox()
    {
        if (bx != null) bx.enabled = true;
    }

    public void DisableHitbox()
    {
        if (bx != null) bx.enabled = false;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            DealotherDamage(other);
            Debug.Log("Hit Player");

        }
        if (other.CompareTag("mob"))
        {
            DealotherDamage(other);
            Debug.Log("Hit Mob");
        }
        
    }
    public void DealotherDamage(Collider2D other)
    {
        BaseMobClass mob = other.GetComponent<BaseMobClass>();
        mob.TakeDamage(damage);
    }
    private int IsCrit()
    {
        float roll = Random.Range(0f, 100f);
        return (roll <= critChance) ? 1 : 0;
    }
}

class Sword : Weapon
{
    override public void Attack1()
    {
        attacktypemultiplier = 1f;
        performattack(AttackType.Slash);
    }
    override public void Attack2()
    {
        attacktypemultiplier = 0.9f;
        performattack(AttackType.Stab);
    }
    override public void Attack3()
    {
        attacktypemultiplier = 2f;
        performattack(AttackType.Charge);
    }

    protected override void Awake()
    {
        base.Awake();
        weapondamage = 10f;
        strength = 5f;
    }
}

