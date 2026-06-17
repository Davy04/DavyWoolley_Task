using TMPro;
using UnityEngine;

public class TooltipUI : MonoBehaviour
{
    public static TooltipUI Instance { get; private set; }

    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private TMP_Text statsText;
    [SerializeField] private float verticalPadding = 8f;

    private RectTransform _rectTransform;

    private void Awake()
    {
        Instance = this;
        _rectTransform = GetComponent<RectTransform>();
        panel.SetActive(false);
    }

    public void Show(Item item, RectTransform sourceRect)
    {
        nameText.text = item.itemName;
        descriptionText.text = item.description;
        statsText.text = BuildStats(item);
        panel.SetActive(true);

        Canvas.ForceUpdateCanvases();

        float itemTop = sourceRect.position.y + sourceRect.rect.height * sourceRect.lossyScale.y / 2f;
        float tooltipHeight = _rectTransform.rect.height * _rectTransform.lossyScale.y;

        Vector2 pos = new Vector2(sourceRect.position.x, itemTop + tooltipHeight / 2f + verticalPadding);

        pos.x = Mathf.Clamp(pos.x, _rectTransform.rect.width / 2f, Screen.width - _rectTransform.rect.width / 2f);
        pos.y = Mathf.Min(pos.y, Screen.height - tooltipHeight / 2f);

        _rectTransform.position = pos;
    }

    public void Hide()
    {
        panel.SetActive(false);
    }

    private string BuildStats(Item item)
    {
        return item.type switch
        {
            ItemType.Weapon => $"Damage: {item.damage}\nAttack Speed: {item.attackSpeed}",
            ItemType.Consumable => $"HP: +{item.healthRestore}\nMP: +{item.manaRestore}",
            _ => string.Empty
        };
    }
}
