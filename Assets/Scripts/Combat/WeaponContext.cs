using UnityEngine;

/// <summary>
/// Everything a <see cref="WeaponBehavior"/> needs to fire one volley, bundled so the behavior
/// stays decoupled from whoever pulls the trigger. Built fresh each shot by the PlayerShooter:
/// where the shot starts, which way it aims, the live stats to read, and the pool to spawn from.
/// </summary>
public readonly struct WeaponContext
{
    public readonly Vector2 Origin;
    public readonly Vector2 AimDirection;
    public readonly PlayerStats Stats;
    public readonly ProjectilePool Pool;

    public WeaponContext(Vector2 origin, Vector2 aimDirection, PlayerStats stats, ProjectilePool pool)
    {
        Origin = origin;
        AimDirection = aimDirection;
        Stats = stats;
        Pool = pool;
    }
}
