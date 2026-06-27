using UnityEngine;

/// <summary>
/// Strategy base for the player's firing pattern. Every weapon shoots projectiles — concrete
/// behaviors decide how many, in what spread, and with what damage/speed multipliers over the
/// player's stats. Stateless ScriptableObject: all per-shot data arrives via <see cref="WeaponContext"/>,
/// so the same behavior asset can be shared and swapped at runtime by the evolution system.
/// </summary>
public abstract class WeaponBehavior : ScriptableObject
{
    public abstract void Fire(in WeaponContext context);
}
