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

    private Item _item;
    private int _count;
    private Vector3 _origin;
    private bool _canPickup;

    private void Update()
    {
        if (!_canPickup)
            return;

        transform.position = _origin + Vector3.up * Mathf.Sin(Time.time * bobFrequency) * bobAmplitude;
    }

    public void Initialize(Item item, int count)
    {
        _item = item;
        _count = count;

        if (spriteRenderer != null)
            spriteRenderer.sprite = item.icon;

        if (rb != null)
        {
            rb.gravityScale = 0f;
            rb.AddForce(Random.insideUnitCircle.normalized * throwForce, ForceMode2D.Impulse);
        }

        StartCoroutine(EnablePickupAfterDelay());
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
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!_canPickup || !other.CompareTag("Player"))
            return;

        InventoryItem added = InventoryManager.Instance.AddItem(_item, _count);
        if (added != null)
            Destroy(gameObject);
    }
}
