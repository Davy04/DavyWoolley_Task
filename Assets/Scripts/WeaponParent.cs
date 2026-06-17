using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponParent : MonoBehaviour
{
    public event Action<bool> OnFacingChanged;

    public bool IsAttacking { get; set; }

    private bool _facingLeft;

    private void Update()
    {
        if (IsAttacking || Mouse.current == null || Camera.main == null)
            return;

        Vector2 mouseWorld = Camera.main.ScreenToWorldPoint(Mouse.current.position.value);
        Vector2 direction = (mouseWorld - (Vector2)transform.position).normalized;
        float aimAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        transform.localRotation = Quaternion.Euler(0f, 0f, aimAngle);

        bool facingLeft = mouseWorld.x < transform.parent.position.x;
        if (facingLeft != _facingLeft)
        {
            _facingLeft = facingLeft;
            OnFacingChanged?.Invoke(_facingLeft);
        }
    }
}
