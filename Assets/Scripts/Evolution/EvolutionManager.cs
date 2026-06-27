using System;
using System.Collections.Generic;
using SerializableMethods;
using UnityEngine;

/// <summary>
/// Drives the player's path through the evolution tree. Starts at the root node and, whenever
/// the player levels up, offers the current node's children that have reached their required
/// level. Choosing one applies its weapon and stat bonuses and advances the current node.
///
/// Decoupled by design: it consumes a level via <see cref="NotifyLevelUp"/> (fed later by the
/// XP system) and equips weapons through an optional <see cref="IWeaponHolder"/>, so neither
/// system needs to exist for the tree to work and be tested.
/// </summary>
public class EvolutionManager : MonoBehaviour
{
    [SerializeField] private EvolutionNode root;
    [SerializeField] private PlayerStats stats;

    [Tooltip("XP source that drives the tiers. Optional — leave empty to drive levels manually.")]
    [SerializeField] private PlayerExperience experience;

    /// <summary>Raised when leveling up unlocks one or more choices — the UI listens to this.</summary>
    public event Action<IReadOnlyList<EvolutionNode>> OnChoicesAvailable;

    /// <summary>Raised after a node is chosen and applied.</summary>
    public event Action<EvolutionNode> OnEvolved;

    public EvolutionNode CurrentNode { get; private set; }

    private IWeaponHolder _weaponHolder;
    private int _currentLevel = 1;

    private void Awake()
    {
        if (root == null)
        {
            Debug.LogError($"{nameof(EvolutionManager)} is missing a root {nameof(EvolutionNode)}.", this);
            enabled = false;
            return;
        }

        if (stats == null)
            stats = GetComponent<PlayerStats>();

        _weaponHolder = GetComponent<IWeaponHolder>();

        CurrentNode = root;
        Apply(root);
    }

    private void OnEnable()
    {
        if (experience != null)
            experience.OnLevelUp += NotifyLevelUp;
    }

    private void OnDisable()
    {
        if (experience != null)
            experience.OnLevelUp -= NotifyLevelUp;
    }

    /// <summary>Called by the XP system on every level-up. Opens a choice if one is unlocked.</summary>
    public void NotifyLevelUp(int newLevel)
    {
        _currentLevel = newLevel;

        IReadOnlyList<EvolutionNode> choices = GetAvailableChoices();
        if (choices.Count > 0)
            OnChoicesAvailable?.Invoke(choices);
    }

    /// <summary>Current node's children that the player's level already unlocks.</summary>
    public List<EvolutionNode> GetAvailableChoices()
    {
        var choices = new List<EvolutionNode>();
        if (CurrentNode == null)
            return choices;

        foreach (EvolutionNode child in CurrentNode.Children)
        {
            if (child != null && _currentLevel >= child.RequiredLevel)
                choices.Add(child);
        }
        return choices;
    }

    /// <summary>Commits an evolution. Must be one of the currently available choices.</summary>
    public void Choose(EvolutionNode node)
    {
        if (node == null || !GetAvailableChoices().Contains(node))
        {
            Debug.LogWarning($"{nameof(EvolutionManager)}: '{node?.DisplayName}' is not a valid choice right now.", this);
            return;
        }

        CurrentNode = node;
        Apply(node);
        OnEvolved?.Invoke(node);
    }

    private void Apply(EvolutionNode node)
    {
        foreach (StatBonus bonus in node.Bonuses)
            stats.ApplyBonus(bonus, node);

        if (node.Weapon != null)
            _weaponHolder?.SetWeapon(node.Weapon);
    }

    // -------------------------------------------------------------------------
    // Test methods — simulate the XP system until it exists.
    // -------------------------------------------------------------------------

    [SerializeMethod]
    private void SimulateLevelUp(int newLevel)
    {
        NotifyLevelUp(newLevel);
        List<EvolutionNode> choices = GetAvailableChoices();

        if (choices.Count == 0)
        {
            Debug.Log($"[Evolution] Nível {newLevel}: nenhuma evolução disponível ainda.");
            return;
        }

        var names = new List<string>(choices.Count);
        foreach (EvolutionNode c in choices)
            names.Add($"{c.DisplayName} (req. {c.RequiredLevel})");

        Debug.Log($"[Evolution] Nível {newLevel} — escolhas: {string.Join(", ", names)}");
    }

    [SerializeMethod]
    private void ChooseFirstAvailable()
    {
        List<EvolutionNode> choices = GetAvailableChoices();
        if (choices.Count == 0)
        {
            Debug.Log("[Evolution] Nada para escolher no nível atual.");
            return;
        }

        Choose(choices[0]);
        Debug.Log($"[Evolution] Evoluiu para: {CurrentNode.DisplayName}");
    }
}
