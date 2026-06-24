using UnityEngine;

/// <summary>
/// Abstracts the source of movement intent. Implementations decide *how* the direction is
/// produced (keyboard, gamepad, on-screen joystick, AI...) while <see cref="PlayerMovement"/>
/// only cares about the resulting vector. Direction is normalized/clamped to magnitude ≤ 1.
/// </summary>
public interface IMovementInput
{
    Vector2 Direction { get; }
}
