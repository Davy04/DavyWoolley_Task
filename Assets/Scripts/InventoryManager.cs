using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [Tooltip("Slots que compõem o inventário.")]
    public List<InventorySlot> slots = new List<InventorySlot>();

    [Tooltip("Prefab do item da UI (precisa ter Image + InventoryItem).")]
    public GameObject itemPrefab;

    // Adiciona um item ao primeiro slot vazio.
    // Retorna o InventoryItem criado, ou null se não houver espaço.
    public InventoryItem AddItem(Item item, int count = 1)
    {
        if (item == null)
            return null;

        InventorySlot freeSlot = GetFirstEmptySlot();
        if (freeSlot == null)
        {
            Debug.Log("Inventário cheio.");
            return null;
        }

        GameObject obj = Instantiate(itemPrefab, freeSlot.transform);
        InventoryItem inventoryItem = obj.GetComponent<InventoryItem>();
        inventoryItem.Initialize(item, count);
        return inventoryItem;
    }

    // Remove um item do inventário (tira do slot e destrói o objeto da UI).
    public void DropItem(InventoryItem inventoryItem)
    {
        if (inventoryItem == null)
            return;

        Destroy(inventoryItem.gameObject);
    }

    private InventorySlot GetFirstEmptySlot()
    {
        foreach (InventorySlot slot in slots)
        {
            if (slot != null && slot.transform.childCount == 0)
                return slot;
        }

        return null;
    }
}
