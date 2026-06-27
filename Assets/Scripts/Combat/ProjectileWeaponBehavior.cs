using UnityEngine;

/// <summary>
/// Data-driven workhorse weapon: fires N projectiles fanned across a spread angle, scaling the
/// player's BulletDamage/BulletSpeed by per-weapon multipliers. A single asset covers single
/// shot, twin, shotgun, radial, sniper and friends — only the serialized values change, so most
/// evolution nodes just point at differently-configured instances of this one behavior.
/// </summary>
[CreateAssetMenu(menuName = "Bullet Crash/Weapons/Projectile")]
public class ProjectileWeaponBehavior : WeaponBehavior
{
    [Header("Volley")]
    [Min(1)]
    [Tooltip("How many projectiles per shot.")]
    [SerializeField] private int projectileCount = 1;

    [Tooltip("Total fan angle (degrees) the projectiles spread across. 0 = parallel/single.")]
    [SerializeField] private float spreadAngle = 0f;

    [Header("Stat multipliers")]
    [Tooltip("Multiplies the player's BulletDamage stat.")]
    [SerializeField] private float damageMultiplier = 1f;

    [Tooltip("Multiplies the player's BulletSpeed stat.")]
    [SerializeField] private float speedMultiplier = 1f;

    [Min(0.05f)]
    [Tooltip("Multiplies the player's Reload (cooldown between shots). <1 fires faster, >1 slower.")]
    [SerializeField] private float reloadMultiplier = 1f;

    /// <summary>Read by the shooter to scale the firing cooldown. Lower = faster.</summary>
    public float ReloadMultiplier => reloadMultiplier;

    public override void Fire(in WeaponContext context)
    {
        if (context.Pool == null || context.Stats == null)
            return;

        float speed = context.Stats.BulletSpeed.Value * speedMultiplier;
        int damage = Mathf.RoundToInt(context.Stats.BulletDamage.Value * damageMultiplier);

        float center = Mathf.Atan2(context.AimDirection.y, context.AimDirection.x) * Mathf.Rad2Deg;
        float start = center - spreadAngle * 0.5f;
        float step = projectileCount > 1 ? spreadAngle / (projectileCount - 1) : 0f;

        for (int i = 0; i < projectileCount; i++)
        {
            float angle = projectileCount > 1 ? start + step * i : center;
            context.Pool.Spawn(context.Origin, AngleToDirection(angle), speed, damage);
        }
    }

    private static Vector2 AngleToDirection(float degrees)
    {
        float rad = degrees * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
    }
}
