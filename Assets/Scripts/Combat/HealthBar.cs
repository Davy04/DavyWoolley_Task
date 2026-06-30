using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Health health;
    [SerializeField] private Slider slider;
    [SerializeField] private TMP_Text label;

    private void Start()
    {
        slider.minValue = 0;
        slider.maxValue = health.Max;
        slider.value    = health.Current;
        UpdateLabel();
    }

    private void OnEnable()
    {
        health.OnDamaged += Refresh;
        health.OnHealed  += Refresh;
    }

    private void OnDisable()
    {
        health.OnDamaged -= Refresh;
        health.OnHealed  -= Refresh;
    }

    private void Refresh(int _)
    {
        slider.value = health.Current;
        UpdateLabel();
    }

    private void UpdateLabel()
    {
        if (label != null)
            label.text = $"{health.Current}/{health.Max}";
    }
}
