using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Health health;
    [SerializeField] private Slider slider;

    private void Start()
    {
        slider.minValue = 0;
        slider.maxValue = health.Max;
        slider.value    = health.Current;
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

    private void Refresh(int _) => slider.value = health.Current;
}
