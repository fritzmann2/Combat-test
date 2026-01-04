using UnityEngine;
using Unity.Netcode;


public enum AttackType
{
    Stab,   
    Slash,
    Charge,
    Throw,
    normal_shot,
    bow_uppercut
}

public enum WeaponType
{
    Sword,
    Bow,
    Staff
}   

public enum Itemtype
{
    Weapon,
    Armor

}



abstract public class Weapon : InventoryItem
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
    private Transform visualTarget;
    public Vector3 animOffset;
    private GameControls controls;
    private float moveInput;
    private float LastmoveInput;
 
    virtual public void Attack1()
    {}
    virtual public void Attack2()
    {}
    virtual public void Attack3()
    {}
    virtual public void Attack4()
    {}

    //Komponenten setzen
    protected override void Awake()
    {
        base.Awake();
        bx = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
        controls = new GameControls();
    }
    
    void OnEnable()
    {
        controls.Enable();
    }

    void OnDisable()
    {
        controls.Disable();
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
            PlayAnimationClientsAndHostRpc(attacktype);
        }
    }
    [Rpc(SendTo.ClientsAndHost)]
    public void PlayAnimationClientsAndHostRpc(AttackType attacktype)
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
    public void SetFollowTarget(Transform target)
    {
        visualTarget = target;
    }

    protected virtual void LateUpdate()
    {
        Vector2 inputVector = controls.Gameplay.Move.ReadValue<Vector2>();
        moveInput = inputVector.x;
        if (moveInput != 0f)
        {
            LastmoveInput = moveInput;
        }
        if (visualTarget != null)
        {
            Vector3 handPos = visualTarget.position;
        
            Vector3 finalPos = handPos + (visualTarget.rotation * animOffset * LastmoveInput);

            transform.position = finalPos;
        }
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
        attacktypemultiplier = 1f;
        Debug.Log("Throw Attack");
        performattack(AttackType.Throw);
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