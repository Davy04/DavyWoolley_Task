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

    public bool HasSpace => item != null && count < item.maxStack;

    /// <summary>
    /// Adds up to <paramref name="amount"/> without exceeding maxStack.
    /// Returns how many were actually added, so the caller can place the leftover elsewhere.
    /// </summary>
    public int AddAmount(int amount)
    {
        if (item == null || amount <= 0)
            return 0;

        int freeSpace = item.maxStack - count;
        int added = Mathf.Clamp(amount, 0, freeSpace);
        count += added;
        RefreshUI();
        return added;
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
    }

    public void ConsumeOne()
    {
        count--;
        if (count <= 0)
        {
            transform.SetParent(null);
            Destroy(gameObject);
            InventoryManager.Instance?.NotifySlotChanged();
            return;
        }

        RefreshUI();
        InventoryManager.Instance?.SaveInventory();
    }
}
