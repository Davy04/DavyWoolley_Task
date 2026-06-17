using System.Runtime.Serialization;

[DataContract]
public class InventorySlotData
{
    [DataMember] public int SlotIndex;
    [DataMember] public string ItemName;
    [DataMember] public int Count;

    public InventorySlotData(int slotIndex, string itemName, int count)
    {
        SlotIndex = slotIndex;
        ItemName = itemName;
        Count = count;
    }
}
