using UnityEngine;

[RequireComponent(typeof(Health))]
public class HealthAudio : MonoBehaviour
{
    [SerializeField] private AudioClip damageClip;
    [SerializeField] private AudioClip deathClip;
    [SerializeField] private AudioClip healClip;

    private Health _health;

    private void Awake() => _health = GetComponent<Health>();

    private void OnEnable()
    {
        _health.OnDamaged += HandleDamaged;
        _health.OnHealed  += HandleHealed;
        _health.OnDeath   += HandleDeath;
    }

    private void OnDisable()
    {
        _health.OnDamaged -= HandleDamaged;
        _health.OnHealed  -= HandleHealed;
        _health.OnDeath   -= HandleDeath;
    }

    private void HandleDamaged(int _) => AudioManager.Instance?.PlaySFX(damageClip);
    private void HandleHealed(int _)  => AudioManager.Instance?.PlaySFX(healClip);
    private void HandleDeath()        => AudioManager.Instance?.PlaySFX(deathClip);
}
