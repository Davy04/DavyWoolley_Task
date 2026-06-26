using System;
using BulletCrash.Core.Stats;
using UnityEngine;

/// <summary>
/// Authorable, serializable description of a single stat change — "which stat, how, how much".
/// Bridges data assets (evolution nodes, upgrade cards) to the runtime <see cref="Stat"/> system:
/// the asset names a <see cref="PlayerStatType"/>; <see cref="ToModifier"/> turns it into a
/// live <see cref="StatModifier"/> tagged with its source so it can be removed later.
/// </summary>
[Serializable]
public struct StatBonus
{
    [SerializeField] private PlayerStatType stat;
    [SerializeField] private StatModifierType type;
    [SerializeField] private float value;

    public PlayerStatType Stat => stat;

    public StatModifier ToModifier(object source) => new(value, type, source);
}
