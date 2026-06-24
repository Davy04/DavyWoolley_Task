using System.Collections.Generic;
using UnityEngine;

namespace BulletCrash.Core.Stats
{
    /// <summary>
    /// A gameplay value (move speed, max HP, magnet range...) expressed as an immutable
    /// <see cref="BaseValue"/> plus a list of stackable <see cref="StatModifier"/>s.
    /// The final <see cref="Value"/> is computed lazily and cached until something changes.
    ///
    /// Formula: <c>(base + ΣFlat) * (1 + ΣPercentAdd) * Π(1 + PercentMult)</c>.
    /// Reusable across any project — depends on nothing but UnityEngine for serialization.
    /// </summary>
    [System.Serializable]
    public class Stat
    {
        [SerializeField] private float baseValue;

        private readonly List<StatModifier> _modifiers = new();
        private bool _isDirty = true;
        private float _cachedValue;

        public Stat() { }

        public Stat(float baseValue)
        {
            this.baseValue = baseValue;
        }

        /// <summary>The unmodified base. Setting it invalidates the cached value.</summary>
        public float BaseValue
        {
            get => baseValue;
            set
            {
                baseValue = value;
                _isDirty = true;
            }
        }

        public IReadOnlyList<StatModifier> Modifiers => _modifiers;

        /// <summary>Final value after all modifiers. Recomputed only when dirty.</summary>
        public float Value
        {
            get
            {
                if (_isDirty)
                {
                    _cachedValue = Calculate();
                    _isDirty = false;
                }
                return _cachedValue;
            }
        }

        public void AddModifier(StatModifier modifier)
        {
            _modifiers.Add(modifier);
            _isDirty = true;
        }

        public bool RemoveModifier(StatModifier modifier)
        {
            if (_modifiers.Remove(modifier))
            {
                _isDirty = true;
                return true;
            }
            return false;
        }

        /// <summary>Removes every modifier created by <paramref name="source"/>. Returns how many were removed.</summary>
        public int RemoveAllFromSource(object source)
        {
            int removed = _modifiers.RemoveAll(m => m.Source == source);
            if (removed > 0)
                _isDirty = true;
            return removed;
        }

        private float Calculate()
        {
            float final = baseValue;
            float sumPercentAdd = 0f;

            foreach (StatModifier modifier in _modifiers)
            {
                switch (modifier.Type)
                {
                    case StatModifierType.Flat:
                        final += modifier.Value;
                        break;
                    case StatModifierType.PercentAdd:
                        sumPercentAdd += modifier.Value;
                        break;
                }
            }

            final *= 1f + sumPercentAdd;

            foreach (StatModifier modifier in _modifiers)
            {
                if (modifier.Type == StatModifierType.PercentMult)
                    final *= 1f + modifier.Value;
            }

            return final;
        }
    }
}
