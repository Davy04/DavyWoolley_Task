using System.Collections;
using UnityEngine;

public class Knockback : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private MonoBehaviour movementComponent;
    [SerializeField] private float force = 6f;
    [SerializeField] private float duration = 0.2f;

    private Coroutine _coroutine;

    public void Apply(Vector2 direction)
    {
        if (_coroutine != null)
            StopCoroutine(_coroutine);
        _coroutine = StartCoroutine(DoKnockback(direction));
    }

    private IEnumerator DoKnockback(Vector2 direction)
    {
        if (movementComponent != null) movementComponent.enabled = false;

        rb.linearVelocity = direction.normalized * force;

        yield return new WaitForSeconds(duration);

        rb.linearVelocity = Vector2.zero;
        if (movementComponent != null) movementComponent.enabled = true;
    }
}
