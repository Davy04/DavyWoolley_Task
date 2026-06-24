using BulletCrash.Core.Stats;
using UnityEngine;

/// <summary>
/// Holds the player's live, modifiable stats for the current run. Built from a
/// <see cref="CharacterData"/> asset on <see cref="Awake"/>. This is the single place
/// upgrades, items and buffs read from and write to (via <see cref="Stat.AddModifier"/>).
/// </summary>
public class PlayerStats : MonoBehaviour
{
    [SerializeField] private CharacterData character;

    /// <summary>Movement speed in units/second. Consumed by <see cref="PlayerMovement"/>.</summary>
    public Stat MoveSpeed { get; private set; }

    private void Awake()
    {
        if (character == null)
        {
            Debug.LogError($"{nameof(PlayerStats)} is missing a {nameof(CharacterData)} asset.", this);
            return;
        }

        MoveSpeed = new Stat(character.BaseMoveSpeed);
    }
}
