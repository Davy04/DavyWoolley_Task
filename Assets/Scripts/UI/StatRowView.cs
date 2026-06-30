using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>One stat row: the +button spends a point, the bar/label show level/cap.</summary>
public class StatRowView : MonoBehaviour
{
    [SerializeField] private StatPointSystem system;
    [SerializeField] private PlayerStatType stat;
    [SerializeField] private Slider slider;
    [SerializeField] private TMP_Text label;   // "3/10"
    [SerializeField] private Button plusButton;

    private void Awake()
    {
        if (system == null) system = GetComponentInParent<StatPointSystem>();
    }

    private void OnEnable()
    {
        plusButton.onClick.AddListener(Upgrade);
        if (system != null) system.OnChanged += Refresh;
        Refresh();
    }

    private void OnDisable()
    {
        plusButton.onClick.RemoveListener(Upgrade);
        if (system != null) system.OnChanged -= Refresh;
    }

    private void Upgrade() => system.Upgrade(stat);

    private void Refresh()
    {
        int level = system.GetLevel(stat);
        slider.value = system.Cap > 0 ? (float)level / system.Cap : 0f;
        if (label != null) label.text = $"{level}/{system.Cap}";
        plusButton.interactable = system.AvailablePoints > 0 && level < system.Cap;
    }
}
