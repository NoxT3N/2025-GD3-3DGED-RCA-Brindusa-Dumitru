using System.Collections.Generic;
using System.Runtime.Serialization;
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

    public UnityEvent OnInventoryUpdated;
    public UnityEvent<int> OnItemSelected;

    [Header("Configuration")]
    public int maxSlots = 10;
    public int maxStackSize = 20; //Maximum quantity per item in a slot
    
    [Header("Debug View")]
    [SerializeField] private List<InventorySlot> slots = new();
    [SerializeField] private ItemData selectedItem;

    public ItemData SelectedItem => selectedItem;
    public int SlotCount => slots.Count;
    public IEnumerable<InventorySlot> Slots => slots;

    public void AddItem(ItemData item, int quantity)
    {
        if (slots.Count >= maxSlots)
        {
            Debug.LogWarning("Inventory is full!");
            return;
        }

        InventorySlot existingSlot = slots.Find(slot => slot.item == item);
        if (existingSlot != null && !existingSlot.IsFull(maxStackSize))
        {
            existingSlot.quantity += quantity;
        }
        else
        {
            slots.Add(new InventorySlot { item = item, quantity = quantity });
        }

        OnInventoryUpdated.Invoke();
    }

    public void RemoveItem(ItemData item, int quantity)
    {
        var existingSlot = slots.Find(slot => slot.item == item);
        if (existingSlot != null)
        {
            existingSlot.quantity -= quantity;
            if (existingSlot.quantity <= 0)
            {
                slots.Remove(existingSlot);
            }
            OnInventoryUpdated.Invoke();
        }
        else
        {
            Debug.LogWarning("Item not found in inventory!");
        }
    }

    public void SelectItem(ItemData item)
    {
        if (slots.Exists(slot => slot.item == item))
        {
            selectedItem = item;
            OnItemSelected.Invoke(slots.FindIndex(slot => slot.item == item));
        }
        else
        {
            Debug.LogWarning("Item not found in inventory!");
        }
    }

}
