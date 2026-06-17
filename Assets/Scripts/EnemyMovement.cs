using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private float speed = 2f;
    [SerializeField] private float detectionRange = 6f;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;

    private static readonly int IsMoving = Animator.StringToHash("isMoving");

    private Transform _player;
    private float _stopRange;

    private void Start()
    {

        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
            _player = playerObj.transform;

        EnemyAttack attack = GetComponent<EnemyAttack>();
        _stopRange = attack != null ? attack.AttackRange : 1.2f;
    }

    private void FixedUpdate()
    {
        if (_player == null)
            return;

        float dist = Vector2.Distance(transform.position, _player.position);
        bool inDetectionRange = dist <= detectionRange;
        bool inStopRange      = dist <= _stopRange;

        if (!inDetectionRange || inStopRange)
        {
            rb.linearVelocity = Vector2.zero;
            animator?.SetBool(IsMoving, false);
            return;
        }

        Vector2 dir = ((Vector2)_player.position - (Vector2)transform.position).normalized;
        rb.linearVelocity = dir * speed;
        animator?.SetBool(IsMoving, true);

        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * (dir.x < 0f ? -1f : 1f);
        transform.localScale = scale;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _stopRange);
    }
}
