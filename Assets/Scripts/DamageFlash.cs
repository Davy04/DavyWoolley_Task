using System.Collections;
using UnityEngine;

public class DamageFlash : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color flashColor = Color.red;
    [SerializeField] private float flashDuration = 0.12f;

    private Health _health;
    private Color _originalColor;
    private Coroutine _coroutine;

    private void Awake()
    {
        _health = GetComponent<Health>();
        if (spriteRenderer != null)
            _originalColor = spriteRenderer.color;
    }

    private void OnEnable()  => _health.OnDamaged += HandleDamaged;
    private void OnDisable() => _health.OnDamaged -= HandleDamaged;

    private void HandleDamaged(int _)
    {
        if (spriteRenderer == null) return;
        if (_coroutine != null) StopCoroutine(_coroutine);
        _coroutine = StartCoroutine(Flash());
    }

    private IEnumerator Flash()
    {
        spriteRenderer.color = flashColor;
        yield return new WaitForSeconds(flashDuration);
        if (this != null)
            spriteRenderer.color = _originalColor;
    }
}
