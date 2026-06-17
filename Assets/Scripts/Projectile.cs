using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private float lifetime = 4f;

    private int _damage;

    public void Initialize(Vector2 direction, float speed, int damage)
    {
        _damage = damage;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        rb.linearVelocity = direction.normalized * speed;

        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            return;

        Health health = other.GetComponentInParent<Health>();
        if (health == null)
            return;

        health.TakeDamage(_damage);

        Knockback knockback = health.GetComponent<Knockback>();
        knockback?.Apply(rb.linearVelocity.normalized);

        if (explosionPrefab != null)
            Instantiate(explosionPrefab, health.transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}
