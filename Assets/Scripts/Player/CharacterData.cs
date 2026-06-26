using UnityEngine;

/// <summary>
/// Base stats for a playable character. Authored as an asset (data-driven).
/// PlayerStats reads these at runtime and wraps each value in a mutable Stat —
/// this asset is never changed at runtime.
/// </summary>
[CreateAssetMenu(menuName = "Bullet Crash/Character Data")]
public class CharacterData : ScriptableObject
{
    [Header("Identity")]
    [SerializeField] private string displayName = "Hero";

    [Header("Health")]
    [Tooltip("Maximum hit points.")]
    [SerializeField] private float baseMaxHealth = 100f;

    [Tooltip("HP restored per second. Zero means no passive regen.")]
    [SerializeField] private float baseHealthRegen = 0f;

    [Header("Movement")]
    [Tooltip("Units per second.")]
    [SerializeField] private float baseMoveSpeed = 5f;

    [Header("Projectile")]
    [Tooltip("Units per second the projectile travels.")]
    [SerializeField] private float baseBulletSpeed = 12f;

    [Tooltip("Damage dealt per hit.")]
    [SerializeField] private float baseBulletDamage = 10f;

    [Tooltip("Units of travel before the projectile despawns.")]
    [SerializeField] private float baseBulletDistance = 8f;

    [Tooltip("Seconds between shots (lower = faster).")]
    [SerializeField] private float baseReload = 0.5f;

    public string DisplayName      => displayName;
    public float BaseMaxHealth     => baseMaxHealth;
    public float BaseHealthRegen   => baseHealthRegen;
    public float BaseMoveSpeed     => baseMoveSpeed;
    public float BaseBulletSpeed   => baseBulletSpeed;
    public float BaseBulletDamage  => baseBulletDamage;
    public float BaseBulletDistance => baseBulletDistance;
    public float BaseReload        => baseReload;
}
