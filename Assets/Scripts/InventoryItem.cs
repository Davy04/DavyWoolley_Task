using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// Item da UI do inventário: cuida do arraste (drag-n-drop) e dos dados/uso do item.
// Fica num GameObject com uma Image, dentro de um InventorySlot.
// - Botão esquerdo: arrastar.
// - Botão direito: usar o item (equipar arma / consumir consumível).
[RequireComponent(typeof(Image))]
public class InventoryItem : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    [Header("Dados")]
    public Item item;
    public int count = 1;

    [Tooltip("Texto opcional que mostra a quantidade de itens empilháveis.")]
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

    // Define qual item este slot da UI representa e atualiza o visual.
    public void Initialize(Item newItem, int amount = 1)
    {
        item = newItem;
        count = amount;
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

    // --- Drag-n-drop ---

    public void OnBeginDrag(PointerEventData eventData)
    {
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
    }

    // --- Uso do item ---

    public void OnPointerClick(PointerEventData eventData)
    {
        // Só reage ao clique com o botão direito; o esquerdo é usado para arrastar.
        if (eventData.button != PointerEventData.InputButton.Right)
            return;

        UseItem();
    }

    // Usa o item. Consumíveis diminuem a contagem (e somem ao zerar); armas só equipam.
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
                return;
            }
        }

        RefreshUI();
    }
}
