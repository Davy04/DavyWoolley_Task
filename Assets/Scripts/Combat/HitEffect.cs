using UnityEngine;

public class HitEffect : MonoBehaviour
{
    [SerializeField] private Health health;
    [SerializeField] private GameObject impactPrefab;
    [SerializeField] private Vector3 offset;

    private void OnEnable()  => health.OnDamaged += HandleDamaged;
    private void OnDisable() => health.OnDamaged -= HandleDamaged;

    private void HandleDamaged(int _)
    {
        if (impactPrefab == null) return;
        Instantiate(impactPrefab, transform.position + offset, Quaternion.identity);
    }
}
