using UnityEngine;

/// <summary>
/// Pools projectiles so the wand doesn't Instantiate/Destroy on every shot.
/// Optional: if no instance exists, WandBehavior falls back to Instantiate.
/// </summary>
public class ProjectilePool : MonoBehaviour
{
    public static ProjectilePool Instance { get; private set; }

    [SerializeField] private GameObject projectilePrefab;

    private GameObjectPool _pool;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        _pool = new GameObjectPool(projectilePrefab, transform);
    }

    public void Spawn(Vector3 position, Vector2 direction, float speed, int damage)
    {
        if (projectilePrefab == null) return;

        GameObject go = _pool.Get(position, Quaternion.identity);
        Projectile proj = go.GetComponent<Projectile>();
        proj.InitializePooled(direction, speed, damage, () => _pool.Release(go));
    }
}
