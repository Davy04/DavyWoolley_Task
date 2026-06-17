using UnityEngine;

public enum ItemType
{
    Weapon,
    Consumable
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

    [Header("Weapon")]
    public int damage;
    public float attackSpeed = 1f;
    public float rotation;

    [Header("Consumable")]
    public int healthRestore;
    public int manaRestore;

    public bool Use(GameObject user)
    {
        switch (type)
        {
            case ItemType.Weapon:
                Debug.Log($"Equipping weapon '{itemName}' (damage: {damage}, speed: {attackSpeed}).");
                return false;

            case ItemType.Consumable:
                Debug.Log($"Consuming '{itemName}' (+{healthRestore} HP, +{manaRestore} MP).");
                return true;
        }

        return false;
    }
}
