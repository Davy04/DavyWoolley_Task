using UnityEngine;

public class ItemMagnet : MonoBehaviour
{
    [SerializeField] private float magnetRadius = 3f;
    [SerializeField] private float magnetSpeed  = 6f;
    [SerializeField] private int   maxItems     = 32;

    private Collider2D[] _hits;
    private ContactFilter2D _filter;

    private void Awake()
    {
        _hits = new Collider2D[maxItems];
        _filter = new ContactFilter2D
        {
            useTriggers = true,
            useLayerMask = false,
            useDepth = false
        };
    }

    private void FixedUpdate()
    {
        int count = Physics2D.OverlapCircle(transform.position, magnetRadius, _filter, _hits);
        for (int i = 0; i < count; i++)
        {
            WorldItem item = _hits[i].GetComponent<WorldItem>();
            if (item != null && item.CanBeAttracted)
                item.AttractToward(transform.position, magnetSpeed);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, magnetRadius);
    }
}
