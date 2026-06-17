using UnityEngine;

public class AddItemButton : MonoBehaviour
{
    public InventoryManager inventoryManager;
    public Item item;
    public int count = 1;

    public void OnClick()
    {
        inventoryManager.AddItem(item, count);
    }

    public void DropItem()
    {
        InventorySlot selectedSlot = inventoryManager.GetSelectedSlot();
        if (selectedSlot == null || selectedSlot.transform.childCount == 0)
        {
            Debug.Log("No item to drop.");
            return;
        }

        InventoryItem inventoryItem = selectedSlot.transform.GetChild(0).GetComponent<InventoryItem>();
        inventoryManager.DropItem(inventoryItem);
    }
}
