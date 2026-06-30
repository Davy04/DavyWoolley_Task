using UnityEngine;

/// <summary>
/// Everything a <see cref="WeaponBehavior"/> needs to fire one volley, bundled so the behavior
/// stays decoupled from whoever pulls the trigger. Built fresh each shot by the PlayerShooter:
/// where the shot starts, which way it aims, the live stats to read, and the pool to spawn from.
/// </summary>
public readonly struct WeaponContext
{
    /// <summary>Player center. Projectiles exit on the ring around it, not from this point.</summary>
    public readonly Vector2 Origin;

    /// <summary>Normalized direction toward the cursor.</summary>
    public readonly Vector2 AimDirection;

    /// <summary>Radius of the firing ring — each projectile spawns at Origin + itsDirection * this.</summary>
    public readonly float MuzzleRadius;

    public readonly PlayerStats Stats;
    public readonly ProjectilePool Pool;

    public WeaponContext(Vector2 origin, Vector2 aimDirection, float muzzleRadius, PlayerStats stats, ProjectilePool pool)
    {
        Origin = origin;
        AimDirection = aimDirection;
        MuzzleRadius = muzzleRadius;
        Stats = stats;
        Pool = pool;
    }
}
