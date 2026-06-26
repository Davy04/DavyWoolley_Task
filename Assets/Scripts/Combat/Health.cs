using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private bool destroyOnDeath = true;

    public event Action<int> OnDamaged;
    public event Action<int> OnHealed;
    public event Action OnDeath;

    public int Current => _current;
    public int Max     => _maxHealth;

    private int _current;
    private int _maxHealth;

    private void Awake()
    {
        _maxHealth = maxHealth;
        _current   = _maxHealth;
    }

    /// <summary>
    /// Overrides the max HP at runtime (called by PlayerStats after applying upgrades).
    /// Current HP is clamped to the new max.
    /// </summary>
    public void Initialize(int newMax)
    {
        _maxHealth = Mathf.Max(1, newMax);
        _current   = Mathf.Min(_current, _maxHealth);
    }

    public void TakeDamage(int amount)
    {
        if (amount <= 0) return;

        _current = Mathf.Max(0, _current - amount);
        OnDamaged?.Invoke(amount);

        if (_current <= 0)
            Die();
    }

    public void Heal(int amount)
    {
        if (amount <= 0) return;
        _current = Mathf.Min(_current + amount, _maxHealth);
        OnHealed?.Invoke(amount);
    }

    private void Die()
    {
        OnDeath?.Invoke();
        if (destroyOnDeath)
            Destroy(gameObject);
    }
}
