using UnityEngine;

public class ItemMagnet : MonoBehaviour
{
    [SerializeField] private float magnetRadius = 3f;
    [SerializeField] private float magnetSpeed  = 6f;

    private void FixedUpdate()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, magnetRadius);
        foreach (Collider2D hit in hits)
        {
            WorldItem item = hit.GetComponent<WorldItem>();
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
