using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private AudioClip impactClip;
    [SerializeField] private float lifetime = 4f;

    private int _damage;
    private float _despawnTime;
    private System.Action _onDespawn;
    private bool _despawned;

    public void Initialize(Vector2 direction, float speed, int damage)
    {
        _damage = damage;
        _despawnTime = Time.time + lifetime;
        _despawned = false;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        rb.linearVelocity = direction.normalized * speed;
    }

    /// <summary>Sets up a pooled instance; <paramref name="onDespawn"/> returns it to the pool.</summary>
    public void InitializePooled(Vector2 direction, float speed, int damage, System.Action onDespawn)
    {
        _onDespawn = onDespawn;
        Initialize(direction, speed, damage);
    }

    private void Update()
    {
        if (Time.time >= _despawnTime)
            Despawn();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            return;

        Health health = other.GetComponentInParent<Health>();
        if (health != null)
        {
            health.TakeDamage(_damage);

            Knockback knockback = health.GetComponent<Knockback>();
            knockback?.Apply(rb.linearVelocity.normalized);

            Impact(health.transform.position);
            return;
        }

        // Construções / paredes: colliders sólidos (não-trigger) sem Health.
        if (!other.isTrigger)
            Impact(transform.position);
    }

    private void Impact(Vector3 position)
    {
        if (explosionPrefab != null)
            Instantiate(explosionPrefab, position, Quaternion.Euler(0f, 0f, Random.Range(0f, 360f)));

        AudioManager.Instance?.PlaySFX(impactClip);

        Despawn();
    }

    private void Despawn()
    {
        if (_despawned)
            return;
        _despawned = true;

        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        if (_onDespawn != null)
        {
            System.Action callback = _onDespawn;
            _onDespawn = null;
            callback();
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
