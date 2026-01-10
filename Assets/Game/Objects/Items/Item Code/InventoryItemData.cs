using UnityEngine;
using Unity.Netcode;


[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class InventoryItemData : ScriptableObject
{
    public string ID;           // Z.B. "sword_01" (WICHTIG für Save/Load!)
    public string ItemName;
    public Sprite Icon;
    public Itemtype Type;
    public bool IsStackable;
    public int MaxStackSize;
    public GameObject itemObject;
}

[System.Serializable]
public class Armorstats : INetworkSerializable
{
    public float defense;
    public float spellresistance;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref defense);
        serializer.SerializeValue(ref spellresistance);
    }
}
[System.Serializable]
public class Accessorystats : INetworkSerializable
{
    public float health;
    public float mana;
    public float healthRegen;
    public float manaRegen;
    public float movementSpeed;
    public float attackSpeed;
    public float critChance;
    public float critDamage;
    public float strength;
    public float defense;
    public float spellresistance;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref health);
        serializer.SerializeValue(ref mana);
        serializer.SerializeValue(ref healthRegen);
        serializer.SerializeValue(ref manaRegen);
        serializer.SerializeValue(ref movementSpeed);
        serializer.SerializeValue(ref attackSpeed);
        serializer.SerializeValue(ref critChance);
        serializer.SerializeValue(ref critDamage);
        serializer.SerializeValue(ref strength);
        serializer.SerializeValue(ref defense);
        serializer.SerializeValue(ref spellresistance);
    }
}

[System.Serializable]
public class EquipmentStats : INetworkSerializable
{
    public Weaponstats weaponstats;
    public Armorstats armorstats;
    public Accessorystats accessorystats;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        // ---------------- WEAPON ----------------
        bool hasWeapon = weaponstats != null;
        serializer.SerializeValue(ref hasWeapon);

        if (hasWeapon)
        {
            if (serializer.IsReader) weaponstats = new Weaponstats();
            
            weaponstats.NetworkSerialize(serializer);
        }

        // ---------------- ARMOR ----------------
        bool hasArmor = armorstats != null;
        serializer.SerializeValue(ref hasArmor);

        if (hasArmor)
        {
            if (serializer.IsReader) armorstats = new Armorstats();
            armorstats.NetworkSerialize(serializer);
        }

        // ---------------- ACCESSORY ----------------
        bool hasAccessory = accessorystats != null;
        serializer.SerializeValue(ref hasAccessory);

        if (hasAccessory)
        {
            if (serializer.IsReader) accessorystats = new Accessorystats();
            accessorystats.NetworkSerialize(serializer);
        }
    }
}