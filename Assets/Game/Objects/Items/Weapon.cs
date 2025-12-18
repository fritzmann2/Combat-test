using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using Unity.Netcode;
using Unity.VisualScripting;


public enum AttackType
{
    Stab,   
    Slash,
    Charge,
    normal_shot,
    bow_uppercut
}

public enum WeaponType
{
    Sword,
    Bow,
    Staff
}

abstract public class Weapon : NetworkBehaviour
{
    public WeaponType Type { get; set; }
    public Animator anim;
    public float weapondamage { get; set; }
    public float attacktypemultiplier { get; set; }
    public float damage { get; set; }
    public float strength { get; set; }
    public float attackSpeed { get; set; }
    public float critChance { get; set; }
    public float critDamage { get; set; }
    public Collider2D bx;
    private AttackType attacktype;
 
    virtual public void Attack1()
    {}
    virtual public void Attack2()
    {}
    virtual public void Attack3()
    {}
    virtual public void Attack4()
    {}

    //Komponenten setzen
    protected virtual void Awake()
    {
        bx = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
    }

    //Waffenstats setzen
    public void setstatsweapon(float str, float atkspeed, float critchc, float critdmg)
    {
        strength = str;
        attackSpeed = atkspeed;
        critChance = critchc;
        critDamage = critdmg;
    }
    //Angriff ausführen
    public void performattack(AttackType attacktype)
    {        
        if(anim != null)
        {   
            this.attacktype = attacktype;
            int crit = IsCrit();
            damage = (weapondamage + 0.1f * strength) * (1 + critDamage * 0.01f * crit) * attacktypemultiplier;
            PlayAnimation(attacktype);
            Debug.Log("Sword Slash executed.");
        }
    }
    public void PlayAnimation(AttackType attacktype)
    {
        if (anim != null)
        {
            anim.SetTrigger(attacktype.ToString());
        }
    }
    
    //Hitbox aktivieren/deaktivieren
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
        BaseEntety mob = other.GetComponent<BaseEntety>();

        if(other.CompareTag("Player"))
        {
            DealotherDamage(mob);
            Debug.Log("Hit Player");

        }
        if (other.CompareTag("mob"))
        {
            DealotherDamage(mob);
            Debug.Log("Hit Mob");
        }
        
    }
    //Schaden bei anderem zufügen
    public void DealotherDamage(BaseEntety mob)
    {
        mob.TakeDamage(damage);
    }
    private int IsCrit()
    {
        float roll = Random.Range(0f, 100f);
        return (roll <= critChance) ? 1 : 0;
    }
    //Animationlänge ermitteln
    public float GetAnimationLength()
    {
        if (anim == null || anim.runtimeAnimatorController == null) return 0f;

        foreach (AnimationClip clip in anim.runtimeAnimatorController.animationClips)
        {
            if (clip.name == attacktype.ToString())
            {
                return clip.length;
            }
        }
        Debug.LogWarning("Animation nicht gefunden: " + attacktype.ToString());
        return 0f;
    }
}

public class Sword : Weapon
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
    override public void Attack4()
    {
        attacktypemultiplier = 0;
        performattack(AttackType.Charge);
    }

    protected override void Awake()
    {
        Type = WeaponType.Sword;
        base.Awake();
        weapondamage = 10f;
        strength = 5f;
    }
}

public class Bow : Weapon
{
    override public void Attack1()
    {
        attacktypemultiplier = 1f;
        performattack(AttackType.normal_shot);
    }
    override public void Attack2()
    {
        attacktypemultiplier = 0.9f;
        performattack(AttackType.bow_uppercut);
    }
    override public void Attack3()
    {
        attacktypemultiplier = 2f;
        performattack(AttackType.Charge);
    }

    protected override void Awake()
    {
        Type = WeaponType.Bow;
        base.Awake();
        weapondamage = 8f;
        strength = 3f;
    }
}