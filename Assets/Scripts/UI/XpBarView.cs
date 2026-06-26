using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Binds <see cref="PlayerExperience"/> to the HUD: fills the XP slider (0–1) and writes the
/// level label. Purely presentational — it only reads the experience events, never the gameplay.
/// </summary>
public class XpBarView : MonoBehaviour
{
    [SerializeField] private PlayerExperience experience;
    [SerializeField] private Slider xpSlider;
    [SerializeField] private TMP_Text levelText;

    private void OnEnable()
    {
        if (experience == null) return;
        experience.OnXpChanged += HandleXpChanged;
        experience.OnLevelUp += HandleLevelUp;
    }

    private void OnDisable()
    {
        if (experience == null) return;
        experience.OnXpChanged -= HandleXpChanged;
        experience.OnLevelUp -= HandleLevelUp;
    }

    private void Start()
    {
        if (experience != null)
            HandleLevelUp(experience.Level);
    }

    private void HandleXpChanged(int current, int required)
    {
        if (xpSlider != null)
            xpSlider.value = required > 0 ? (float)current / required : 0f;
    }

    private void HandleLevelUp(int level)
    {
        if (levelText != null)
            levelText.text =
                $"<size=80%><color=#7A69B3>LVL </color></size><size=100%><color=#F4C44E>{level}</color></size>";
    }
}
