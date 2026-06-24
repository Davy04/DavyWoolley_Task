using UnityEngine;

/// <summary>
/// Base stats for a playable character, authored as an asset (data-driven, per GDD).
/// <see cref="PlayerStats"/> reads these at runtime and wraps them in mutable Stats so
/// upgrades can modify them without ever touching the source asset.
/// </summary>
[CreateAssetMenu(menuName = "Bullet Crash/Character Data")]
public class CharacterData : ScriptableObject
{
    [Header("Identity")]
    [SerializeField] private string displayName = "Hero";

    [Header("Base Stats")]
    [Tooltip("Units per second fed to the Rigidbody2D.")]
    [SerializeField] private float baseMoveSpeed = 0.5f;

    public string DisplayName => displayName;
    public float BaseMoveSpeed => baseMoveSpeed;
}
