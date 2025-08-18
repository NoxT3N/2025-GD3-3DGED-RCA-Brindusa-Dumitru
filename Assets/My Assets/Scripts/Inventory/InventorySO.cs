using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "InventorySO", menuName = "Scriptable Objects/InventorySO")]
public class InventorySO : ScriptableObject
{
    [System.Serializable]
    public class InventorySlot
    {
        public ItemData item;
        public int quantity;
        public bool IsFull(int maxStackSize) => quantity >= maxStackSize;
    }

    [Header("Debug")]
    [SerializeField] private bool debugMode = true;
    
    [Header("Events")]
    public UnityEvent OnInventoryUpdated;
    public UnityEvent<int> OnItemSelected;
    
    [Header("Configuration")]
    public int maxSlots = 10;
    public int maxStackSize = 20;
    
    [Header("Data")]
    [SerializeField] private List<InventorySlot> slots = new List<InventorySlot>();
    [SerializeField] private ItemData selectedItem;
    private int selectedIndex = -1;

    public ItemData SelectedItem => selectedItem;
    public int SlotCount => slots.Count;
    public IEnumerable<InventorySlot> Slots => slots;
    public int SelectedIndex => selectedIndex;

    public void AddItem(ItemData item, int quantity)
    {
        if (item == null)
        {
            if (debugMode) Debug.LogError("Tried to add null item!");
            return;
        }

        if (debugMode) Debug.Log($"Adding {quantity}x {item.name}");
        
        // Try stacking first
        foreach (InventorySlot slot in slots)
        {
            if (slot.item == item && !slot.IsFull(maxStackSize))
            {
                int spaceLeft = maxStackSize - slot.quantity;
                int addAmount = Mathf.Min(spaceLeft, quantity);
                
                slot.quantity += addAmount;
                quantity -= addAmount;
                
                if (debugMode) Debug.Log($"Stacked {addAmount}x in existing slot (now {slot.quantity})");
                
                if (quantity <= 0) break;
            }
        }
        
        // Add new slots if needed
        while (quantity > 0 && slots.Count < maxSlots)
        {
            int addAmount = Mathf.Min(maxStackSize, quantity);
            slots.Add(new InventorySlot { item = item, quantity = addAmount });
            quantity -= addAmount;
            
            if (debugMode) Debug.Log($"Created new slot with {addAmount}x");
        }
        
        if (quantity > 0)
        {
            if (debugMode) Debug.LogWarning($"Couldn't add {quantity}x {item.name} - inventory full");
        }
        
        OnInventoryUpdated?.Invoke();
    }

    public void RemoveItem(ItemData item, int quantity)
    {
        if (item == null)
        {
            if (debugMode) Debug.LogError("Tried to remove null item!");
            return;
        }

        if (debugMode) Debug.Log($"Removing {quantity}x {item.name}");
        
        // Remove from last slot first
        for (int i = slots.Count - 1; i >= 0; i--)
        {
            if (slots[i].item == item)
            {
                int removeAmount = Mathf.Min(quantity, slots[i].quantity);
                slots[i].quantity -= removeAmount;
                quantity -= removeAmount;
                
                if (debugMode) Debug.Log($"Removed {removeAmount}x from slot {i} (now {slots[i].quantity})");
                
                if (slots[i].quantity <= 0)
                {
                    // Deselect if removing selected item
                    if (selectedIndex == i) SelectItemByIndex(-1);
                    
                    slots.RemoveAt(i);
                    if (debugMode) Debug.Log($"Removed empty slot {i}");
                }
                
                if (quantity <= 0) break;
            }
        }
        
        if (quantity > 0)
        {
            if (debugMode) Debug.LogWarning($"Couldn't remove {quantity}x {item.name} - not enough items");
        }
        
        OnInventoryUpdated?.Invoke();
    }

    public void SelectItemByIndex(int index)
    {
        if (debugMode) Debug.Log($"Selecting slot {index}");

        if (index >= 0 && index < slots.Count)
        {
            selectedItem = slots[index].item;
            selectedIndex = index;
            if (debugMode) Debug.Log($"Selected item: {selectedItem.name} at index {selectedIndex}");
        }
        else
        {
            selectedItem = null;
            selectedIndex = -1;
        }
        
        OnItemSelected?.Invoke(selectedIndex);
    }
}