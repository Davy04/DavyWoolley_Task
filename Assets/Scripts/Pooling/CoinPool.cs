using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Spawns coins from a pool with a hard cap on how many can exist at once.
/// When the cap is reached, the oldest coin in the scene is recycled to make
/// room for the new one (FIFO).
/// </summary>
public class CoinPool : MonoBehaviour
{
    public static CoinPool Instance { get; private set; }

    [SerializeField] private GameObject coinPrefab; // prefab com WorldItem
    [SerializeField] private Item coinItem;
    [SerializeField] private int maxActive = 20;
    [SerializeField] private float throwForce = 4f;

    private GameObjectPool _pool;
    private readonly LinkedList<WorldItem> _active = new LinkedList<WorldItem>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        _pool = new GameObjectPool(coinPrefab, transform);
    }

    public void SpawnCoin(Vector3 position)
    {
        if (coinPrefab == null || coinItem == null) return;

        // Cap atingido: recicla a moeda mais antiga (sua callback a remove da lista e devolve ao pool).
        if (_active.Count >= maxActive && _active.First != null)
            _active.First.Value.Despawn();

        GameObject go = _pool.Get(position, Quaternion.identity);
        WorldItem coin = go.GetComponent<WorldItem>();

        LinkedListNode<WorldItem> node = _active.AddLast(coin);

        coin.InitializePooled(coinItem, 1, throwForce, () =>
        {
            if (node.List != null)
                _active.Remove(node);
            _pool.Release(go);
        });
    }
}
