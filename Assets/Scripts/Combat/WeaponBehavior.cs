using UnityEngine;

/// <summary>
/// Strategy base for the player's firing pattern. Every weapon shoots projectiles — concrete
/// behaviors decide how many, in what spread, and with what damage/speed multipliers over the
/// player's stats. Stateless ScriptableObject: all per-shot data arrives via <see cref="WeaponContext"/>,
/// so the same behavior asset can be shared and swapped at runtime by the evolution system.
/// </summary>
public abstract class WeaponBehavior : ScriptableObject
{
    [Header("Cadence")]
    [Min(0.05f)]
    [Tooltip("Multiplies the player's Reload (cooldown between shots). <1 fires faster, >1 slower.")]
    [SerializeField] private float reloadMultiplier = 1f;

    /// <summary>Read by the shooter to scale the firing cooldown. Lower = faster.</summary>
    public float ReloadMultiplier => reloadMultiplier;

    /// <summary>How many points shots come out of — drives the firing indicators on the ring.</summary>
    public virtual int MuzzleCount => 1;

    /// <summary>Angle (deg) of muzzle <paramref name="index"/> relative to the aim direction.</summary>
    public virtual float MuzzleAngleOffset(int index) => 0f;

    public abstract void Fire(in WeaponContext context);
}
