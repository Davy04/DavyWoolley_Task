using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A single class/build in the evolution tree, authored as an asset (diep.io-style).
/// Holds what the node grants (a weapon behavior and/or passive stat bonuses) and which
/// nodes it can evolve into. The whole tree is just these assets referencing each other —
/// adding a new class is creating an asset and wiring it, no code changes.
/// </summary>
[CreateAssetMenu(menuName = "Bullet Crash/Evolution Node")]
public class EvolutionNode : ScriptableObject
{
    [Header("Identity")]
    [SerializeField] private string displayName = "Tank";
    [SerializeField] private Sprite icon;

    [Header("Unlock")]
    [Tooltip("Minimum player level required to choose this node.")]
    [SerializeField] private int requiredLevel;

    [Header("Grants")]
    [Tooltip("Firing behavior equipped when this node is chosen. Null keeps the current weapon.")]
    [SerializeField] private WeaponBehavior weapon;

    [Tooltip("Passive stat bonuses applied when this node is chosen.")]
    [SerializeField] private StatBonus[] bonuses;

    [Header("Tree")]
    [Tooltip("Nodes this one can evolve into at the next tier.")]
    [SerializeField] private EvolutionNode[] children;

    public string DisplayName => displayName;
    public Sprite Icon => icon;
    public int RequiredLevel => requiredLevel;
    public WeaponBehavior Weapon => weapon;
    public IReadOnlyList<StatBonus> Bonuses => bonuses;
    public IReadOnlyList<EvolutionNode> Children => children;
}
