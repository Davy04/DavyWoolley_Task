using System;
using SerializableMethods;
using UnityEngine;

/// <summary>
/// Tracks the player's XP and level for the current session. Knows nothing about where XP
/// comes from (kills, destructibles) or who reacts to leveling — it just accumulates XP,
/// rolls levels over, and raises events. The HUD and the evolution system subscribe.
/// </summary>
public class PlayerExperience : MonoBehaviour
{
    [Header("Leveling Curve")]
    [Tooltip("XP required to go from level 1 to 2.")]
    [SerializeField] private int baseXp = 10;

    [Tooltip("XP requirement multiplier per level (exponential curve).")]
    [SerializeField] private float growth = 1.2f;

    /// <summary>XP accumulated inside the current level (resets to the carry-over on level up).</summary>
    public int CurrentXp { get; private set; }

    /// <summary>XP needed to reach the next level.</summary>
    public int XpToNextLevel { get; private set; }

    public int Level { get; private set; } = 1;

    /// <summary>(currentXp, xpToNextLevel) — fired whenever XP or the requirement changes.</summary>
    public event Action<int, int> OnXpChanged;

    /// <summary>(newLevel) — fired once per level gained.</summary>
    public event Action<int> OnLevelUp;

    private void Awake()
    {
        XpToNextLevel = XpRequiredForLevel(Level);
    }

    private void Start()
    {
        // Let the HUD draw its initial state once subscriptions are in place.
        OnXpChanged?.Invoke(CurrentXp, XpToNextLevel);
    }

    /// <summary>
    /// Grants XP and rolls over as many levels as the amount covers (carry-over preserved).
    /// The base entry point — XP sources (and the debug button) all funnel through here.
    /// </summary>
    [SerializeMethod]
    public void GainXp(int amount)
    {
        if (amount <= 0) return;

        CurrentXp += amount;

        while (CurrentXp >= XpToNextLevel)
        {
            CurrentXp -= XpToNextLevel;
            Level++;
            XpToNextLevel = XpRequiredForLevel(Level);
            OnLevelUp?.Invoke(Level);
        }

        OnXpChanged?.Invoke(CurrentXp, XpToNextLevel);
    }

    private int XpRequiredForLevel(int level) =>
        Mathf.RoundToInt(baseXp * Mathf.Pow(growth, level - 1));
}
