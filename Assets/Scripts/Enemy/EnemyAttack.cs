using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField] private int damage = 10;
    [SerializeField] private float attackRange = 1.5f;
    public float AttackRange => attackRange;
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private Animator animator;
    [SerializeField] private AudioClip attackClip;

    private static readonly int AttackTrigger = Animator.StringToHash("Attack");

    private Transform _player;
    private Health _playerHealth;
    private Knockback _playerKnockback;
    private float _nextAttackTime;

    private void Start()
    {
        _player          = PlayerLocator.Player;
        _playerHealth    = PlayerLocator.PlayerHealth;
        _playerKnockback = PlayerLocator.PlayerKnockback;
    }

    private void Update()
    {
        if (_player == null || _playerHealth == null) return;
        if (Time.time < _nextAttackTime) return;

        float dist = Vector2.Distance(transform.position, _player.position);
        if (dist <= attackRange)
            PerformAttack();
    }

    private void PerformAttack()
    {
        _nextAttackTime = Time.time + attackCooldown;
        animator?.SetTrigger(AttackTrigger);
        AudioManager.Instance?.PlaySFX(attackClip);
        _playerHealth.TakeDamage(damage);

        Vector2 knockbackDir = ((Vector2)_player.position - (Vector2)transform.position).normalized;
        _playerKnockback?.Apply(knockbackDir);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
