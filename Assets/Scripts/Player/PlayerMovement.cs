using UnityEngine;

/// <summary>
/// Moves the player's Rigidbody2D. Deliberately "dumb": it owns no input logic and no speed
/// value of its own — it reads direction from an <see cref="IMovementInput"/> and the final
/// speed from <see cref="PlayerStats"/>, so upgrades affect movement without this class
/// knowing they exist.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private PlayerStats stats;

    private IMovementInput _input;

    private void Awake()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        _input = GetComponent<IMovementInput>();
        if (_input == null)
            Debug.LogError($"{nameof(PlayerMovement)} requires a component implementing {nameof(IMovementInput)}.", this);
    }

    private void FixedUpdate()
    {
        if (_input == null || stats == null || stats.MoveSpeed == null)
            return;

        rb.linearVelocity = _input.Direction * stats.MoveSpeed.Value;
    }
}
