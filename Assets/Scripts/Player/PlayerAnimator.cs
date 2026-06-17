using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private WeaponParent weaponParent;
    [SerializeField] private SpriteRenderer headRenderer;
    [SerializeField] private SpriteRenderer bodyRenderer;

    private static readonly int IsMoving = Animator.StringToHash("isMoving");

    private void OnEnable()
    {
        weaponParent.OnFacingChanged += HandleFacingChanged;
    }

    private void OnDisable()
    {
        weaponParent.OnFacingChanged -= HandleFacingChanged;
    }

    private void Update()
    {
        animator.SetBool(IsMoving, rb.linearVelocity.sqrMagnitude > 0.01f);
    }

    private void HandleFacingChanged(bool facingLeft)
    {
        headRenderer.flipX = facingLeft;
        bodyRenderer.flipX = facingLeft;
    }
}
