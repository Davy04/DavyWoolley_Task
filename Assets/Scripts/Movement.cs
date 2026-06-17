using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    public float speed = 0.5f;
    [SerializeField] private Rigidbody2D rb;
    private Vector2 input;

    void Update()
    {
        if (Keyboard.current == null)
        {
            input = Vector2.zero;
            return;
        }

        input = new Vector2(
            (Keyboard.current.dKey.isPressed ? 1 : 0) - (Keyboard.current.aKey.isPressed ? 1 : 0),
            (Keyboard.current.wKey.isPressed ? 1 : 0) - (Keyboard.current.sKey.isPressed ? 1 : 0));
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = input.normalized * speed;
    }
}
