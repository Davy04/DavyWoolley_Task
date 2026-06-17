using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class InventoryItem : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler,
    IPointerEnterHandler, IPointerExitHandler
{
    [Header("Dados")]
    public Item item;
    public int count = 1;

    [Tooltip("Optional text showing the count of stackable items.")]
    public TMP_Text countText;

    public Image image;
    [HideInInspector] public Transform parentAfterDrag;

    private void Awake()
    {
        if (image == null)
            image = GetComponent<Image>();
    }

    private void Start()
    {
        Initialize(item, count);
    }

    public void Initialize(Item newItem, int amount = 1)
    {
        item = newItem;
        count = amount;
        RefreshUI();
    }

    public void AddCount(int amount)
    {
        count = Mathf.Min(count + amount, item != null ? item.maxStack : int.MaxValue);
        RefreshUI();
    }

    private void RefreshUI()
    {
        if (item != null && image != null)
        {
            image.sprite = item.icon;
            image.enabled = item.icon != null;
        }

        if (countText != null)
        {
            bool showCount = item != null && item.stackable && count > 1;
            countText.enabled = showCount;
            countText.text = showCount ? count.ToString() : string.Empty;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (item != null && InventoryManager.Instance != null && InventoryManager.Instance.IsBagOpen)
            TooltipUI.Instance?.Show(item, transform as RectTransform);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipUI.Instance?.Hide();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        TooltipUI.Instance?.Hide();
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
        image.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Mouse.current.position.value;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(parentAfterDrag);
        image.raycastTarget = true;
        InventoryManager.Instance?.NotifySlotChanged();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Right)
            return;

        if (InventoryManager.Instance != null && InventoryManager.Instance.IsBagOpen)
            InventoryManager.Instance.DropItem(this);
        else
            UseItem();
    }

    public void UseItem()
    {
        if (item == null)
            return;

        bool consumed = item.Use(gameObject);

        if (consumed)
        {
            count--;
            if (count <= 0)
            {
                Destroy(gameObject);
                InventoryManager.Instance?.SaveInventory();
                return;
            }
        }

        RefreshUI();
        InventoryManager.Instance?.SaveInventory();
    }
}
