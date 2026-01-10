using UnityEngine;
using Unity.Netcode;
using Unity.VisualScripting.Antlr3.Runtime.Misc;


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
    Staff, 
    None
}   
public enum ArmorType
{
    Helmet,
    Chestplate,
    Leggings,
    Boots,
    None
}

public enum Itemtype
{
    Weapon,
    Armor,
    Accessory

}

[System.Serializable]
public class Weaponstats : INetworkSerializable
{
    public float weapondamage;
    public float strength;
    public float critChance;
    public float critDamage;
    public float attackSpeed;
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref weapondamage);
        serializer.SerializeValue(ref strength);
        serializer.SerializeValue(ref critChance);
        serializer.SerializeValue(ref critDamage);
        serializer.SerializeValue(ref attackSpeed);
    }
}





abstract public class Weapon : InventoryItem
{
    public Weaponstats weaponstats { get; set; }
    public PlayerStats playerStats;
    public WeaponType Type { get; set; }
    public float attacktypemultiplier;
    public Animator anim;
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
    protected virtual void Awake()
    {
        bx = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
        controls = new GameControls();
        playerStats = GetComponentInParent<PlayerStats>();
    }
    
    void OnEnable()
    {
        controls.Enable();
    }

    void OnDisable()
    {
        controls.Disable();
    }

    //Angriff ausführen
    public void performattack(AttackType attacktype)
    {        
        if(anim != null)
        {   
            this.attacktype = attacktype;
            //int crit = IsCrit();
            //damage = (weapondamage + 0.1f * strength) * (1 + critDamage * 0.01f * crit) * attacktypemultiplier;
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
            playerStats.DealotherDamage(mob);
            Debug.Log("Hit Player");

        }
        if (other.CompareTag("mob"))
        {
            playerStats.DealotherDamage(mob);
            Debug.Log("Hit Mob");
        }
        
    }
    //Schaden bei anderem zufügen

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
        //weapondamage = 8f;
        //strength = 3f;
    }
}