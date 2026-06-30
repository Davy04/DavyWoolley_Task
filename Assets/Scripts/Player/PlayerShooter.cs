using System;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Orchestrates firing. Holds the active weapon and the cooldown, reads the fire input and the
/// aim, then delegates the actual shot to the <see cref="WeaponBehavior"/>. Owns no projectile,
/// damage or pattern logic — it just decides *when* to fire and hands over the context.
///
/// Implements <see cref="IWeaponHolder"/> so the evolution tree can swap the weapon at runtime.
/// </summary>
[RequireComponent(typeof(PlayerStats))]
public class PlayerShooter : MonoBehaviour, IWeaponHolder
{
    [SerializeField] private PlayerStats stats;

    [Tooltip("Mouse-aim transform whose right vector points at the cursor (WeaponParent).")]
    [SerializeField] private WeaponParent aim;

    [SerializeField] private ProjectilePool pool;

    [Min(0f)]
    [Tooltip("Radius of the firing ring around the player — projectiles exit on this ring.")]
    [SerializeField] private float muzzleRadius = 0.5f;

    [Tooltip("Weapon used before any evolution. The Base node's weapon overrides this.")]
    [SerializeField] private WeaponBehavior startingWeapon;

    /// <summary>Raised each time a shot fires. Presentation (muzzle flash, SFX) hooks here.</summary>
    public event Action OnFired;

    /// <summary>Raised when the active weapon changes (evolution). The indicator ring listens.</summary>
    public event Action<WeaponBehavior> OnWeaponChanged;

    public WeaponBehavior CurrentWeapon => _currentWeapon;

    private WeaponBehavior _currentWeapon;
    private float _cooldownTimer;

    public void SetWeapon(WeaponBehavior weapon)
    {
        _currentWeapon = weapon;
        OnWeaponChanged?.Invoke(weapon);
    }

    private void Awake()
    {
        if (stats == null)
            stats = GetComponent<PlayerStats>();

        // Don't clobber a weapon the evolution tree may have set before this Awake ran.
        if (_currentWeapon == null)
            _currentWeapon = startingWeapon;
    }

    private void Update()
    {
        if (Time.timeScale == 0f)
            return; // paused / level-up screen open

        if (_cooldownTimer > 0f)
            _cooldownTimer -= Time.deltaTime;

        if (!IsFireHeld() || _currentWeapon == null || pool == null)
            return;

        if (_cooldownTimer > 0f)
            return;

        Fire();
    }

    private static bool IsFireHeld()
    {
        Mouse mouse = Mouse.current;
        return mouse != null && mouse.leftButton.isPressed;
    }

    private void Fire()
    {
        Vector2 origin = transform.position;
        Vector2 direction = aim != null ? (Vector2)aim.transform.right : Vector2.right;

        var context = new WeaponContext(origin, direction, muzzleRadius, stats, pool);
        _currentWeapon.Fire(context);
        OnFired?.Invoke();

        _cooldownTimer = stats.Reload.Value * _currentWeapon.ReloadMultiplier;
    }
}
