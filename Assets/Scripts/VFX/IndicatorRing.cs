using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Arranges the firing indicators around the player so they match the active weapon's muzzle
/// pattern: one indicator per shot origin, placed on the ring at that shot's angle. Rebuilds when
/// the weapon changes (evolution). Presentation only — it reads the weapon's pattern, the same
/// one <see cref="ProjectileWeaponBehavior.Fire"/> uses, so indicators always line up with shots.
/// </summary>
public class IndicatorRing : MonoBehaviour
{
    [SerializeField] private PlayerShooter shooter;

    [Tooltip("Transform the indicators are parented to — must rotate with aim (the WeaponParent).")]
    [SerializeField] private Transform ringRoot;

    [SerializeField] private GameObject indicatorPrefab;

    [Min(0f)]
    [Tooltip("Distance from the center each indicator sits (match the PlayerShooter muzzleRadius).")]
    [SerializeField] private float radius = 0.5f;

    [Tooltip("Rotation added to every indicator so its art points outward (0 if the art faces +X/right).")]
    [SerializeField] private float artAngleOffset = 0f;

    private readonly List<GameObject> _indicators = new();

    private void Awake()
    {
        if (shooter == null)
            shooter = GetComponentInParent<PlayerShooter>();

        if (ringRoot == null)
            ringRoot = transform;
    }

    private void OnEnable()
    {
        if (shooter != null)
            shooter.OnWeaponChanged += Rebuild;
    }

    private void OnDisable()
    {
        if (shooter != null)
            shooter.OnWeaponChanged -= Rebuild;
    }

    private void Start()
    {
        if (shooter != null)
            Rebuild(shooter.CurrentWeapon);
    }

    private void Rebuild(WeaponBehavior weapon)
    {
        Clear();
        if (weapon == null || indicatorPrefab == null)
            return;

        int count = Mathf.Max(1, weapon.MuzzleCount);
        for (int i = 0; i < count; i++)
        {
            float angle = weapon.MuzzleAngleOffset(i);
            float rad = angle * Mathf.Deg2Rad;
            Vector2 localPosition = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)) * radius;

            GameObject indicator = Instantiate(indicatorPrefab, ringRoot);
            indicator.transform.localPosition = localPosition;
            indicator.transform.localRotation = Quaternion.Euler(0f, 0f, angle + artAngleOffset);
            _indicators.Add(indicator);
        }
    }

    private void Clear()
    {
        foreach (GameObject indicator in _indicators)
            if (indicator != null)
                Destroy(indicator);

        _indicators.Clear();
    }
}
