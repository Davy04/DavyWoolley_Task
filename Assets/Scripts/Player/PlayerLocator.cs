using UnityEngine;

// Single source of truth for the player's transform/components, so enemies
// don't each call GameObject.FindWithTag("Player").
public class PlayerLocator : MonoBehaviour
{
    public static Transform Player { get; private set; }
    public static Health PlayerHealth { get; private set; }
    public static Knockback PlayerKnockback { get; private set; }

    private void Awake()
    {
        Player          = transform;
        PlayerHealth    = GetComponent<Health>();
        PlayerKnockback = GetComponent<Knockback>();
    }

    private void OnDestroy()
    {
        if (Player == transform)
        {
            Player          = null;
            PlayerHealth    = null;
            PlayerKnockback = null;
        }
    }
}
