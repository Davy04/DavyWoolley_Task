using UnityEngine;

public enum ItemType
{
    Wand,
    Consumable,
    Coin
}

[CreateAssetMenu(menuName = "Scriptable Object/Item")]
public class Item : ScriptableObject
{
    [Header("General")]
    public string itemName = "New Item";
    [TextArea] public string description;
    public Sprite icon;
    public ItemType type;
    public bool stackable;
    public int maxStack = 1;
    public Vector3 weaponScale = Vector3.one;
    public Vector3 worldScale = Vector3.one;

    [Header("Behavior")]
    public WeaponBehavior weaponBehavior;

    [Header("Weapon")]
    public int damage;
    public float attackSpeed = 1f;
    public float rotation;

    [Header("Consumable")]
    public int healthRestore;

    [Header("Audio")]
    public AudioClip pickupSound;
}
