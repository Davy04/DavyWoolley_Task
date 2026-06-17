using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Minimal reusable GameObject pool. Reuses inactive instances instead of
/// Instantiate/Destroy, cutting GC churn for high-frequency objects.
/// </summary>
public class GameObjectPool
{
    private readonly GameObject _prefab;
    private readonly Transform _parent;
    private readonly Queue<GameObject> _free = new Queue<GameObject>();

    public GameObjectPool(GameObject prefab, Transform parent = null)
    {
        _prefab = prefab;
        _parent = parent;
    }

    public GameObject Get(Vector3 position, Quaternion rotation)
    {
        GameObject go = _free.Count > 0
            ? _free.Dequeue()
            : Object.Instantiate(_prefab, _parent);

        go.transform.SetPositionAndRotation(position, rotation);
        go.SetActive(true);
        return go;
    }

    public void Release(GameObject go)
    {
        if (go == null) return;
        go.SetActive(false);
        _free.Enqueue(go);
    }
}
