using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IDropHandler
{

    public Image slotImage;
    public Color selectedColor, unselectedColor;

    private void Awake()
    {
        Deselect();
    }

    public void Select()
    {
        slotImage.color = selectedColor;
    }

    public void Deselect()
    {
        slotImage.color = unselectedColor;
    }

    public void OnDrop(PointerEventData eventData)
    {
        GameObject dropped = eventData.pointerDrag;
        InventoryItem draggedItem = dropped.GetComponent<InventoryItem>();

        if (transform.childCount == 0)
        {
            draggedItem.parentAfterDrag = transform;
            return;
        }

        InventoryItem slotItem = transform.GetChild(0).GetComponent<InventoryItem>();
        Transform originalParent = draggedItem.parentAfterDrag;

        draggedItem.parentAfterDrag = transform;
        slotItem.transform.SetParent(originalParent);
    }
}
