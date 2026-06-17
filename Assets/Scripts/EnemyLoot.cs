using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyLoot : MonoBehaviour
{
    [Header("Coin Drop (on hit)")]
    [SerializeField] private GameObject worldItemPrefab;
    [SerializeField] private Item coinItem;
    [SerializeField] private int coinsPerHit = 3;
    [SerializeField] private float coinThrowForce = 4f;

    [Header("Death Loot")]
    [SerializeField] private LootDrop[] deathLoot;

    private Health _health;

    private void Awake()
    {
        _health = GetComponent<Health>();
    }

    private void OnEnable()
    {
        _health.OnDamaged += HandleDamaged;
        _health.OnDeath   += HandleDeath;
    }

    private void OnDisable()
    {
        _health.OnDamaged -= HandleDamaged;
        _health.OnDeath   -= HandleDeath;
    }

    private void HandleDamaged(int _) => DropCoins();
    private void HandleDeath()        => DropDeathLoot();

    private void DropCoins()
    {
        if (worldItemPrefab == null || coinItem == null || coinsPerHit <= 0) return;

        for (int i = 0; i < coinsPerHit; i++)
        {
            GameObject obj = Instantiate(worldItemPrefab, transform.position, Quaternion.identity);
            obj.GetComponent<WorldItem>()?.Initialize(coinItem, 1, coinThrowForce);
        }
    }

    private void DropDeathLoot()
    {
        if (worldItemPrefab == null)
        {
            Debug.LogWarning($"{name}: worldItemPrefab not assigned in EnemyLoot.");
            return;
        }

        if (deathLoot == null || deathLoot.Length == 0)
        {
            Debug.LogWarning($"{name}: deathLoot is empty.");
            return;
        }

        foreach (LootDrop drop in deathLoot)
        {
            if (drop.item == null) { Debug.LogWarning($"{name}: LootDrop has null item."); continue; }

            float roll = Random.value;
            if (roll > drop.chance) { Debug.Log($"{name}: {drop.item.itemName} did not drop (roll {roll:F2} > chance {drop.chance:F2})."); continue; }

            int count = Random.Range(drop.minCount, drop.maxCount + 1);
            if (count <= 0) continue;

            Debug.Log($"{name}: Dropping {count}x {drop.item.itemName}.");
            GameObject obj = Instantiate(worldItemPrefab, transform.position, Quaternion.identity);
            obj.GetComponent<WorldItem>()?.Initialize(drop.item, count);
        }
    }
}

[Serializable]
public class LootDrop
{
    public Item item;
    [Range(0f, 1f)] public float chance = 0.5f;
    public int minCount = 1;
    public int maxCount = 1;
}
