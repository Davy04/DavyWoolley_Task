using UnityEngine;

/// <summary>
/// Plays a muzzle effect at this object (the firing indicator) every time the player shoots.
/// Presentation only — it listens to <see cref="PlayerShooter.OnFired"/> and never touches
/// gameplay. Reuses a single ParticleSystem (Play on each shot) instead of instantiating, so
/// it stays cheap even at high fire rate (WebGL budget).
/// </summary>
public class MuzzleFlash : MonoBehaviour
{
    [SerializeField] private PlayerShooter shooter;
    [SerializeField] private ParticleSystem effect;

    private void Awake()
    {
        if (shooter == null)
            shooter = GetComponentInParent<PlayerShooter>();

        if (effect == null)
            effect = GetComponent<ParticleSystem>();
    }

    private void OnEnable()
    {
        if (shooter != null)
            shooter.OnFired += Play;
    }

    private void OnDisable()
    {
        if (shooter != null)
            shooter.OnFired -= Play;
    }

    private void Play()
    {
        if (effect != null)
            effect.Play();
    }
}
