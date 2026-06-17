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
    private bool _playerNearby;
    private bool _blockPickup;
    private bool _magnetImmune;

    public bool CanPickup     => _canPickup && !_blockPickup;
    public bool CanBeAttracted => CanPickup && !_magnetImmune;

    private void Start()
    {
        if (preplacedItem != null && _item == null)
        {
            _item  = preplacedItem;
            _count = preplacedCount;

            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = preplacedItem.icon;
                spriteRenderer.transform.localScale = preplacedItem.worldScale;
            }

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

    public void Initialize(Item item, int count, float throwForceOverride = 0f, bool magnetImmune = false)
    {
        _item = item;
        _count = count;
        _magnetImmune = magnetImmune;

        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = item.icon;
            spriteRenderer.transform.localScale = item.worldScale;
        }

        if (rb != null)
        {
            rb.gravityScale = 0f;
            float force = throwForceOverride > 0f ? throwForceOverride : throwForce;
            rb.AddForce(Random.insideUnitCircle.normalized * force, ForceMode2D.Impulse);
        }

        StartCoroutine(EnablePickupAfterDelay());
    }

    public void AttractToward(Vector3 target, float speed)
    {
        _isBeingAttracted = true;
        Vector2 newPos = Vector2.MoveTowards(transform.position, target, speed * Time.fixedDeltaTime);
        rb.MovePosition(newPos);
    }

    private IEnumerator EnablePickupAfterDelay()
    {
        yield return new WaitForSecondsRealtime(pickupDelay);

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        _origin = transform.position;
        _canPickup = true;

        // Bloqueia pickup imediato apenas para itens dropados pelo player (magnetImmune).
        if (_playerNearby && _magnetImmune)
            _blockPickup = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        _playerNearby = true;

        if (!_canPickup || _blockPickup) return;

        if (InventoryManager.Instance == null) return;

        int added = InventoryManager.Instance.AddItem(_item, _count);
        _count -= added;

        if (_count <= 0)
            Destroy(gameObject);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        _playerNearby = false;
        _blockPickup = false;
    }
}
