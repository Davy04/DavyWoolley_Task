using UnityEngine;
using Random = UnityEngine.Random;

public class Footsteps : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private AudioClip[] footstepClips;
    [SerializeField] private float stepInterval = 0.4f;
    [SerializeField] private float moveThreshold = 0.1f;
    [Range(0f, 1f)] [SerializeField] private float volume = 1f;

    private float _stepTimer;

    private void Update()
    {
        if (rb == null || footstepClips == null || footstepClips.Length == 0)
            return;

        bool moving = rb.linearVelocity.sqrMagnitude > moveThreshold * moveThreshold;

        if (!moving)
        {
            _stepTimer = 0f; // toca o primeiro passo imediatamente ao voltar a andar
            return;
        }

        _stepTimer -= Time.deltaTime;
        if (_stepTimer <= 0f)
        {
            PlayStep();
            _stepTimer = stepInterval;
        }
    }

    private void PlayStep()
    {
        AudioClip clip = footstepClips[Random.Range(0, footstepClips.Length)];
        AudioManager.Instance?.PlaySFX(clip, volume);
    }
}
