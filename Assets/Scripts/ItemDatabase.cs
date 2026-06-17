using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Object/Item Database")]
public class ItemDatabase : ScriptableObject
{
    public List<Item> items = new List<Item>();

    public Item GetItem(string itemName)
    {
        return items.Find(i => i.itemName == itemName);
    }
}
