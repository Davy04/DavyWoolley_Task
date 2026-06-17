using UnityEngine;

public class DustEffect : MonoBehaviour
{
    [SerializeField] private ParticleSystem dustParticles;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float moveThreshold = 0.1f;

    private void Update()
    {
        if (dustParticles == null || rb == null) return;

        bool isMoving = rb.linearVelocity.sqrMagnitude > moveThreshold * moveThreshold;

        var emission = dustParticles.emission;
        emission.enabled = isMoving;

        if (isMoving)
        {
            Vector2 opposite = -rb.linearVelocity.normalized;
            float angle = Mathf.Atan2(opposite.y, opposite.x) * Mathf.Rad2Deg;
            dustParticles.transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }
    }
}
