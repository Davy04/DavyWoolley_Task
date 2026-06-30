using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Grants one stat point per level-up and spends it on a stat (capped). Each point applies the
/// authored per-point <see cref="StatBonus"/> to <see cref="PlayerStats"/>. UI rows read this.
/// </summary>
public class StatPointSystem : MonoBehaviour
{
    [SerializeField] private PlayerStats stats;
    [SerializeField] private PlayerExperience experience;
    [SerializeField] private TMP_Text pointsLabel;
    [SerializeField] private int cap = 10;

    [Tooltip("Per-point increment for each upgradable stat (one entry per stat, with its type).")]
    [SerializeField] private StatBonus[] perPointBonuses;

    public int AvailablePoints { get; private set; }
    public int Cap => cap;
    public event Action OnChanged;

    private readonly Dictionary<PlayerStatType, int> _spent = new();

    public int GetLevel(PlayerStatType type) => _spent.TryGetValue(type, out int n) ? n : 0;

    private void OnEnable()
    {
        if (experience != null) experience.OnLevelUp += HandleLevelUp;
    }

    private void OnDisable()
    {
        if (experience != null) experience.OnLevelUp -= HandleLevelUp;
    }

    private void Start() => Refresh();

    private void HandleLevelUp(int _)
    {
        AvailablePoints++;
        Refresh();
    }

    public void Upgrade(PlayerStatType type)
    {
        if (AvailablePoints <= 0 || GetLevel(type) >= cap) return;

        StatBonus? bonus = Find(type);
        if (bonus == null) return; // stat not configured

        stats.ApplyBonus(bonus.Value, this);
        _spent[type] = GetLevel(type) + 1;
        AvailablePoints--;
        Refresh();
    }

    private StatBonus? Find(PlayerStatType type)
    {
        foreach (StatBonus b in perPointBonuses)
            if (b.Stat == type) return b;
        return null;
    }

    private void Refresh()
    {
        if (pointsLabel != null) pointsLabel.text = $"+{AvailablePoints} PTS";
        OnChanged?.Invoke();
    }
}
