using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Default <see cref="IMovementInput"/>: reads WASD and the gamepad left stick (8-directional,
/// per GDD). Keyboard takes priority; the stick's analog magnitude is preserved (clamped to 1)
/// so partial tilts walk slower. Diagonals are clamped so they aren't faster than cardinals.
/// </summary>
public class PlayerInputReader : MonoBehaviour, IMovementInput
{
    public Vector2 Direction { get; private set; }

    private void Update()
    {
        Vector2 raw = ReadKeyboard();

        if (raw == Vector2.zero)
            raw = ReadGamepad();

        Direction = Vector2.ClampMagnitude(raw, 1f);
    }

    private static Vector2 ReadKeyboard()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null)
            return Vector2.zero;

        return new Vector2(
            (keyboard.dKey.isPressed ? 1f : 0f) - (keyboard.aKey.isPressed ? 1f : 0f),
            (keyboard.wKey.isPressed ? 1f : 0f) - (keyboard.sKey.isPressed ? 1f : 0f));
    }

    private static Vector2 ReadGamepad()
    {
        Gamepad gamepad = Gamepad.current;
        if (gamepad == null)
            return Vector2.zero;

        Vector2 stick = gamepad.leftStick.ReadValue();
        return stick.sqrMagnitude > 0.01f ? stick : Vector2.zero;
    }
}
