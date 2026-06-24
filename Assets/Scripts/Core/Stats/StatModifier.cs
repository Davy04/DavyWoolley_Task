namespace BulletCrash.Core.Stats
{
    /// <summary>How a <see cref="StatModifier"/> combines into the final value.</summary>
    public enum StatModifierType
    {
        /// <summary>Flat bonus added to the base before any percentage. e.g. +25 HP.</summary>
        Flat,
        /// <summary>Additive percentage. All of these are summed, then applied once. e.g. +15% speed.</summary>
        PercentAdd,
        /// <summary>Multiplicative percentage, applied last and compounding. e.g. a 2x boss-rush buff.</summary>
        PercentMult
    }

    /// <summary>
    /// A single change applied to a <see cref="Stat"/>. Immutable. Keeps a reference to its
    /// <see cref="Source"/> (the upgrade, item or buff that created it) so it can be removed later
    /// without touching the others — the key requirement for a roguelike upgrade system.
    /// </summary>
    public class StatModifier
    {
        public float Value { get; }
        public StatModifierType Type { get; }
        public object Source { get; }

        public StatModifier(float value, StatModifierType type, object source = null)
        {
            Value = value;
            Type = type;
            Source = source;
        }
    }
}
