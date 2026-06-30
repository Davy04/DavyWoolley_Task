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

    public override int MuzzleCount => projectileCount;

    public override float MuzzleAngleOffset(int index)
    {
        if (projectileCount <= 1)
            return 0f;

        // Muzzle 0 sits on the aim direction; the rest fan out from it, so the player aims
        // with the first indicator instead of the gap between two.
        float step = spreadAngle / (projectileCount - 1);
        return step * index;
    }

    public override void Fire(in WeaponContext context)
    {
        if (context.Pool == null || context.Stats == null)
            return;

        float speed = context.Stats.BulletSpeed.Value * speedMultiplier;
        int damage = Mathf.RoundToInt(context.Stats.BulletDamage.Value * damageMultiplier);

        float center = Mathf.Atan2(context.AimDirection.y, context.AimDirection.x) * Mathf.Rad2Deg;

        for (int i = 0; i < projectileCount; i++)
        {
            float angle = center + MuzzleAngleOffset(i);
            Vector2 direction = AngleToDirection(angle);
            Vector2 spawnPosition = context.Origin + direction * context.MuzzleRadius;
            context.Pool.Spawn(spawnPosition, direction, speed, damage);
        }
    }

    private static Vector2 AngleToDirection(float degrees)
    {
        float rad = degrees * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
    }
}
