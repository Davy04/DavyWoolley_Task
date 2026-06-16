using UnityEngine;

public enum ItemType
{
    Weapon,
    Consumable
}

[CreateAssetMenu(menuName = "Scriptable Object/Item")]
public class Item : ScriptableObject
{
    [Header("Geral")]
    public string itemName = "New Item";
    [TextArea] public string description;
    public Sprite icon;
    public ItemType type;
    public bool stackable;
    public int maxStack = 1;

    [Header("Weapon")]
    public int damage;
    public float attackSpeed = 1f;

    [Header("Consumable")]
    public int healthRestore;
    public int manaRestore;

    // Executa o efeito do item. Retorna true se o item deve ser consumido (gasto) ao usar.
    public bool Use(GameObject user)
    {
        switch (type)
        {
            case ItemType.Weapon:
                Debug.Log($"Equipando arma '{itemName}' (dano: {damage}, velocidade: {attackSpeed}).");
                // Uma arma é equipada, não consumida.
                return false;

            case ItemType.Consumable:
                Debug.Log($"Consumindo '{itemName}' (+{healthRestore} HP, +{manaRestore} MP).");
                // Aqui você aplicaria os efeitos no 'user' (ex.: user.GetComponent<Health>().Heal(healthRestore)).
                // Um consumível é gasto ao usar.
                return true;
        }

        return false;
    }
}
