using System.Collections;
using UnityEngine;

public class WorldItem : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float throwForce = 3f;
    [SerializeField] private float pickupDelay = 0.5f;
    [SerializeField] private float bobAmplitude = 0.08f;
    [SerializeField] private float bobFrequency = 2f;

    [Header("Pre-placed (deixe vazio se for spawnar por código)")]
    [SerializeField] private Item preplacedItem;
    [SerializeField] private int preplacedCount = 1;

    private Item _item;
    private int _count;
    private Vector3 _origin;
    private bool _canPickup;
    private bool _isBeingAttracted;
    private bool _magnetImmune;
    private float _pickupBlockedUntil;

    public bool CanPickup      => _canPickup && Time.time >= _pickupBlockedUntil;
    public bool CanBeAttracted => CanPickup && !_magnetImmune;

    private void Start()
    {
        if (preplacedItem != null && _item == null)
        {
            _item  = preplacedItem;
            _count = preplacedCount;
            ApplyVisual(preplacedItem);

            if (rb != null)
                rb.bodyType = RigidbodyType2D.Kinematic;

            _origin    = transform.position;
            _canPickup = true;
        }
    }

    private void Update()
    {
        if (!_canPickup || _isBeingAttracted)
            return;

        transform.position = _origin + Vector3.up * Mathf.Sin(Time.time * bobFrequency) * bobAmplitude;
    }

    public void Initialize(Item item, int count, float throwForceOverride = 0f,
        bool magnetImmune = false, Vector2 direction = default, float pickupBlockSeconds = 0f)
    {
        _item = item;
        _count = count;
        _magnetImmune = magnetImmune;
        _pickupBlockedUntil = Time.time + pickupBlockSeconds;

        ApplyVisual(item);

        if (rb != null)
        {
            rb.gravityScale = 0f;
            float force = throwForceOverride > 0f ? throwForceOverride : throwForce;
            Vector2 dir = direction == Vector2.zero ? Random.insideUnitCircle.normalized : direction.normalized;
            rb.AddForce(dir * force, ForceMode2D.Impulse);
        }

        StartCoroutine(EnablePickupAfterDelay());
    }

    public void AttractToward(Vector3 target, float speed)
    {
        _isBeingAttracted = true;
        Vector2 newPos = Vector2.MoveTowards(transform.position, target, speed * Time.fixedDeltaTime);
        rb.MovePosition(newPos);
    }

    private void ApplyVisual(Item item)
    {
        if (spriteRenderer == null) return;
        spriteRenderer.sprite = item.icon;
        spriteRenderer.transform.localScale = item.worldScale;
    }

    private IEnumerator EnablePickupAfterDelay()
    {
        // WaitForSeconds (tempo de jogo) — não conta durante a pausa da bag,
        // então o arremesso só acontece depois que o jogo volta a rodar.
        yield return new WaitForSeconds(pickupDelay);

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        _origin = transform.position;
        _canPickup = true;
    }

    private void OnTriggerEnter2D(Collider2D other) => TryPickup(other);
    private void OnTriggerStay2D(Collider2D other)  => TryPickup(other);

    private void TryPickup(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (!CanPickup) return;
        if (InventoryManager.Instance == null) return;

        int added = InventoryManager.Instance.AddItem(_item, _count);
        _count -= added;

        if (_count <= 0)
            Destroy(gameObject);
    }
}
