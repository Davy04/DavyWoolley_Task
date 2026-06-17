using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    [Header("Active Inventory (Hotbar)")]
    [Tooltip("Slots that make up the hotbar.")]
    public List<InventorySlot> slots = new List<InventorySlot>();

    [Header("Bag Inventory")]
    [Tooltip("Slots that make up the bag.")]
    public List<InventorySlot> bagSlots = new List<InventorySlot>();

    [Tooltip("UI panel that contains the bag slots.")]
    public GameObject bagPanel;

    [Tooltip("Key used to open/close the bag.")]
    public Key bagKey = Key.Tab;

    [Header("References")]
    [Tooltip("UI item prefab (must have Image + InventoryItem).")]
    public GameObject itemPrefab;

    [Tooltip("SpriteRenderer of the Weapon part of the character.")]
    public SpriteRenderer weaponRenderer;

    [Tooltip("Database with all available items.")]
    public ItemDatabase itemDatabase;

    [Tooltip("Prefab spawned in the world when an item is dropped.")]
    public GameObject worldItemPrefab;

    [Tooltip("Transform used as spawn point when dropping items.")]
    public Transform dropPoint;

    [Tooltip("How far from the player the dropped item spawns.")]
    public float dropDistance = 0.6f;

    [Tooltip("Force applied to the item when dropped.")]
    public float dropForce = 4f;

    [Tooltip("Seconds the dropped item cannot be picked up (game time, after the bag closes).")]
    public float dropPickupBlock = 0.8f;

    [Tooltip("Sound played when an item is dropped to the world.")]
    public AudioClip dropSound;

    public bool IsBagOpen { get; private set; }

    public Item CurrentItem => _currentItem != null ? _currentItem.item : null;

    public WeaponBehavior CurrentBehavior =>
        _currentItem != null && _currentItem.item != null ? _currentItem.item.weaponBehavior : null;

    // Fired both when the selected slot changes and when its contents change.
    public event Action OnSelectedItemChanged;

    private int _selectedSlotIndex = -1;
    private string _savePath;
    private InventoryItem _currentItem;

    private static readonly Key[] HotbarKeys =
    {
        Key.Digit1, Key.Digit2, Key.Digit3, Key.Digit4, Key.Digit5
    };

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        _savePath = Path.Combine(Application.persistentDataPath, "InventoryData");
    }

    private void Start()
    {
        if (bagPanel != null)
            bagPanel.SetActive(false);

        if (slots.Count > 0)
            SelectSlot(0);

        LoadInventory();
    }

    private void OnDestroy()
    {
        Time.timeScale = 1f;
    }

    private void Update()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null)
            return;

        if (keyboard[bagKey].wasPressedThisFrame)
            ToggleBag();

        if (IsBagOpen)
            return;

        for (int i = 0; i < HotbarKeys.Length; i++)
        {
            if (keyboard[HotbarKeys[i]].wasPressedThisFrame)
            {
                SelectSlot(i);
                break;
            }
        }
    }

    public void ToggleBag()
    {
        IsBagOpen = !IsBagOpen;

        if (bagPanel != null)
            bagPanel.SetActive(IsBagOpen);

        if (!IsBagOpen)
            TooltipUI.Instance?.Hide();

        Time.timeScale = IsBagOpen ? 0f : 1f;
    }

    public void SelectSlot(int index)
    {
        if (index < 0 || index >= slots.Count)
            return;

        if (_selectedSlotIndex >= 0 && _selectedSlotIndex < slots.Count)
            slots[_selectedSlotIndex].Deselect();

        _selectedSlotIndex = index;
        slots[_selectedSlotIndex].Select();
        RefreshWeapon();
    }

    private void RefreshWeapon()
    {
        InventorySlot slot = GetSelectedSlot();
        InventoryItem held = slot != null && slot.transform.childCount > 0
            ? slot.transform.GetChild(0).GetComponent<InventoryItem>()
            : null;

        _currentItem = held;

        if (weaponRenderer != null)
        {
            if (held != null && held.item != null)
            {
                weaponRenderer.sprite = held.item.icon;
                weaponRenderer.transform.localScale = held.item.weaponScale;
                weaponRenderer.transform.localRotation = Quaternion.Euler(0f, 0f, held.item.rotation);
            }
            else
            {
                weaponRenderer.sprite = null;
                weaponRenderer.transform.localScale = Vector3.one;
                weaponRenderer.transform.localRotation = Quaternion.identity;
            }
        }

        OnSelectedItemChanged?.Invoke();
    }

    public void ConsumeCurrent()
    {
        _currentItem?.ConsumeOne();
    }

    public InventorySlot GetSelectedSlot()
    {
        if (_selectedSlotIndex < 0 || _selectedSlotIndex >= slots.Count)
            return null;

        return slots[_selectedSlotIndex];
    }

    /// <summary>
    /// Adds items to the inventory, filling existing stacks before using empty slots.
    /// Returns how many were actually added (less than <paramref name="count"/> if the inventory filled up).
    /// </summary>
    public int AddItem(Item item, int count = 1)
    {
        if (item == null || count <= 0)
            return 0;

        int remaining = count;
        
        if (item.stackable)
        {
            InventoryItem stack = FindStackWithSpace(item);
            while (remaining > 0 && stack != null)
            {
                remaining -= stack.AddAmount(remaining);
                stack = FindStackWithSpace(item);
            }
        }

        while (remaining > 0)
        {
            InventorySlot freeSlot = GetFirstEmptySlot();
            if (freeSlot == null)
            {
                Debug.Log("Inventory full.");
                break;
            }

            int amount = item.stackable ? Mathf.Min(remaining, item.maxStack) : 1;
            CreateItemInSlot(item, amount, freeSlot);
            remaining -= amount;
        }

        int added = count - remaining;
        if (added > 0)
        {
            RefreshWeapon();
            SaveInventory();
        }

        return added;
    }

    private InventoryItem CreateItemInSlot(Item item, int count, InventorySlot slot)
    {
        GameObject obj = Instantiate(itemPrefab, slot.transform);
        InventoryItem inventoryItem = obj.GetComponent<InventoryItem>();
        inventoryItem.Initialize(item, count);
        return inventoryItem;
    }

    public void NotifySlotChanged()
    {
        RefreshWeapon();
        SaveInventory();
    }

    public void DropItem(InventoryItem inventoryItem)
    {
        if (inventoryItem == null)
            return;

        SpawnWorldItem(inventoryItem.item, inventoryItem.count);
        Destroy(inventoryItem.gameObject);
        RefreshWeapon();
        SaveInventory();
    }

    private void SpawnWorldItem(Item item, int count)
    {
        if (worldItemPrefab == null)
        {
            Debug.LogWarning("World item prefab not assigned.");
            return;
        }

        Vector2 dir = UnityEngine.Random.insideUnitCircle.normalized;

        Vector3 basePosition = dropPoint != null ? dropPoint.position : transform.position;
        Vector3 spawnPosition = basePosition + (Vector3)(dir * dropDistance);

        GameObject obj = Instantiate(worldItemPrefab, spawnPosition, Quaternion.identity);
        obj.GetComponent<WorldItem>().Initialize(
            item, count,
            throwForceOverride: dropForce,
            magnetImmune: true,
            direction: dir,
            pickupBlockSeconds: dropPickupBlock);

        AudioManager.Instance?.PlaySFX(dropSound);
    }

    public void SaveInventory()
    {
        if (!Directory.Exists(_savePath))
            Directory.CreateDirectory(_savePath);

        SaveSlotList(slots, "slot");
        SaveSlotList(bagSlots, "bag");
    }

    public void LoadInventory()
    {
        if (!Directory.Exists(_savePath))
            return;

        LoadSlotList("slot", slots);
        LoadSlotList("bag", bagSlots);

        RefreshWeapon();
    }

    private void SaveSlotList(List<InventorySlot> slotList, string prefix)
    {
        var serializer = new DataContractJsonSerializer(typeof(InventorySlotData));

        for (int i = 0; i < slotList.Count; i++)
        {
            string filePath = Path.Combine(_savePath, $"{prefix}_{i}.json");

            if (slotList[i] == null || slotList[i].transform.childCount == 0)
            {
                if (File.Exists(filePath))
                    File.Delete(filePath);
                continue;
            }

            InventoryItem inventoryItem = slotList[i].transform.GetChild(0).GetComponent<InventoryItem>();
            if (inventoryItem == null) continue;

            InventorySlotData data = new InventorySlotData(i, inventoryItem.item.itemName, inventoryItem.count);

            using (FileStream file = new FileStream(filePath, FileMode.Create))
                serializer.WriteObject(file, data);
        }
    }

    private void LoadSlotList(string prefix, List<InventorySlot> slotList)
    {
        string[] files = Directory.GetFiles(_savePath, $"{prefix}_*.json");
        var serializer = new DataContractJsonSerializer(typeof(InventorySlotData));

        foreach (string filePath in files)
        {
            using (FileStream file = new FileStream(filePath, FileMode.Open))
            {
                InventorySlotData data = (InventorySlotData)serializer.ReadObject(file);

                Item item = itemDatabase != null ? itemDatabase.GetItem(data.ItemName) : null;
                if (item == null)
                {
                    Debug.LogWarning($"Item '{data.ItemName}' not found in ItemDatabase.");
                    continue;
                }

                PlaceItemInSlotList(item, data.SlotIndex, data.Count, slotList);
            }
        }
    }

    private void PlaceItemInSlotList(Item item, int slotIndex, int count, List<InventorySlot> slotList)
    {
        if (slotIndex < 0 || slotIndex >= slotList.Count)
            return;

        InventorySlot slot = slotList[slotIndex];
        if (slot.transform.childCount > 0)
            return;

        CreateItemInSlot(item, count, slot);
    }

    private InventoryItem FindStackWithSpace(Item item)
    {
        foreach (InventorySlot slot in slots)
        {
            if (slot == null || slot.transform.childCount == 0) continue;
            InventoryItem existing = slot.transform.GetChild(0).GetComponent<InventoryItem>();
            if (existing != null && existing.item == item && existing.HasSpace)
                return existing;
        }

        foreach (InventorySlot slot in bagSlots)
        {
            if (slot == null || slot.transform.childCount == 0) continue;
            InventoryItem existing = slot.transform.GetChild(0).GetComponent<InventoryItem>();
            if (existing != null && existing.item == item && existing.HasSpace)
                return existing;
        }

        return null;
    }

    private InventorySlot GetFirstEmptySlot()
    {
        foreach (InventorySlot slot in slots)
            if (slot != null && slot.transform.childCount == 0)
                return slot;

        foreach (InventorySlot slot in bagSlots)
            if (slot != null && slot.transform.childCount == 0)
                return slot;

        return null;
    }
}
