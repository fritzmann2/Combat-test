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

public enum AccessoryType
{
    Ring,
    Necklace,
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