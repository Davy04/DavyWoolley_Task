using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponParent : MonoBehaviour
{
    public event Action<bool> OnFacingChanged;

    public bool IsAttacking { get; set; }
    public bool FacingLeft => _facingLeft;

    private bool _facingLeft;
    private Camera _camera;

    private void Awake()
    {
        _camera = Camera.main;
    }

    private void Update()
    {
        if (IsAttacking || Mouse.current == null)
            return;

        if (_camera == null)
            _camera = Camera.main;
        if (_camera == null)
            return;

        Vector2 mouseWorld = _camera.ScreenToWorldPoint(Mouse.current.position.value);
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
